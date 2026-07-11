using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

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
    public bool nonDebug;
    public int ellipseSegments;
    public float fontSize;
    public float alpha;
    private bool gui;
    public enum Shape
    {
        HollowRectangle,
        FilledRectangle,
        DottedRectangle,
        Circle,
        Ellipse,
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
        alpha = data.Float("alpha", 1f);
        width = data.Width;
        height = data.Height;
        shape = data.Enum("shape", Shape.HollowRectangle);
        node = data.Nodes.Length > 0 ? data.Nodes[0] + offset : Vector2.Zero;
        flagName = data.Attr("flag", "");
        message = data.Attr("message", "text");
        font = data.Enum("font", Font.Consolas12);
        imagePath = data.Attr("imagePath", "");
        scaled = data.Bool("scaled", true);
        nonDebug = data.Bool("nonDebug", true);
        fontSize = data.Float("fontSize", 1f);
        ellipseSegments = data.Int("ellipseSegments", 99);
        Depth = data.Int("depth", -999999);
        gui = data.Bool("gui", false);
        if (gui)
        {
            base.Tag = TagsExt.SubHUD;
        }
    }

    public override void Update()
    {
        base.Update();
        if (!string.IsNullOrEmpty(flagName))
            flagValue = SceneAs<Level>().Session.GetFlag(flagName);
    }

    public override void Render()
    {
        if (nonDebug)
            Rendering();
        base.Render();
    }

    public override void DebugRender(Camera camera)
    {
        Rendering();
        base.DebugRender(camera);
    }

    public void Rendering()
    {
        Level level = SceneAs<Level>();
        Vector2 guiPosition;

        if (flagValue || string.IsNullOrEmpty(flagName))
        {
            switch (shape)
            {
                case Shape.HollowRectangle:
                    if (gui)
                    {
                        guiPosition = 6f * (this.Position - level.Camera.Position) + Vector2.One;
                        Draw.HollowRect(guiPosition.X, guiPosition.Y, 6f * width, 6f * height, color * alpha);
                    }
                    else
                        Draw.HollowRect(X, Y, width, height, color * alpha);
                    break;
                case Shape.DottedRectangle: // Taken from Cherry Helper (Assist Rectangle) and tweaked for alphas so corners don't overlap
                    // todo gui
                    int num = (int)Left;
                    int num2 = (int)(Left + width);
                    int num3 = (int)Top;
                    int num4 = (int)(Top + height);
                    // Corners
                    Draw.Rect(num, num3, 2f, 2f, color * alpha);
                    Draw.Rect(num2 - 2, num3, 2f, 2f, color * alpha);
                    Draw.Rect(num, num4 - 2, 2f, 2f, color * alpha);
                    Draw.Rect(num2 - 2, num4 - 2, 2f, 2f, color * alpha);
                    // Dotted lines
                    for (float num5 = num + 3; num5 < (float)(num2 - 3); num5 += 3f)
                    {
                        Draw.Line(num5, num3, num5 + 2f, num3, color * alpha);
                        Draw.Line(num5, num4 - 1, num5 + 2f, num4 - 1, color * alpha);
                    }
                    for (float num6 = num3 + 3; num6 < (float)(num4 - 3); num6 += 3f)
                    {
                        Draw.Line(num + 1, num6, num + 1, num6 + 2f, color * alpha);
                        Draw.Line(num2, num6, num2, num6 + 2f, color * alpha);
                    }
                    break;
                case Shape.FilledRectangle:
                    if (gui)
                    {
                        guiPosition = 6f * (this.Position - level.Camera.Position) + Vector2.One;
                        Draw.Rect(guiPosition.X, guiPosition.Y, 6f * width, 6f * height, color * alpha);
                    }
                    else
                        Draw.Rect(X, Y, width, height, color * alpha);
                    break;
                case Shape.Circle:
                    if (gui)
                    {
                        guiPosition = 6f * (this.Position - level.Camera.Position) + 6f * new Vector2(12f, 12f);
                        Draw.Circle(guiPosition, 6 * width / 2, color * alpha, 10);
                    }
                    else
                        Draw.Circle(new Vector2(X + width / 2, Y + height / 2), width / 2, color * alpha, 1);
                    break;
                case Shape.Ellipse:
                    if (gui)
                    {
                        guiPosition = 6f * (this.Position - level.Camera.Position) + 6f * new Vector2(12f, 12f);
                        DrawEllipse(guiPosition.X, guiPosition.Y, 6f * width / 2, 6f * height / 2, color * alpha);
                    }
                    else
                        DrawEllipse(X + width / 2, Y + height / 2, width / 2, height / 2, color * alpha);
                    break;
                case Shape.Point:
                    if (gui)
                    {
                        guiPosition = 6f * (this.Position - level.Camera.Position) + Vector2.One;
                        Draw.Point(guiPosition, color * alpha);
                    }
                    else
                        Draw.Point(this.Position, color * alpha);
                    break;
                case Shape.Line:
                    if (gui)
                    {
                        guiPosition = 6f * (this.Position - level.Camera.Position) + Vector2.One;
                        Draw.Line(guiPosition, 6f * (node - level.Camera.Position) + Vector2.One, color * alpha);
                    }
                    else
                        Draw.Line(new Vector2(X, Y), node, color * alpha);
                    break;
                case Shape.Text:
                    if (gui)
                    {
                        guiPosition = 6f * (this.Position - level.Camera.Position) + Vector2.One;
                        switch (font)
                        {
                            case Font.Consolas12:
                                Draw.Text(Draw.DefaultFont, message, guiPosition - new Vector2(0, 16f), color * alpha, Vector2.Zero, Vector2.One * fontSize, 0);
                                break;
                            case Font.Renogare:
                                ActiveFont.Draw(message, guiPosition - new Vector2(0, 16f), Vector2.Zero, Vector2.One * fontSize / 2, color * alpha);
                                break;
                        }
                    }
                    else
                    {
                        switch (font)
                        {
                            case Font.Consolas12:
                                Draw.Text(Draw.DefaultFont, message, new Vector2(X, Y), color * alpha, Vector2.Zero, Vector2.One * fontSize, 0);
                                break;
                            case Font.Renogare:
                                ActiveFont.Draw(message, new Vector2(X, Y), Vector2.Zero, Vector2.One * fontSize / 2, color * alpha);
                                break;
                        }
                    }
                    break;
                case Shape.Image:
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        Vector2 imagePos = gui ? 6f * (Position - level.Camera.Position) + Vector2.One : Position;
                        Image image = new Image(gui ? GFX.Gui[imagePath] : GFX.Game[imagePath]);
                        image.Position = imagePos;
                        if (scaled)
                        {
                            image.Scale = gui ? new Vector2(6f * width / image.Width, 6f * height / image.Height) : new Vector2(width / image.Width, height / image.Height);
                        }
                        image.Color = color * alpha;
                        image.Render();
                    }
                    break;
            }
        }
    }

    private void DrawEllipse(float x, float y, float rx, float ry, Color color)
    {
        Vector2 prevPoint = new Vector2(x + rx, y);
        for (int i = 1; i <= ellipseSegments; i++)
        {
            float theta = MathHelper.TwoPi * i / ellipseSegments;
            float dx = rx * (float)Math.Cos(theta);
            float dy = ry * (float)Math.Sin(theta);
            Vector2 newPoint = new Vector2(x + dx, y + dy);
            Draw.Line(prevPoint, newPoint, color);
            prevPoint = newPoint;
        }
    }
}