using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomBirdTutorialGui")]
public class CustomBirdTutorialGui : BirdTutorialGui
{
    public object CustomInfo;
    public Color titleColor = Color.White;
    public Color secondaryTextColor = Color.White; // shadows below text
    public Color directionColor = Color.White;
    public Color buttonColor = Color.White;
    public Color imageColor = Color.White;
    public float sizeMultiplier = 1f;
    public bool renderTriangleBelow = true;
    public bool rectangleShape = true;
    public CustomBirdTutorialGui(Entity entity, Vector2 position, object info, params object[] controls) : base(entity, position, info, controls)
    {
        CustomInfo = info;
    }

    public override void Render()
    {
        Level level = SceneAs<Level>();
        if (level == null || level.FrozenOrPaused || level.RetryPlayerCorpse != null || Scale <= 0f)
            return;

        float border = 6f;
        Vector2 vector = Entity.Position + Position - level.Camera.Position.Floor();
        if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
            vector.X = 320f - vector.X;

        vector.X *= 6f;
        vector.Y *= 6f;

        float lineHeight = ActiveFont.LineHeight;
        float width = (Math.Max(controlsWidth, infoWidth) + 64f) * Scale * sizeMultiplier;
        float height = (infoHeight + lineHeight + 32f) * sizeMultiplier;
        float x = vector.X - width / 2f;
        float y = vector.Y - height - 32f * sizeMultiplier;

        if (rectangleShape)
        {
            //Draw.Rect(x - border, y - border, width + 12f, height + 12f, lineColor); // border
            //Draw.Rect(x, y, width, height, bgColor); // bg

            Draw.Rect(x, y, width, height, bgColor);
            Draw.Rect(x - border, y - border, width + border * 2f, border, lineColor); // top
            Draw.Rect(x - border, y, border, height, lineColor); // left
            Draw.Rect(x + width, y, border, height, lineColor); // right
            //Draw.Rect(x - border, y + height, width + border * 2f, border, lineColor); // bottom (full line, unused because triangle)

            if (renderTriangleBelow)
            {
                float triangleWidth = 73f * Scale * sizeMultiplier;
                float triangleLeft = vector.X - triangleWidth / 2f;
                float triangleRight = vector.X + triangleWidth / 2f;
                Draw.Rect(x - border, y + height, triangleLeft - (x - border * 2f), border, lineColor); // bottom left
                Draw.Rect(triangleRight - border, y + height, (x + width + border * 2f) - triangleRight, border, lineColor); // bottom right

                for (int i = 0; i <= border * border; i++)
                {
                    float tailWidth = (border * border * 2f - i * 2f) * Scale * sizeMultiplier;
                    float tailLeft = vector.X - tailWidth / 2f;
                    float innerWidth = tailWidth - border * 2f;

                    if (innerWidth > 0f)
                    {
                        float innerLeft = vector.X - innerWidth / 2f;
                        Draw.Rect(tailLeft, y + height + i, innerLeft - tailLeft, 1f, lineColor); // left
                        Draw.Rect(innerLeft + innerWidth, y + height + i, tailLeft + tailWidth - (innerLeft + innerWidth), 1f, lineColor); // right
                        Draw.Rect(innerLeft, y + height + i, innerWidth, 1f, bgColor); // inner
                    }
                    else
                    {
                        Draw.Rect(tailLeft, y + height + i, tailWidth, 1f, lineColor);
                    }
                }
            }
        }
        else
        {
            DrawEllipse(x - border + width / 2f, y + border + height / 2f, width / 2f + 32f, height / 2f + 16f, bgColor);
            DrawHollowEllipse(x - border + width / 2f, y + border + height / 2f, width / 2f + 32f, height / 2f + 16f, lineColor, 8f);
            if (renderTriangleBelow) // it looks copypasted from above, but it's different
            {
                for (int i = 0; i <= border * border; i++)
                {
                    float tailWidth = (border * border * 2 - i * 2f) * Scale * sizeMultiplier;
                    float tailLeft = vector.X - tailWidth / 2f;
                    float innerWidth = tailWidth - border * 2f;
                    float ellipseOffset = border * 4f;
                    if (innerWidth > 0f)
                    {
                        float innerLeft = vector.X - innerWidth / 2f;
                        Draw.Rect(tailLeft, y + height + i + ellipseOffset, innerLeft - tailLeft, 1f, lineColor); // left
                        Draw.Rect(innerLeft + innerWidth, y + height + i + ellipseOffset, tailLeft + tailWidth - (innerLeft + innerWidth), 1f, lineColor); // right
                        Draw.Rect(innerLeft, y + height + i + ellipseOffset, innerWidth, 1f, bgColor); // inner
                    }
                    else
                    {
                        Draw.Rect(tailLeft, y + height + i + ellipseOffset, tailWidth, 1f, lineColor);
                    }
                }
            }
        }

        if (width <= 3f)
            return;
        Vector2 textPosition = new Vector2(vector.X, y + 16f * sizeMultiplier);

        if (CustomInfo is string infoText)
            ActiveFont.Draw(infoText, textPosition, new Vector2(0.5f, 0f), new Vector2(Scale, 1f) * sizeMultiplier, titleColor);
        else if (CustomInfo is MTexture infoTexture)
            infoTexture.DrawJustified(textPosition, new Vector2(0.5f, 0f), Color.White, new Vector2(Scale, 1f) * sizeMultiplier);

        textPosition.Y += (infoHeight + lineHeight * 0.5f) * sizeMultiplier;
        Vector2 controlPosition = new Vector2(-controlsWidth / 2f, 0f);

        foreach (object control in controls)
        {
            if (control is BirdTutorialGui.ButtonPrompt prompt) // it has to be done differently to vanilla's but if it works it works
            {
                VirtualButton button = BirdTutorialGui.ButtonPromptToVirtualButton(prompt);
                MTexture texture = Input.GuiButton(button, "controls/keyboard/oemquestion");
                controlPosition.X += buttonPadding;
                texture.Draw(textPosition, new Vector2(-controlPosition.X, texture.Height / 2f), buttonColor, new Vector2(Scale, 1f) * sizeMultiplier);
                controlPosition.X += texture.Width + buttonPadding;
            }
            else if (control is Vector2 direction)
            {
                if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
                    direction.X = -direction.X;
                MTexture texture = Input.GuiDirection(direction);
                controlPosition.X += buttonPadding;
                texture.Draw(textPosition, new Vector2(-controlPosition.X, texture.Height / 2f), directionColor, new Vector2(Scale, 1f) * sizeMultiplier);
                controlPosition.X += texture.Width + buttonPadding;
            }
            else if (control is string text)
            {
                float textWidth = ActiveFont.Measure(text).X;
                ActiveFont.Draw(text, textPosition + new Vector2(1f, 2f), new Vector2(-controlPosition.X / textWidth, 0.5f), new Vector2(Scale, 1f) * sizeMultiplier, secondaryTextColor);
                ActiveFont.Draw(text, textPosition + new Vector2(1f, -2f), new Vector2(-controlPosition.X / textWidth, 0.5f), new Vector2(Scale, 1f) * sizeMultiplier, textColor);
                controlPosition.X += textWidth + 1f;
            }
            else if (control is MTexture texture)
            {
                texture.Draw(textPosition, new Vector2(-controlPosition.X, texture.Height / 2f), imageColor, new Vector2(Scale, 1f) * sizeMultiplier);
                controlPosition.X += texture.Width;
            }
        }
    }

    private void DrawEllipse(float x, float y, float rx, float ry, Color color)
    {
        int height = (int)Math.Ceiling(ry);

        for (int iy = -height; iy <= height; iy++)
        {
            float normalizedY = iy / ry;
            if (Math.Abs(normalizedY) > 1f)
                continue;
            float halfWidth = rx * (float)Math.Sqrt(1f - normalizedY * normalizedY);
            Draw.Rect(x - halfWidth, y + iy, halfWidth * 2f, 1f, color);
        }
    }

    private void DrawHollowEllipse(float x, float y, float rx, float ry, Color color, float thickness = 1f)
    {
        const int ellipseSegments = 40;
        for (float t = 0f; t < thickness; t += 1f)
        {
            float offset = t - (thickness - 1f) / 2f;
            float currentRx = rx + offset;
            float currentRy = ry + offset;
            Vector2 prevPoint = new Vector2(x + currentRx, y);
            for (int i = 1; i <= ellipseSegments; i++)
            {
                float theta = MathHelper.TwoPi * i / ellipseSegments;
                float dx = currentRx * (float)Math.Cos(theta);
                float dy = currentRy * (float)Math.Sin(theta);
                Vector2 newPoint = new Vector2(x + dx, y + dy);
                Draw.Line(prevPoint, newPoint, color);
                prevPoint = newPoint;
            }
        }
    }
}