using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DebugRenderer")]
public class DebugRenderer : Entity
{
    public Color color;
    public float width, height;
    public Vector2 node;
    public string flagName;
    public bool flagValue;
    public string message;
    public string imagePath;
    public bool scaled;
    public enum Shape
    {
        HollowRectangle,
        FilledRectangle,
        Circle,
        Point,
        Line,
        Text,
        Image
    }
    public Shape shape;
    public enum Font
    {
        Consolas12,
        Renogare
    }
    public Font font;

    public DebugRenderer(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        color = data.HexColor("color", Calc.HexToColor("ffffff"));
        width = data.Width;
        height = data.Height;
        shape = data.Enum("shape", Shape.HollowRectangle);
        node = data.Nodes.Length > 0 ? data.Nodes[0] + offset : Vector2.Zero;
        flagName = data.Attr("flag", "");
        message = data.Attr("message", "text");
        font = data.Enum("font", Font.Consolas12);
        imagePath = data.Attr("imagePath", "");
        scaled = data.Bool("scaled", true);
    }

    public override void Update()
    {
        base.Update();
        if (!string.IsNullOrEmpty(flagName))
            flagValue = SceneAs<Level>().Session.GetFlag(flagName);
    }

    public override void DebugRender(Camera camera)
    {
        if (flagValue || string.IsNullOrEmpty(flagName))
        {
            switch (shape)
            {
                case Shape.HollowRectangle:
                    Draw.HollowRect(X, Y, width, height, color);
                    break;
                case Shape.FilledRectangle:
                    Draw.Rect(X, Y, width, height, color);
                    break;
                case Shape.Circle:
                    Draw.Circle(new Vector2(X + width / 2, Y + height / 2), width / 2, color, 1);
                    break;
                case Shape.Point:
                    Draw.Point(this.Position, color);
                    break;
                case Shape.Line:
                    Draw.Line(new Vector2(X, Y), node, color);
                    break;
                case Shape.Text:
                    switch (font)
                    {
                        case Font.Consolas12:
                            Draw.Text(Draw.DefaultFont, message, new Vector2(X, Y), color);
                            break;
                        case Font.Renogare:
                            ActiveFont.Draw(message, new Vector2(X, Y), color);
                            break;
                    }
                    break;
                case Shape.Image:
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        Image image = new Image(GFX.Game[imagePath]);
                        image.Position = Position;
                        if (scaled)
                            image.Scale = new Vector2(width / image.Width, height / image.Height);
                        image.Render();
                    }
                    break;
            }

            base.DebugRender(camera);
        }
    }
}
