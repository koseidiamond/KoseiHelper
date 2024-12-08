using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/PufferRefill")]
public class PufferRefill : Entity
{
    public static ParticleType P_Shatter = Refill.P_ShatterTwo;
    public static ParticleType P_Regen = Refill.P_RegenTwo;
    public static ParticleType P_Glow = Refill.P_GlowTwo;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private readonly SineWave sine;
    public Sprite sprite;
    private readonly bool outBound;
    public bool outline;
    public float respawnTimer;
    public bool hasPufferDash;
    public bool oneUse;

    public PufferRefill(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        oneUse = data.Bool("oneUse", false);
        Add(new PlayerCollider(OnPlayer));
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_boundRefill"));
        sprite.Play("InBound");
        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.5f, 16f));
        Add(light = new VertexLight(Color.White, 0.5f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f));
        sine.Randomize();
        UpdateY();
        base.Depth = -100;
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
                Respawn();
        }
        if (base.Scene.OnInterval(0.1f))
        {
            level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
        }
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
        if (player != null)
        {

            Logger.Debug(nameof(KoseiHelperModule), $"StartedDashing {player.StartedDashing} DashDir {player.DashDir} explodePos {player.Center - 8 * player.DashDir}");
            if (player.StartedDashing && hasPufferDash)
            {
                Add(new Coroutine(PufferDash(player)));
            }
        }
    }

    private void UpdateY()
    {
        Sprite obj = sprite;
        float num = (bloom.Y = sine.Value * 2f);
        float y = (obj.Y = num);
        obj.Y = y;
    }

    public override void Render()
    {
        if (sprite.Visible)
        {
            sprite.DrawOutline();

        }
        base.Render();
    }

    private void OnPlayer(Player player)
    {
        Collidable = false;
        Audio.Play("event:/new_content/game/10_farewell/pinkdiamond_touch", Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        sprite.Play("NoBound");
        Add(new Coroutine(RefillRoutine(player)));
        player.RefillDash();
        respawnTimer = 2.5f;
        hasPufferDash = true;
        if (oneUse)
            ;//RemoveSelf();
    }

    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
        Level level = SceneAs<Level>();
        yield return null;
        level.Shake();
        sprite.Visible = false;
        outline = true;
        base.Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, angle + (float)Math.PI / 2f);
        SlashFx.Burst(Position, angle);
    }

    private void Respawn()
    {
        Level level = SceneAs<Level>();
        if (!Collidable)
        {
            Collidable = true;
            sprite.Visible = true;
            outline = false;
            base.Depth = -100;
            Audio.Play("event:/new_content/game/10_farewell/pinkdiamond_return", Position);
            level.ParticlesFG.Emit(P_Regen, 16, Position, Vector2.One * 2f);
        }
    }

    public IEnumerator PufferDash(Player player)
    {
        Level level = SceneAs<Level>();
        hasPufferDash = false;
        Audio.Play("event:/new_content/game/10_farewell/puffer_splode");
        level.Shake();
        level.Displacement.AddBurst(player.Center - 8 * player.DashDir, 0.4f, 12f, 36f, 0.5f);
        level.Displacement.AddBurst(player.Center - 8 * player.DashDir, 0.4f, 24f, 48f, 0.5f);
        level.Displacement.AddBurst(player.Center - 8 * player.DashDir, 0.4f, 36f, 60f, 0.5f);
        for (float num = 0f; num < MathF.PI * 2f; num += 0.17453292f)
        {
            Vector2 position = player.Center - 8 * player.DashDir + Calc.AngleToVector(num + Calc.Random.Range(-MathF.PI / 90f, MathF.PI / 90f), Calc.Random.Range(12, 18));
            level.Particles.Emit(Seeker.P_Regen, position, num);
        }
        yield return null;
        player.ExplodeLaunch(player.Center - 2 * player.DashDir, false);
    }

    private class PufferStateComponent() : Component(false, false)
    {
        public PufferRefill puffered;
    }
}