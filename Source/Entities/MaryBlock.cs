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
    public static ParticleType maryParticle = SwitchGate.P_Behind;
    public bool outline;
    public bool affectTheo;

    public float respawnTimer;
    private bool oneUse = true;
    private Wiggler wiggler;

    // darkmary variables
    private MTexture distortionTexture;
    private float distortionAlpha;
    private Vector2 distortionVector;
    private float oscillationPhase = 0f;

    private readonly BloomPoint bloom;
    private readonly VertexLight light;

    // bubblemary variables
    private Vector2 previousPlayerPos = Vector2.Zero;
    private Queue<Vector2> playerPositionHistory = new Queue<Vector2>(15);

    public enum MaryType
    {
        Potted,
        Idle,
        Mary,
        Bubble,
        Dark
    };
    public MaryType maryType;

    public MaryBlock(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = -9500;
        outline = data.Bool("outline", false);
        affectTheo = data.Bool("affectTheo", false);
        oneUse = data.Bool("oneUse", true);
        potted = data.Bool("potted", false); // legacy
        maryType = data.Enum("maryType", MaryType.Idle);
        if (potted) // legacy
            maryType = MaryType.Potted;
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_maryBlock"));
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
            maryParticle.Color = Color.Purple;
            maryParticle.Color2 = Color.DarkViolet;
        }

        Add(bloom = new BloomPoint(0.2f, 12f));
        Add(light = new VertexLight(Color.LightYellow, 0.3f, 12, 32));
        Add(new PlayerCollider(OnPlayer));
    }

    private void RenderDisplacement()
    {
        distortionTexture.DrawCentered(Position + new Vector2(0,6), Color.White * 0.8f * distortionAlpha, distortionVector);
    }

    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        if (player != null)
        {
            direction = Math.Sign(Scene.Tracker.GetEntity<Player>().Position.X - this.Position.X);
            if (maryType == MaryType.Bubble)
            {
                previousPlayerPos = player.Position;
                if (playerPositionHistory.Count >= 15)
                {
                    playerPositionHistory.Dequeue();  // Remove the oldest position if we have 15 positions (0.25s = 15 frames)
                }
                playerPositionHistory.Enqueue(player.Position);
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
            if (flag2)
                Respawn();
        }
        if (maryType == MaryType.Dark) // TODO
        {
            if (Collidable)
            {
                distortionAlpha = Calc.Approach(distortionAlpha, 1f, Engine.DeltaTime * 4f);
                oscillationPhase += Engine.DeltaTime * 200f;
                if (oscillationPhase > Math.PI * 2)
                    oscillationPhase -= (float)(Math.PI * 2);
                if (Scene.OnInterval(0.5f))
                    SceneAs<Level>().Displacement.AddBurst(Center, 0.3f, 12, 32, 0.1f);
                if (Scene.OnInterval(0.1f))
                {
                    distortionAlpha = 0f;
                    distortionVector = new Vector2(
                        (float)(Math.Sin(oscillationPhase) * 0.075f + 0.125f));
                }
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
            if (maryType == MaryType.Potted || maryType == MaryType.Idle)
            player.SideBounce(direction, Position.X, Position.Y);
            if (maryType == MaryType.Potted)
                player.ExplodeLaunch(Position, false);
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
            }
            float angle = player.Speed.Angle();
            level.ParticlesFG.Emit(maryParticle, 5, Center + new Vector2(0, -2), Vector2.One * 4f, angle - (float)Math.PI / 2f);
            Add(new Coroutine(RefillRoutine(player)));
            respawnTimer = 2.5f;
        }
    }

    private Vector2 GetPositionFromHistory()
    {
        if (playerPositionHistory.Count < 15)
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
        if (flag)
        {
            Collidable = true;
            sprite.Visible = true;
            outline = true;
            Depth = -100;
            wiggler.Start();
            Audio.Play("event:/KoseiHelper/maryReappear", this.Position);
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
        SceneAs<Level>().ParticlesFG.Emit(maryParticle, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
        SceneAs<Level>().ParticlesFG.Emit(maryParticle, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
        SlashFx.Burst(Position, angle);
        bool flag2 = oneUse;
        if (flag2)
            this.RemoveSelf();
        yield break;
    }
}