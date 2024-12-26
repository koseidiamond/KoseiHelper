using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/CounterBarController")]
    public class CounterBarController : Entity
    {

        private Color color;
        private int xPosition, yPosition, width, height, maxCounterValue;
        private string countName, framePath;
        private int currentCountValue;
        private float filled;
        private bool vertical, canOverflow, slider;
        private float currentSliderValue, maxSliderValue;
        private MTexture texture;

        public CounterBarController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Tag = TagsExt.SubHUD;

            color = data.HexColor("color", Color.LimeGreen);
            xPosition = data.Int("xPosition", 80);
            yPosition = data.Int("yPosition", 1000);
            width = data.Int("barWidth", 1760);
            height = data.Int("barHeight", 16);
            maxCounterValue = data.Int("maxValue", 10);
            maxSliderValue = data.Float("maxValue", 10);
            countName = data.Attr("countName", "koseiHelper_counterBar");
            framePath = data.Attr("framePath", "");
            vertical = data.Bool("vertical", false);
            canOverflow = data.Bool("canOverflow", false);
            slider = data.Bool("slider", false);

            Collider = new Hitbox(width, height);
            texture = GFX.Gui[framePath];
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            Level level = SceneAs<Level>();
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
                    filled = ((float)currentCountValue / maxCounterValue) * width;
                else
                    filled = ((float)currentSliderValue / maxSliderValue) * width;
            }
            else
            {
                if (!slider)
                    filled = ((float)currentCountValue / maxCounterValue) * height;
                else
                    filled = ((float)currentSliderValue / maxSliderValue) * height;
            }
            base.Update();
        }

        public override void Render()
        {
            if (!string.IsNullOrEmpty(framePath))
            {
                if (!vertical)
                    texture.DrawJustified(new Vector2(xPosition + width / 2, yPosition + height / 2), new Vector2(0.5f, 0.5f));
                else
                    texture.DrawJustified(new Vector2(xPosition + width / 2, yPosition + height / 2), new Vector2(0.5f, 0.5f));
            }
            if (!vertical)
                Draw.Rect(new Vector2(xPosition, yPosition), filled, Collider.Height, color);
            else
                Draw.Rect(new Vector2(xPosition, yPosition), Collider.Width, filled, color);
            base.Render();
        }
    }
}