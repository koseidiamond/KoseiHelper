using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/EvilHoldableController")]
    public class EvilHoldableController : Entity
    {
        public float timeToKill;
        public float cooldown;
        public bool drainsStamina = true;
        public bool drainsDash = false;
        public bool dropIfNoStamina;

        private bool dashUsed, sounded;

        public EvilHoldableController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            timeToKill = data.Float("timeToKill", 1f);
            drainsStamina = data.Bool("drainsStamina", true);
            drainsDash = data.Bool("drainsDash", false);
            dropIfNoStamina = data.Bool("dropIfNoStamina", false);
            Depth = -11000;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            cooldown = timeToKill;
        }

        public override void Update()
        {
            
            base.Update();
            Level level = SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
            {
                if (player.Holding != null)
                {
                    if (!sounded)
                    {
                        Audio.Play("event:/game/05_mirror_temple/eyebro_eyemove", player.TopCenter);
                        sounded = true;
                    }
                    if (timeToKill >= 0f)
                        cooldown -= Engine.DeltaTime;
                    if (drainsStamina)
                        player.Stamina -= 45.4545441f * Engine.DeltaTime;
                    if (drainsDash && dashUsed == false)
                    {
                        player.Dashes -= 1;
                        dashUsed = true;
                    }
                    if (dropIfNoStamina && player.Stamina <= 0f)
                        player.Drop();
                }
                else
                {
                    dashUsed = false;
                    if (timeToKill >= 0f)
                        cooldown = timeToKill;
                    sounded = false;
                }
                if (cooldown <= 0 && timeToKill >= 0f)
                    player.Die(player.Center);
            } 
        }

        public override void Render()
        {
            Level level = SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            base.Render();
            if (player != null && player.Holding != null && timeToKill >= 0f)
            {
                float radius = MathHelper.Lerp(1, 24, cooldown / timeToKill);
                Color color = Color.Lerp(Color.Lavender, Color.DarkViolet, 1f - (cooldown / timeToKill));
                float alpha = MathHelper.Lerp(1f, 0f, cooldown / timeToKill);
                Draw.Circle(player.Center, radius, color * alpha, 999);
            }
        }
    }
}