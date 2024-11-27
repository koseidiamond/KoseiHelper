using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using On.Celeste;
using System.Collections.Generic;
using static Celeste.Session;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomPlayerSeeker")]
public class CustomPlayerSeeker : Actor
{
    private Facings facing;
    private Sprite sprite;
    private Vector2 speed;
    public float speedMultiplier = 1;
    private bool enabled;
    private float dashTimer;
    private Vector2 dashDirection;
    private float trailTimerA;
    private float trailTimerB;
    private Shaker shaker;

    private Vector2 cameraTarget;

    public string nextRoom, colorgrade, seekerDash;
    public bool vanillaEffects = false;
    public bool isFriendly = false;

    public bool canDash = true;
    private bool hasDied, isDying = false;
    private CoreModes coreMode;
    private bool fireMode;
    public float explodeLaunchBoostTimer, explodeLaunchBoostSpeed;

    private float windTimeout;
    private bool windMovedUp;
    private Vector2 windDirection;
    private bool justRespawned;
    public bool canSwitchCharacters, isMadeline;
    private Collider bounceCollider;

    public static ParticleType boosterParticle = Booster.P_Burst;
    public static ParticleType boosterRedParticle = Booster.P_BurstRed;
    public static ParticleType featherParticle = FlyFeather.P_Collect;
    public static ParticleType seekerParticle = Seeker.P_Stomp;


    HashSet<Type> hazardTypes = new HashSet<Type>
    {
        typeof(SeekerBarrier),
        typeof(BladeRotateSpinner),
        typeof(BladeTrackSpinner),
        typeof(Spikes),
        typeof(DustStaticSpinner),
        typeof(DustTrackSpinner),
        typeof(RotateSpinner),
        typeof(StarRotateSpinner),
        typeof(StarTrackSpinner),
        typeof(DustRotateSpinner),
        typeof(CrystalStaticSpinner),
        typeof(AngryOshiro),
        typeof(SandwichLava),
        typeof(IceBlock),
        typeof(FireBall),
        typeof(FireBarrier),
        typeof(FinalBossBeam),
        typeof(FinalBossShot)
    };

    public Vector2 CameraTarget
    {
        get
        {
            Rectangle bounds = (base.Scene as Level).Bounds;
            return (Position + new Vector2(-160f, -90f)).Clamp(bounds.Left, bounds.Top, bounds.Right - 320, bounds.Bottom - 180);
        }
    }
    public CustomPlayerSeeker(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        nextRoom = data.Attr("nextRoom", "c-00");
        colorgrade = data.Attr("colorgrade", "templevoid");
        vanillaEffects = data.Bool("vanillaEffects", false);
        seekerDash = data.Attr("seekerDashSound", "event:/game/05_mirror_temple/seeker_dash");
        canSwitchCharacters = data.Bool("canSwitchCharacters", false);
        speedMultiplier = data.Float("speedMultiplier", 1f);
        isFriendly = data.Bool("isFriendly", false);
        Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite","seeker"))); //IMPORTANT: the sprite needs the tags hatch, flipMouth, flipEyes, idle, spotted, attacking and statue to work
        if (vanillaEffects)
            sprite.Play("statue");
        else
            sprite.Play("spotted");
        sprite.OnLastFrame = (string a) =>
        {
            if (a == "flipMouth" || a == "flipEyes")
            {
                facing = (Facings)(0 - facing);
            }
        };
        base.Collider = new Hitbox(10f, 10f, -5f, -5f);
        if (isFriendly)
            bounceCollider = new Hitbox(10f, 6f, -5f, -4f);
        Add(new PlayerCollider(OnPlayerBounce, bounceCollider));
        Add(new MirrorReflection());
        Add(new PlayerCollider(OnPlayer));
        Add(new VertexLight(Color.White, 1f, 32, 64));
        facing = Facings.Right;
        Add(shaker = new Shaker(on: false));
        Add(new Coroutine(IntroSequence()));
        Add(new CoreModeListener(OnChangeMode));
        Add(new WindMover(WindMove));
    }
    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = scene as Level;
        if (!string.IsNullOrEmpty(colorgrade))
            level.Session.ColorGrade = colorgrade;
        level.Session.SetFlag("kosei_PlayerSeeker", true);
        if (vanillaEffects)
            level.ScreenPadding = 32f;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        fireMode = SceneAs<Level>().CoreMode == Session.CoreModes.Hot;
        justRespawned = true;
    }

    private IEnumerator IntroSequence()
    {
        Level level = Scene as Level;
        yield return null;
        if (vanillaEffects)
            Glitch.Value = 0.05f;
        Player player = level.Tracker.GetEntity<Player>();
        player.Sprite.Play("asleep");
        if (player.Position.X <= Position.X)
            player.Facing = Facings.Right;
        else
            player.Facing = Facings.Left;
        player.StateMachine.State = 11;
        player.StateMachine.Locked = true;
        player.DummyAutoAnimate = false;
        player.DummyGravity = true;
        if (vanillaEffects)
            yield return 1f;
        Vector2 from = level.Camera.Position;

        if (vanillaEffects)
        {
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, start: true);
            tween.OnUpdate = (Tween f) =>
            {
                Vector2 cameraTarget = CameraTarget;
                level.Camera.Position = from + (cameraTarget - from) * f.Eased;
            };
            Add(tween);
            yield return 1f;
            shaker.ShakeFor(0.5f, removeOnFinish: false);
            BreakOutParticles();
            Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
            yield return 0.5f;
            shaker.ShakeFor(0.5f, removeOnFinish: false);
            BreakOutParticles();
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
            yield return 0.5f;
            BreakOutParticles();
            Audio.Play("event:/game/05_mirror_temple/seeker_statue_break", Position);
            shaker.ShakeFor(0.5f, removeOnFinish: false);
            sprite.Play("hatch");
            Input.Rumble(RumbleStrength.Strong, RumbleLength.FullSecond);
        }
        else
            level.Camera.Position = Position - new Vector2(180,90);
        enabled = true;
        if (vanillaEffects)
        {
            yield return 0.4f;
            BreakOutParticles();
            yield return 0.35f;
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
    private void OnPlayer(Player player)
    {
        if (!player.Dead && !isFriendly)
        {
            Leader.StoreStrawberries(player.Leader);
            PlayerDeadBody playerDeadBody = player.Die((player.Position - Position).SafeNormalize(), evenIfInvincible: true, registerDeathInStats: false);
            playerDeadBody.DeathAction = End;
            playerDeadBody.ActionDelay = 0.3f;
        }
    }

    private void OnPlayerBounce(Player player)
    {
        Level level = SceneAs<Level>();
        if (isMadeline)
        {
            player.Bounce(base.Top - 2f);
            float angle = player.Speed.Angle();
            level.ParticlesFG.Emit(seekerParticle, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
            Audio.Play("event:/game/general/thing_booped", Position);
            Audio.Play("event:/game/05_mirror_temple/seeker_booped", Position);
        }
    }
    private void End()
    {
        Level level = base.Scene as Level;
        level.OnEndOfFrame += () =>
        {
            Glitch.Value = 0f;
            Distort.Anxiety = 0f;
            level.Session.ColorGrade = null;
            level.UnloadLevel();
            level.CanRetry = true;
            level.Session.Level = nextRoom;
            level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
            level.LoadLevel(Player.IntroTypes.WakeUp);
            Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
        };
    }
    public override void Update()
    {
        foreach (Entity entity2 in base.Scene.Tracker.GetEntities<SeekerBarrier>())
            entity2.Collidable = true;
        Level level = base.Scene as Level;
        Player player = Scene.Tracker.GetEntity<Player>();
        base.Update();
        if (speed != Vector2.Zero)
            justRespawned = false;

        SquishCallback = (CollisionData d) =>
        {
            if (!TrySquishWiggle(d, 3, 3))
            {
                Entity entity = new Entity(Position);
                PlayerSeekerDie();

            }
        };
        if (player != null)
        {
            if (player.Position.Y > level.Bounds.Bottom + 16)
                PlayerSeekerDie();
            cameraTarget = player.Position;
            sprite.Scale.X = Calc.Approach(sprite.Scale.X, 1f, 2f * Engine.DeltaTime * 1.5f);
            sprite.Scale.Y = Calc.Approach(sprite.Scale.Y, 1f, 2f * Engine.DeltaTime * 1.5f);
            if (!isMadeline)
            {
                if (enabled && sprite.CurrentAnimationID != "hatch")
                {
                    if (dashTimer > 0f)
                    {
                        speed = Calc.Approach(speed, Vector2.Zero, 800f * Engine.DeltaTime);
                        dashTimer -= Engine.DeltaTime;
                        if (dashTimer <= 0f)
                            sprite.Play("spotted");
                        if (trailTimerA > 0f)
                        {
                            trailTimerA -= Engine.DeltaTime;
                            if (trailTimerA <= 0f)
                                CreateTrail();
                        }
                        if (trailTimerB > 0f)
                        {
                            trailTimerB -= Engine.DeltaTime;
                            if (trailTimerB <= 0f)
                                CreateTrail();
                        }
                        if (base.Scene.OnInterval(0.04f))
                        {
                            Vector2 vector = speed.SafeNormalize();
                            SceneAs<Level>().Particles.Emit(Seeker.P_Attack, 2, Position + vector * 4f, Vector2.One * 4f, vector.Angle());
                        }
                    }
                    else
                    {
                        Vector2 vector2 = Input.Aim.Value.SafeNormalize();
                        speed += vector2 * 600f * Engine.DeltaTime * speedMultiplier;
                        float num = (speed).Length();
                        if (num > 120f)
                        {
                            num = Calc.Approach(num, 120f, Engine.DeltaTime * 700f);
                            speed = speed.SafeNormalize(num);
                        }
                        if (vector2.Y == 0f)
                            speed.Y = Calc.Approach(speed.Y, 0f, 400f * Engine.DeltaTime);
                        if (vector2.X == 0f)
                            speed.X = Calc.Approach(speed.X, 0f, 400f * Engine.DeltaTime);
                        if (vector2.Length() > 0f && sprite.CurrentAnimationID == "idle")
                        {
                            if (vanillaEffects)
                            {
                                level.Displacement.AddBurst(Position, 0.5f, 8f, 32f);
                                sprite.Play("spotted");
                                Audio.Play("event:/game/05_mirror_temple/seeker_playercontrolstart");
                            }
                        }
                        int num2 = Math.Sign((int)facing);
                        int num3 = Math.Sign(speed.X);
                        if (num3 != 0 && num2 != num3 && Math.Sign(Input.Aim.Value.X) == Math.Sign(speed.X) && Math.Abs(speed.X) > 20f && sprite.CurrentAnimationID != "flipMouth" && sprite.CurrentAnimationID != "flipEyes")
                            sprite.Play("flipMouth");
                        if (Input.Dash.Pressed && canDash)
                            Dash(Input.Aim.Value.EightWayNormal());
                    }
                    if (!isDying || !hasDied)
                    {
                        MoveH(speed.X * Engine.DeltaTime, OnCollide);
                        MoveV(speed.Y * Engine.DeltaTime, OnCollide);
                    }
                    foreach (var hazardType in hazardTypes)
                    {
                        // Get all entities in the scene
                        foreach (Entity hazardEntity in base.Scene.Entities)
                        {
                            // Check if the entity matches the current hazard type
                            if (hazardEntity.GetType() == hazardType && CollideCheck(hazardEntity))
                            {
                                PlayerSeekerDie();
                                break; // Exit the loop if a collision is detected
                            }
                        }
                    }
                    foreach (Entity entity2 in base.Scene.Entities)
                    {
                        if (entity2 is Puffer puffer) // Eat the fishes
                        {
                            if (CollideCheck(puffer) && puffer.state == Puffer.States.Idle)
                            {
                                puffer.Explode();
                                puffer.RemoveSelf();
                                break;
                            }
                        }
                        if (entity2 is CoreModeToggle coreModeToggle) // Changes core mode
                        {
                            if (CollideCheck(coreModeToggle))
                            {
                                if (level.Session.CoreMode == CoreModes.Cold)
                                {
                                    level.Session.CoreMode = CoreModes.Hot;
                                    coreModeToggle.OnPlayer(player);
                                    break;
                                }
                                if (level.Session.CoreMode == CoreModes.Hot)
                                {
                                    level.Session.CoreMode = CoreModes.Cold;
                                    coreModeToggle.OnPlayer(player);
                                    break;
                                }
                            }
                        }
                        if (entity2 is Water water) // Can't dash inside water
                        {
                            if (CollideCheck(water))
                            {
                                canDash = false;
                                break;
                            }
                            else
                            {
                                canDash = true;
                                break;
                            }
                        }
                        if (entity2 is Seeker seeker)
                        {
                            if (CollideCheck(seeker))
                            {
                                seeker.GotBouncedOn(this);
                                break;
                            }
                        }
                        if (entity2 is FlagTrigger flagTrigger)
                        {
                            if (CollideCheck(flagTrigger))
                            {
                                flagTrigger.OnEnter(player);
                                flagTrigger.OnLeave(player);
                                break;
                            }
                        }
                        if (entity2 is TouchSwitch touchSwitch)
                        {
                            if (CollideCheck(touchSwitch))
                            {
                                touchSwitch.TurnOn();
                                break;
                            }
                        }
                        if (entity2 is Bumper bumper)
                        {
                            if (CollideCheck(bumper))
                            {
                                if (!fireMode)
                                {
                                    bumper.OnPlayer(player);
                                    ExplodeLaunch(bumper.Position);
                                }
                                if (fireMode)
                                    PlayerSeekerDie();
                                break;
                            }
                        }
                        if (entity2 is Booster booster)
                        {
                            if (CollideCheck(booster))
                            {
                                if (!booster.red)
                                {
                                    level.ParticlesFG.Emit(boosterParticle, 5, booster.Position, Vector2.One * 4f, speed.Angle() - (float)Math.PI / 2f);
                                    Audio.Play("event:/game/05_mirror_temple/redbooster_end");
                                    level.Lighting.Alpha -= 0.03f;
                                }
                                else
                                {
                                    level.ParticlesFG.Emit(boosterRedParticle, 5, booster.Position, Vector2.One * 4f, speed.Angle() - (float)Math.PI / 2f);
                                    Audio.Play("event:/game/04_cliffside/greenbooster_end");
                                    level.Lighting.Alpha -= 0.03f;
                                }
                                booster.RemoveSelf();
                                break;
                            }
                        }
                        if (entity2 is FlyFeather feather)
                        {
                            if (CollideCheck(feather))
                            {
                                level.ParticlesFG.Emit(featherParticle, 5, feather.Position, Vector2.One * 4f, speed.Angle() - (float)Math.PI / 2f);
                                Audio.Play("event:/game/06_reflection/badeline_feather_slice", feather.Position);
                                feather.RemoveSelf();
                                speedMultiplier += speedMultiplier * 0.1f;
                                break;
                            }
                        }
                    }
                    Position = Position.Clamp(level.Bounds.X, level.Bounds.Y, level.Bounds.Right, level.Bounds.Bottom);
                    Player entity = base.Scene.Tracker.GetEntity<Player>();
                    if (entity != null)
                    {
                        float num4 = (Position - entity.Position).Length();
                        if (num4 < 200f && entity.Sprite.CurrentAnimationID == "asleep" && !isFriendly)
                        {
                            entity.Sprite.Rate = 2f;
                            entity.Sprite.Play("wakeUp");
                        }
                        else if (num4 < 100f && entity.Sprite.CurrentAnimationID != "wakeUp" && !isFriendly)
                        {
                            entity.Sprite.Rate = 1f;
                            entity.Sprite.Play("runFast");
                            entity.Facing = ((!(base.X > entity.X)) ? Facings.Right : Facings.Left);
                        }
                        if (num4 < 50f && dashTimer <= 0f)
                        {
                            if ((!isDying || !hasDied) && !isFriendly)
                                Dash((entity.Center - base.Center).SafeNormalize());
                        }
                        Camera camera = level.Camera;
                        Vector2 cameraTarget = CameraTarget;
                        if (!isMadeline)
                        {
                            camera.Position += (cameraTarget - camera.Position) * (1f - (float)Math.Pow(0.009999999776482582, Engine.DeltaTime));
                            player.Sprite.Play("asleep");
                            player.StateMachine.State = 11;
                            player.StateMachine.Locked = false;
                            player.DummyAutoAnimate = false;
                        }
                        if (vanillaEffects)
                        {
                            Distort.Anxiety = Calc.ClampedMap(num4, 0f, 200f, 0.25f, 0f) + Calc.Random.NextFloat(0.05f);
                            Distort.AnxietyOrigin = (new Vector2(entity.X, level.Camera.Top) - level.Camera.Position) / new Vector2(320f, 180f);
                        }
                    }
                }
                foreach (Entity entity3 in base.Scene.Tracker.GetEntities<SeekerBarrier>())
                {
                    entity3.Collidable = false;
                }
            }
            if (KoseiHelperModule.Settings.SwapCharacter.Pressed)
                swapCharacters(cameraTarget, player);
        }
    }
    private void CreateTrail()
    {
        Vector2 scale = sprite.Scale;
        sprite.Scale.X *= (float)facing;
        TrailManager.Add(this, Seeker.TrailColor, 1);
        sprite.Scale = scale;
    }
    private void OnCollide(CollisionData data)
    {
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (dashTimer <= 0f)
        {
            if (data.Direction.X != 0f)
            {
                speed.X = 0f;
            }
            if (data.Direction.Y != 0f)
            {
                speed.Y = 0f;
            }
            return;
        }
        float direction;
        Vector2 position;
        Vector2 positionRange;
        if (data.Direction.X > 0f)
        {
            direction = MathF.PI;
            position = new Vector2(base.Right, base.Y);
            positionRange = Vector2.UnitY * 4f;
        }
        else if (data.Direction.X < 0f)
        {
            direction = 0f;
            position = new Vector2(base.Left, base.Y);
            positionRange = Vector2.UnitY * 4f;
        }
        else if (data.Direction.Y > 0f)
        {
            direction = -MathF.PI / 2f;
            position = new Vector2(base.X, base.Bottom);
            positionRange = Vector2.UnitX * 4f;
        }
        else
        {
            direction = MathF.PI / 2f;
            position = new Vector2(base.X, base.Top);
            positionRange = Vector2.UnitX * 4f;
        }
        SceneAs<Level>().Particles.Emit(Seeker.P_HitWall, 12, position, positionRange, direction);
        if (data.Hit is SeekerBarrier)
        {
            (data.Hit as SeekerBarrier).OnReflectSeeker();
            Logger.Debug(nameof(KoseiHelperModule), $"Seeker barrier hit. Barrier Right: {data.Hit.Right} Barrier Left: {data.Hit.Left}");
            Logger.Debug(nameof(KoseiHelperModule), $"Seeker barrier hit. Seeker Right: {Right} Seeker Left: {Left}");
            Audio.Play("event:/game/05_mirror_temple/seeker_hit_lightwall", Position);
            if (data.Hit.Left > Left + 10 || data.Hit.Right < Right - 10 || data.Hit.Top > Top + 10 || data.Hit.Bottom < Bottom - 10)
                PlayerSeekerDie();
        }
        else
        {
            Audio.Play("event:/game/05_mirror_temple/seeker_hit_normal", Position);
        }
        if (data.Direction.X != 0f)
        {
            speed.X *= -0.8f;
            if (sprite.Scale.X < 1.8 && sprite.Scale.Y < 1.8)
                sprite.Scale = new Vector2(sprite.Scale.X * 0.6f, sprite.Scale.Y * 1.4f);
        }
        else if (data.Direction.Y != 0f)
        {
            speed.Y *= -0.8f;
            if (sprite.Scale.X < 1.8 && sprite.Scale.Y < 1.8)
                sprite.Scale = new Vector2(sprite.Scale.X * 1.4f, sprite.Scale.Y * 0.6f);
        }

        if (data.Hit is TempleCrackedBlock)
        {
            global::Celeste.Celeste.Freeze(0.15f);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            (data.Hit as TempleCrackedBlock).Break(Position);
        }
        if (data.Hit is DashBlock)
        {
            global::Celeste.Celeste.Freeze(0.03f);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            (data.Hit as DashBlock).Break(Position, Position, true);
        }
        if (data.Hit is CrushBlock)
        {
            global::Celeste.Celeste.Freeze(0.03f);
            (data.Hit as CrushBlock).Attack(-data.Direction.FourWayNormal());
        }
        if (data.Hit is BounceBlock)
        {
            (data.Hit as BounceBlock).Break();
        }
        if (data.Hit is DreamBlock)
        {
            if ((data.Hit as DreamBlock).playerHasDreamDash)
                (data.Hit as DreamBlock).DeactivateNoRoutine();
            else
                (data.Hit as DreamBlock).ActivateNoRoutine();
        }
    }
    private void Dash(Vector2 dir)
    {
        if (!isDying || !hasDied)
        {
            if (dashTimer <= 0f)
            {
                CreateTrail();
                trailTimerA = 0.1f;
                trailTimerB = 0.25f;
            }
            dashTimer = 0.3f;
            dashDirection = dir;
            if (dashDirection == Vector2.Zero)
                dashDirection.X = Math.Sign((int)facing);
            if (dashDirection.X != 0f)
                facing = (Facings)Math.Sign(dashDirection.X);
            speed = dashDirection * 400f * speedMultiplier; // Dash Speed
            sprite.Play("attacking");
            SceneAs<Level>().DirectionalShake(dashDirection);
            foreach (DashListener component in base.Scene.Tracker.GetComponents<DashListener>())
            {
                if (component.OnDash != null)
                    component.OnDash(dashDirection);
            }
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            Audio.Play(seekerDash, Position);
            if (dashDirection.X == 0f && sprite.Scale.X < 1.8 && sprite.Scale.Y < 1.8)
                sprite.Scale = new Vector2(sprite.Scale.X * 0.6f, sprite.Scale.Y * 1.4f);
            else
            {
                if (sprite.Scale.X < 1.8 && sprite.Scale.Y < 1.8)
                    sprite.Scale = new Vector2(sprite.Scale.X * 1.4f, sprite.Scale.Y * 0.6f);
            }
        }
    }
    public override void Render()
    {
        if (!SaveData.Instance.Assists.InvisibleMotion || !enabled || !(speed.LengthSquared() > 100f))
        {
            Vector2 position = Position;
            Position += shaker.Value;
            Vector2 scale = sprite.Scale;
            sprite.Scale.X *= (float)facing;
            base.Render();
            Position = position;
            sprite.Scale = scale;
        }
    }

    public void PlayerSeekerDie()
    {
        if (hasDied) return;
        hasDied = true;
        isDying = true;
        speed = Vector2.Zero;
        sprite.Visible = false;
        Collider = null;
        DeathEffect component = new DeathEffect(Seeker.TrailColor, base.Center - Position)
        {
            OnEnd = delegate
            {
                this.RemoveSelf();
            }
        };
        Audio.Play("event:/game/05_mirror_temple/seeker_death", Position);
        this.Add(component);
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (player != null)
            player.Die(Vector2.Zero, true);
    }

    public Vector2 ExplodeLaunch(Vector2 from, bool snapUp = true, bool sidesOnly = false)
    {
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        Celeste.Freeze(0.1f);
        Vector2 vector = (base.Center - from).SafeNormalize(-Vector2.UnitY);
        float num = Vector2.Dot(vector, Vector2.UnitY);
        if (snapUp && num <= -0.7f)
        {
            vector.X = 0f;
            vector.Y = -1f;
        }
        else if (num <= 0.65f && num >= -0.55f)
        {
            vector.Y = 0f;
            vector.X = Math.Sign(vector.X);
        }

        if (sidesOnly && vector.X != 0f)
        {
            vector.Y = 0f;
            vector.X = Math.Sign(vector.X);
        }

        speed = 280f * vector * speedMultiplier;
        if (speed.Y <= 50f)
        {
            speed.Y = Math.Min(-150f, speed.Y);
        }

        if (speed.X != 0f)
        {
            if (Input.MoveX.Value == Math.Sign(speed.X))
            {
                explodeLaunchBoostTimer = 0f;
                speed.X *= 1.2f;
            }
            else
            {
                explodeLaunchBoostTimer = 0.01f;
                explodeLaunchBoostSpeed = speed.X * 1.2f;
            }
        }

        SlashFx.Burst(base.Center, speed.Angle());
        dashTimer = 0.2f;
        return vector;
    }

    public void OnChangeMode(Session.CoreModes coreMode)
    {
        fireMode = coreMode == Session.CoreModes.Hot;
    }

    private void WindMove(Vector2 move)
    {
        Level level = SceneAs<Level>();
        if (justRespawned)
        {
            return;
        }
        if (!hasDied && !isDying)
        {
            if (move.X != 0f)
            {
                windTimeout = 0.2f;
                windDirection.X = Math.Sign(move.X);
                if (!CollideCheck<Solid>(Position + Vector2.UnitX * -Math.Sign(move.X) * 3f))
                {
                    if (move.X < 0f)
                    {
                        move.X = Math.Max(move.X, (float)level.Bounds.Left - (base.ExactPosition.X + base.Collider.Left));
                    }
                    else
                    {
                        move.X = Math.Min(move.X, (float)level.Bounds.Right - (base.ExactPosition.X + base.Collider.Right));
                    }
                    MoveH(move.X);
                }
            }
            if (move.Y == 0f)
            {
                return;
            }
            windTimeout = 0.2f;
            windDirection.Y = Math.Sign(move.Y);
            if (!(base.Bottom > (float)level.Bounds.Top) || (!(speed.Y < 0f)))
            {
                return;
            }
            if (!(move.Y > 0f))
            {
                return;
            }
            move.Y *= 0.4f;
            if (move.Y < 0f)
            {
                windMovedUp = true;
            }
            MoveV(move.Y);
        }
    }

    public void swapCharacters(Vector2 cameraTarget, Player player)
    {
        if (canSwitchCharacters)
        {
            Logger.Debug(nameof(KoseiHelperModule), $"Characters swapped! Playing as Madeline: {!isMadeline}");
            if (isMadeline) // Become Seeker
            {
                Audio.Play("event:/game/05_mirror_temple/seeker_revive", player.Position);
                sprite.Play("spot");
                SceneAs<Level>().Session.SetFlag("kosei_PlayerSeeker", true);
                player.Sprite.Play("asleep");
                player.StateMachine.State = 11;
                player.StateMachine.Locked = true;
                player.DummyAutoAnimate = false;
                isMadeline = false;
            }
            else // Become Madeline
            {
                Audio.Play("event:/game/05_mirror_temple/seeker_revive", Position);
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 2f, start: true);
                tween.OnUpdate = (Tween f) =>
                {
                    Vector2 cameraTarget = CameraTarget;
                    SceneAs<Level>().Camera.Position = SceneAs<Level>().Camera.Position + (cameraTarget - SceneAs<Level>().Camera.Position) * f.Eased;
                };
                sprite.Play("takeHit");
                SceneAs<Level>().Session.SetFlag("kosei_PlayerSeeker", false);
                player.StateMachine.State = 0;
                player.StateMachine.Locked = false;
                player.DummyAutoAnimate = true;
                isMadeline = true;
            }
        }
    }
}