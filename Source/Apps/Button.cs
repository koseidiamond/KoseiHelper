using Microsoft.Xna.Framework;
using Monocle;
using System;
namespace Celeste.Mod.KoseiHelper.Apps;

public class Button
{
    public Rectangle Bounds;
    public string Text;
    public MTexture Texture;
    public Color FillColor = Color.LightGray;
    public Color BorderColor = Color.Black;
    public Color TextColor = Color.Gray;
    public Action OnClick;
    public bool RenderBg;
    public float TextureScale;
    public Color TextureColor;
    public bool IsCircle;

    public Button(Rectangle bounds, string text, Action onClick, Color? fillColor = null, Color? borderColor = null, Color? textColor = null, MTexture texture = null,
        bool renderBg = true, float textureSize = 1f, Color? textureColor = null, bool isCircle = false)
    {
        Bounds = bounds;
        Text = text;
        OnClick = onClick;
        FillColor = fillColor ?? Color.LightGray;
        BorderColor = borderColor ?? Color.Black;
        TextColor = textColor ?? Color.Gray;
        Texture = texture;
        RenderBg = renderBg;
        TextureScale = textureSize;
        TextureColor = textureColor ?? Color.White;
        IsCircle = isCircle;
    }

    public void Update(Vector2 offset)
    {
        Rectangle rect = new(Bounds.X + (int)offset.X, Bounds.Y + (int)offset.Y, Bounds.Width, Bounds.Height);
        Vector2 mouse = MInput.Mouse.Position;

        if (IsCircle)
        {
            Vector2 center = new(rect.Center.X, rect.Center.Y);
            float radius = rect.Width / 2f;
            if (Vector2.DistanceSquared(mouse, center) <= radius * radius && MInput.Mouse.PressedLeftButton)
                OnClick?.Invoke();
        }
        else if (rect.Contains((int)mouse.X, (int)mouse.Y) && MInput.Mouse.PressedLeftButton)
        {
            OnClick?.Invoke();
        }
    }

    public void Render(Vector2 offset)
    {
        Rectangle rect = new(Bounds.X + (int)offset.X, Bounds.Y + (int)offset.Y, Bounds.Width, Bounds.Height);
        Vector2 mouse = MInput.Mouse.Position;
        bool hovered;
        if (IsCircle)
        {
            Vector2 center = new(rect.Center.X, rect.Center.Y);
            float radius = rect.Width / 2f;
            hovered = Vector2.DistanceSquared(mouse, center) <= radius * radius;
        }
        else
        {
            hovered = rect.Contains((int)mouse.X, (int)mouse.Y);
        }
        Color drawColor = hovered ?
            new Color(Math.Min(FillColor.R + 40, 255), Math.Min(FillColor.G + 40, 255), Math.Min(FillColor.B + 40, 255), FillColor.A) : FillColor;
        if (RenderBg)
        {
            if (IsCircle)
            {
                Draw.Circle(new Vector2(rect.Center.X, rect.Center.Y), rect.Width / 2f, BorderColor, rect.Width / 2);
            }
            else
            {
                Draw.Rect(rect, drawColor);
                Draw.HollowRect(rect, BorderColor);
            }
        }
        if (Texture != null)
        {
            Color mTextureColor = hovered ?
                new Color(Math.Min(TextureColor.R + 40, 255), Math.Min(TextureColor.G + 40, 255), Math.Min(TextureColor.B + 40, 255), TextureColor.A) : TextureColor;
            Texture?.DrawCentered(new Vector2(rect.Center.X, rect.Center.Y), mTextureColor, TextureScale);
        }
        ActiveFont.DrawOutline(Text, new Vector2(rect.Center.X, rect.Center.Y), new Vector2(0.5f), Vector2.One * 0.3f, TextColor, 2, Color.Black);
    }
}