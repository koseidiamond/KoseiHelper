using Celeste.Mod.KoseiHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FrostHelper;
using Celeste.Mod.MaxHelpingHand.Entities;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    [Tracked]
    public class Bullet : Entity
    {
        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, 6, 6);
        private Vector2 velocity;
        private readonly Actor owner;
        private int lifetime;
        private bool dead;
        private int updateCount;
        private const int extraUpdates = 30;
        private bool updateFrame;
        private MTexture bulletTexture;
        private ParticleType p_fire = FireBall.P_FireTrail, p_ice = FireBall.P_IceTrail, p_feather = BirdNPC.P_Feather;

        private readonly List<Bumper> BouncedOffBumper;
        private readonly List<Spring> BouncedOffSpring;
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
            BouncedOffBumper = new List<Bumper>();
            BouncedOffSpring = new List<Spring>();
            bulletTexture = GFX.Game[Extensions.bulletTexture];
            if (CanDoShit(owner))
                (owner.Scene as Level).Add(this);
            (owner.Scene as Level).Session.SetFlag("KoseiHelper_playerIsShooting", true);
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
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.SparkyDust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                        case DustType.Chimney:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.Chimney, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                        case DustType.Steam:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.Steam, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                        case DustType.VentDust:
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.VentDust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                        case DustType.None:
                            break;
                        case DustType.Fire:
                            (owner.Scene as Level).Particles.Emit(p_fire, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                        case DustType.Ice:
                            (owner.Scene as Level).Particles.Emit(p_ice, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                        case DustType.Feather:
                            (owner.Scene as Level).Particles.Emit(p_feather, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                        default: // Normal (Dust)
                            (owner.Scene as Level).Particles.Emit(ParticleTypes.Dust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                            break;
                    }
                }
            }
            if (--lifetime <= 0 && owner != null)
                DestroyBullet();
            updateCount++;
            Update();
        }

        /*public override void Render()
        {
            if (CanDoShit(owner))
            {
                (owner.Scene as Level).Particles.Emit(ParticleTypes.Dust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                bulletTexture.DrawCentered(Position, Color.White, 1, 0f); // In radians
            }
        }*/

        public override void Render()
        {
            if (CanDoShit(owner))
            {
                (owner.Scene as Level).Particles.Emit(ParticleTypes.Dust, Position, Color.Lerp(Extensions.color1, Extensions.color2, Calc.Random.NextFloat()) * Extensions.particleAlpha);
                float angle = (float)Math.Atan2(-velocity.Y, velocity.X);
                bulletTexture.DrawCentered(Position, Color.White, 1, angle);
            }
        }

        // This is where all interactions with entities occur
        private void CollisionCheck()
        {
            if (owner.Collider.Bounds.Intersects(Hitbox) && (BouncedOffBumper.Count > 0 || BouncedOffSpring.Count > 0))
            {
                if (owner is Player p && Extensions.canKillPlayer)
                {
                    p.Die(velocity, true);
                    DestroyBullet();
                }
                return;
            }

            if (Extensions.canKillPlayer && owner.Scene.CollideFirst<Player>(Hitbox) is Player player && player != owner
                && !dead && !SaveData.Instance.Assists.Invincible)
            {
                player.Die(velocity, true);
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<Seeker>(Hitbox) is Seeker seeker && !dead)
            {
                if (BootlegStunSeeker(seeker))
                {
                    DestroyBullet();
                    return;
                }
            }

            if (owner.Scene.CollideFirst<FlyFeather>(Hitbox) is FlyFeather feather && Extensions.useFeathers && !dead)
            {
                if ((bool)feather_shielded.GetValue(feather) == true)
                {
                    feather_shielded.SetValue(feather, false);
                    if (Extensions.canBounce)
                        velocity = (Center - feather.Center).SafeNormalize();
                    else
                        DestroyBullet();
                }
                else if (owner is Player featherPlayer)
                {
                    feather.OnPlayer(featherPlayer);
                }
                return;
            }

            if (owner.Scene.CollideFirst<AngryOshiro>(Hitbox) is AngryOshiro angryOshiro && Extensions.harmEnemies && !dead)
            {
                BootlegOshiroBounce(angryOshiro);
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<Water>(Hitbox) is Water water && !dead)
            {
                velocity *= Extensions.waterFriction;
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
                            if (theo.Top <= Bottom)
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
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<DashBlock>(Hitbox) is DashBlock dBlock && !dead)
            {
                dBlock.Break(Position, velocity, true, true);
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<CustomTempleCrackedBlock>(Hitbox) is CustomTempleCrackedBlock ccrackedblock && !dead)
            {
                ccrackedblock.health -= 1;
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<TempleCrackedBlock>(Hitbox) is TempleCrackedBlock crackedblock && !dead)
            {
                crackedblock.Break(Center);
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<CrystalStaticSpinner>(Hitbox) is CrystalStaticSpinner spinner && Extensions.breakSpinners && !dead)
            {
                spinner.Destroy();
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<CustomSpinner>(Hitbox) is CustomSpinner customSpinner && Extensions.breakSpinners && !dead)
            {
                customSpinner.Destroy();
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<BadelineBoost>(Hitbox) is BadelineBoost badelineBoost && !dead && owner is Player playerBadelineBoost)
            {
                badelineBoost.OnPlayer(playerBadelineBoost);
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<HeartGem>(Hitbox) is HeartGem heartGem && Extensions.collectBadelineOrbs && !dead)
            {
                if (owner is Player p && !heartGem.collected)
                {
                    NemesisGun.heartGemCollect.Invoke(heartGem, new object[] { p });
                    DestroyBullet();
                }
                return;
            }

            if (owner.Scene.CollideFirst<FlingBird>(Hitbox) is FlingBird flingBird && Extensions.scareBirds && !dead)
            {
                if (owner is Player p_bird && flingBird.state == FlingBird.States.Wait)
                {
                    flingBird.Skip();
                    DestroyBullet();
                }
                return;
            }

            if (owner.Scene.Tracker.GetEntity<FlutterBird>() != null && Extensions.scareBirds && !dead)
            {
                FlutterBird flutter = owner.Scene.Tracker.GetEntity<FlutterBird>();
                if (Math.Abs(X - flutter.X) < 48f && Y > flutter.Y -  40f && Y < flutter.Y + 8f)
                {
                    flutter.FlyAway(Math.Sign(flutter.X - X), Calc.Random.NextFloat(0.2f));
                }
                return;
            }

            if (owner.Scene.CollideFirst<TouchSwitch>(Hitbox) is TouchSwitch touchSwitch && Extensions.collectTouchSwitches && !dead)
            {
                if (owner is Player p_tswitch)
                {
                    touchSwitch.TurnOn();
                }
                return;
            }

            if (owner.Scene.CollideFirst<FlagTouchSwitch>(Hitbox) is FlagTouchSwitch flagTouchSwitch && Extensions.collectTouchSwitches && !dead)
            {
                if (owner is Player p_tswitch)
                {
                    flagTouchSwitch.TurnOn();
                }
                return;
            }

            if (owner.Scene.CollideFirst<FinalBoss>(Hitbox) is FinalBoss boss && Extensions.harmEnemies && !dead)
            {
                Level level = SceneAs<Level>();
                if (!boss.Sitting && owner is Player p)
                    boss.OnPlayer(p);
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<DustStaticSpinner>(Hitbox) is DustStaticSpinner dSSpinner && Extensions.breakSpinners && !dead)
            {
                dSSpinner.RemoveSelf();
                DestroyBullet();
                return;
            }

            if (owner.Scene.CollideFirst<Door>(Hitbox) is Door door && !dead && !door.disabled)
            {
                door.Open(X);
                return;
            }

            if (owner.Scene.CollideFirst<StrawberrySeed>(Hitbox) is StrawberrySeed seed && Extensions.collectables && !dead)
            {
                if (owner is Player p)
                    NemesisGun.strawberrySeedOnPlayer.Invoke(seed, new object[] { p });
                DestroyBullet();
                return;
            }

            foreach (Entity entity in (owner.Scene as Level).Entities)
            {

                if (entity is CoreModeToggle coreModeToggle && coreModeToggle.Collider.Bounds.Intersects(Hitbox) && Extensions.coreModeToggles &&!dead)
                {
                    if (owner is Player playerCoreModeToggle)
                        coreModeToggle.OnPlayer(playerCoreModeToggle);
                    //DestroyBullet();
                    return;
                }

                if (entity is FakeHeart fakeHeart && fakeHeart.Collider.Bounds.Intersects(Hitbox) && !dead && fakeHeart.Visible)
                {
                    if (fakeHeart.bounceSfxDelay <= 0f)
                    {
                        Audio.Play("event:/game/general/crystalheart_bounce", fakeHeart.Position);
                        fakeHeart.bounceSfxDelay = 0.1f;
                    }

                    if (owner is Player playerFakeHeart && Extensions.collectables)
                        fakeHeart.Collect(playerFakeHeart, velocity.Angle());

                    if (Extensions.canBounce)
                    {
                        velocity = (Center - fakeHeart.Center).SafeNormalize();
                        velocity.X *= 1.5f;
                    }
                    fakeHeart.moveWiggler.Start();
                    fakeHeart.ScaleWiggler.Start();
                    fakeHeart.moveWiggleDir = (fakeHeart.Center - Center).SafeNormalize(Vector2.UnitY);
                    Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                    return;
                }

                if (entity is TrackSpinner tSpinner && Extensions.breakMovingBlades && tSpinner.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    tSpinner.RemoveSelf();
                    DestroyBullet();
                    return;
                }

                if (entity is RotateSpinner rSpinner && Extensions.breakMovingBlades && rSpinner.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    rSpinner.RemoveSelf();
                    DestroyBullet();
                    return;
                }

                if (entity is Bumper bumper && bumper.Collider.Bounds.Intersects(Hitbox) && !dead && !BouncedOffBumper.Contains(bumper))
                {
                    if ((bool)NemesisGun.bumperFireMode.GetValue(bumper))
                    {
                        DestroyBullet();
                        return;
                    }
                    else if (bumper.respawnTimer <= 0 && Extensions.canBounce)
                        velocity = BootlegBumperHit(bumper);
                    if (Extensions.canBounce)
                        BouncedOffBumper.Add(bumper);
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
                    if (owner is Player plSilver)
                        plSilver.Die(Vector2.Zero, true);
                    DestroyBullet();
                    return;
                }

                if (entity is Key key && key.Collider.Bounds.Intersects(Hitbox) && Extensions.collectables && !dead)
                {
                    if (owner is Player p_key && !key.follower.HasLeader)
                    {
                        key.OnPlayer(p_key);
                        DestroyBullet();
                    }
                    return;
                }


                if (entity is Booster booster && booster.Collider.Bounds.Intersects(Hitbox) && Extensions.collectables && Extensions.useBoosters && !dead && owner is Player p_booster)
                {
                    if (booster.respawnTimer <= 0 && booster.cannotUseTimer <= 0 && !booster.BoostingPlayer)
                    {
                        booster.OnPlayer(p_booster);
                        p_booster.Position = booster.Position;
                        DestroyBullet();
                    }
                    return;
                }

                if (entity is Spring spring && spring.Collider.Bounds.Intersects(Hitbox) && !BouncedOffSpring.Contains(spring) && Extensions.canBounce &&
                    !dead && owner is Player p_spring && spring.Collidable)
                {
                    if (Extensions.canBounce)
                    {
                        velocity = (Center - spring.Center).SafeNormalize();
                        BouncedOffSpring.Add(spring);
                    }
                    else
                        DestroyBullet();
                    return;
                }

                if (entity is Refill refill && refill.Collider.Bounds.Intersects(Hitbox) && Extensions.useRefills && !dead && owner is Player refill_player)
                {
                    if (refill.Collidable)
                    {
                        refill.OnPlayer(refill_player);
                        if (refill.respawnTimer >0)
                            DestroyBullet();
                    }
                    return;
                }

                if (entity is SummitGem sGem && sGem.Collider.Bounds.Intersects(Hitbox) && !dead && owner is Player p)
                {
                    if (sGem.Collidable)
                    {
                        if (Extensions.collectables)
                        {
                            sGem.Add(new Coroutine((IEnumerator)NemesisGun.summitGemSmashRoutine.Invoke(sGem, new object[] { p, p.Scene as Level })));
                            DestroyBullet();
                        }
                        else
                        {
                            sGem.moveWiggler.Start();
                            sGem.scaleWiggler.Start();
                            sGem.moveWiggleDir = (sGem.Center - Center).SafeNormalize(Vector2.UnitY);
                            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                            if (sGem.bounceSfxDelay <= 0f)
                            {
                                Audio.Play("event:/game/general/crystalheart_bounce", Position);
                                sGem.bounceSfxDelay = 0.1f;
                            }
                        }
                        if (Extensions.canBounce)
                        {
                            velocity = (Center - sGem.Center).SafeNormalize();
                            velocity.X *= 1.5f;
                        }
                    }
                    return;
                }

                if (entity is Puffer puffer && puffer.Collider.Bounds.Intersects(Hitbox) && !dead && puffer.Collidable)
                {
                    switch (Extensions.pufferInteraction)
                    {
                        case Extensions.PufferInteraction.Explode:
                            NemesisGun.pufferExplode.Invoke(puffer, null);
                            NemesisGun.pufferGotoGone.Invoke(puffer, null);
                            break;
                        case Extensions.PufferInteraction.HitSpring:
                            if (puffer.Left > Left)
                            {
                                puffer.facing.X = 1f;
                                puffer.GotoHitSpeed(280f * Vector2.UnitX);
                                Audio.Play("event:/new_content/game/10_farewell/puffer_boop", puffer.Position);
                                puffer.MoveTowardsY(CenterY, 4f);
                                puffer.bounceWiggler.Start();
                                puffer.Alert(restart: true, playSfx: false);
                            }
                            else if (Math.Round(puffer.Right) < Right)
                            {
                                puffer.facing.X = -1f;
                                puffer.GotoHitSpeed(280f * -Vector2.UnitX);
                                Audio.Play("event:/new_content/game/10_farewell/puffer_boop", puffer.Position);
                                puffer.MoveTowardsY(CenterY, 4f);
                                puffer.bounceWiggler.Start();
                                puffer.Alert(restart: true, playSfx: false);
                            }
                            else
                            {
                                if (puffer.Top <= Bottom)
                                {
                                    puffer.GotoHitSpeed(224f * -Vector2.UnitY);
                                    Audio.Play("event:/new_content/game/10_farewell/puffer_boop", puffer.Position);
                                    puffer.MoveTowardsX(CenterX, 4f);
                                    puffer.bounceWiggler.Start();
                                    puffer.Alert(restart: true, playSfx: false);
                                }
                                else
                                {
                                    puffer.GotoHit(Center);
                                    puffer.MoveToX(puffer.anchorPosition.X);
                                    puffer.idleSine.Reset();
                                    puffer.anchorPosition = (puffer.lastSinePosition = puffer.Position);
                                    puffer.eyeSpin = 1f;
                                    puffer.cannotHitTimer = 0.1f;
                                }
                            }
                            break;
                        case Extensions.PufferInteraction.None:
                            break;
                        default:
                            break;
                    }
                    DestroyBullet();
                    return;
                }

                // KoseiHelper interactions

                if (entity is Goomba goomba && Extensions.harmEnemies && goomba.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    goomba.Killed((owner as Player), (owner as Player).SceneAs<Level>());
                    DestroyBullet();
                    return;
                }

                if (entity is DefrostableBlock defrostableBlock && defrostableBlock.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    defrostableBlock.defrosting = true;
                    DestroyBullet();
                    return;
                }

                if (entity is Plant plant && Extensions.harmEnemies && plant.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    plant.RemoveSelf();
                    SceneAs<Level>().ParticlesFG.Emit(Player.P_Split, 5, Position, Vector2.One * 4f, velocity.Angle() - (float)Math.PI / 2f);
                    Audio.Play("event:/KoseiHelper/goomba", plant.Center);
                    DestroyBullet();
                    return;
                }

                if (entity is MaryBlock mary && mary.Collider.Bounds.Intersects(Hitbox) && !dead)
                {
                    Audio.Play("event:/KoseiHelper/mary", Position);
                    SceneAs<Level>().ParticlesFG.Emit(SwitchGate.P_Behind, 5, Center + new Vector2(0, -2), Vector2.One * 4f, CenterX - (float)Math.PI / 2f);
                    mary.RemoveSelf();
                    DestroyBullet();
                    return;
                }

                if (entity is FallingPlatform fallingPlatform && fallingPlatform.Collider.Bounds.Intersects(Hitbox) && Extensions.activateFallingBlocks && !dead && !fallingPlatform.triggered)
                {
                    fallingPlatform.StartFalling();
                    DestroyBullet();
                    return;
                }
            }

            // Solid interactions
            if (owner.Scene.CollideFirst<Solid>(Hitbox) is Solid solid && !dead)
            {
                if (solid is DreamBlock dreamBlock)
                {
                    switch (Extensions.dreamBlockBehavior)
                    {
                        case Extensions.DreamBlockBehavior.Destroy:
                            Audio.Play("event:/KoseiHelper/shatter_dream_block", dreamBlock.Center);
                            Audio.Play("event:/new_content/game/10_farewell/glider_emancipate", dreamBlock.Center);
                            dreamBlock.OneUseDestroy();
                            break;
                        case Extensions.DreamBlockBehavior.GoThrough:
                            if ((owner.Scene as Level).Session.Inventory.DreamDash)
                                return;
                            else
                                DestroyBullet();
                            break;
                        default: // None
                            DestroyBullet();
                            break;
                    }
                    return;
                }

                if (solid is CustomOshiroDoor customOshiroDoor && !dead)
                {
                    if (owner is Player p_oshirodoor)
                    {
                            customOshiroDoor.OnDashCollide(p_oshirodoor, customOshiroDoor.Center);
                        DestroyBullet();
                    }
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
                    DestroyBullet();
                }

                if (solid is LightningBreakerBox lBBox && owner is Player pl)
                    lBBox.Dashed(pl, velocity);

                if (solid is DashSwitch dSwitch && Extensions.pressDashSwitches)
                    dSwitch.OnDashCollide(null, (Vector2)NemesisGun.dashSwitchPressDirection.GetValue(dSwitch));

                if (solid is FallingBlock fallingBlock && Extensions.activateFallingBlocks)
                    fallingBlock.Triggered = true;

                if (solid is BounceBlock bounceBlock && Extensions.breakBounceBlocks)
                    bounceBlock.Break();
                DestroyBullet();
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
        private void DestroyBullet()
        {
            if (CanDoShit(owner) && Extensions.bulletExplosion)
            {
                for (int i = 0; i < 6; i++)
                {
                    (owner.Scene as Level).Particles.Emit(ParticleTypes.Steam, Position + Calc.Random.ShakeVector(), Color.Lerp(Extensions.color1,Color.White,0.5f));
                }
            }
            dead = true;
            (Scene as Level).Session.SetFlag("KoseiHelper_playerIsShooting", false);
            if (owner != null && this != null)
                RemoveSelf();
        }

        public override void DebugRender(Camera camera)
        {
            base.DebugRender(camera);
            Draw.HollowRect(Hitbox, Color.Red);

        }
        public static bool CanDoShit(Actor owner)
            => owner != null && owner.Scene != null && owner.Scene.Tracker != null;
    }
}