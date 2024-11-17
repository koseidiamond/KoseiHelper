using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/TileRefill")]
public class ForceThrowTrigger : Entity
{
    public static ParticleType P_Shatter = Refill.P_ShatterTwo;
    public static ParticleType P_Regen = Refill.P_RegenTwo;
    public static ParticleType P_Glow = Refill.P_GlowTwo;
    private readonly BloomPoint bloom;
    private Level level;
    private readonly VertexLight light;
    private readonly SineWave sine;
    public Sprite sprite;
    public bool outline;
    public float respawnTimer;
    public bool collidable;
    public bool visible;
    public bool oneUse;

    public ForceThrowTrigger(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        //Read the custom properties from data
        collidable = data.Bool("collidable", true);
        visible = data.Bool("visible", true);
        oneUse = data.Bool("oneUse", false);
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_tileRefill"));
            sprite.Play("tiles");
        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.5f, 16f));
        Add(light = new VertexLight(Color.White, 0.5f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f));
        sine.Randomize();
        UpdateY();
        base.Depth = -100;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        foreach (SolidTiles solid in level.Entities.FindAll<SolidTiles>())
        {
            solid.Collidable = true;
            solid.Visible = true;
        }
    }

public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
            {
                Respawn();
            }
        }
        if (base.Scene.OnInterval(0.1f))
        {
            level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
        }
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
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
        if (sprite.Visible && outline)
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
        sprite.Visible = false;
        Add(new Coroutine(RefillRoutine(player)));
        respawnTimer = 2.5f;
        foreach (SolidTiles solid in level.Entities.FindAll<SolidTiles>())
        {
            if (collidable)
                solid.Collidable = !solid.Collidable;
            if (visible)
                solid.Visible = !solid.Visible;
        }
    }

    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
        yield return null;
        level.Shake();
        sprite.Visible = false;
        if (!oneUse)
            outline = true;
        base.Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
        level.ParticlesFG.Emit(P_Shatter, 5, Position, Vector2.One * 4f, angle + (float)Math.PI / 2f);
        SlashFx.Burst(Position, angle);
        if (oneUse)
            RemoveSelf();
    }

    private void Respawn()
    {
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
}