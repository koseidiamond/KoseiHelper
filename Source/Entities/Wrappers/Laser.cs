using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [Pooled]
    [Tracked(false)]
    public class Laser : Entity
    {
        public ParticleType P_Dissipate = new ParticleType(FinalBossBeam.P_Dissipate);
        public const float ActiveTime = 0.12f;
        private PregnantFlutterbird source;
        private Player player;
        private Sprite beamSprite;
        private Sprite beamStartSprite;
        private float chargeTimer;
        private float followTimer;
        private float activeTimer;
        private float angle;
        private float beamAlpha;
        private float sideFadeAlpha;
        public PregnantFlutterbird owner;
        public Laser()
        {
            base.Depth = -1000000;
        }

        public Laser Init(PregnantFlutterbird source, Player target, float chargeTimer = 1.4f, float followTimer = 0.9f)
        {
            this.source = source;
            this.chargeTimer = chargeTimer;
            this.followTimer = followTimer;
            this.activeTimer = 0.12f;
            if (beamSprite != null)
            {
                base.Remove(beamSprite);
            }

            if (beamStartSprite != null)
            {
                base.Remove(beamStartSprite);
            }

            base.Add(this.beamSprite = GFX.SpriteBank.Create("badeline_beam"));
            this.beamSprite.OnLastFrame = delegate (string anim)
            {
                if (anim == "shoot")
                {
                    this.Destroy();
                }
            };
            base.Add(this.beamStartSprite = GFX.SpriteBank.Create("badeline_beam"));
            this.beamSprite.Visible = false;
            this.beamSprite.Play("charge", false, false);
            this.sideFadeAlpha = 0f;
            this.beamAlpha = 0f;
            int num;
            if (target.Y <= this.source.Y)
            {
                num = 1;
            }
            else
            {
                num = -1;
            }
            if (target.X >= this.source.X)
            {
                num *= -1;
            }
            this.angle = Calc.Angle(BeamOrigin, target.Center);
            Vector2 vector = Calc.ClosestPointOnLine(BeamOrigin, BeamOrigin + Calc.AngleToVector(this.angle, 2000f), target.Center);
            vector += (target.Center - BeamOrigin).Perpendicular().SafeNormalize(100f) * (float)num;
            this.angle = Calc.Angle(BeamOrigin, vector);
            return this;
        }

        public override void Update()
        {
            if (source.shootLasers)
            {
                base.Update();
                this.player = base.Scene.Tracker.GetEntity<Player>();
                this.beamAlpha = Calc.Approach(this.beamAlpha, 1f, 2f * Engine.DeltaTime);
                if (this.chargeTimer > 0f)
                {
                    this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 1f, Engine.DeltaTime);
                    if (this.player != null && !this.player.Dead)
                    {
                        this.followTimer -= Engine.DeltaTime;
                        this.chargeTimer -= Engine.DeltaTime;
                        if (this.followTimer > 0f && this.player.Center != BeamOrigin)
                        {
                            Vector2 vector = Calc.ClosestPointOnLine(BeamOrigin, BeamOrigin + Calc.AngleToVector(this.angle, 2000f), this.player.Center);
                            Vector2 center = this.player.Center;
                            vector = Calc.Approach(vector, center, 200f * Engine.DeltaTime);
                            this.angle = Calc.Angle(BeamOrigin, vector);
                        }
                        else if (this.beamSprite.CurrentAnimationID == "charge")
                        {
                            this.beamSprite.Play("lock", false, false);
                        }
                        if (this.chargeTimer <= 0f)
                        {
                            base.SceneAs<Level>().DirectionalShake(Calc.AngleToVector(this.angle, 1f), 0.15f);
                            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                            this.DissipateParticles();
                            return;
                        }
                    }
                }
                else if (this.activeTimer > 0f)
                {
                    this.sideFadeAlpha = Calc.Approach(this.sideFadeAlpha, 0f, Engine.DeltaTime * 8f);
                    if (this.beamSprite.CurrentAnimationID != "shoot")
                    {
                        this.beamSprite.Play("shoot", false, false);
                        this.beamStartSprite.Play("shoot", true, false);
                    }
                    this.activeTimer -= Engine.DeltaTime;
                    if (this.activeTimer > 0f)
                    {
                        this.PlayerCollideCheck();
                    }
                }
            }
            else
            {
                Destroy();
            }
        }

        private void DissipateParticles()
        {
            Level level = base.SceneAs<Level>();
            Vector2 vector = level.Camera.Position + new Vector2(160f, 90f);
            Vector2 vector2 = BeamOrigin + Calc.AngleToVector(this.angle, 12f);
            Vector2 vector3 = BeamOrigin + Calc.AngleToVector(this.angle, 2000f);
            Vector2 vector4 = (vector3 - vector2).Perpendicular().SafeNormalize();
            Vector2 value = (vector3 - vector2).SafeNormalize();
            Vector2 min = -vector4 * 1f;
            Vector2 max = vector4 * 1f;
            float direction = vector4.Angle();
            float direction2 = (-vector4).Angle();
            float num = Vector2.Distance(vector, vector2) - 12f;
            vector = Calc.ClosestPointOnLine(vector2, vector3, vector);
            for (int i = 0; i < 200; i += 12)
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    level.ParticlesFG.Emit(P_Dissipate, vector + value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction);
                    level.ParticlesFG.Emit(P_Dissipate, vector + value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction2);
                    if (i != 0 && (float)i < num)
                    {
                        level.ParticlesFG.Emit(P_Dissipate, vector - value * (float)i + vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction);
                        level.ParticlesFG.Emit(P_Dissipate, vector - value * (float)i - vector4 * 2f * (float)j + Calc.Random.Range(min, max), direction2);
                    }
                }
            }
        }

        private void PlayerCollideCheck()
        {
            Vector2 vector = BeamOrigin + Calc.AngleToVector(this.angle, 12f);
            Vector2 vector2 = BeamOrigin + Calc.AngleToVector(this.angle, 2000f);
            Vector2 value = (vector2 - vector).Perpendicular().SafeNormalize(2f);
            Player player = base.Scene.CollideFirst<Player>(vector + value, vector2 + value);
            if (player == null)
            {
                player = base.Scene.CollideFirst<Player>(vector - value, vector2 - value);
                player = base.Scene.CollideFirst<Player>(vector, vector2);
            }
            if (player != null)
            {
                player.Die((player.Center - this.source.Center).SafeNormalize(), false, true);
            }
        }

        public override void Render()
        {
            Vector2 vector = BeamOrigin;
            Vector2 vector2 = Calc.AngleToVector(this.angle, this.beamSprite.Width);
            this.beamSprite.Rotation = this.angle;
            this.beamStartSprite.Rotation = this.angle;
            if (this.beamSprite.CurrentAnimationID == "shoot")
            {
                vector += Calc.AngleToVector(this.angle, 0f);
            }
            for (int i = 0; i < 15; i++)
            {
                this.beamSprite.RenderPosition = vector;
                this.beamSprite.Render();
                vector += vector2;
            }
            if (this.beamSprite.CurrentAnimationID == "shoot")
            {
                this.beamStartSprite.RenderPosition = BeamOrigin - Calc.AngleToVector(this.angle, 8f);
                this.beamStartSprite.Render();
            }
            GameplayRenderer.End();
            Vector2 vector3 = vector2.SafeNormalize();
            Vector2 vector4 = vector3.Perpendicular();
            vector3 *= 4000f;
            vector4 *= 120f;
            int num = 0;
            GameplayRenderer.Begin();
        }


        public void Destroy()
        {
            base.RemoveSelf();
        }

        public Vector2 BeamOrigin
        {
            get
            {
                return source.Center + new Vector2(0f, 0f);
            }
        }
    }
}