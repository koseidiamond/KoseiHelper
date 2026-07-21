using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
namespace Celeste.Mod.KoseiHelper.Other.Apps;

public class BerryPaint : Entity
{
    private Rectangle window;
    private Rectangle canvas;
    private List<(Vector2 from, Vector2 to, Color color, int thickness)> drawnLines;

    private Vector2? lastMousePos;
    private Vector2? lineStart;

    private bool dragging;
    private Vector2 dragOffset;

    private Color currentColor;
    private float currentAlpha;
    private int lineThickness;

    private Button buttonClearDrawing;
    private Button buttonSaveDrawing;
    private Button buttonThickness;
    private Button buttonTransparency;

    private readonly List<Button> buttons = new();

    private static Color Black => Calc.HexToColor(0x0c0c0c);
    private static Color White => Calc.HexToColor(0xfbfbf7);
    private static Color DarkGray => Calc.HexToColor(0x87888f);
    private static Color LightGray => Calc.HexToColor(0xc0c7c8);
    private static Color DarkCream => Calc.HexToColor(0xd5cda3);
    private static Color Cream => Calc.HexToColor(0xe8e3ce);
    private static Color DarkRed => Calc.HexToColor(0xab0e03);
    private static Color Red => Calc.HexToColor(0xf63527);
    private static Color DarkOrange => Calc.HexToColor(0xa96814);
    private static Color Orange => Calc.HexToColor(0xf99c23);
    private static Color DarkYellow => Calc.HexToColor(0xaaa759);
    private static Color Yellow => Calc.HexToColor(0xf1ec58);
    private static Color DarkGreen => Calc.HexToColor(0x0ab20a);
    private static Color Green => Calc.HexToColor(0x2bf72b);
    private static Color DarkCyan => Calc.HexToColor(0x5aa6ab);
    private static Color Cyan => Calc.HexToColor(0x27effb);
    private static Color DarkBlue => Calc.HexToColor(0x0b188f);
    private static Color Blue => Calc.HexToColor(0x1f34f6);
    private static Color DarkMagenta => Calc.HexToColor(0xa958a2);
    private static Color Magenta => Calc.HexToColor(0xf940ea);
    private static Color DarkBrown => Calc.HexToColor(0x896642);
    private static Color Brown => Calc.HexToColor(0xe8ae73);

    private enum DrawMode
    {
        Free,
        Line,
        Circle,
        Rectangle
    }
    private DrawMode currentMode;
    private bool noCanvas;

    public BerryPaint(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        // TODO: button for closing. polish saving files. order buttons. resizing. and maybe more tools like ellipses or text
        Tag = TagsExt.SubHUD;
        AddTag(Tags.Persistent);
        AddTag(Tags.Global);
        window = new Rectangle(50, 50, 600, 400);
        noCanvas = data.Bool("noCanvas", false); // todo
        canvas = new Rectangle(window.X, window.Y + 120, window.Width, window.Height - 120);
        drawnLines = new List<(Vector2, Vector2, Color, int)>();

        currentColor = Black;
        currentAlpha = 1f;
        lineThickness = 2;
        currentMode = DrawMode.Free;

        const int buttonOffset = 5; // margin between buttons and the border
        const int buttonSpacing = 65; // space between the position of each button
        const int buttonWidth = 65;
        const int buttonHeight = 20;

        buttonClearDrawing = new Button(new Rectangle(buttonOffset, buttonOffset, buttonWidth, buttonHeight), "Clear", () =>
            drawnLines.Clear());

        buttonSaveDrawing = new Button(new Rectangle(buttonOffset + buttonSpacing, buttonOffset, buttonWidth, buttonHeight), "Save", SaveCanvasAsPng);

        buttonThickness = new Button(new Rectangle(buttonOffset + buttonSpacing * 2, buttonOffset, buttonWidth, buttonHeight), "Size: 2", () =>
        {
            lineThickness = lineThickness % 8 + 1; buttonThickness.Text = $"Size {lineThickness}";
        });

        buttonTransparency = new Button(new Rectangle(buttonOffset + buttonSpacing * 3, buttonOffset, buttonWidth, buttonHeight), "100%", () =>
        {
            currentAlpha += 0.1f;
            if (currentAlpha > 1f)
                currentAlpha = 0.1f;
            buttonTransparency.Text = $"{(int)(currentAlpha * 100)}%";
        });

        buttons.Add(buttonClearDrawing);
        buttons.Add(buttonSaveDrawing);
        buttons.Add(buttonThickness);
        buttons.Add(buttonTransparency);

        Color[] palette = {
            Black, DarkGray, DarkCream, DarkRed, DarkOrange, DarkYellow, DarkGreen, DarkCyan, DarkBlue, DarkMagenta, DarkBrown,
            White, LightGray, Cream, Red, Orange, Yellow, Green, Cyan, Blue, Magenta, Brown };

        // Add color buttons
        for (int i = 0; i < palette.Length; i++)
        {
            Color c = palette[i];
            Button button = new Button(new Rectangle(), "", () => currentColor = c);
            button.FillColor = c;
            buttons.Add(button);
        }

        // Add tool buttons
        buttons.Add(new Button(new Rectangle(), "Pencil", () => currentMode = DrawMode.Free));
        buttons.Add(new Button(new Rectangle(), "Line", () => currentMode = DrawMode.Line));
        buttons.Add(new Button(new Rectangle(), "Circle", () => currentMode = DrawMode.Circle));
        buttons.Add(new Button(new Rectangle(), "Rect", () => currentMode = DrawMode.Rectangle));
    }

    public override void Update()
    {
        base.Update();
        Dragging();
        foreach (Button button in buttons)
            button.Update(new Vector2(window.X, window.Y + 20));
        Drawing();
    }

    public override void Render()
    {
        base.Render();

        DrawWindow();
        DrawCanvas();
        DrawShapePreview();
        DrawButtons();
    }

    private void Dragging()
    {
        Vector2 mouse = MInput.Mouse.Position;
        Rectangle titleBar = new(window.X, window.Y, window.Width, 20);
        if (MInput.Mouse.PressedLeftButton && titleBar.Contains((int)mouse.X, (int)mouse.Y))
        {
            dragging = true;
            dragOffset = mouse - new Vector2(window.X, window.Y);
        }

        if (!MInput.Mouse.CheckLeftButton)
            dragging = false;

        if (dragging)
        {
            Vector2 newPosition = mouse - dragOffset;
            window.Location = new Point((int)newPosition.X, (int)newPosition.Y);
            canvas.Location = new Point(window.X, window.Y + 120);
        }
    }

    private void Drawing()
    {
        if (dragging)
        {
            lastMousePos = null;
            lineStart = null;
            return;
        }

        Vector2 mousePos = MInput.Mouse.Position;
        Vector2 canvasOffset = new(canvas.X, canvas.Y);
        Vector2 canvasRelative = mousePos - canvasOffset;

        if (!canvas.Contains((int)mousePos.X, (int)mousePos.Y))
        {
            lastMousePos = null;
            lineStart = null;
            return;
        }

        if (MInput.Mouse.PressedLeftButton)
        {
            if (currentMode == DrawMode.Free)
                lastMousePos = canvasRelative;
            else
                lineStart = canvasRelative;
        }

        if (MInput.Mouse.CheckLeftButton)
        {
            if (currentMode == DrawMode.Free)
            {
                if (lastMousePos.HasValue)
                    drawnLines.Add((lastMousePos.Value, canvasRelative, currentColor * currentAlpha, lineThickness));
                lastMousePos = canvasRelative;
            }
        }

        if (MInput.Mouse.ReleasedLeftButton)
        {
            if (lineStart.HasValue)
            {
                switch (currentMode)
                {
                    case DrawMode.Line:
                        drawnLines.Add((lineStart.Value, canvasRelative, currentColor * currentAlpha, lineThickness));
                        break;

                    case DrawMode.Circle:
                        AddCircle(lineStart.Value, canvasRelative, currentColor * currentAlpha);
                        break;

                    case DrawMode.Rectangle:
                        AddRectangle(lineStart.Value, canvasRelative, currentColor * currentAlpha);
                        break;
                }
            }
            lineStart = null;
            lastMousePos = null;
        }
    }

    private void DrawWindow()
    {
        // todo
        if (noCanvas)
            Draw.Rect(window.X, window.Y, window.Width, 120, LightGray);
        else
            Draw.Rect(window, LightGray);
        Draw.Rect(window.X, window.Y, window.Width, 20, DarkBlue);
        Draw.HollowRect(window, Black);
        ActiveFont.Draw("Berry Paint", new Vector2(window.X + 6f, window.Y - 1f), Vector2.Zero, Vector2.One * 0.35f, White);
    }



    private void DrawCanvas()
    {
        if (!noCanvas)
        {
            Draw.Rect(canvas, White);
            Draw.HollowRect(canvas, Black);
        }

        GraphicsDevice device = Draw.SpriteBatch.GraphicsDevice;
        Rectangle oldScissor = device.ScissorRectangle;
        Draw.SpriteBatch.End();

        device.ScissorRectangle = canvas;

        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, new RasterizerState { ScissorTestEnable = true });

        Vector2 offset = new(canvas.X, canvas.Y);

        foreach (var line in drawnLines)
        {
            Draw.Line(line.from + offset, line.to + offset, line.color, line.thickness);
        }

        Draw.SpriteBatch.End();
        device.ScissorRectangle = oldScissor;
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
    }

    private void DrawShapePreview()
    {
        if (!lineStart.HasValue || currentMode == DrawMode.Free || !MInput.Mouse.CheckLeftButton)
            return;

        GraphicsDevice device = Draw.SpriteBatch.GraphicsDevice;
        Rectangle oldScissor = device.ScissorRectangle;
        Draw.SpriteBatch.End();
        device.ScissorRectangle = canvas;
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, new RasterizerState { ScissorTestEnable = true });

        Vector2 start = lineStart.Value + new Vector2(canvas.X, canvas.Y);
        Vector2 end = MInput.Mouse.Position;

        Color previewColor = currentColor * currentAlpha;

        switch (currentMode)
        {
            case DrawMode.Line:
                Draw.Line(start, end, previewColor, lineThickness);
                break;

            case DrawMode.Circle:
                float radius = Vector2.Distance(start, end);
                const int segments = 32;
                Vector2 previous = start + new Vector2(radius, 0);
                for (int i = 1; i <= segments; i++)
                {
                    float angle = MathHelper.TwoPi * i / segments;
                    Vector2 next = start + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                    Draw.Line(previous, next, previewColor, lineThickness);
                    previous = next;
                }
                break;

            case DrawMode.Rectangle:
                Vector2 topLeft = new(Math.Min(start.X, end.X), Math.Min(start.Y, end.Y));

                Vector2 bottomRight = new(Math.Max(start.X, end.X), Math.Max(start.Y, end.Y));
                Vector2 topRight = new(bottomRight.X, topLeft.Y);
                Vector2 bottomLeft = new(topLeft.X, bottomRight.Y);
                Draw.Line(topLeft, topRight, previewColor, lineThickness);
                Draw.Line(topRight, bottomRight, previewColor, lineThickness);
                Draw.Line(bottomRight, bottomLeft, previewColor, lineThickness);
                Draw.Line(bottomLeft, topLeft, previewColor, lineThickness);
                break;
        }
        Draw.SpriteBatch.End();
        device.ScissorRectangle = oldScissor;
        Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
    }


    private void DrawButtons()
    {
        Vector2 offset = new(window.X, window.Y + 20);
        int x = 5;
        int y = 5;
        buttonClearDrawing.Bounds = new Rectangle(x, y, 60, 20);
        x += 65;
        buttonSaveDrawing.Bounds = new Rectangle(x, y, 60, 20);
        x += 65;
        buttonThickness.Bounds = new Rectangle(x, y, 70, 20);
        x += 75;
        buttonTransparency.Bounds = new Rectangle(x, y, 70, 20);

        foreach (Button button in buttons)
            button.Render(offset);

        const int paletteStart = 4;
        const int paletteSize = 16;
        const int spacing = 2;
        x = 5;
        y = 30;

        for (int i = paletteStart; i < paletteStart + 22; i++)
        {
            buttons[i].Bounds = new Rectangle(x, y, paletteSize, paletteSize);

            x += paletteSize + spacing;

            if ((i - paletteStart + 1) % 11 == 0)
            {
                x = 5;
                y += paletteSize + spacing;
            }
        }

        int toolStart = paletteStart + 22;

        x = 210;
        y = 30;

        buttons[toolStart + 0].Bounds = new Rectangle(x, y, 70, 18);
        buttons[toolStart + 1].Bounds = new Rectangle(x, y + 20, 70, 18);
        buttons[toolStart + 2].Bounds = new Rectangle(x, y + 40, 70, 18);
        buttons[toolStart + 3].Bounds = new Rectangle(x, y + 60, 70, 18);

        for (int i = toolStart; i < buttons.Count; i++)
            buttons[i].Render(offset);
    }


    private void AddCircle(Vector2 center, Vector2 edge, Color color)
    {
        float radius = Vector2.Distance(center, edge);
        const int segments = 32;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = MathHelper.TwoPi * i / segments;
            float angle2 = MathHelper.TwoPi * (i + 1) / segments;
            Vector2 point1 = center + new Vector2((float)Math.Cos(angle1), (float)Math.Sin(angle1)) * radius;
            Vector2 point2 = center + new Vector2((float)Math.Cos(angle2), (float)Math.Sin(angle2)) * radius;
            drawnLines.Add((point1, point2, color, lineThickness));
        }
    }

    private void AddRectangle(Vector2 from, Vector2 to, Color color)
    {
        Vector2 topLeft = new(Math.Min(from.X, to.X), Math.Min(from.Y, to.Y));
        Vector2 bottomRight = new(Math.Max(from.X, to.X), Math.Max(from.Y, to.Y));
        Vector2 topRight = new(bottomRight.X, topLeft.Y);
        Vector2 bottomLeft = new(topLeft.X, bottomRight.Y);

        drawnLines.Add((topLeft, topRight, color, lineThickness));
        drawnLines.Add((topRight, bottomRight, color, lineThickness));
        drawnLines.Add((bottomRight, bottomLeft, color, lineThickness));
        drawnLines.Add((bottomLeft, topLeft, color, lineThickness));
    }

    private void SaveCanvasAsPng()
    {
        if (canvas.Width <= 0 || canvas.Height <= 0)
            return;

        GraphicsDevice device = Draw.SpriteBatch.GraphicsDevice;
        RenderTargetBinding[] previousTargets = device.GetRenderTargets();
        using RenderTarget2D renderTarget = new RenderTarget2D(device, canvas.Width, canvas.Height, false, SurfaceFormat.Color, DepthFormat.None);

        try
        {
            device.SetRenderTarget(renderTarget);
            device.Clear(White);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            foreach (var line in drawnLines)
            {
                Draw.Line(line.from, line.to, line.color, line.thickness);
            }

            Draw.SpriteBatch.End();
            device.SetRenderTargets(previousTargets);
            Directory.CreateDirectory("KoseiHelper");
            string file = Path.Combine("Saves/KoseiHelper", $"berryPaint_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
            using FileStream stream = File.Create(file);
            renderTarget.SaveAsPng(stream, canvas.Width, canvas.Height);
            Logger.Log("KoseiHelper", $"Drawing saved to: {file}");
            Audio.Play("event:/ui/main/savefile_rename_start");
        }
        catch (Exception e)
        {
            device.SetRenderTargets(previousTargets);
            Logger.Error("KoseiHelper", $"Failed to save Berry Paint:\n{e}");
        }
    }
}