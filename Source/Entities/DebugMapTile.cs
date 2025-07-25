using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

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
    public bool above;
    public Shape shape;
    public float thickness;
    public int resolution;
    public bool hollow;
    public float textSize;
    public string message;
    public bool altFont;
    public string texture;
    public float scaleX, scaleY;
    public float rotation;
    public bool gui;
    public int animationFrames;
    public float animationSpeed;
    public float angle, length;
    public DebugMapTile(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        width = data.Width;
        height = data.Height;
        above = data.Bool("above", true);
        color = data.HexColor("color", Calc.HexToColor("fc03f4"));
        shape = data.Enum("shape", Shape.Tile);
        thickness = data.Float("thickness", 1f);
        resolution = data.Int("resolution", 500);
        hollow = data.Bool("hollow", false);
        textSize = data.Float("textSize", 1f);
        message = data.Attr("message", "hello");
        altFont = data.Bool("altFont", false);
        texture = data.Attr("texture", "characters/bird/Recover03");
        scaleX = data.Float("scaleX", 1f);
        scaleY = data.Float("scaleY", 1f);
        rotation = (float) (data.Float("rotation", 0f) * Math.PI / 180f);
        gui = data.Bool("gui", false);
        animationFrames = data.Int("animationFrames", 0);
        animationSpeed = data.Float("animationSpeed", 10);
        angle = data.Float("angle", 0f);
        length = data.Float("length", 0f);
        
    }

    public DebugMapTile(Vector2 position, bool above, Color color, float width, float height, Shape shape, float thickness, int resolution, bool hollow,
        float textSize, string message, bool altFont, string texture, float scaleX, float scaleY, float rotation, int animationFrames, float animationSpeed, bool gui,
        float angle, float length)
    {
        Position = position;
        this.above = above;
        this.color = color;
        this.width = width;
        this.height = height;
        this.shape = shape;
        this.thickness = thickness;
        this.resolution = resolution;
        this.hollow = hollow;
        this.textSize = textSize;
        this.message = message;
        this.altFont = altFont;
        this.texture = texture;
        this.scaleX = scaleX;
        this.scaleY = scaleY;
        this.rotation = rotation;
        this.gui = gui;
        this.animationFrames = animationFrames;
        this.animationSpeed = animationSpeed;
        this.angle = angle;
        this.length = length;
    }
}