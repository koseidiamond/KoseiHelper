using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MaryBlock")]
[Tracked]
public class MaryBlock : Entity
{
    private Sprite sprite;
    public bool potted;
    private int direction = -1;
    public static ParticleType maryParticle;
    public static ParticleType darkMaryParticle;
    public bool outline;
    public bool affectTheo;

    public float respawnTimer;
    private bool oneUse = true;
    private Wiggler wiggler;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;

    // darkmary variables
    private MTexture distortionTexture;
    private float distortionAlpha;
    private Vector2 distortionVector;
    private float oscillationPhase = 0f;

    // bubblemary variables
    private Vector2 previousPlayerPos = Vector2.Zero;
    private const int bubbleFrames = 15;
    private Queue<Vector2> playerPositionHistory = new Queue<Vector2>(bubbleFrames);

    // baldmary variables
    private float baldTimer = 0f;
    private bool playerIsBald;
    private bool shutUp;

    public enum MaryType
    {
        Potted,
        Idle,
        Mary,
        Bubble,
        Dark,
        Bald
    };
    public MaryType maryType;

    public MaryBlock(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        // 
        Depth = -9500;
        outline = data.Bool("outline", false);
        affectTheo = data.Bool("affectTheo", false);
        oneUse = data.Bool("oneUse", true);
        potted = data.Bool("potted", false); // legacy
        maryType = data.Enum("maryType", MaryType.Idle);
        shutUp = data.Bool("shutUp", false);
        if (potted) // legacy
            maryType = MaryType.Potted;
        Add(sprite = GFX.SpriteBank.Create(data.Attr("spriteID","koseiHelper_maryBlock")));
        if (maryType == MaryType.Potted)
        {
            sprite.Play("potted");
            Collider = new Hitbox(8, 16, -4, 0);
        }
        if (maryType == MaryType.Idle)
        {
            sprite.Play("idle");
            Collider = new Hitbox(8, 12, -4, 4);
        }
        if (maryType == MaryType.Bald)
        {
            sprite.Play("bald");
            Collider = new Hitbox(8, 12, -4, 4);
        }
        if (maryType == MaryType.Mary)
        {
            sprite.Play("mary");
            Collider = new Hitbox(8, 16, -4, 0);
        }
        if (maryType == MaryType.Bubble)
        {
            sprite.Play("bubble");
            Collider = new Hitbox(8, 16, -4, 0);
        }
        if (maryType == MaryType.Dark)
        {
            sprite.Play("dark");
            Collider = new Hitbox(8, 16, -4, 0);
        }
        if (!oneUse)
        {
            Add(wiggler = Wiggler.Create(1f, 4f, delegate (float v)
            {
                sprite.Scale = Vector2.One * (1f + v * 0.2f);
            }, false, false));
        }

        MTexture mTexture = GFX.Game["util/displacementcirclehollow"];
        distortionTexture = mTexture.GetSubtexture(0, 0, mTexture.Width, mTexture.Height);
        if (maryType == MaryType.Dark)
        {
            Add(new DisplacementRenderHook(RenderDisplacement));
        }

        Add(bloom = new BloomPoint(0.2f, 12f));
        Add(light = new VertexLight(Color.LightYellow, 0.3f, 12, 32));
        Add(new PlayerCollider(OnPlayer));
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        maryParticle = new ParticleType(SwitchGate.P_Behind)
        {
            Color = Color.Yellow,
            Color2 = Color.Orange
        };
        darkMaryParticle = new ParticleType(SwitchGate.P_Behind)
        {
            Color = Color.Purple,
            Color2 = Color.DarkViolet
        };
    }

    private void RenderDisplacement()
    {
        distortionTexture.DrawCentered(Position + new Vector2(0,6), Color.White * 0.8f * distortionAlpha, distortionVector);
    }

    public override void Update()
    {
        base.Update();
        //Logger.Debug(nameof(KoseiHelperModule), $"{baldTimer}");
        Player player = Scene.Tracker.GetEntity<Player>();
        if (player != null)
        {
            direction = Math.Sign(Scene.Tracker.GetEntity<Player>().Position.X - this.Position.X);
            if (maryType == MaryType.Bubble)
            {
                previousPlayerPos = player.Position;
                if (playerPositionHistory.Count >= bubbleFrames)
                {
                    playerPositionHistory.Dequeue();  // Remove the oldest position if we have bubbleFrames positions
                }
                playerPositionHistory.Enqueue(player.Position);
            }
            if (maryType == MaryType.Bald)
            {
                baldTimer += Engine.DeltaTime;
                if (playerIsBald)
                {
                    if (baldTimer >= 0.5f || (Input.Dash.Pressed || Input.CrouchDash.Pressed))
                    {
                        playerIsBald = false;
                        player.Add(player.Hair);
                        player.ResetSpriteNextFrame(player.Sprite.Mode);
                        baldTimer = 0f;
                        player.StateMachine.State = Player.StNormal;
                    }
                }

            }
        }

        TheoCrystal theo = SceneAs<Level>().Tracker.GetNearestEntity<TheoCrystal>(Center);
        if (theo != null && affectTheo && CollideCheck(theo) && Collidable)
            OnTheo(theo);
        bool flag = this.respawnTimer > 0f;
        if (flag)
        {
            this.respawnTimer -= Engine.DeltaTime;
            bool flag2 = this.respawnTimer <= 0f;
            if (flag2 && Visible)
                Respawn();
        }
        if (maryType == MaryType.Dark)
        {
            if (Collidable)
            {
                distortionAlpha = Calc.Approach(distortionAlpha, 0.15f, Engine.DeltaTime * 4f);
                oscillationPhase += Engine.DeltaTime * (float)Math.PI;
                if (oscillationPhase > 100000)
                    oscillationPhase = 0f;
                if (Scene.OnInterval(0.6f))
                    SceneAs<Level>().Displacement.AddBurst(Center, 0.3f, 12, 40, 0.2f);
                if (Scene.OnInterval(0.075f))
                {
                    distortionAlpha = 0f;
                    distortionVector = new Vector2(
                        (float)(Math.Sin(oscillationPhase) * 0.3f + 0.1f));
                }
                float anxietyDistance = Vector2.DistanceSquared(player.Center, Center) / 1.5f;
                float darkEffectTarget = (!(anxietyDistance >= 0f) ? 1f : Calc.ClampedMap(anxietyDistance, 256f, 4096f, 0.5f));
                Distort.AnxietyOrigin = new Vector2((player.Center.X - Position.X) / 320f, (player.Center.Y - Position.Y) / 180f);
                Engine.TimeRate = Calc.Approach(Engine.TimeRate, darkEffectTarget, 4f * Engine.DeltaTime);
                Distort.GameRate = Calc.Approach(Distort.GameRate, Calc.Map(Engine.TimeRate, 0.5f, 1f), Engine.DeltaTime * 2f);
            }
            else
                distortionAlpha = 0f;
        }
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
    }

    private void OnPlayer(Player player)
    {
        Level level = SceneAs<Level>();
        if (player.Scene != null)
        {
            Collidable = false;
            Audio.Play("event:/KoseiHelper/mary", Position);
            Vector2 bubblePosition = GetPositionFromHistory();
            if (maryType == MaryType.Bald && !playerIsBald)
            {
                if (!shutUp)
                    Scene.Add(new MiniTextbox("KoseiHelper_MaryBlock_Bald"));
                player.StateMachine.State = StBald;
                baldTimer = 0f;
                playerIsBald = true;
            }
            if (maryType == MaryType.Potted || maryType == MaryType.Idle)
            {
                player.SideBounce(direction, Position.X, Position.Y);
            }
            if (maryType == MaryType.Potted)
            {
                player.ExplodeLaunch(Position, false);
            }
            if (maryType == MaryType.Mary)
            {
                player.SuperJump();
                player.RefillDash();
            }
            if (maryType == MaryType.Bubble)
            {
                player.StartCassetteFly(bubblePosition, Center);
                player.RefillDash();
            }
            if (maryType == MaryType.Dark)
            {
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                Audio.Play("event:/game/05_mirror_temple/eye_pulse", player.Position);
                Distort.AnxietyOrigin = new Vector2((player.Center.X - Position.X) / 320f, (player.Center.Y - Position.Y) / 180f);
                Engine.TimeRate = 1f;
                Distort.GameRate = 1f;
            }
            float angle = player.Speed.Angle();
            if (maryType == MaryType.Dark)
                level.ParticlesFG.Emit(darkMaryParticle, 5, Center + new Vector2(0, -2), Vector2.One * 4f, angle - (float)Math.PI / 2f);
            else
                level.ParticlesFG.Emit(maryParticle, 5, Center + new Vector2(0, -2), Vector2.One * 4f, angle - (float)Math.PI / 2f);
            Add(new Coroutine(RefillRoutine(player)));
            respawnTimer = 2.5f;
        }
    }

    private Vector2 GetPositionFromHistory()
    {
        if (playerPositionHistory.Count < bubbleFrames)
            return previousPlayerPos;
        return playerPositionHistory.Peek();
    }

    private void OnTheo(TheoCrystal theo)
    {
        Level level = SceneAs<Level>();
        Collidable = false;
        sprite.Visible = false;
        Audio.Play("event:/KoseiHelper/mary", Position);
        theo.HitSpinner(this);
        if (maryType == MaryType.Potted)
            theo.ExplodeLaunch(Position);
        float angle = theo.Speed.Angle();
        if (maryType == MaryType.Dark)
            level.ParticlesFG.Emit(darkMaryParticle, 5, Center + new Vector2(0, -2), Vector2.One * 4f, angle - (float)Math.PI / 2f);
        else
            level.ParticlesFG.Emit(maryParticle, 5, Center + new Vector2(0, -2), Vector2.One * 4f, angle - (float)Math.PI / 2f);
        if (maryType == MaryType.Dark)
            theo.Die();
        if (oneUse)
            RemoveSelf();
        else
            respawnTimer = 2.5f;
    }

    public override void Render()
    {
        if (outline && sprite.Visible)
            sprite.DrawOutline();
        base.Render();
    }

    private void Respawn()
    {
        bool flag = !this.Collidable;
        Player player = Scene.Tracker.GetEntity<Player>();
        if (flag)
        {
            Collidable = true;
            sprite.Visible = true;
            outline = true;
            Depth = -100;
            wiggler.Start();
            Audio.Play("event:/KoseiHelper/maryReappear", this.Position);
            if (maryType == MaryType.Dark)
                SceneAs<Level>().ParticlesFG.Emit(darkMaryParticle, 16, this.Position, Vector2.One * 2f);
            else
                SceneAs<Level>().ParticlesFG.Emit(maryParticle, 16, this.Position, Vector2.One * 2f);
            if (maryType == MaryType.Potted)
                sprite.Play("potted");
            if (maryType == MaryType.Idle)
                sprite.Play("idle");
            if (maryType == MaryType.Mary)
                sprite.Play("mary");
        }
    }

    private IEnumerator RefillRoutine(Player player)
    {
        yield return null;
        SceneAs<Level>().Shake(0.3f);
        sprite.Visible = false;
        bool flag = !this.oneUse;
        if (flag)
            outline = true;
        this.Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        if (maryType == MaryType.Dark)
        {
            SceneAs<Level>().ParticlesFG.Emit(darkMaryParticle, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
            SceneAs<Level>().ParticlesFG.Emit(darkMaryParticle, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
        }
        else
        {
            SceneAs<Level>().ParticlesFG.Emit(maryParticle, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
            SceneAs<Level>().ParticlesFG.Emit(maryParticle, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
        }
        SlashFx.Burst(Position, angle);
        if (oneUse)
            if (maryType == MaryType.Bald)
            {
                Collidable = false;
                Visible = false;
            }
            else
                this.RemoveSelf();
        yield break;
    }

    private class BaldStateComponent : Component
    {
        public BaldStateComponent() : base(false, false) { }
        public MaryBlock CurrentMary;
    }

    public static int StBald;

    public static void RegisterBaldState(Player player)
    {
        StBald = player.AddState("KoseiHelper_StBald", StBaldUpdate, null, StBaldBegin, null);
        player.Add(new BaldStateComponent());
    }

    private static void StBaldBegin(Player player)
    {
        player.Get<PlayerHair>()?.RemoveSelf();
    }

    private static int StBaldUpdate(Player player)
    {

        return StBald;
    }
}