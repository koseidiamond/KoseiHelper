using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;
using static Celeste.GaussianBlur;

namespace Celeste.Mod.KoseiHelper.Entities;

public enum PlantType
{
    Jumping,
    Red,
    Green,
    Black
}

public enum PlantDirection
{
    Up,
    Left,
    Right,
    Down
}

[CustomEntity("KoseiHelper/Plant")]
[Tracked]
public class Plant : Actor
{
    public Sprite sprite;
    public PlantType plantType;
    public PlantDirection plantDirection;
    public bool canShoot;
    public float shootSpeed = 0f;
    public Vector2 Speed = Vector2.Zero;
    public bool moving;
    private bool isMoving = false; // to check if the red ones should wait until moving again
    public float movingSpeed = 1f;
    private bool isShooting, isJumping = false;
    private bool isWaitingAtTop = false;
    private bool isGreenMovingUp = false;
    private bool isRedMovingUp = false;
    public int distance = 64;
    public bool cycleOffset = false;

    public Plant(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        plantType = data.Enum("plantType", PlantType.Jumping);
        plantDirection = data.Enum("direction", PlantDirection.Up);
        movingSpeed = data.Float("movingSpeed", 1f);
        canShoot = data.Bool("canShoot", false);
        shootSpeed = data.Float("shootSpeed", 1.5f);
        distance = data.Int("distance", 64);
        cycleOffset = data.Bool("cycleOffset", false);
        Depth = -100;
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_Plant"));

        if (plantType == PlantType.Jumping)
        {
            Collider = new Hitbox(14, 14, -7, -6);
            sprite.Play("JumpingIdle");
        }
        if (plantType == PlantType.Black)
        {
            Collider = new Hitbox(14, 14, -7, -6);
            sprite.Play("Black");
            switch (plantDirection)
            {
                case PlantDirection.Up:
                    Speed = new Vector2(0, 40);
                    break;
                case PlantDirection.Down:
                    Speed = new Vector2(0, -40);
                    break;
                case PlantDirection.Left:
                    Speed = new Vector2(40, 0);
                    break;
                case PlantDirection.Right:
                    Speed = new Vector2(-40, 0);
                    break;
            }
        }
        if (plantType == PlantType.Green)
        {
            switch (plantDirection)
            {
                case PlantDirection.Left:
                    Collider = new Hitbox(30, 16, -14, -8);
                    break;
                case PlantDirection.Right:
                    Collider = new Hitbox(30, 16, -16, -8);
                    break;
                case PlantDirection.Down:
                    Collider = new Hitbox(16, 30, -8, -16);
                    break;
                default:
                    Collider = new Hitbox(16, 30, -8, -14);
                    break;
            }
            if (!canShoot)
                sprite.Play("GreenIdle");
            else
                sprite.Play("GreenShootDown");
        }
        if (plantType == PlantType.Red)
        {
            switch (plantDirection)
            {
                case PlantDirection.Left:
                    Collider = new Hitbox(30, 16, -14, -8);
                    break;
                case PlantDirection.Right:
                    Collider = new Hitbox(30, 16, -16, -8);
                    break;
                case PlantDirection.Down:
                    Collider = new Hitbox(16, 30, -8, -16);
                    break;
                default:
                    Collider = new Hitbox(16, 30, -8, -14);
                    break;
            }
            if (!canShoot)
                sprite.Play("RedIdle");
            else
                sprite.Play("RedShootDown");
        }
        if (plantDirection == PlantDirection.Left)
            sprite.Rotation = -(float)Math.PI / 2;
        if (plantDirection == PlantDirection.Right)
        {
            sprite.Rotation = -(float)Math.PI * 1.5f;
            sprite.FlipX = true;
        }
        if (plantDirection == PlantDirection.Down)
        {
            sprite.Rotation = -(float)Math.PI;
            sprite.FlipX = true;
        }

        Add(new PlayerCollider(OnPlayer));
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (Top > level.Bounds.Bottom || Left > level.Bounds.Right || Right < level.Bounds.Left) // || Bottom < level.Bounds.Top)
            RemoveSelf();
        if (plantType == PlantType.Jumping)
        {
            if (plantDirection == PlantDirection.Up || plantDirection == PlantDirection.Down)
                MoveV(Speed.Y * Engine.DeltaTime * movingSpeed);
            if (plantDirection == PlantDirection.Left || plantDirection == PlantDirection.Right)
                MoveH(Speed.X * Engine.DeltaTime * movingSpeed);
            switch (plantDirection)
                {
                    case PlantDirection.Down:
                        // slight falling acceleration until it reaches max fastfall speed
                        Speed.Y += Engine.DeltaTime * -160;
                        Speed.Y = Calc.Clamp(Speed.Y, -180, 150);
                        if (CollidingWithGround(TopCenter + new Vector2(0, -2)))
                            sprite.Play("JumpingIdle");
                        else
                            sprite.Play("Jumping");
                        break;
                    case PlantDirection.Left:
                        Speed.X += Engine.DeltaTime * 160;
                        Speed.X = Calc.Clamp(Speed.X, -150, 180);
                        if (CollidingWithGround(CenterRight + new Vector2(2, 0)))
                            sprite.Play("JumpingIdle");
                        else
                            sprite.Play("Jumping");
                        break;
                    case PlantDirection.Right:
                        Speed.X += Engine.DeltaTime * -160;
                        Speed.X = Calc.Clamp(Speed.X, -180, 150);
                        if (CollidingWithGround(CenterLeft + new Vector2(-2, 0)))
                            sprite.Play("JumpingIdle");
                        else
                            sprite.Play("Jumping");
                        break;
                    default:
                        Speed.Y += Engine.DeltaTime * 160;
                        Speed.Y = Calc.Clamp(Speed.Y, -150, 180);
                        if (CollidingWithGround(BottomCenter + new Vector2(0, 2)))
                            sprite.Play("JumpingIdle");
                        else
                            sprite.Play("Jumping");
                        break;
                }
            if (player != null)
            {
                if (!player.JustRespawned && !player.IsIntroState && !isJumping)
                { switch (plantDirection)
                    {
                        case PlantDirection.Down:
                            if (player.Right > Left && player.Left < Right && player.Top > Top - Math.Abs(distance) && player.Bottom > Top &&
                                CollidingWithGround(TopCenter + new Vector2(0, -2)))
                            {
                                Add(new Coroutine(Jump()));
                            }
                            break;
                        case PlantDirection.Left:
                            if (player.Bottom > Top && player.Top < Bottom && player.Right > Left - Math.Abs(distance) && player.Left < Right &&
                                CollidingWithGround(CenterRight + new Vector2(2, 0)))
                            {
                                Add(new Coroutine(Jump()));
                            }
                            break;
                        case PlantDirection.Right:
                            if (player.Bottom > Top && player.Top < Bottom && player.Left < Right + Math.Abs(distance) && player.Right > Left &&
                                CollidingWithGround(CenterLeft + new Vector2(-2, 0)))
                            {
                                Add(new Coroutine(Jump()));
                            }
                            break;
                        default:
                            if (player.Right > Left && player.Left < Right && player.Bottom >= Top - Math.Abs(distance) && player.Top < Bottom &&
                                CollidingWithGround(BottomCenter + new Vector2(0, 2)))
                            {
                                Add(new Coroutine(Jump()));
                            }
                            break;
                    }
                }
                bool flipX = false;
                if (player.Center.X > Center.X)
                    flipX = (plantDirection == PlantDirection.Up);
                else
                    flipX = (plantDirection == PlantDirection.Down);

                if (player.Center.Y > Center.Y)
                    flipX = (plantDirection == PlantDirection.Right) ? !flipX : flipX;
                else
                    flipX = (plantDirection == PlantDirection.Left) ? !flipX : flipX;
                sprite.FlipX = flipX;
            }
        }

        if (plantType == PlantType.Black)
        {
            if (plantDirection == PlantDirection.Up || plantDirection == PlantDirection.Down)
                MoveV(Speed.Y * Engine.DeltaTime * movingSpeed);
            if (plantDirection == PlantDirection.Left || plantDirection == PlantDirection.Right)
                MoveH(Speed.X * Engine.DeltaTime * movingSpeed);
            switch (plantDirection)
            {
                case PlantDirection.Down:
                    Speed.Y += Engine.DeltaTime * -160;
                    Speed.Y = Calc.Clamp(Speed.Y, -180, 150);
                    break;
                case PlantDirection.Left:
                    Speed.X += Engine.DeltaTime * 160;
                    Speed.X = Calc.Clamp(Speed.X, -150, 180);
                    break;
                case PlantDirection.Right:
                    Speed.X += Engine.DeltaTime * -160;
                    Speed.X = Calc.Clamp(Speed.X, -180, 150);
                    break;
                default:
                    Speed.Y += Engine.DeltaTime * 160;
                    Speed.Y = Calc.Clamp(Speed.Y, -150, 180);
                    break;
            }
            sprite.Play("Black");
        }
        if (plantType == PlantType.Green && player != null)
        {
            if (!moving)
                Add(new Coroutine(MovingCycle()));
            switch (plantDirection)
            {
                case PlantDirection.Left:
                    if (player.Center.Y > Center.Y)
                        sprite.FlipX = false;
                    else
                        sprite.FlipX = true;
                    if (canShoot && player.CenterX < Left + 4)
                        sprite.Play("GreenShootUp");
                    if (canShoot && player.CenterX > Left + 4)
                        sprite.Play("GreenShootDown");
                        break;
                case PlantDirection.Right:
                    if (player.Center.Y > Center.Y)
                        sprite.FlipX = true;
                    else
                        sprite.FlipX = false;
                    if (canShoot && player.CenterX > Right - 4)
                        sprite.Play("GreenShootUp");
                    if (canShoot && player.CenterX < Right - 4)
                        sprite.Play("GreenShootDown");
                    break;
                case PlantDirection.Down:
                    if (player.Center.X > Center.X)
                        sprite.FlipX = false;
                    else
                        sprite.FlipX = true;
                    if (canShoot && player.CenterY < Bottom -4)
                        sprite.Play("GreenShootDown");
                    if (canShoot && player.CenterY > Bottom - 4)
                        sprite.Play("GreenShootUp");
                    break;
                default:
                    if (player.Center.X > Center.X)
                        sprite.FlipX = true;
                    else
                        sprite.FlipX = false;
                    if (canShoot && player.CenterY < Top + 4)
                        sprite.Play("GreenShootUp");
                    if (canShoot && player.CenterY > Top + 4)
                        sprite.Play("GreenShootDown");
                    break;
            }
        }
        //TODO rotation
        if (plantType == PlantType.Red && player != null)
        {
            switch (plantDirection)
            {
                case PlantDirection.Left:
                    if (Math.Abs(player.Center.Y - Center.Y) <= distance && player.CenterX <= Right)
                    {
                        if (!moving && Scene != null)
                            Add(new Coroutine(MoveUp()));
                    }
                    else
                    {
                        if (moving && Scene != null)
                            Add(new Coroutine(MoveDown()));
                    }

                    if (player.Center.Y > Center.Y)
                        sprite.FlipX = false;
                    else
                        sprite.FlipX = true;
                    if (canShoot && player.CenterX < Left + 4)
                        sprite.Play("RedShootUp");
                    if (canShoot && player.CenterX > Left + 4)
                        sprite.Play("RedShootDown");
                    break;
                case PlantDirection.Right:
                    if (Math.Abs(player.Center.Y - Center.Y) <= distance && player.CenterX >= Left)
                    {
                        if (!moving && Scene != null)
                            Add(new Coroutine(MoveUp()));
                    }
                    else
                    {
                        if (moving && Scene != null)
                            Add(new Coroutine(MoveDown()));
                    }

                    if (player.Center.Y > Center.Y)
                        sprite.FlipX = true;
                    else
                        sprite.FlipX = false;
                    if (canShoot && player.CenterX > Right - 4)
                        sprite.Play("RedShootUp");
                    if (canShoot && player.CenterX < Right - 4)
                        sprite.Play("RedShootDown");
                    break;
                case PlantDirection.Down:
                    if (Math.Abs(player.Center.X - Center.X) <= distance)
                    {
                        if (!moving && Scene != null)
                            Add(new Coroutine(MoveUp()));
                    }
                    else
                    {
                        if (moving && Scene != null)
                            Add(new Coroutine(MoveDown()));
                    }

                    if (player.Center.X > Center.X)
                        sprite.FlipX = false;
                    else
                        sprite.FlipX = true;
                    if (canShoot && player.CenterY < Bottom - 4)
                        sprite.Play("RedShootDown");
                    if (canShoot && player.CenterY > Bottom - 4)
                        sprite.Play("RedShootUp");
                    break;
                default:
                    if (Math.Abs(player.Center.X - Center.X) <= distance)
                    {
                        if (!moving && Scene != null)
                            Add(new Coroutine(MoveUp()));
                    }
                    else
                    {
                        if (moving && Scene != null)
                            Add(new Coroutine(MoveDown()));
                    }

                    if (player.Center.X > Center.X)
                        sprite.FlipX = true;
                    else
                        sprite.FlipX = false;
                    if (canShoot && player.CenterY < Top +4)
                        sprite.Play("RedShootUp");
                    if (canShoot && player.CenterY > Top +4)
                        sprite.Play("RedShootDown");
                    break;
            }
        }

        if (player != null && canShoot && !player.JustRespawned && !player.IsIntroState)
        {
            if (plantType == PlantType.Green && isWaitingAtTop)
            {
                if (!isShooting)
                {
                    isShooting = true;
                    Add(new Coroutine(ShootCycle()));
                }
            }
            if (plantType == PlantType.Red && isWaitingAtTop)
            {
                if (!isShooting)
                {
                    isShooting = true;
                    Add(new Coroutine(ShootCycle()));
                }
            }
        }
    }

    private IEnumerator Jump() // For the white ones
    {
        isJumping = true;
        switch (plantDirection)
        {
            case PlantDirection.Down:
                Speed.Y = 100f;
                yield return 0.5f;
                Speed.Y = -100f;
                yield return 0.5f;
                isJumping = false;
                yield break;
            case PlantDirection.Left:
                Speed.X = -100f;
                yield return 0.5f;
                Speed.X = 100f;
                yield return 0.5f;
                isJumping = false;
                yield break;
            case PlantDirection.Right:
                Speed.X = 100f;
                yield return 0.5f;
                Speed.X = -100f;
                yield return 0.5f;
                isJumping = false;
                yield break;
            default:
                Speed.Y = -100f;
                yield return 0.5f;
                Speed.Y = 100f;
                yield return 0.5f;
                isJumping = false;
                yield break;
        }
    }

    private IEnumerator MovingCycle()
    {
        moving = true;
        isGreenMovingUp = true;
        if (cycleOffset)
        {
            yield return 2f;
            cycleOffset = false;
        }
        if (plantDirection == PlantDirection.Up || plantDirection == PlantDirection.Down)
        {
            for (int i = 0; i < 32; i++)
            {
                Y -= 1;
                yield return 0.01f * 1 / movingSpeed;
            }
            isGreenMovingUp = false;
            isWaitingAtTop = true;
            yield return 1f;
            isWaitingAtTop = false;
            for (int i = 0; i < 32; i++)
            {
                Y += 1;
                yield return 0.01f * 1 / movingSpeed;
            }
        }
        else
        {
            for (int i = 0; i < 32; i++)
            {
                X -= 1;
                yield return 0.01f * 1 / movingSpeed;
            }
            isGreenMovingUp = false;
            isWaitingAtTop = true;
            yield return 1f;
            isWaitingAtTop = false;
            for (int i = 0; i < 32; i++)
            {
                X += 1;
                yield return 0.01f * 1 / movingSpeed;
            }
        }
        yield return 1f;
        Add(new Coroutine(MovingCycle()));
    }

    private IEnumerator MoveUp()
    {
        if (isMoving) yield break;
        isMoving = true;
        moving = true;
        isRedMovingUp = true;
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        switch (plantDirection)
        {
            case PlantDirection.Left:
                for (int i = 0; i < 32; i++)
                {
                    X -= 1;
                    yield return 0.01f * 1 / movingSpeed;
                }

                isRedMovingUp = false;
                isWaitingAtTop = true;
                if (p != null)
                {
                    while (isWaitingAtTop && Math.Abs(p.Center.Y - Center.Y) <= distance)
                    {
                        if (p.Scene == null)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                break;
            case PlantDirection.Right:
                for (int i = 0; i < 32; i++)
                {
                    X += 1;
                    yield return 0.01f * 1 / movingSpeed;
                }

                isRedMovingUp = false;
                isWaitingAtTop = true;
                if (p != null)
                {
                    while (isWaitingAtTop && Math.Abs(p.Center.Y - Center.Y) <= distance)
                    {
                        if (p.Scene == null)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                break;
            case PlantDirection.Down:
                for (int i = 0; i < 32; i++)
                {
                    Y += 1;
                    yield return 0.01f * 1 / movingSpeed;
                }

                isRedMovingUp = false;
                isWaitingAtTop = true;
                if (p != null)
                {
                    while (isWaitingAtTop && Math.Abs(p.Center.X - Center.X) <= distance)
                    {
                        if (p.Scene == null)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                break;
            default:
                for (int i = 0; i < 32; i++)
                {
                    Y -= 1;
                    yield return 0.01f * 1 / movingSpeed;
                }

                isRedMovingUp = false;
                isWaitingAtTop = true;
                if (p != null)
                {
                    while (isWaitingAtTop && Math.Abs(p.Center.X - Center.X) <= distance)
                    {
                        if (p.Scene == null)
                        {
                            break;
                        }
                        yield return null;
                    }
                }
                break;
        }

        isMoving = false;
        Add(new Coroutine(MoveDown()));
    }

    private IEnumerator MoveDown()
    {
        if (isMoving) yield break;

        isMoving = true;
        moving = false;
        isRedMovingUp = false;
        isWaitingAtTop = false;
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        switch (plantDirection)
        {
            case PlantDirection.Left:
                for (int i = 0; i < 32; i++)
                {
                    X += 1;
                    yield return 0.01f * 1 / movingSpeed;
                }
                isMoving = false;
                if (p != null)
                {
                    if (Math.Abs(p.Center.Y - Center.Y) <= distance)
                        Add(new Coroutine(MoveUp()));
                }
                break;
            case PlantDirection.Right:
                for (int i = 0; i < 32; i++)
                {
                    X -= 1;
                    yield return 0.01f * 1 / movingSpeed;
                }
                isMoving = false;
                if (p != null)
                {
                    if (Math.Abs(p.Center.Y - Center.Y) <= distance)
                        Add(new Coroutine(MoveUp()));
                }
                break;
            case PlantDirection.Down:
                for (int i = 0; i < 32; i++)
                {
                    Y -= 1;
                    yield return 0.01f * 1 / movingSpeed;
                }
                isMoving = false;
                if (p != null)
                {
                    if (Math.Abs(p.Center.X - Center.X) <= distance)
                        Add(new Coroutine(MoveUp()));
                }
                break;
            default:
                for (int i = 0; i < 32; i++)
                {
                    Y += 1;
                    yield return 0.01f * 1 / movingSpeed;
                }
                isMoving = false;
                if (p != null)
                {
                    if (Math.Abs(p.Center.X - Center.X) <= distance)
                        Add(new Coroutine(MoveUp()));
                }
                break;
        }
    }

    private IEnumerator ShootCycle()
    {
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (player != null)
        {
            Shoot();
            yield return shootSpeed;
        }
        isShooting = false;
    }

    private void Shoot()
    {
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (player != null)
        {
            SceneAs<Level>().Add(Engine.Pooler.Create<Shot>().Init(this, player));
            Audio.Play("event:/KoseiHelper/bullet", Center);
        }
    }

    private void OnPlayer(Player player)
    {
        Level level = SceneAs<Level>();
        if (player.Scene != null)
        {
            player.Die(Center);
        }
    }

    public override void Render()
    {
        base.Render();
    }

    public Vector2 ShotOrigin
    {
        get
        {
            return base.Center + sprite.Position + new Vector2(0f * sprite.Scale.X, -6f);
        }
    }

    private bool CollidingWithGround(Vector2 position)
    {
        return CollideCheckOutside<Solid>(position) || CollideCheckOutside<Platform>(position);
    }
}