using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
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
}

[CustomEntity("KoseiHelper/PaintBarrier")]
[Tracked]
public class PaintBarrier : Solid
{
    private float timer;
    public PaintBarrier(Vector2 position, Collider collider, float ttl) : base(position, 1, 1, safe: false)
    {
        Collider = collider;
        timer = ttl;
        if (timer == -1f)
        {
            base.AddTag(Tags.Persistent);
            base.AddTag(Tags.Global);
        }
        base.AddTag(Tags.TransitionUpdate);
        SurfaceSoundIndex = 8;
    }

    public override void Update()
    {
        base.Update();
        if (timer > 0)
            timer -= Engine.DeltaTime;
        if (timer <= 0 && timer > -1f)
            RemoveSelf();
    }
}