using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/CustomHahaha")]
    public class CustomHahaha : Entity
    {
        private class Ha
        {
            public Sprite Sprite;
            public float Percent;
            public float Duration;

            public Ha(string spritePath)
            {
                Sprite = new Sprite(GFX.Game, spritePath);
                Sprite.Add("normal", "ha", 0.15f, 0, 1, 0, 1, 0, 1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
                Sprite.Play("normal");
                Sprite.JustifyOrigin(0.5f, 0.5f);
                Duration = (float)Sprite.CurrentAnimationTotalFrames * 0.15f;
            }
        }

        private bool enabled;
        private string flag;
        private float timer;
        private int counter;
        private List<Ha> has = new List<Ha>();
        private bool autoTriggerLaughSfx = true;
        private string spritePath, audioPath;
        public float timeForHahaha, timeForHa, timeToSfx;
        public bool synchronizedSfx;
        public bool left;

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                if (!enabled && value)
                {
                    timer = 0f;
                    counter = 0;
                }
                enabled = value;
            }
        }

        public CustomHahaha(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            spritePath = data.Attr("sprite", "characters/oldlady/");
            audioPath = data.Attr("sound", "event:/char/granny/laugh_oneha");
            timeForHahaha = data.Float("timeForHahaha", 1.5f);
            timeForHa = data.Float("timeForHa", 0.6f);
            timeToSfx = data.Float("timeToSfx", 0.4f);
            left = data.Bool("left", false);
            synchronizedSfx = data.Bool("synchronizedSfx", false);
            base.Depth = data.Int("depth", -10001);
            this.flag = data.Attr("flag", "");
            if (data.Bool("synchronizedSfx", false))
            {
                autoTriggerLaughSfx = true;
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!string.IsNullOrEmpty(flag) && !(base.Scene as Level).Session.GetFlag(flag))
            {
                Enabled = false;
            }
        }

        public override void Update()
        {
            if (Enabled)
            {
                timer -= Engine.DeltaTime;
                if (timer <= 0f)
                {
                    if (synchronizedSfx)
                        Audio.Play(audioPath, Center);
                    has.Add(new Ha(spritePath));
                    counter++;
                    if (counter >= 3)
                    {
                        counter = 0;
                        timer = timeForHahaha;
                    }
                    else
                    {
                        timer = timeForHa;
                    }
                }
                if (autoTriggerLaughSfx && base.Scene.OnInterval(timeToSfx) && !synchronizedSfx)
                {
                    Audio.Play(audioPath, Center);
                }
            }

            for (int i = has.Count - 1; i >= 0; i--)
            {
                if (has[i].Percent > 1f)
                {
                    has.RemoveAt(i);
                }
                else
                {
                    has[i].Sprite.Update();
                    has[i].Percent += Engine.DeltaTime / has[i].Duration;
                }
            }

            if (!string.IsNullOrEmpty(flag) && (base.Scene as Level).Session.GetFlag(flag))
                Enabled = true;
            if (string.IsNullOrEmpty(flag) || !(base.Scene as Level).Session.GetFlag(flag))
                Enabled = false;

            base.Update();
        }

        public override void Render()
        {
            foreach (Ha ha in has)
            {
                if (!left)
                ha.Sprite.Position = Position + new Vector2(ha.Percent * 60f, -10f + (float)(0.0 - Math.Sin(ha.Percent * 13f)) * 4f + ha.Percent * -16f);
                else
                    ha.Sprite.Position = Position + new Vector2(ha.Percent * -60f, -10f + (float)(0.0 - Math.Sin(ha.Percent * 13f)) * 4f + ha.Percent * -16f);
                ha.Sprite.Render();
            }
        }
    }
}