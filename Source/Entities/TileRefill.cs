using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/TileRefill")]
public class TileRefill : Entity
{
    public static ParticleType P_Shatter;
    public static ParticleType P_Regen;
    public static ParticleType P_Glow;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private readonly SineWave sine;
    public Sprite sprite;
    public bool outline;
    public float respawnTimer;
    public bool collidable;
    public bool visible;
    public bool oneUse;
    public float respawnTime = 2.5f;

    public TileRefill(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        //Read the custom properties from data
        respawnTime = data.Float("respawnTime", 2.5f);
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
        Level level = SceneAs<Level>();
        foreach (SolidTiles solid in level.Entities.FindAll<SolidTiles>())
        {
            solid.Collidable = true;
            solid.Visible = true;
        }
    }

    public override void Added(Scene scene)
    {
        P_Shatter = new ParticleType(Refill.P_ShatterTwo)
        {
            Color = Color.DimGray,
            Color2 = Color.DarkGray
        };
        P_Regen = new ParticleType(Refill.P_RegenTwo)
        {
            Color = Color.LightGray,
            Color2 = Color.Gray
        };
        P_Glow = new ParticleType(Refill.P_Glow)
        {
            Color = Color.LightGray,
            Color2 = Color.Gray
        };
        base.Added(scene);
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
            {
                Respawn();
            }
        }
        else
        {
            if (base.Scene.OnInterval(0.1f))
                level.ParticlesFG.Emit(P_Glow, 1, Position, Vector2.One * 5f);
        }
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
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

    private IEnumerator RefillRoutine(Player player)
    {
        Level level = SceneAs<Level>();
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

    private void UpdateY()
    {
        Sprite obj = sprite;
        float num = (bloom.Y = sine.Value * 2f);
        float y = (obj.Y = num);
        obj.Y = y;
    }

    private void OnPlayer(Player player)
    {
        Collidable = false;
        Level level = SceneAs<Level>();
        Audio.Play("event:/new_content/game/10_farewell/pinkdiamond_touch", Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        sprite.Visible = false;
        Add(new Coroutine(RefillRoutine(player)));
        respawnTimer = respawnTime;
        foreach (SolidTiles solid in level.Entities.FindAll<SolidTiles>())
        {
            if (collidable)
                solid.Collidable = !solid.Collidable;
            if (visible)
                solid.Visible = !solid.Visible;
        }
    }
}