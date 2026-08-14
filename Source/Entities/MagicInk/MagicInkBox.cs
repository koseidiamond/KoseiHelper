using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Apps;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MagicInkBox")]
[Tracked]
public class MagicInkBox : Solid
{
    private Vector2 previousPosition;
    private float leftoverDistance;
    public string bumpSound;

    private float pushSpeed = 200f;
    private float maxSpeed;
    private Vector2 velocity;
    private bool singleUse;
    private bool disintegrateWhenStopped;


    private static readonly ParticleType BoxParticles = new ParticleType
    {
        Source = GFX.Game["particles/triggerspike"],
        LifeMin = 0.25f,
        LifeMax = 0.6f,
        SpeedMin = 30f,
        SpeedMax = 80f,
        DirectionRange = MathHelper.TwoPi,
        FadeMode = ParticleType.FadeModes.Linear,
        ScaleOut = true
    };

    public MagicInkBox(EntityData data, Vector2 offset) : base(data.Position + offset, 32, 32, true)
    {
        Depth = data.Int("depth", -1000);
        pushSpeed = data.Float("pushSpeed", 200f);
        singleUse = data.Bool("singleUse", false);
        maxSpeed = pushSpeed * 1.5f;
        bumpSound = data.Attr("bumpSound", "event:/game/03_resort/forcefield_bump");
        OnDashCollide = OnDashed;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        previousPosition = Center;
    }

    /// <summary>
    /// Moves the box when it is pushed by the player dashing.
    /// It temporarily sets the paint barriers to uncollidable to avoid getting stuck.
    /// </summary>
    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();


        List<PaintBarrier> disabled = new();
        try
        {
            foreach (PaintBarrier barrier in Scene.Tracker.GetEntities<PaintBarrier>())
            {
                if (barrier.PaintOwner == this)
                {
                    barrier.Collidable = false;
                    disabled.Add(barrier);
                }
            }

            if (velocity.X != 0f)
            {
                MoveHCollideSolids(velocity.X * Engine.DeltaTime, false);
                velocity.X = Calc.Approach(velocity.X, 0f, 600f * Engine.DeltaTime);
            }

            if (velocity.Y != 0f)
            {
                MoveVCollideSolids(velocity.Y * Engine.DeltaTime, false);
                velocity.Y = Calc.Approach(velocity.Y, 0f, 600f * Engine.DeltaTime);
            }
        }
        finally
        {

            foreach (PaintBarrier barrier in disabled)
            {
                barrier.Collidable = true;
            }
        }

        MagicInkController controller = level.Tracker.GetEntity<MagicInkController>();
        if (controller == null)
            return;
        Vector2 current = Center;
        float distance = Vector2.Distance(previousPosition, current);
        const float spacing = 1f;
        leftoverDistance += distance;
        while (leftoverDistance >= spacing && distance > 0f)
        {
            float progress = 1f - leftoverDistance / distance;
            float nextProgress = progress + spacing / distance;
            Vector2 from = Vector2.Lerp(previousPosition, current, MathHelper.Clamp(progress, 0f, 1f));
            Vector2 to = Vector2.Lerp(previousPosition, current, MathHelper.Clamp(nextProgress, 0f, 1f));
            if (!controller.TrySpawnPaint(from, to, this))
                break;
            leftoverDistance -= spacing;
        }
        if (disintegrateWhenStopped && velocity == Vector2.Zero)
        {
            Disintegrate();
            return;
        }
        previousPosition = current;
    }

    public DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
        velocity += direction * pushSpeed;
        velocity = velocity.Clamp(-maxSpeed, -maxSpeed, maxSpeed, maxSpeed);
        Audio.Play(bumpSound, Center);
        if (singleUse)
            disintegrateWhenStopped = true;

        if (player != null)
        {
            KoseiHelperUtils.SideBounce((int)direction.X, player.Position.X, player.Position.Y, player);
        }
        return DashCollisionResults.Rebound;
    }
    private void Disintegrate()
    {
        Level level = SceneAs<Level>();
        for (int x = 2; x < Width; x += 4)
        {
            for (int y = 2; y < Height; y += 4)
            {
                Vector2 pos = Position + new Vector2(x, y);
                Color color = Calc.HsvToColor(((level.TimeActive * 0.25f) + (x + y) * 0.01f) % 1f, 1f, 1f);
                BoxParticles.Color = color;
                BoxParticles.Color2 = Color.Lerp(color, Color.White, 0.3f);
                level.ParticlesFG.Emit(BoxParticles, 1, pos, Vector2.One, color, Calc.Random.NextAngle());
            }
        }
        Audio.Play("event:/KoseiHelper/magicShatter", Center); // unhardcode maybe??
        RemoveSelf();
    }


    // Code partially based on GameHelper's push boxes
    public override void Render()
    {
        base.Render();
        Level level = SceneAs<Level>();
        Color border = Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f);
        Color fill = Color.Lerp(Calc.HexToColor("1a1038"), border, 0.2f);

        Vector2 w = Vector2.UnitX * (Width - 4);
        Vector2 h = Vector2.UnitY * (Height - 4);
        Vector2 p = Position + Vector2.One * 2;

        Draw.Rect(X, Y, Width, Height, Color.Black);
        Draw.Rect(X + 1, Y + 1, Width - 2, Height - 2, fill);

        Draw.Line(p, p + w, border, 2);
        Draw.Line(p + h, p + h + w, border, 2);
        Draw.Line(p, p + h, border, 2);
        Draw.Line(p + w, p + h + w, border, 2);

        Draw.Line(p, p + h + w, border, 2);
        Draw.Line(p + w, p + h, border, 2);
    }
}
