using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

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
            if (string.IsNullOrEmpty(flag) || flagTrue)
            {
                if (!string.IsNullOrEmpty(framePath))
                {
                    var center = new Vector2(xPosition + width / 2, yPosition + height / 2);
                    texture.DrawJustified(center, new Vector2(0.5f, 0.5f));
                }
                if (string.IsNullOrEmpty(innerTexturePath))
                {
                    if (!vertical)
                    {
                        var pos = reverse
                            ? new Vector2(xPosition + width - filled, yPosition)
                            : new Vector2(xPosition, yPosition);
                        Draw.Rect(pos, filled, Collider.Height, color);
                    }
                    else
                    {
                        var pos = reverse
                            ? new Vector2(xPosition, yPosition)
                            : new Vector2(xPosition, yPosition + height - filled);
                        Draw.Rect(pos, Collider.Width, filled, color);
                    }

                    if (outline)
                    {
                        Draw.HollowRect(
                            new Vector2(xPosition, yPosition), Collider.Width, Collider.Height, Color.Black);
                    }
                }
                else
                {
                    if (!vertical)
                        innerTexture.GetSubtexture(0, 0, (int)filled, innerTexture.Height).Draw(reverse ?
                            new Vector2(xPosition + width - (int)filled, yPosition) : new Vector2(xPosition, yPosition));
                    else
                        innerTexture.GetSubtexture(0, 0, innerTexture.Width, (int)filled).Draw(new Vector2(xPosition, reverse ?
                            yPosition : yPosition + height - (int)filled));
                }
                base.Render();
            }
        }
    }
}
