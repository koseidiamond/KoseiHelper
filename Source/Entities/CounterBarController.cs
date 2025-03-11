using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/CounterBarController")]
    public class CounterBarController : Entity
    {

        private Color color;
        private int xPosition, yPosition, width, height, maxCounterValue;
        private string countName, framePath, innerTexturePath;
        private int currentCountValue;
        private float filled;
        private bool vertical, canOverflow, slider;
        private float currentSliderValue, maxSliderValue;
        private MTexture texture, innerTexture;
        private bool persistent, flagTrue, outline;
        private string flag;
        private bool reverse;

        public CounterBarController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Tag = TagsExt.SubHUD;
            persistent = data.Bool("persistent", false);
            if (persistent)
                Tag = Tags.Persistent;
            flag = data.Attr("flag", "");
            color = data.HexColor("color", Color.LimeGreen);
            xPosition = data.Int("xPosition", 80);
            yPosition = data.Int("yPosition", 1000);
            width = data.Int("barWidth", 1760);
            height = data.Int("barHeight", 16);
            maxCounterValue = data.Int("maxValue", 10);
            maxSliderValue = data.Float("maxValue", 10);
            currentCountValue = data.Int("initialValue", 1);
            currentSliderValue = data.Float("initialValue", 1f);
            countName = data.Attr("countName", "koseiHelper_counterBar");
            framePath = data.Attr("framePath", "");
            innerTexturePath = data.Attr("innerTexturePath", "");
            vertical = data.Bool("vertical", false);
            outline = data.Bool("outline", true);
            canOverflow = data.Bool("canOverflow", false);
            slider = data.Bool("slider", false);

            Collider = new Hitbox(width, height);
            texture = GFX.Gui[framePath];
            innerTexture = GFX.Gui[innerTexturePath];
            reverse = data.Bool("reverse", false);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            Level level = SceneAs<Level>();
            flagTrue = level.Session.GetFlag(flag);
            if (!slider)
                currentCountValue = level.Session.GetCounter(countName);
            else
                currentSliderValue = level.Session.GetSlider(countName);
            if (!canOverflow)
            {
                if (!slider)
                    currentCountValue = Math.Min(currentCountValue, maxCounterValue);
                else
                    currentSliderValue = Math.Min(currentSliderValue, maxSliderValue);
            }
            if (!vertical)
            {
                if (!slider)
                    filled = Math.Max(((float)currentCountValue / maxCounterValue) * width, 0);
                else
                    filled = Math.Max(((float)currentSliderValue / maxSliderValue) * width, 0);
            }
            else
            {
                if (!slider)
                    filled = Math.Max(((float)currentCountValue / maxCounterValue) * height, 0);
                else
                    filled = Math.Max(((float)currentSliderValue / maxSliderValue) * height, 0);
            }
            base.Update();
        }

        public override void Render()
        {
            if (string.IsNullOrEmpty(flag) || (!string.IsNullOrEmpty(flag) && flagTrue))
            {
                if (string.IsNullOrEmpty(innerTexturePath))
                {
                    if (!string.IsNullOrEmpty(framePath))
                    {
                        if (!vertical)
                        {
                            texture.DrawJustified(new Vector2(xPosition + width / 2, yPosition + height / 2), new Vector2(0.5f, 0.5f));
                        }
                        else
                        {
                            texture.DrawJustified(new Vector2(xPosition + width / 2, yPosition + height / 2), new Vector2(0.5f, 0.5f));
                        }
                    }

                    if (!vertical)
                    {
                        if (reverse)
                        {
                            Draw.Rect(new Vector2(xPosition + width - filled, yPosition), filled, Collider.Height, color);
                        }
                        else
                        {
                            Draw.Rect(new Vector2(xPosition, yPosition), filled, Collider.Height, color);
                        }

                        if (outline)
                            Draw.HollowRect(new Vector2(xPosition, yPosition), Collider.Width, Collider.Height, Color.Black);
                    }
                    else
                    {
                        if (reverse)
                        {
                            Draw.Rect(new Vector2(xPosition, yPosition), Collider.Width, filled, color);
                        }
                        else
                        {
                            Draw.Rect(new Vector2(xPosition, yPosition + height - filled), Collider.Width, filled, color);
                        }
                        if (outline)
                            Draw.HollowRect(new Vector2(xPosition, yPosition), Collider.Width, Collider.Height, Color.Black);
                    }
                }
                else // Inner texture provided, render using the texture
                {
                    if (!vertical)
                    {
                        int textureWidth = (int)filled;
                        MTexture subtexture = innerTexture.GetSubtexture(0, 0, textureWidth, innerTexture.Height);
                        if (reverse)
                        {
                            subtexture.Draw(new Vector2(xPosition + width - textureWidth, yPosition));
                        }
                        else
                        {
                            subtexture.Draw(new Vector2(xPosition, yPosition));
                        }
                    }
                    else
                    {
                        int textureHeight = (int)filled;
                        MTexture subtexture = innerTexture.GetSubtexture(0, 0, innerTexture.Width, textureHeight);
                        if (reverse)
                        {
                            subtexture.Draw(new Vector2(xPosition, yPosition + height - textureHeight));
                        }
                        else
                        {
                            subtexture.Draw(new Vector2(xPosition, yPosition + height - textureHeight));
                        }
                    }
                }

                base.Render();
            }
        }

    }
}
