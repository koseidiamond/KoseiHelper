using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/BoundRefill")]
public class BoundRefill : Entity
{
    public static ParticleType P_Shatter = Refill.P_ShatterTwo;
    public static ParticleType P_Regen = Refill.P_RegenTwo;
    public static ParticleType P_Glow = Refill.P_GlowTwo;
    private readonly BloomPoint bloom;
    private Level level;
    private readonly VertexLight light;
    private readonly SineWave sine;
    public Sprite sprite;
    private readonly bool outBound;
    public bool outline;
    public float respawnTimer;

    public BoundRefill (EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        //Read the custom properties from data
        outBound = data.Bool("outBound",true);

        Add(sprite = GFX.SpriteBank.Create("koseiHelper_boundRefill"));
        if (outBound)
            sprite.Play("OutBound");
        else
            sprite.Play("InBound");
        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.5f, 16f));
        Add(light = new VertexLight(Color.White, 0.5f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f));
        sine.Randomize();
        UpdateY();
        base.Depth = -100;
    }


    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
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
        if (outBound)
            player.EnforceLevelBounds = false;

        else
        {
            player.EnforceLevelBounds = true;
            //Ideally this should check if the refill is not on the level bounds, and set a new spawn, to make filler rooms possible to transition to
        }
    }

    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
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
        RemoveSelf();
    }
}