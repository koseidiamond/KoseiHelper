using System;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/CustomSeekerStatue")]
    public class CustomSeekerStatue : Entity
    {
        private enum Hatch
        {
            Distance,
            PlayerRightOfX,
            PlayerLeftOfX,
            Flag
        }

        private Hatch hatchMode;
        private Sprite sprite;

        public string sound, flag, statueSprite, hatchSprite;
        public new int depth;
        public float distance;
        public bool particles;
        public CustomSeekerStatue(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            sound = data.Attr("sound", "event:/game/05_mirror_temple/seeker_statue_break");
            hatchMode = data.Enum("hatchMode", Hatch.Distance);
            flag = data.Attr("flag", "");
            statueSprite = data.Attr("statueSprite", "statue");
            hatchSprite = data.Attr("hatchSprite", "hatch");
            depth = data.Int("depth", 8999);
            distance = data.Float("distance", 220f);
            particles = data.Bool("particles", true);

            CustomSeekerStatue seekerStatue = this;
            Depth = depth;
            Add(sprite = GFX.SpriteBank.Create("seeker"));
            sprite.Play(statueSprite);
            sprite.OnLastFrame = (string f) =>
            {
                if (f == hatchSprite)
                {
                    Seeker entity = new Seeker(data, offset)
                    {
                        Light =
                    {
                        Alpha = 0f
                    }
                    };
                    seekerStatue.Scene.Add(entity);
                    seekerStatue.RemoveSelf();
                }
            };
        }

        public override void Update()
        {
            base.Update();
            Level level = SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null && sprite.CurrentAnimationID == statueSprite)
            {
                bool hatched = false;
                switch (hatchMode)
                {
                    case Hatch.PlayerLeftOfX:
                        if (player.X < base.X - distance)
                            hatched = true;
                        break;
                    case Hatch.PlayerRightOfX:
                        if (player.X > base.X + distance)
                            hatched = true;
                        break;
                    case Hatch.Flag:
                        if (level.Session.GetFlag(flag))
                            hatched = true;
                        break;
                    default: // Hatch.Distance
                        if ((player.Position - Position).Length() < distance)
                            hatched = true;
                        break;
                }
                if (hatched)
                {
                    if (particles)
                        BreakOutParticles();
                    sprite.Play(hatchSprite);
                    Audio.Play(sound, Position);
                    Alarm.Set(this, 0.8f, BreakOutParticles);
                }
            }
        }

        private void BreakOutParticles()
        {
            Level level = SceneAs<Level>();
            for (float num = 0f; num < MathF.PI * 2f; num += 0.17453292f)
            {
                Vector2 position = base.Center + Calc.AngleToVector(num + Calc.Random.Range(-MathF.PI / 90f, MathF.PI / 90f), Calc.Random.Range(12, 20));
                level.Particles.Emit(Seeker.P_BreakOut, position, num);
            }
        }
    }
}