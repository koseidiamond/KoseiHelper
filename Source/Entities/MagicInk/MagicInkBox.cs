using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Apps;
using Microsoft.Xna.Framework;
using Monocle;
using System;
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
    private bool canBreak;
    private bool disintegrateWhenStopped;
    public string breakSfx;
    private int health, maxHealth;


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

    public MagicInkBox(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, true)
    {
        Depth = data.Int("depth", -1000);
        pushSpeed = data.Float("pushSpeed", 200f);
        canBreak = data.Bool("canBreak", false);
        maxSpeed = pushSpeed * 1.5f;
        bumpSound = data.Attr("bumpSfx", "event:/game/03_resort/forcefield_bump");
        OnDashCollide = OnDashed;
        breakSfx = data.Attr("breakSfx", "event:/KoseiHelper/magicShatter");
        health = maxHealth = data.Int("health", 1);

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

            // Move boxes and ensure they stay inside the bounds of the room
            if (velocity.X != 0f)
            {
                float moveX = velocity.X * Engine.DeltaTime;
                float newX = X + moveX;
                if (newX < level.Bounds.Left)
                {
                    moveX = level.Bounds.Left - X;
                    velocity.X = 0f;
                }
                else if (newX + Width > level.Bounds.Right)
                {
                    moveX = level.Bounds.Right - (X + Width);
                    velocity.X = 0f;
                }
                MoveHCollideSolids(moveX, false);
                velocity.X = Calc.Approach(velocity.X, 0f, 600f * Engine.DeltaTime);
            }

            if (velocity.Y != 0f)
            {
                float moveY = velocity.Y * Engine.DeltaTime;
                float newY = Y + moveY;
                if (newY < level.Bounds.Top)
                {
                    moveY = level.Bounds.Top - Y;
                    velocity.Y = 0f;
                }
                else if (newY + Height > level.Bounds.Bottom)
                {
                    moveY = level.Bounds.Bottom - (Y + Height);
                    velocity.Y = 0f;
                }
                MoveVCollideSolids(moveY, false);
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
            if (!controller.TrySpawnPaint(from, to, this, controller.killIfNoInk))
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
        if (!canBreak || (canBreak && health > 0))
        {
            velocity += direction * pushSpeed;
            velocity = velocity.Clamp(-maxSpeed, -maxSpeed, maxSpeed, maxSpeed);
            Audio.Play(bumpSound, Center);
        }

        if (canBreak && health < 2)
            disintegrateWhenStopped = true;

        if (player != null)
        {
            KoseiHelperUtils.SideBounce((int)direction.X, player.Position.X, player.Position.Y, player);
        }
        health--;
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
        Audio.Play(breakSfx, Center);
        RemoveSelf();
    }


    // Code partially based on GameHelper's push boxes (at least when the box is unbreakable)
    public override void Render()
    {
        base.Render();
        Level level = SceneAs<Level>();
        Color border = Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f);
        Color fill = Color.Lerp(Calc.HexToColor("1a1038"), border, 0.2f);

        Vector2 w = Vector2.UnitX * (Width - 4);
        Vector2 h = Vector2.UnitY * (Height - 4);
        Vector2 p = Position + Vector2.One * 2;

        Draw.Rect(X + 1, Y + 1, Width - 2, Height - 2, fill);

        if (canBreak && maxHealth > 1 && Math.Max(Width, Height) / Math.Min(Width, Height) <= 1.99f)
        {
            float radius = (Width + Height) / 6f;
            Vector2[] points = new Vector2[maxHealth];

            for (int i = 0; i < maxHealth; i++)
            {
                float angle = MathHelper.TwoPi * i / maxHealth - MathHelper.PiOver2;
                points[i] = Center + new Vector2(0f, 1f) + Calc.AngleToVector(angle, radius);
            }

            // faint glyph silhouette
            for (int i = 0; i < maxHealth; i++)
            {
                for (int j = i + 1; j < maxHealth; j++)
                {
                    Draw.Line(points[i], points[j], Color.Lerp(border, fill, 0.85f), 1f);
                }
            }

            // dot connections
            for (int i = 0; i < health; i++)
            {
                for (int j = i + 1; j < health; j++)
                {
                    Draw.Line(points[i], points[j], border * 0.4f, 1.5f);
                }
            }
        }

        Draw.Line(p, p + w, border, 2);
        Draw.Line(p + h, p + h + w, border, 2);
        Draw.Line(p, p + h, border, 2);
        Draw.Line(p + w, p + h + w, border, 2);

        if (!canBreak)
        {
            Draw.Line(p - Vector2.One, p + h + w + Vector2.One, Color.Lerp(border, Calc.HexToColor("1a1038"), 0.4f), 2);
            Draw.Line(p + w + new Vector2(1f, -1f), p + h + new Vector2(-1f, 1f), Color.Lerp(border, Calc.HexToColor("1a1038"), 0.4f), 2);
        }

        Draw.HollowRect(X, Y, Width, Height, Color.Black);
    }
}
