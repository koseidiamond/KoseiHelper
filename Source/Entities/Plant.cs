using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;

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
    public Vector2 Speed = new Vector2 (0, 40);
    public bool moving;
    private bool isMoving = false; // to check if the red ones should wait until moving again
    public float movingSpeed = 1f;
    private bool isShooting, isJumping = false;
    private bool isWaitingAtTop = false;
    private bool isGreenMovingUp = false;
    private bool isRedMovingUp = false;
    public int distance = 64;

    public Plant(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        plantType = data.Enum("plantType", PlantType.Jumping);
        plantDirection = data.Enum("plantDirection", PlantDirection.Up);
        movingSpeed = data.Float("movingSpeed", 1f);
        canShoot = data.Bool("canShoot", false);
        shootSpeed = data.Float("shootSpeed", 1.5f);
        distance = data.Int("distance", 64);
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
        }
        if (plantType == PlantType.Green)
        {
            Collider = new Hitbox(16, 30, -8, -14);
            if (!canShoot)
                sprite.Play("GreenIdle");
            else
                sprite.Play("GreenShootDown");
        }
        if (plantType == PlantType.Red)
        {
            Collider = new Hitbox(16, 30, -8, -14);
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
            sprite.Rotation = -(float)Math.PI;

        Add(new PlayerCollider(OnPlayer));
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (Top > level.Bounds.Bottom || Bottom < level.Bounds.Top || Left > level.Bounds.Right || Right < level.Bounds.Left)
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
                        Speed.Y += Engine.DeltaTime * -100;
                        Speed.Y = Calc.Clamp(Speed.Y, -150, 150);
                        if (CollidingWithGround(TopCenter + new Vector2(0, -2)))
                            sprite.Play("Jumping");
                        else
                            sprite.Play("JumpingIdle");
                        break;
                    case PlantDirection.Left:
                        Speed.X += Engine.DeltaTime * 100;
                        Speed.X = Calc.Clamp(Speed.X, -150, 150);
                        if (CollidingWithGround(CenterRight + new Vector2(2, 0)))
                            sprite.Play("Jumping");
                        else
                            sprite.Play("JumpingIdle");
                        break;
                    case PlantDirection.Right:
                        Speed.X += Engine.DeltaTime * -100;
                        Speed.X = Calc.Clamp(Speed.X, -150, 150);
                        if (CollidingWithGround(CenterLeft + new Vector2(-2, 0)))
                            sprite.Play("Jumping");
                        else
                            sprite.Play("JumpingIdle");
                        break;
                    default:
                        Speed.Y += Engine.DeltaTime * 100;
                        Speed.Y = Calc.Clamp(Speed.Y, -150, 150);
                        if (CollidingWithGround(BottomCenter + new Vector2(0, 2)))
                            sprite.Play("Jumping");
                        else
                            sprite.Play("JumpingIdle");
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
                                Logger.Debug(nameof(KoseiHelperModule), $"player.Bottom: {player.Bottom} > Top: {Top}");
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
                if (player.Center.X > Center.X)
                    sprite.FlipX = true;
                else
                    sprite.FlipX = false;
            }
        }

        if (plantType == PlantType.Black)
            sprite.Play("Black");

        if (plantType == PlantType.Green)
        {
            if (player != null)
            {
                if (!moving)
                    Add(new Coroutine(MovingCycle()));

                if (player.Center.X > Center.X)
                    sprite.FlipX = true;
                else
                    sprite.FlipX = false;
                if (canShoot && player.Bottom < Top)
                    sprite.Play("GreenShootUp");
                if (canShoot && player.Top > Top - 16)
                    sprite.Play("GreenShootDown");
            }
        }

        if (plantType == PlantType.Red)
        {
            if (player != null)
            {
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
                if (canShoot && player.Bottom < Top)
                {
                    sprite.Play("RedShootUp");
                }
                if (canShoot && player.Top > Top - 16)
                    sprite.Play("RedShootDown");
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
                Speed.Y = 80f;
                yield return 0.5f;
                Speed.Y = -80f;
                yield return 0.5f;
                isJumping = false;
                yield break;
            case PlantDirection.Left:
                Speed.X = -80f;
                yield return 0.5f;
                Speed.X = 80f;
                yield return 0.5f;
                isJumping = false;
                yield break;
            case PlantDirection.Right:
                Speed.X = 80f;
                yield return 0.5f;
                Speed.X = -80f;
                yield return 0.5f;
                isJumping = false;
                yield break;
            default:
                Speed.Y = -80f;
                yield return 0.5f;
                Speed.Y = 80f;
                yield return 0.5f;
                isJumping = false;
                yield break;
        }
    }

    private IEnumerator MovingCycle()
    {
        moving = true;
        isGreenMovingUp = true;
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

        yield return 1f;
        Add(new Coroutine(MovingCycle()));
    }

    private IEnumerator MoveUp()
    {
        if (isMoving) yield break;

        isMoving = true;
        moving = true;
        isRedMovingUp = true;

        for (int i = 0; i < 32; i++)
        {
            Y -= 1;
            yield return 0.01f * 1 / movingSpeed;
        }

        isRedMovingUp = false;
        isWaitingAtTop = true;
        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
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

        for (int i = 0; i < 32; i++)
        {
            Y += 1;
            yield return 0.01f * 1 / movingSpeed;
        }

        isMoving = false;

        Player p = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (p != null)
        {
            if (Math.Abs(p.Center.X - Center.X) <= distance)
                Add(new Coroutine(MoveUp()));
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
            SceneAs<Level>().Add(Engine.Pooler.Create<Shot>().Init(this, player));
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