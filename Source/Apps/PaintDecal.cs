using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
namespace Celeste.Mod.KoseiHelper.Apps;

public class PaintDecal : Entity
{
    private readonly List<(Vector2 from, Vector2 to, Color color, int thickness)> lines;

    public PaintDecal(Vector2 position, List<(Vector2 from, Vector2 to, Color color, int thickness)> source, int depth) : base(position)
    {
        lines = new(source);
        base.AddTag(Tags.Persistent);
        base.AddTag(Tags.Global);
        Depth = depth;
    }

    public override void Render()
    {
        foreach (var line in lines)
        {
            Draw.Line(Position + line.from, Position + line.to, line.color, line.thickness);
        }
    }
}