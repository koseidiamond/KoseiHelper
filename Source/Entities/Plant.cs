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

[CustomEntity("KoseiHelper/Plant")]
[Tracked]
public class Plant : Actor
{
    public Sprite sprite;
    public PlantType plantType;
    public bool canShoot;
    public float shootSpeed = 0f;
    public Vector2 Speed = Vector2.Zero;
    public bool moving;
    private bool isMoving = false; // to check if the red ones should wait until moving again
    public float movingSpeed = 1f;
    private bool isShooting = false;
    private bool isWaitingAtTop = false;
    private bool isGreenMovingUp = false;
    private bool isRedMovingUp = false;
    public int distance = 88;

    public Plant(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        plantType = data.Enum("plantType", PlantType.Jumping);
        movingSpeed = data.Float("movingSpeed", 1f);
        canShoot = data.Bool("canShoot", false);
        shootSpeed = data.Float("shootSpeed", 1.5f);
        distance = data.Int("distance", 88);
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

        Add(new PlayerCollider(OnPlayer));
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();

        if (plantType == PlantType.Jumping)
        {
            MoveV(Speed.Y * Engine.DeltaTime * movingSpeed);
            if (!OnGround())
            {
                Speed.Y += Engine.DeltaTime;
                sprite.Play("Jumping");
            }
            if (OnGround())
            {
                sprite.Play("JumpingIdle");
            }
            if (player != null)
            {
                if (player.Right >= Left && player.Left <= Right && player.Bottom >= Top - 80 && OnGround() && !player.JustRespawned)
                {
                    Add(new Coroutine(Jump()));
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

        if (player != null && canShoot && !player.JustRespawned)
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
        Speed.Y = -80f;
        yield return 0.5f;
        Speed.Y = 80f;
        yield return 0.75;
        yield break;
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
}