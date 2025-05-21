using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;
using Celeste.Mod.MaxHelpingHand.Entities;

namespace Celeste.Mod.KoseiHelper.Entities;
public enum GoombaBehavior
{
    Chaser,
    Dumb,
    Smart,
    SuperSmart
}

[CustomEntity("KoseiHelper/Goomba")]
[Tracked]
public class Goomba : Actor
{
    private Sprite sprite;
    private Collider bounceCollider;
    private int walkDirection = 1;
    private Collision onCollideH;
    private Collision onCollideV = null;
    public float speedX = 50;
    public GoombaBehavior behavior;
    public float originalSpeedX;
    public float speedY;
    public bool outline;
    public float noGravityTimer, springTimer;
    public bool isWide;
    public bool isWinged;
    public bool canBeBounced;
    public bool canSpawnMinis;
    public float flapSpeed;
    private Wiggler rotateWiggler;
    private bool flyingAway;
    public bool flyAway;
    public float timeToSpawnMinis;
    public float gravityMult = 1f;
    private SineWave sine;
    public int minisSpawned = 0;
    public ParticleType goombaParticle;
    public bool canEnableTouchSwitches;
    public string deathSound;
    public bool isBaby;
    public int springDirection;
    public int minisAmount;
    public bool slowdown;
    public float slowdownDistanceMax, slowdownDistanceMin;

    private float number2 = -1f;
    private float number3, randomAnxietyOffset;

    public string spriteID;
    public bool deathAnimation;
    public string flagOnDeath;
    public Color color;
    public Color particleColor;

    private float previousTimeRate, previousGameRate, previousAnxiety;
    private bool dead = false;
    
    public Goomba(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        isBaby = false;
        Depth = -1;
        originalSpeedX = speedX = data.Float("speed", 50f);
        if (!isWinged)
            speedY = 60;
        else
            speedY = 0;
        outline = data.Bool("outline", false);
        isWide = data.Bool("isWide", false);
        isWinged = data.Bool("isWinged", false);
        canBeBounced = data.Bool("canBeBounced", true);
        canSpawnMinis = data.Bool("spawnMinis", true);
        flyAway = data.Bool("flyAway", false);
        behavior = data.Enum("behavior", GoombaBehavior.Chaser);
        timeToSpawnMinis = data.Float("timeToSpawnMinis", 1);
        gravityMult = data.Float("gravityMultiplier", 1f);
        slowdown = data.Bool("slowdown", false);
        canEnableTouchSwitches = data.Bool("canEnableTouchSwitches", false);
        deathSound = data.Attr("deathSound", "event:/KoseiHelper/goomba");
        minisAmount = data.Int("minisAmount", 10);
        color = data.HexColor("color", Color.White);
        particleColor = data.HexColor("particleColor", Player.P_Split.Color); // aka ff6def
        slowdownDistanceMax = data.Float("slowdownDistanceMax", 40f);
        slowdownDistanceMin = data.Float("slowdownDistanceMin", 16f);
        if (!isWide)
        {
            Collider = new Hitbox(13, 12, -7, -4);
            if (canBeBounced)
                bounceCollider = new Hitbox(13, 4, -7, -8);
        }
        else
        {
            Collider = new Hitbox(16, 12, -8, -4);
            if (canBeBounced)
                bounceCollider = new Hitbox(16, 4, -8, -8);
        }
        if (isWinged)
            Add(new DashListener { OnDash = OnDashFlyAway });
        Add(new PlayerCollider(OnPlayer));
        Add(new PlayerCollider(OnPlayerBounce, bounceCollider));
        spriteID = data.Attr("spriteID", "koseiHelper_goomba");
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_goomba"));
        sprite.Color = color;
        sprite.Play("idle");
        deathAnimation = data.Bool("deathAnimation", false);
        flagOnDeath = data.Attr("flagOnDeath", "");
        Add(sine = new SineWave(2f, 0f));
        sprite.OnFinish = anim =>
        {
            CeasedToExist();
        };
    }
    // this ctor is used for the minigoombas when a goomba can spawn minis
    public Goomba(Vector2 position, float newSpeed, bool newOutline, bool newIsWide, bool newIsWinged, bool newCanBeBounced) : base(position)
    {
        isBaby = true;
        Depth = -1;
        originalSpeedX = speedX = newSpeed;
        if (!isWinged)
            speedY = 60;
        else
            speedY = 0;
        outline = newOutline;
        isWide = false;
        isWinged = newIsWinged;
        canBeBounced = newCanBeBounced;
        canSpawnMinis = false;
        Collider = new Hitbox(8, 4, -4, 0);
        if (canBeBounced)
            bounceCollider = new Hitbox(8, 8, -4, -8);
        Add(new PlayerCollider(OnPlayer));
        Add(new PlayerCollider(OnPlayerBounce, bounceCollider));
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_goomba"));
        sprite.Play("idle");
        sprite.Scale = new Vector2(0.5f, 0.5f);
        sprite.OnFinish = anim =>
        {
            CeasedToExist();
        };
    }

    public override void Added(Scene scene)
    {
        goombaParticle = new ParticleType(Player.P_Split)
        {
            Color = particleColor,
            Color2 = particleColor
        };
        base.Added(scene);
        Level level = SceneAs<Level>();
        previousTimeRate = Engine.TimeRate;
        previousGameRate = Distort.GameRate;
        previousAnxiety = Distort.Anxiety;
        Add(rotateWiggler = Wiggler.Create(0.5f, 4f, (float v) =>
        {
            sprite.Rotation = v * 30f * (MathF.PI / 180f);
        }));
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (slowdown)
        {
            if (base.Scene.OnInterval(0.05f))
            {
                randomAnxietyOffset = Calc.Random.Range(-0.2f, 0.2f);
            }
            float target;
            if (player != null && !player.Dead)
            {
                foreach (Goomba entity2 in base.Scene.Tracker.GetEntities<Goomba>())
                {
                    float num3 = Vector2.DistanceSquared(player.Center, entity2.Center);
                    float distance = Vector2.Distance(player.Center, entity2.Center);
                    if (Vector2.Distance(player.Center, entity2.Center) < slowdownDistanceMax)
                        number2 = (!(number2 < 0f)) ? Math.Min(number2, num3) : num3; // Stops applying the slowdown effect if outside of radius
                    else
                        number2 = -1f;
                }
                // By default clamped to 16^2, 40^2
                target = (!(number2 >= 0f)) ? 1f : Calc.ClampedMap(number2, (float)Math.Pow(slowdownDistanceMin, 2), (float)Math.Pow(slowdownDistanceMax,2), 0.5f);
                Distort.AnxietyOrigin = new Vector2((player.Center.X - Position.X) / 320f, (player.Center.Y - Position.Y) / 180f);
                number3 = ((!(-1 >= 0f)) ? 0f : Calc.ClampedMap(-1, 256f, 16384f, 1f, 0f));
            }
            else
            {
                target = 1f;
                number3 = 0f;
            }
            Engine.TimeRate = Calc.Approach(Engine.TimeRate, target, 4f * Engine.DeltaTime);
            Distort.GameRate = Calc.Approach(Distort.GameRate, Calc.Map(Engine.TimeRate, 0.5f, 1f), Engine.DeltaTime * 2f);
            Distort.Anxiety = Calc.Approach(Distort.Anxiety, (0.5f + randomAnxietyOffset) * number3, 8f * Engine.DeltaTime);
        }
        if (Scene.OnInterval(timeToSpawnMinis) && canSpawnMinis && minisSpawned < minisAmount)
        {
            this.Scene.Add(new Goomba(new Vector2(this.Position.X, this.Position.Y + 4), originalSpeedX + originalSpeedX / 4, false, false, false, true));
            minisSpawned += 1;
        }
        if (Collidable & !dead)
        {
            if (speedX != 0)
            {
                if (!isWide)
                {
                    if (!isWinged)
                        sprite.Play("walk");
                    else
                        sprite.Play("winged");
                }
                else
                {
                    if (!isWinged)
                        sprite.Play("widewalk");
                    else
                        sprite.Play("widewinged");
                }
            }
            else
            {
                if (!isWide)
                {
                    if (!isWinged)
                        sprite.Play("idle");
                    else
                        sprite.Play("wingedidle");
                }
                else
                {
                    if (!isWinged)
                        sprite.Play("wideidle");
                    else
                        sprite.Play("widewingedidle");
                }
            }
        }
        if (canEnableTouchSwitches)
            EnableTouchSwitch();
        speedY = Math.Min(speedY * 0.6f, 0f);
        if (speedX != 0f && speedY == 0f && !isWinged)
            speedY = 60f;
        if (OnGround() && speedY < 0f)
            noGravityTimer = 0.15f;
        float num = 800f;
        if (Math.Abs(speedY) <= 30f)
            num *= 0.5f;
        float num2 = 350f;
        if (speedY < 0f)
            num2 *= 0.5f;
        speedX = Calc.Approach(speedX, originalSpeedX, Math.Abs(speedX) > Math.Abs(originalSpeedX) ? num2 * Engine.DeltaTime : num2 * Engine.DeltaTime * 2);
        if (noGravityTimer > 0f)
            noGravityTimer -= Engine.DeltaTime;
        else
            speedY = Calc.Approach(speedY, 200f, num * Engine.DeltaTime);
        if (behavior == GoombaBehavior.Smart || behavior == GoombaBehavior.SuperSmart)
        {
            if (CollidingWithGround(Position + new Vector2(0, 2)) && 
                (walkDirection > 0 && !CollidingWithGround(Position + new Vector2(10, 2)) || walkDirection < 0 && !CollidingWithGround(Position + new Vector2(-10, 2))))
                walkDirection = -walkDirection;
        }
        if (behavior == GoombaBehavior.SuperSmart)
        {
            if (level.CollideCheck<Solid>(CenterLeft + new Vector2(-1,0)) ||
                level.CollideCheck<Solid>(CenterRight))
            {
                walkDirection = -walkDirection;
            }
        }
        if (player != null)
        {
            if (behavior == GoombaBehavior.Chaser)
            {
                if (springTimer == 0)
                {
                    walkDirection = (int)(Scene.Tracker.GetEntity<Player>().Position.X - this.Position.X);
                    MoveH(speedX * Math.Sign(walkDirection) * Engine.DeltaTime, onCollideH);
                }
                else
                    MoveH(speedX * Math.Sign(springDirection) * Engine.DeltaTime, onCollideH);
            }
            else
            {
                MoveH(speedX * Math.Sign(walkDirection) * Engine.DeltaTime, onCollideH);
            }
            if (!isWinged)
                MoveVExact((int)(speedY * Engine.DeltaTime * gravityMult), onCollideV);
        }

        foreach (Spring spring in level.Entities.FindAll<Spring>())
        {
            if (CollideCheck(spring) && springTimer == 0)
            {
                Audio.Play("event:/game/general/spring", spring.BottomCenter);
                spring.sprite.Play("bounce", restart: true);
                spring.wiggler.Start();
                HitSpring(spring);
            }
        }
        if (springTimer > 0)
            springTimer -= Engine.DeltaTime;
        else
            springTimer = 0;

        if (base.Bottom > (float)level.Bounds.Bottom + 16)
        {
            CeaseToExist();
        }
        base.Update();
        if (isWinged && flyAway) // flyAway behavior
        {
            MoveVExact((int)Engine.DeltaTime, onCollideV);
            if (flyingAway)
            {
                MoveVExact((int)(flapSpeed * Engine.DeltaTime), onCollideV);
                if (base.Y < (float)(SceneAs<Level>().Bounds.Top - 16))
                {
                    CeaseToExist();
                }
            }
            else
            {
                flapSpeed = Calc.Approach(flapSpeed, 20f, 170f * Engine.DeltaTime);
            }
        }

        if (isWinged)
        {
            speedY = 0;
            noGravityTimer = 1f;
            MoveV(-30 * Engine.DeltaTime * sine.Value, onCollideV);
        }

        SquishCallback = (CollisionData d) =>
        {
            if (!TrySquishWiggle(d, 2, 2))
            {
                Audio.Play(deathSound, Center);
                CeaseToExist();
            }
        };
    }

    private void CeaseToExist()
    {
        dead = true;
        if (deathAnimation)
        {
            Collidable = false;
            sprite.Play("dead");
            if (slowdown)
            {
                Engine.TimeRate = previousTimeRate;
                Distort.GameRate = previousGameRate;
                Distort.Anxiety = previousAnxiety;
            }
        }
        else
            CeasedToExist();
    }

    private void CeasedToExist()
    {
        dead = true;
        Session session = SceneAs<Level>().Session;
        if (slowdown)
        {
            Engine.TimeRate = previousTimeRate;
            Distort.GameRate = previousGameRate;
            Distort.Anxiety = previousAnxiety;
        }
        if (!string.IsNullOrEmpty(flagOnDeath))
            session.SetFlag(flagOnDeath, true);
        this.RemoveSelf();
    }

    private void OnPlayer(Player player)
    {
        if (player.Scene != null)
            player.Die(player.Center);
    }

    private void OnPlayerBounce(Player player)
    {
        Level level = SceneAs<Level>();
        Celeste.Freeze(0.05f);
        player.Bounce(base.Top - 2f);
        if (!isBaby)
            Audio.Play(deathSound, Position);
        CeaseToExist();
        float angle = player.Speed.Angle();
        level.ParticlesFG.Emit(goombaParticle, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
    }

    public override void Render()
    {
        if (outline)
            sprite.DrawOutline();
        base.Render();
    }

    public bool HitSpring(Spring spring) // I have no clue how this works but it took me HOURS to fix it
    {
        if (spring.Orientation == Spring.Orientations.Floor && speedY >= 0f)
        {
            speedX *= 0.5f;
            speedY = -1000f;
            noGravityTimer = 0.15f;
            return true;
        }
        if (spring.Orientation == Spring.Orientations.WallLeft)
        {
            MoveTowardsY(spring.CenterY + 5f, 4f);
            if (behavior != GoombaBehavior.Chaser)
            {
                if (speedX > 0)
                    speedX = 220f;
                else
                    speedX = 220f;
            }
            else
            {
                springTimer = 0.25f;
                springDirection = 1;
                speedX = 220f;
            }

            speedY = -500f;
            noGravityTimer = 0.1f;
            return true;
        }
        if (spring.Orientation == Spring.Orientations.WallRight)
        {
            MoveTowardsY(spring.CenterY + 5f, 4f);
            if (behavior != GoombaBehavior.Chaser)
            {
                if (speedX > 0)
                    speedX = -220f;
                else
                    speedX = -220f;
            }
            else
            {
                springTimer = 0.25f;
                springDirection = -1;
                speedX = 220f;
            }
            speedY = -500f;
            noGravityTimer = 0.1f;
            return true;
        }
        return false;
    }

    public void OnDashFlyAway(Vector2 dir)
    {
        if (!flyingAway && isWinged && flyAway)
        {
            Add(new Coroutine(FlyAwayCoroutine()));
            flyingAway = true;
        }
    }

    private IEnumerator FlyAwayCoroutine()
    {
        rotateWiggler.Start();
        flapSpeed = -200f;
        Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, start: true);
        tween.OnUpdate = (Tween t) => { flapSpeed = MathHelper.Lerp(-200f, 0f, t.Eased); };
        Add(tween);
        yield return 0.1f;
        Audio.Play("event:/game/general/strawberry_laugh", base.Position);
        yield return 0.2f;
        Audio.Play("event:/game/general/strawberry_flyaway", Position);
        tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.5f, start: true);
        tween.OnUpdate = (Tween t) => { flapSpeed = MathHelper.Lerp(0f, -200f, t.Eased); };
        Add(tween);
    }

    private bool CollidingWithGround(Vector2 position)
    {
        return CollideCheckOutside<Solid>(position) || CollideCheckOutside<Platform>(position);
    }

    public void Killed(Player player, Level level)
    {
        
        if (!isBaby)
        {
            Audio.Play(deathSound, Position);
        }
        Celeste.Freeze(0.05f);
        CeaseToExist();
        float angle = player.Speed.Angle();
        level.ParticlesFG.Emit(goombaParticle, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
    }

    public void EnableTouchSwitch()
    {
        foreach (Entity entity in Scene.Entities)
        {
            if (entity is TouchSwitch touchSwitch)
            {
                if (CollideCheck(touchSwitch) && !touchSwitch.Switch.Activated)
                    touchSwitch.TurnOn();
            }
            if (entity is FlagTouchSwitch flagTouchSwitch)
            {
                if (CollideCheck(flagTouchSwitch) && !flagTouchSwitch.Activated)
                    flagTouchSwitch.TurnOn();
            }
        }
    }
}