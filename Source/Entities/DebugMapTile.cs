using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DebugMapTile")]
public class DebugMapTile : Entity
{
    public Color color;
    public float width, height;
    public enum Shape
    {
        Tile,
        Circle,
        Text
    };
    public Shape shape;
    public float textSize;

    public DebugMapTile(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        color = data.HexColor("color", Calc.HexToColor("fc03f4"));
        width = data.Width;
        height = data.Height;
        shape = data.Enum("shape", Shape.Tile);
        textSize = data.Float("textSize", 1f);
    }
}