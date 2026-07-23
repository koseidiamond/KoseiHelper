using FrostHelper.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
namespace Celeste.Mod.KoseiHelper.Other.Apps;

public class BerryPaint : App
{
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

    private enum DrawMode
    {
        Free,
        Line,
        Circle,
        Rectangle
    }
    private DrawMode currentMode;
    private bool noCanvas;

    public BerryPaint(EntityData data, Vector2 offset) : base(data, data.Position + offset)
    {
        // TODO: polish saving files. and maybe more tools like ellipses or text. clipboard too maybe??
        // TODO: darken button color while holding click
        // TODO: fix minimize/maximize (it tries to drag which breaks stuff)
        Tag = TagsExt.SubHUD;
        AddTag(Tags.Persistent);
        AddTag(Tags.Global);
        AddTag(Tags.FrozenUpdate);
        AddTag(Tags.TransitionUpdate);

        window = new Rectangle((int)MInput.Mouse.Position.X - 4, (int)MInput.Mouse.Position.Y - 4, minWidth, minHeight);
        noCanvas = data.Bool("noCanvas", false);

        drawnLines = new List<(Vector2, Vector2, Color, int)>();

        currentColor = Black;
        currentAlpha = 1f;
        lineThickness = 2;
        currentMode = DrawMode.Free;

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // General buttons
        buttonClearDrawing = new Button(Rectangle.Empty, "Clear", () => {
        drawnLines.Clear();
            Audio.Play("event:/ui/main/savefile_delete");
        });
        buttonSaveDrawing = new Button(Rectangle.Empty, "Save", ExportCanvasAsPng);
        buttonThickness = new Button(Rectangle.Empty, "Size: 2", () =>
        {
            lineThickness = lineThickness % 8 + 1; buttonThickness.Text = $"Size: {lineThickness}";
        });
        buttonTransparency = new Button(Rectangle.Empty, "100%", () =>
        {
            if (currentAlpha >= 1f)
                currentAlpha = 0.1f;
            else
                currentAlpha += 0.1f;
            buttonTransparency.Text = $"{(int)(currentAlpha * 100)}%";
        });
        buttons.Add(buttonClearDrawing);
        buttons.Add(buttonSaveDrawing);
        buttons.Add(buttonThickness);
        buttons.Add(buttonTransparency);

        // Color buttons
        Color[] palette = { Black, DarkGray, DarkCream, DarkRed, DarkOrange, DarkYellow, DarkGreen, DarkCyan, DarkBlue, DarkMagenta, DarkBrown,
        White, LightGray, Cream, Red, Orange, Yellow, Green, Cyan, Blue, Magenta, Brown };
        foreach (Color color in palette)
        {
            Button button = new Button(Rectangle.Empty, "", () => currentColor = color) { FillColor = color };
            buttons.Add(button);
        }

        // Tool buttons
        buttons.Add(new Button(Rectangle.Empty, "Pencil", () => currentMode = DrawMode.Free));
        buttons.Add(new Button(Rectangle.Empty, "Line", () => currentMode = DrawMode.Line));
        buttons.Add(new Button(Rectangle.Empty, "Circle", () => currentMode = DrawMode.Circle));
        buttons.Add(new Button(Rectangle.Empty, "Rect", () => currentMode = DrawMode.Rectangle));

        // Bar buttons
        buttons.Add(buttonMinSize);
        buttons.Add(buttonMaxSize);
        buttons.Add(buttonClose);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        UpdateCanvas();
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

    public override void Maximize()
    {
        base.Maximize();
        UpdateCanvas();
    }

    public override void Minimize()
    {
        base.Minimize();
        UpdateCanvas();
    }

    private void Dragging()
    {
        Vector2 mouse = MInput.Mouse.Position;
        Rectangle titleBar = new(window.X, window.Y, window.Width, 20);

        bool overButton = false;
        Vector2 buttonOffset = new(window.X, window.Y + 20);

        foreach (Button button in buttons)
        {
            Rectangle rect = new(button.Bounds.X + (int)buttonOffset.X, button.Bounds.Y + (int)buttonOffset.Y, button.Bounds.Width, button.Bounds.Height);

            if (button.IsCircle) // TODO test this
            {
                Vector2 center = new(rect.Center.X, rect.Center.Y);
                float radius = rect.Width / 2f;
                if (Vector2.DistanceSquared(mouse, center) <= radius * radius)
                {
                    overButton = true;
                    break;
                }
            }
            else if (rect.Contains((int)mouse.X, (int)mouse.Y))
            {
                overButton = true;
                break;
            }
        }


        if (MInput.Mouse.PressedLeftButton && titleBar.Contains((int)mouse.X, (int)mouse.Y) && !overButton && !maximized)
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

        Color previewColor = currentColor * currentAlpha * 0.4f;

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
        LayoutButtons();
        Vector2 offset = new(window.X, window.Y + 20);

        foreach (Button button in buttons)
            button.Render(offset);
    }


    private void LayoutButtons()
    {
        
        const int paletteColumns = 11;
        const int paletteRows = 2;
        const int paletteSpacing = 2;
        int x = 5;

        // General buttons
        buttonClearDrawing.Bounds = new Rectangle(x, buttonOffset, buttonWidth, buttonHeight);
        x += buttonWidth + buttonOffset;
        buttonSaveDrawing.Bounds = new Rectangle(x, buttonOffset, buttonWidth, buttonHeight);
        x += buttonWidth + buttonOffset;
        buttonThickness.Bounds = new Rectangle(x, buttonOffset, buttonWidth, buttonHeight);
        x += buttonWidth + buttonOffset;
        buttonTransparency.Bounds = new Rectangle(x, buttonOffset, buttonWidth, buttonHeight);
        x = buttonOffset;

        // Palette
        int y = buttonHeight + buttonOffset * 2;
        for (int i = 0; i < paletteRows * paletteColumns; i++)
        {
            buttons[4 + i].Bounds = new Rectangle(x, y, smallButtonSize, smallButtonSize);
            x += smallButtonSize + paletteSpacing;
            if ((i + 1) % paletteColumns == 0)
            {
                x = buttonOffset;
                y += smallButtonSize + paletteSpacing;
            }
        }

        // Tools
        int toolStart = 4 + paletteRows * paletteColumns;
        for (int i = 0; i < 4; i++)
        {
            buttons[toolStart + i].Bounds = new Rectangle(buttonOffset + i * (buttonWidth + buttonOffset), buttonOffset + buttonHeight * 3 + paletteSpacing * paletteRows, buttonWidth, buttonHeight);
        }
        buttonMinSize.Bounds = new Rectangle(window.Width - smallButtonSize * 4 - (smallButtonSize / 4), -18, smallButtonSize, smallButtonSize);
        buttonMaxSize.Bounds = new Rectangle(window.Width - smallButtonSize * 2 -12, -18, smallButtonSize, smallButtonSize);
        buttonClose.Bounds = new Rectangle(window.Width - smallButtonSize - (smallButtonSize / 4), -18, smallButtonSize, smallButtonSize);
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

    public void UpdateCanvas()
    { // uhhh, it just works
        canvas = new Rectangle(window.X, window.Y + 120, window.Width, window.Height - 120);
    }

    private void ExportCanvasAsPng()
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
            string directory = Path.Combine("Saves", "KoseiHelper");
            Directory.CreateDirectory(directory);
            string file = Path.Combine(directory, $"berryPaint_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
            using FileStream stream = File.Create(file);
            renderTarget.SaveAsPng(stream, canvas.Width, canvas.Height);
            Logger.Log(LogLevel.Info, "KoseiHelper", $"Drawing saved to: Celeste\\{file}");
            Audio.Play("event:/ui/main/savefile_rename_start");

        }
        catch (Exception e)
        {
            device.SetRenderTargets(previousTargets);
            Logger.Error("KoseiHelper", $"Failed to save Berry Paint:\n{e}");
        }
    }
}