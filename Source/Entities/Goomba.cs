using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Reflection.Metadata;

namespace Celeste.Mod.KoseiHelper.Entities;
public enum GoombaBehavior
{
    Chaser,
    Dumb,
    Smart
}

[CustomEntity("KoseiHelper/Goomba")]
[Tracked]
public class Goomba : Actor
{
    private Sprite sprite;
    private Collider bounceCollider;
    private int walkDirection = 1;
    private Collision onCollideH = null;
    private Collision onCollideV = null;
    public float speedX = 50;
    public GoombaBehavior behavior;
    public float originalSpeedX;
    public float speedY;
    public bool outline;
    public float noGravityTimer;
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
    public static ParticleType goombaParticle = Player.P_Split;
    public bool canEnableTouchSwitches;
    public Goomba(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
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
        canEnableTouchSwitches = data.Bool("canEnableTouchSwitches", false);
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
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_goomba"));
        sprite.Play("idle");
        Add(sine = new SineWave(2f, 0f));
    }
    // this ctor is used for the minigoombas when a goomba can spawn minis
    public Goomba(Vector2 position, float newSpeed, bool newOutline, bool newIsWide, bool newIsWinged, bool newCanBeBounced) : base(position)
    {
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
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        Add(rotateWiggler = Wiggler.Create(0.5f, 4f, (float v) =>
        {
            sprite.Rotation = v * 30f * (MathF.PI / 180f);
        }));
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();
        if (Scene.OnInterval(timeToSpawnMinis) && canSpawnMinis && minisSpawned < 10)
        {
            this.Scene.Add(new Goomba(new Vector2(this.Position.X, this.Position.Y + 6), originalSpeedX + originalSpeedX / 4, false, false, false, true));
            minisSpawned += 1;
        }
        if (speedX != 0)
            if (!isWide)
                if (!isWinged)
                    sprite.Play("walk");
                else
                    sprite.Play("winged");
            else
                if (!isWinged)
                sprite.Play("widewalk");
            else
                sprite.Play("widewinged");
        else
            if (!isWide)
            if (!isWinged)
                sprite.Play("idle");
            else
                sprite.Play("wingedidle");
        else
                if (!isWinged)
            sprite.Play("wideidle");
        else
            sprite.Play("widewingedidle");
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
        if (behavior == GoombaBehavior.Smart)
        {
            if (CollidingWithGround(Position + new Vector2(0, 2)) && 
                (walkDirection > 0 && !CollidingWithGround(Position + new Vector2(10, 2)) || walkDirection < 0 && !CollidingWithGround(Position + new Vector2(-10, 2))))
                walkDirection = -walkDirection;
        }
        if (Scene.Tracker.GetEntity<Player>() != null)
        {
            if (behavior == GoombaBehavior.Chaser)
            {
                walkDirection = (int)(Scene.Tracker.GetEntity<Player>().Position.X - this.Position.X);
                MoveH(speedX * Math.Sign(walkDirection) * Engine.DeltaTime, onCollideH);
            }
            else
            {
                MoveH(speedX * Math.Sign(walkDirection) * Engine.DeltaTime, onCollideH);
            }
            if (!isWinged)
                MoveVExact((int)(speedY * Engine.DeltaTime * gravityMult), onCollideV);
        }

        foreach (Spring spring in SceneAs<Level>().Entities.FindAll<Spring>())
        {
            if (CollideCheck(spring))
            {
                Audio.Play("event:/game/general/spring", spring.BottomCenter);
                spring.sprite.Play("bounce", restart: true);
                spring.wiggler.Start();
                HitSpring(spring);
            }
        }

        if (base.Bottom > (float)level.Bounds.Bottom + 16)
            this.RemoveSelf();
        base.Update();

        if (isWinged && flyAway) // flyAway behavior
        {
            MoveVExact((int)Engine.DeltaTime, onCollideV);
            if (flyingAway)
            {
                MoveVExact((int)(flapSpeed * Engine.DeltaTime), onCollideV);
                if (base.Y < (float)(SceneAs<Level>().Bounds.Top - 16))
                    RemoveSelf();
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

        if (CollideCheck<Solid>())
        {
            RemoveSelf();
        }
    }

    private void OnPlayer(Player player)
    {
        if (player.Scene != null)
        {
            player.Die(player.Center);
        }
    }

    private void OnPlayerBounce(Player player)
    {
        Level level = SceneAs<Level>();
        Celeste.Freeze(0.05f);
        player.Bounce(base.Top - 2f);
        this.RemoveSelf();
        float angle = player.Speed.Angle();
        level.ParticlesFG.Emit(goombaParticle, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
        Audio.Play("event:/KoseiHelper/goomba", Position);
    }

    public override void Render()
    {
        if (outline)
            sprite.DrawOutline();
        base.Render();
    }

    public bool HitSpring(Spring spring)
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
            if (behavior == GoombaBehavior.Chaser)
                speedX = -220f;
            if (behavior != GoombaBehavior.Chaser)
            {
                speedX = 220f;
                walkDirection = -walkDirection;
            }
            speedY = -500f;
            noGravityTimer = 0.1f;
            return true;
        }
        if (spring.Orientation == Spring.Orientations.WallRight)
        {
            MoveTowardsY(spring.CenterY + 5f, 4f);
            if (behavior == GoombaBehavior.Chaser)
            speedX = -220f;
            if (behavior != GoombaBehavior.Chaser)
            {
                speedX = 220f;
                walkDirection = -walkDirection;
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
        Celeste.Freeze(0.05f);
        this.RemoveSelf();
        float angle = player.Speed.Angle();
        level.ParticlesFG.Emit(goombaParticle, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
    }

    public void EnableTouchSwitch()
    {
        foreach (Entity entity in Scene.Entities)
            if (entity is TouchSwitch touchSwitch)
            {
                if (CollideCheck(touchSwitch) && !touchSwitch.Switch.Activated)
                    touchSwitch.TurnOn();
            }
    }
}