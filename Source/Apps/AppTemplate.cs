using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
namespace Celeste.Mod.KoseiHelper.Apps;

public class AppTemplate : App
{
    private bool dragging;
    private Vector2 dragOffset;
    private Button someButton;
    private readonly List<Button> buttons = new();
    private bool someSetting;

    public AppTemplate(EntityData data, Vector2 offset) : base(data, data.Position + offset)
    {
        window = new Rectangle((int)MInput.Mouse.Position.X - 4, (int)MInput.Mouse.Position.Y - 4, minWidth, minHeight);
        someSetting = data.Bool("someSetting", false);

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // General buttons
        someButton = new Button(Rectangle.Empty, "TheName", () =>
        {
            Audio.Play("event:/none");
        });
        buttons.Add(someButton);

        // Bar buttons
        buttons.Add(buttonMinSize);
        buttons.Add(buttonMaxSize);
        buttons.Add(buttonClose);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
    }

    public override void Update()
    {
        base.Update();
        Dragging();
        foreach (Button button in buttons)
            button.Update(new Vector2(window.X, window.Y + 20));
    }

    public override void Render()
    {
        base.Render();

        DrawWindow();
        DrawButtons();
    }

    public override void Maximize()
    {
        base.Maximize();
    }

    public override void Minimize()
    {
        base.Minimize();
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

            if (button.IsCircle)
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
        }
    }


    private void DrawWindow()
    {
        Draw.Rect(window.X, window.Y, window.Width, window.Height, someSetting ? LightGray : White);
        Draw.Rect(window.X, window.Y, window.Width, 20, DarkBlue);
        Draw.HollowRect(window, Black);
        ActiveFont.Draw("Template App", new Vector2(window.X + 6f, window.Y - 1f), Vector2.Zero, Vector2.One * 0.35f, White);
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
        someButton.Bounds = new Rectangle(buttonOffset, buttonOffset, buttonWidth, buttonHeight);
        buttonMinSize.Bounds = new Rectangle(window.Width - smallButtonSize * 4 - smallButtonSize / 4, -18, smallButtonSize, smallButtonSize);
        buttonMaxSize.Bounds = new Rectangle(window.Width - smallButtonSize * 2 - 12, -18, smallButtonSize, smallButtonSize);
        buttonClose.Bounds = new Rectangle(window.Width - smallButtonSize - smallButtonSize / 4, -18, smallButtonSize, smallButtonSize);
    }
}