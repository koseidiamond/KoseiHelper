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
        Text,
        Decal
    };
    public Shape shape;
    public float textSize;
    public string message;
    public string texture;
    public DebugMapTile(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        width = data.Width;
        height = data.Height;
        color = data.HexColor("color", Calc.HexToColor("fc03f4"));
        shape = data.Enum("shape", Shape.Tile);
        textSize = data.Float("textSize", 1f);
        message = data.Attr("message", "hello");
    }

    public DebugMapTile(Vector2 position, Color color, float width, float height, Shape shape, float textSize, string message)
    {
        Position = position;
        this.color = color;
        this.width = width;
        this.height = height;
        this.shape = shape;
        this.textSize = textSize;
        this.message = message;
    }
}