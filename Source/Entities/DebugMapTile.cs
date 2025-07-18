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
        Decal,
        Line
    };
    public Shape shape;
    public float thickness;
    public int resolution;
    public float textSize;
    public string message;
    public string texture;
    public float scaleX, scaleY;
    public float rotation;
    public DebugMapTile(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        width = data.Width;
        height = data.Height;
        color = data.HexColor("color", Calc.HexToColor("fc03f4"));
        shape = data.Enum("shape", Shape.Tile);
        thickness = data.Float("thickness", 1f);
        resolution = data.Int("resolution", 500);
        textSize = data.Float("textSize", 1f);
        message = data.Attr("message", "hello");
        texture = data.Attr("texture", "characters/bird/Recover03");
        scaleX = data.Float("scaleX", 1f);
        scaleY = data.Float("scaleY", 1f);
        rotation = data.Float("rotation", 0f);
    }

    public DebugMapTile(Vector2 position, Color color, float width, float height, Shape shape, float thickness, int resolution,
        float textSize, string message, string texture, float scaleX, float scaleY, float rotation)
    {
        Position = position;
        this.color = color;
        this.width = width;
        this.height = height;
        this.shape = shape;
        this.thickness = thickness;
        this.resolution = resolution;
        this.textSize = textSize;
        this.message = message;
        this.texture = texture;
        this.scaleX = scaleX;
        this.scaleY = scaleY;
        this.rotation = rotation;
    }
}