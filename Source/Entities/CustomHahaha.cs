using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
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
            public Ha(string spritePath, string spriteFileName, Color tint, int frameNumber, bool noDisappearAnim, float spriteDelay)
            {
                Sprite = new Sprite(GFX.Game, spritePath);

                List<int> frames = new List<int>();
                for (int i = 0; i < frameNumber; i++)
                {
                    for (int j = 0; j < frameNumber; j++)
                    {
                        frames.Add(i);
                    }
                }
                if (!noDisappearAnim)
                {
                    for (int i = frameNumber; i < frameNumber + 8; i++)
                    {
                        frames.Add(i);
                    }
                }
                int[] frameArray = frames.ToArray();


                Sprite.Add("normal", spriteFileName, spriteDelay, frameArray);
                //Sprite.Add("normal", "ha", 0.15f, 0, 1, 0, 1, 0, 1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
                Sprite.Play("normal");
                Sprite.JustifyOrigin(0.5f, 0.5f);
                Sprite.Color = tint;
                Duration = (float)Sprite.CurrentAnimationTotalFrames * 0.15f;
            }
        }

        private bool enabled;
        private bool noDisappearAnim;
        private string flag;
        private float timer;
        private int counter;
        private int frameNumber = 2;
        private List<Ha> has = new List<Ha>();
        private bool autoTriggerLaughSfx = true;
        private string spritePath, audioPath, spriteFileName;
        public float timeForHahaha, timeForHa, timeToSfx;
        public bool synchronizedSfx;
        public bool left;
        public float spriteDelay = 0.15f;
        public float distance;
        public float sineAmplitude;
        public int groupsSize;
        public bool vertical;
        public Color tint;

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
            spriteFileName = data.Attr("spriteFileName", "ha");
            spriteDelay = data.Float("spriteDelay", 0.15f);
            audioPath = data.Attr("sound", "event:/char/granny/laugh_oneha");
            timeForHahaha = data.Float("timeForHahaha", 1.5f);
            timeForHa = data.Float("timeForHa", 0.6f);
            timeToSfx = data.Float("timeToSfx", 0.4f);
            frameNumber = data.Int("spriteCount", 2);
            noDisappearAnim = data.Bool("noDisappearAnim", false);
            groupsSize = data.Int("groupSize", 3);
            left = data.Bool("left", false);
            distance = data.Float("distance", 60f);
            sineAmplitude = data.Float("sineAmplitude", 1f);
            synchronizedSfx = data.Bool("synchronizedSfx", false);
            vertical = data.Bool("vertical", false);
            tint = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("tint", out object c1) ? c1.ToString() : null, Color.White);
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
                    has.Add(new Ha(spritePath, spriteFileName, tint, frameNumber, noDisappearAnim, spriteDelay));
                    counter++;
                    if (counter >= groupsSize)
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
                {
                    if (vertical)
                        ha.Sprite.Position = Position + new Vector2(-10f + (float)(0.0 - Math.Sin(ha.Percent * 13f)) * 4f * sineAmplitude + ha.Percent * -16f, ha.Percent * distance);
                    else
                        ha.Sprite.Position = Position + new Vector2(ha.Percent * distance, -10f + (float)(0.0 - Math.Sin(ha.Percent * 13f)) * 4f * sineAmplitude + ha.Percent * -16f);
                }
                else
                {
                    if (vertical)
                        ha.Sprite.Position = Position + new Vector2(-10f + (float)(0.0 - Math.Sin(ha.Percent * 13f)) * 4f * sineAmplitude + ha.Percent * -16f, ha.Percent * -distance);
                    else
                        ha.Sprite.Position = Position + new Vector2(ha.Percent * -distance, -10f + (float)(0.0 - Math.Sin(ha.Percent * 13f)) * 4f * sineAmplitude + ha.Percent * -16f);
                }
                ha.Sprite.Render();
            }
        }
    }
}