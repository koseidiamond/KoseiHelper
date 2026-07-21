using Microsoft.Xna.Framework;
using Monocle;
using System;
namespace Celeste.Mod.KoseiHelper.Other.Apps;


public class Button
{
    public Rectangle Bounds;
    public string Text;
    public Color FillColor = Color.LightGray;
    public Color BorderColor = Color.Black;
    public Color TextColor = Color.Gray;

    public Action OnClick;

    public Button(Rectangle bounds, string text, Action onClick)
    {
        Bounds = bounds;
        Text = text;
        OnClick = onClick;
    }

    public Button(Rectangle bounds, string text, Action onClick, Color? fillColor = null, Color? borderColor = null, Color? textColor = null, MTexture texture = null)
    {
        Bounds = bounds;
        Text = text;
        OnClick = onClick;

        FillColor = fillColor ?? Color.LightGray;
        BorderColor = borderColor ?? Color.Black;
        TextColor = textColor ?? Color.Gray;
    }

    public void Update(Vector2 offset)
    {
        Rectangle rect = new(Bounds.X + (int)offset.X, Bounds.Y + (int)offset.Y, Bounds.Width, Bounds.Height);
        Vector2 mouse = MInput.Mouse.Position;
        if (rect.Contains((int)mouse.X, (int)mouse.Y) && MInput.Mouse.PressedLeftButton)
        {
            OnClick?.Invoke();
        }
    }

    public void Render(Vector2 offset)
    {
        Rectangle rect = new(Bounds.X + (int)offset.X, Bounds.Y + (int)offset.Y, Bounds.Width, Bounds.Height);
        Vector2 mouse = MInput.Mouse.Position;
        bool hovered = rect.Contains((int)mouse.X, (int)mouse.Y);
        Color drawColor = hovered ? new Color(Math.Min(FillColor.R + 40, 255), Math.Min(FillColor.G + 40, 255), Math.Min(FillColor.B + 40, 255), FillColor.A) : FillColor;
        Draw.Rect(rect, drawColor);
        Draw.HollowRect(rect, BorderColor);
        ActiveFont.DrawOutline(Text, new Vector2(rect.Center.X, rect.Center.Y), new Vector2(0.5f), Vector2.One * 0.3f, TextColor, 2, Color.Black);
    }
}