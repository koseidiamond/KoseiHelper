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
        private string ifSet;
        private float timer;
        private int counter;
        private List<Ha> has = new List<Ha>();
        private bool autoTriggerLaughSfx = true;
        private Vector2 autoTriggerLaughOrigin;
        private string spritePath, audioPath;

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
            base.Depth = -10001;
            this.ifSet = data.Attr("ifset", "");
            if (data.Bool("triggerLaughSfx", false))
            {
                autoTriggerLaughSfx = true;
                autoTriggerLaughOrigin = offset;
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (!string.IsNullOrEmpty(ifSet) && !(base.Scene as Level).Session.GetFlag(ifSet))
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
                    has.Add(new Ha(spritePath));
                    counter++;
                    if (counter >= 3)
                    {
                        counter = 0;
                        timer = 1.5f;
                    }
                    else
                    {
                        timer = 0.6f;
                    }
                }
                if (autoTriggerLaughSfx && base.Scene.OnInterval(0.4f))
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

            if (!Enabled && !string.IsNullOrEmpty(ifSet) && (base.Scene as Level).Session.GetFlag(ifSet))
            {
                Enabled = true;
            }

            base.Update();
        }

        public override void Render()
        {
            foreach (Ha ha in has)
            {
                ha.Sprite.Position = Position + new Vector2(ha.Percent * 60f, -10f + (float)(0.0 - Math.Sin(ha.Percent * 13f)) * 4f + ha.Percent * -16f);
                ha.Sprite.Render();
            }
        }
    }
}
