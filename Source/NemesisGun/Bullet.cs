using Celeste;
using Celeste.Mod.KoseiHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    [Tracked]
    public class Bullet : Entity
    {
        public Rectangle Hitbox => new Rectangle((int)Position.X - 3, (int)Position.Y - 4, 6, 8);
        private Vector2 velocity;
        private readonly Actor owner;
        private int lifetime;
        private bool dead;
        private int updateCount;
        private const int extraUpdates = 30;
        private bool updateFrame;

        private readonly List<Bumper> alreadyBouncedOffOf;
        private static FieldInfo feather_shielded = typeof(FlyFeather).GetField("shielded", BindingFlags.NonPublic | BindingFlags.Instance);

        public Bullet(Vector2 position, Vector2 velocity, Actor owner)
        {
            Player player = (Player)owner;
            if (player.Ducking == true)
                position.Y = position.Y - 2;
            else
                position.Y = position.Y - 4;
            Position = position;
            this.velocity = velocity;
            this.owner = owner;
            lifetime = Extensions.lifetime;
            alreadyBouncedOffOf = new List<Bumper>();

            if (CanDoShit(owner))
                (owner.Scene as Level).Add(this);
        }

        public override void Update()
        {
            if (updateCount > extraUpdates)
            {
                updateCount = 0;
                return;
            }
            if (updateFrame == true)
            {
                Position += velocity;
                updateFrame = false;
            }
            else
                updateFrame = true;
            if (CanDoShit(owner))
            {
                CollisionCheck();
                if (Calc.Random.Next(6) == 0)
                {
                    switch (Extensions.shotDustType)
                    {
                        case DustType.Sparkly:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.SparkyDust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()));
                            break;
                        case DustType.Chimney:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.Chimney, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()));
                            break;
                        case DustType.Steam:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.Steam, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()));
                            break;
                        case DustType.VentDust:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.VentDust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()));
                            break;
                        case DustType.None:
                            break;
                        default:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.Dust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()));
                            break;
                    }
                }
            }
            if (--lifetime <= 0)
                Kill();
            updateCount++;
            Update();
        }

        public override void Render()
        {
            if (CanDoShit(owner))
                (owner.Scene as Level).Particles.Emit(ParticleTypes.Dust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()));
        }

        // This is where all interactions with entities occur
        private void CollisionCheck()
        {
            if (owner.Collider.Bounds.Intersects(Hitbox) && alreadyBouncedOffOf.Count > 0)
            {
                if (owner is Player p && Extensions.canKillPlayer)
                    p.Die(velocity, true);

                Kill();
                return;
            }

            if (Extensions.canKillPlayer && owner.Scene.CollideFirst<Player>(Hitbox) is Player player && player != owner
                && !dead && !SaveData.Instance.Assists.Invincible)
            {
                player.Die(velocity, true);
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<Seeker>(Hitbox) is Seeker seeker && !dead)
            {
                if (BootlegStunSeeker(seeker))
                {
                    Kill();
                    return;
                }
            }

            if (owner.Scene.CollideFirst<FlyFeather>(Hitbox) is FlyFeather feather && !dead) //behavior by ashley_bl
            {
                if ((bool)feather_shielded.GetValue(feather) == true)
                {
                    feather_shielded.SetValue(feather, false);
                    Kill();
                }
                return;
            }

            if (owner.Scene.CollideFirst<AngryOshiro>(Hitbox) is AngryOshiro angryOshiro && Extensions.harmEnemies && !dead)
            {
                BootlegOshiroBounce(angryOshiro);
                if (Extensions.canBounce)
                    velocity = (Center - angryOshiro.Center).SafeNormalize() / 4;
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<TheoCrystal>(Hitbox) is TheoCrystal theo && !dead)
            {
                switch (Extensions.theoInteraction)
                {
                    case Extensions.TheoInteraction.None:
                        break;
                    case Extensions.TheoInteraction.HitSpinner:
                        theo.HitSpinner(this);
                        break;
                    case Extensions.TheoInteraction.HitSpring:
                        if (theo.Left > Left)
                        {
                            theo.MoveTowardsY(CenterY + 5f, 4f);
                            theo.Speed.X = 220f;
                            theo.Speed.Y = -80f;
                            theo.noGravityTimer = 0.1f;
                        }
                        else if (Math.Round(theo.Right) < Right)
                        {
                            theo.MoveTowardsY(CenterY + 5f, 4f);
                            theo.Speed.X = -220f;
                            theo.Speed.Y = -80f;
                            theo.noGravityTimer = 0.1f;
                        }
                        else
                        {
                            if (theo.CenterY <= theo.CenterY)
                            {
                                theo.Speed.X *= 0.5f;
                                theo.Speed.Y = -160f;
                                theo.noGravityTimer = 0.15f;
                            }
                        }
                        break;
                    default:
                        theo.Die();
                        break;
                }
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<DashBlock>(Hitbox) is DashBlock dBlock && !dead)
            {
                dBlock.Break(Position, velocity, true, true);
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<CustomTempleCrackedBlock>(Hitbox) is CustomTempleCrackedBlock ccrackedblock && !dead)
            {
                ccrackedblock.health -= 1;
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<TempleCrackedBlock>(Hitbox) is TempleCrackedBlock crackedblock && !dead)
            {
                crackedblock.Break(Center);
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<CrystalStaticSpinner>(Hitbox) is CrystalStaticSpinner spinner && Extensions.breakSpinners && !dead)
            {
                spinner.Destroy();
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<HeartGem>(Hitbox) is HeartGem heartGem && !dead)
            {
                if (owner is Player p)
                    NemesisGun.heartGemCollect.Invoke(heartGem, new object[] { p });
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<FinalBoss>(Hitbox) is FinalBoss boss && Extensions.harmEnemies && !dead)
            {
                if (!boss.Sitting && owner is Player p)
                    boss.OnPlayer(p);
                Kill();
                return;
            }

            if (owner.Scene.CollideFirst<DustStaticSpinner>(Hitbox) is DustStaticSpinner dSSpinner && Extensions.breakSpinners && !dead)
            {
                dSSpinner.RemoveSelf();
                Kill();
                return;
            }

            foreach (Entity entity in (owner.Scene as Level).Entities)
            {
                if (entity is TrackSpinner tSpinner && Extensions.breakMovingBlades && tSpinner.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    tSpinner.RemoveSelf();
                    Kill();
                    return;
                }

                if (entity is RotateSpinner rSpinner && Extensions.breakMovingBlades && rSpinner.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    rSpinner.RemoveSelf();
                    Kill();
                    return;
                }

                if (entity is Bumper bumper && bumper.Collider.Bounds.Intersects(Hitbox) && !dead && !alreadyBouncedOffOf.Contains(bumper))
                {
                    if ((bool)NemesisGun.bumperFireMode.GetValue(bumper))
                    {
                        Kill();
                        return;
                    }
                    else if (bumper.respawnTimer <= 0)
                        velocity = BootlegBumperHit(bumper);

                    alreadyBouncedOffOf.Add(bumper);
                }

                if (entity is Strawberry strawberry && strawberry.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    if (owner is Player pl)
                    {
                        if (strawberry.Golden && Extensions.canKillPlayer)
                            pl.Die(Vector2.Zero, true);
                        else if (Extensions.collectables)
                            strawberry.OnPlayer(pl);
                    }
                    return;
                }
                if (entity is CollabUtils2.Entities.SilverBerry silverBerry && silverBerry.Collider.Bounds.Intersects(Hitbox) && Extensions.canKillPlayer && !dead)
                {
                    if (owner is Player pl)
                        pl.Die(Vector2.Zero, true);
                    Kill();
                    return;
                }
                if (entity is Key key && key.Collider.Bounds.Intersects(Hitbox) && Extensions.collectables && !dead && owner is Player p_key)
                {
                    key.OnPlayer(p_key);
                    return;
                }

                if (entity is Spring spring && spring.Collider.Bounds.Intersects(Hitbox) && Extensions.collectables && !dead && owner is Player p_spring && spring.Collidable)
                {
                    if (Extensions.canBounce)
                        velocity = (Center - spring.Center).SafeNormalize();
                    else
                        Kill();
                    return;
                }

                if (entity is Refill refill && refill.Collider.Bounds.Intersects(Hitbox) && Extensions.useRefills && !dead && owner is Player refill_player)
                {
                    if (refill.Collidable)
                    {
                        refill.OnPlayer(refill_player);
                        if (refill.respawnTimer >0)
                            Kill();
                    }
                    return;
                }

                if (entity is SummitGem sGem && sGem.Collider.Bounds.Intersects(Hitbox) && Extensions.collectables && !dead && owner is Player p)
                {
                    sGem.Add(new Coroutine((IEnumerator)NemesisGun.summitGemSmashRoutine.Invoke(sGem, new object[] { p, p.Scene as Level })));
                }

                if (entity is Puffer puffer && puffer.Collider.Bounds.Intersects(Hitbox) && Extensions.explodeFishes && !dead)
                {
                    NemesisGun.pufferExplode.Invoke(puffer, null);
                    NemesisGun.pufferGotoGone.Invoke(puffer, null);
                    Kill();
                    return;
                }

                // KoseiHelper interactions

                if (entity is Goomba goomba && Extensions.harmEnemies && goomba.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    goomba.Killed((owner as Player), (owner as Player).SceneAs<Level>());
                    Kill();
                    return;
                }

                if (entity is DefrostableBlock defrostableBlock && defrostableBlock.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    defrostableBlock.defrosting = true;
                    Kill();
                    return;
                }

                if (entity is Plant plant && Extensions.harmEnemies && plant.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    plant.RemoveSelf();
                    Audio.Play("event:/KoseiHelper/goomba", plant.Center);
                    Kill();
                    return;
                }

                if (entity is MaryBlock mary && mary.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    Audio.Play("event:/KoseiHelper/mary", Position);
                    SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Behind, 5, Center + new Vector2(0, -2), Vector2.One * 4f, CenterX - (float)Math.PI / 2f);
                    mary.RemoveSelf();
                    Kill();
                    return;
                }

                if (entity is FallingPlatform fallingPlatform && Extensions.activateFallingBlocks && !dead)
                {
                    fallingPlatform.StartFalling();
                    Kill();
                    return;
                }
            }

            if (owner.Scene.CollideFirst<StrawberrySeed>(Hitbox) is StrawberrySeed seed && Extensions.collectables && !dead)
            {
                if (owner is Player p)
                    NemesisGun.strawberrySeedOnPlayer.Invoke(seed, new object[] { p });
                Kill();
                return;
            }



            // Solid interactions
            if (owner.Scene.CollideFirst<Solid>(Hitbox) is Solid solid && !dead)
            {
                if (solid is DreamBlock dreamBlock && Extensions.canGoThroughDreamBlocks)
                {
                    if ((owner.Scene as Level).Session.Inventory.DreamDash)
                        return;
                }

                if (solid is CrushBlock cBlock && Extensions.enableKevins && owner is Player p)
                {
                    if (Center.Y > cBlock.Center.Y + (cBlock.Height / 2))
                        cBlock.OnDashCollide(p, -Vector2.UnitY);
                    else if (Center.Y < cBlock.Center.Y - (cBlock.Height / 2))
                        cBlock.OnDashCollide(p, Vector2.UnitY);
                    else if (Center.X > cBlock.Center.X + (cBlock.Width / 2))
                        cBlock.OnDashCollide(p, -Vector2.UnitX);
                    else if (Center.X < cBlock.Center.X - (cBlock.Width / 2))
                        cBlock.OnDashCollide(p, Vector2.UnitX);
                    else
                        cBlock.OnDashCollide(p, velocity);
                }

                if (solid is LightningBreakerBox lBBox && owner is Player pl)
                    lBBox.Dashed(pl, velocity);

                if (solid is DashSwitch dSwitch && Extensions.pressDashSwitches)
                    dSwitch.OnDashCollide(null, (Vector2)NemesisGun.dashSwitchPressDirection.GetValue(dSwitch));

                if (solid is FallingBlock fallingBlock && Extensions.activateFallingBlocks)
                    fallingBlock.Triggered = true;

                if (solid is BounceBlock bounceBlock && Extensions.breakBounceBlocks)
                    bounceBlock.Break();

                Kill();
                return;
            }
        }

        private bool BootlegStunSeeker(Seeker seeker)
        {
            if (Extensions.harmEnemies && !seeker.Regenerating)
            {
                Audio.Play("event:/game/05_mirror_temple/seeker_booped", seeker.Position);
                NemesisGun.seekerDead.SetValue(seeker, true);
                NemesisGun.seekerGotBouncedOn.Invoke(seeker, new object[] { this });
                return true;
            }

            return false;
        }

        private void BootlegOshiroBounce(AngryOshiro oshiro)
        {
            Audio.Play("event:/game/general/thing_booped", oshiro.Position);
            Celeste.Freeze(0.2f);
            ((StateMachine)NemesisGun.oshiroStateMachine.GetValue(oshiro)).State = 5;
            ((SoundSource)NemesisGun.oshiroPreChargeSFX.GetValue(oshiro)).Stop();
            ((SoundSource)NemesisGun.oshiroChargeSFX.GetValue(oshiro)).Stop();
        }

        private Vector2 BootlegBumperHit(Bumper bumper)
        {
            Level level = bumper.Scene as Level;
            if (level.Session.Area.ID == 9)
                Audio.Play("event:/game/09_core/pinballbumper_hit", bumper.Position);
            else
                Audio.Play("event:/game/06_reflection/pinballbumper_hit", bumper.Position);
            NemesisGun.bumperRespawnTimer.SetValue(bumper, 0.6f);
            Vector2 vector = (Center - bumper.Center).SafeNormalize();
            ((Sprite)NemesisGun.bumperSprite.GetValue(bumper)).Play("hit", restart: true);
            ((Sprite)NemesisGun.bumperSpriteEvil.GetValue(bumper)).Play("hit", restart: true);
            ((VertexLight)NemesisGun.bumperLight.GetValue(bumper)).Visible = false;
            ((BloomPoint)NemesisGun.bumperBloom.GetValue(bumper)).Visible = false;
            level.DirectionalShake(vector, 0.15f);
            level.Displacement.AddBurst(Center, 0.3f, 8, 32, 0.8f);
            level.Particles.Emit(Bumper.P_Launch, 12, Center + vector * 12, Vector2.One * 3, vector.Angle());
            if (Extensions.canBounce)
                return vector;
            else
                return velocity;
        }

        // Removes bullets
        private void Kill()
        {
            if (CanDoShit(owner) && Extensions.bulletExplosion)
            {
                for (int i = 0; i < 8; i++)
                {
                    (owner.Scene as Level).Particles.Emit(ParticleTypes.Steam, Position + Calc.Random.ShakeVector(), Color.Lerp(Extensions.color1,Color.White,0.5f));
                }
            }
            dead = true;
            Audio.Play(Extensions.bulletSound, Center);
            RemoveSelf();
        }
        public static bool CanDoShit(Actor owner)
            => owner != null && owner.Scene != null && owner.Scene.Tracker != null;
    }
}