using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.NemesisGun;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
namespace Celeste.Mod.KoseiHelper.Apps;

public struct PaintStroke(Vector2 from, Vector2 to, Color color, int thickness)
{
    public Vector2 From = from;
    public Vector2 To = to;
    public Color Color = color;
    public int Thickness = thickness;
}

[CustomEntity("KoseiHelper/PaintDecal")]
[Tracked]
public class PaintDecal : Entity
{
    private readonly List<PaintStroke> lines;
    private float timer, initialTimer;
    private ParticleType PaintParticles = new ParticleType
    {
        Source = GFX.Game["particles/triggerspike"],
        LifeMin = 0.25f,
        LifeMax = 0.6f,
        SpeedMin = 30f,
        SpeedMax = 80f,
        DirectionRange = MathHelper.PiOver2,
        FadeMode = ParticleType.FadeModes.Linear,
        ScaleOut = true
    };

    public PaintDecal(Vector2 position, List<PaintStroke> source, int depth, float ttl) : base(position)
    {
        lines = new(source);
        timer = initialTimer = ttl;
        if (timer == -1f)
        {
            base.AddTag(Tags.Persistent);
            base.AddTag(Tags.Global);
        }
        base.AddTag(Tags.TransitionUpdate);
        Depth = depth;
    }

    public override void Update()
    {
        base.Update();
        if (timer > 0)
            timer -= Engine.DeltaTime;
        if (timer <= 0 && timer > -1f)
            RemoveSelf();
    }

    public override void Render()
    {
        float alpha = 1f;
        if (initialTimer > 0f)
            alpha = Ease.QuintOut(Calc.Clamp(timer / initialTimer, 0f, 1f));
        foreach (PaintStroke line in lines)
        {
            Draw.Line(Position + line.From, Position + line.To, line.Color * alpha, line.Thickness);
        }
    }

    public void Shatter(Vector2 forceDirection)
    {
        Level level = SceneAs<Level>();

        forceDirection = forceDirection.SafeNormalize();
        Audio.Play("event:/KoseiHelper/magicShatter", Position);
        foreach (PaintStroke stroke in lines)
        {
            float length = Vector2.Distance(stroke.From, stroke.To);
            int count = Math.Max(2, (int)(length / 6f));

            for (int i = 0; i <= count; i++)
            {
                Vector2 pos = Position + Vector2.Lerp(stroke.From, stroke.To, i / (float)count);
                PaintParticles.Color = stroke.Color;
                PaintParticles.Color2 = Color.Lerp(stroke.Color, Color.White, 0.3f);
                float angle = forceDirection.Angle() + Calc.Random.Range(-0.8f, 0.8f);
                level.ParticlesFG.Emit(PaintParticles, 1, pos, Vector2.One * 2f, stroke.Color, angle);
            }
        }

        RemoveSelf();
    }
}

[CustomEntity("KoseiHelper/PaintBarrier")]
[Tracked]
public class PaintBarrier : Solid
{
    private float timer;
    public PaintDecal Decal { get; set; }
    public PaintBarrier(Vector2 position, Collider collider, PaintDecal decal, float ttl, int surfaceSoundIndex = 8) : base(position, 1, 1, safe: false)
    {
        Collider = collider;
        Decal = decal;
        timer = ttl;
        if (timer == -1f)
        {
            base.AddTag(Tags.Persistent);
            base.AddTag(Tags.Global);
        }
        base.AddTag(Tags.TransitionUpdate);
        SurfaceSoundIndex = surfaceSoundIndex;
    }

    public override void Update()
    {
        base.Update();
        if (timer > 0)
            timer -= Engine.DeltaTime;
        if (timer <= 0 && timer > -1f)
        {
            RemoveSelf();
            return;
        }

        // And now a list of entities that can break the Paintstrokes yay
        AngryOshiro oshiro = CollideFirst<AngryOshiro>();
        if (oshiro != null && oshiro.state == AngryOshiro.StAttack)
        {
            Break(Vector2.UnitX);
            return;
        }

        foreach (FinalBossBeam beam in Scene.Tracker.GetEntities<FinalBossBeam>())
        {
            if (BeamHitsPaint(beam))
            {
                Break(Calc.AngleToVector(beam.angle, 1f));
                return;
            }
        }
        FinalBossShot shot = CollideFirst<FinalBossShot>();
        if (shot != null)
        {
            Break(shot.speed.SafeNormalize());
            return;
        }
        Bullet bullet = CollideFirst<Bullet>();
        if (bullet != null)
        {
            Break(bullet.velocity.SafeNormalize());
            return;
        }
    }

    private bool BeamHitsPaint(FinalBossBeam beam)
    { // I'm sorry for making things this jank
        Vector2 start = beam.boss.BeamOrigin + Calc.AngleToVector(beam.angle, 12f);
        Vector2 end = beam.boss.BeamOrigin + Calc.AngleToVector(beam.angle, 2000f);
        ColliderList list = Collider as ColliderList;
        foreach (Hitbox hitbox in list.colliders)
        {
            if (beam.beamSprite.CurrentAnimationID == "shoot" && Collide.RectToLine(Position.X + hitbox.Left, Position.Y + hitbox.Top, hitbox.Width, hitbox.Height, start, end))
            {
                return true;
            }
        }
        return false;
    }

    private void Break(Vector2 dir)
    {
        Decal?.Shatter(dir);
        RemoveSelf();
    }
}