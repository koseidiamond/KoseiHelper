using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System.Reflection;
using System;
using Mono.Cecil.Cil;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/PufferBall")]
public class PufferBall : Puffer
{
    private const float ResetTime = 0.8f;
    private float resetTimer;
    private Level level;
    private SineWave sine;
    private float atY;
    private float atX;
    private SoundSource spawnSfx;
    private static Vector2 spawnPosition;
    public float speed = 200f;
    public float sineLength;
    public float sineSpeed;
    public bool vertical;
    public string spawnSound;
    private static bool currentPufferFacesRight;
    public bool horizontalFix;
    public float spawnOffset;
    public string flag;

    public PufferBall(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Float("speed") < 0)
    {
        base.Depth = -12500;
        base.Collider = new Hitbox(12f, 12f, -6f, -6f);
        speed = data.Float("speed", 100f);
        sineLength = data.Float("sineLength", 4f);
        sineSpeed = data.Float("sineSpeed", 0.5f);
        vertical = data.Bool("vertical", false);
        spawnSound = data.Attr("spawnSound", "event:/none");
        horizontalFix = data.Bool("horizontalFix", true);
        spawnOffset = data.Float("offset", 0f);
        flag = data.Attr("flag", "");
        Add(sine = new SineWave(sineSpeed, 0f));
        Add(spawnSfx = new SoundSource());
        Get<SineWave>()?.RemoveSelf();
    }

    public static void Load()
    {
        IL.Celeste.Puffer.ctor_Vector2_bool += onPufferConstructor;
        On.Celeste.Puffer.GotoGone += modPufferGotoGone;
    }

    public static void Unload()
    {
        IL.Celeste.Puffer.ctor_Vector2_bool -= onPufferConstructor;
        On.Celeste.Puffer.GotoGone -= modPufferGotoGone;
    }

    private static void onPufferConstructor(ILContext il) // IL hook made by Maddie480!
    {
        ILCursor cursor = new ILCursor(il);

        while (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<SineWave>("Randomize")))
        {
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.Emit(OpCodes.Ldfld, typeof(Puffer).GetField("idleSine", BindingFlags.NonPublic | BindingFlags.Instance));
            cursor.EmitDelegate<Action<Puffer, SineWave>>((self, idleSine) => {
                if (self is PufferBall)
                {
                    idleSine.Reset();
                }
            });
        }
    }

    private static void modPufferGotoGone(On.Celeste.Puffer.orig_GotoGone orig, Puffer self)
    {
        if (self is PufferBall pufferball)
        {
            self.startPosition = spawnPosition;
            self.returnCurve = new SimpleCurve(spawnPosition, spawnPosition, spawnPosition);

        }
        else
            orig(self);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Collidable = Visible = false;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        Collidable = Visible = false;
        if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag) && !string.IsNullOrEmpty(flag))
        {
            waitABit();
            state = States.Gone;
        }
    }

    private void ResetPosition()
    {
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        { // Makes sure that the player is not close to the bounds of the screen. TODO: fix transitions
            if (((!vertical && speed >= 0 && player.Right < (level.Bounds.Right - 64)) ||
                (!vertical && speed < 0 && player.Left > level.Bounds.Left + 64) ||
                (vertical && speed >= 0 && player.Bottom < level.Bounds.Bottom - 48) ||
                (vertical && speed < 0 && player.Top > level.Bounds.Top + 48)))
            {
                spawnSfx.Play(spawnSound);
                Collidable = Visible = true;
                resetTimer = 0f;
                if (!vertical)// Set position to offscreen right/left and same vertical position as player
                {
                    if (speed >= 0)
                        spawnPosition = new Vector2(level.Camera.Right + 10f, player.CenterY + spawnOffset);
                    else
                        spawnPosition = new Vector2(level.Camera.Left - 10f, player.CenterY + spawnOffset);
                    atY = spawnPosition.Y;
                }
                else// Set position to offscreen bottom/top and same horizontal position as player
                {
                    if (speed >= 0)
                        spawnPosition = new Vector2(player.CenterX + spawnOffset, level.Camera.Bottom + 16f);
                    else
                        spawnPosition = new Vector2(player.CenterX + spawnOffset, level.Camera.Top - 16f);
                    atX = spawnPosition.X;
                }
                sine.Reset();
                base.Position = spawnPosition;
                currentPufferFacesRight = speed >= 0;
            }
        }
        else
        {
            resetTimer = 0.05f;
        }
    }

    public override void Update()
    {
        base.Update();
        if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag) && !string.IsNullOrEmpty(flag))
        {
            if (!vertical)
            {
                base.X -= speed * Engine.DeltaTime;
                if (!horizontalFix)
                    base.Y = atY + sineLength * sine.Value;

                // Check if it's off-screen (left/right)
                if (base.X < level.Camera.Left - 60f || (speed < 0 && base.X > level.Camera.Right + 60f))
                {
                    resetTimer += Engine.DeltaTime;
                    if (resetTimer >= ResetTime)
                    {
                        ResetPosition();
                    }
                }
            }
            else
            {
                base.Y -= speed * Engine.DeltaTime;
                base.X = atX + sineLength * sine.Value;

                // Check if it's off-screen (top/bottom)
                if (base.Y < level.Camera.Top - 60f || (speed < 0 && base.Y > level.Camera.Bottom + 60f))
                {
                    resetTimer += Engine.DeltaTime;
                    if (resetTimer >= ResetTime)
                    {
                        ResetPosition();
                    }
                }
            }
        }
        else
            Collidable = Visible = false;

    }

    private IEnumerator waitABit() // Waits a bit to spawn for the first time, to prevent softlocks when transitioning
    {
        yield return 0.5f;
        if (Scene.Tracker.GetEntity<Player>() is not { } player)
            yield break;
        ResetPosition();
    }
}