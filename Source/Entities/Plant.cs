using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;
using ExtendedVariants.Variants;

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
    public Vector2 Speed = Vector2.Zero;
    public bool moving;
    public float movingSpeed = 1f;
    public Plant(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        plantType = data.Enum("plantType", PlantType.Jumping);
        movingSpeed = data.Float("movingSpeed", 1f);
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
            sprite.Play("GreenIdle");
        }
        if (plantType == PlantType.Red)
        {
            Collider = new Hitbox(16, 30, -8, -14);
            sprite.Play("RedIdle");
        }
        Add(new PlayerCollider(OnPlayer));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
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
                    sprite.FlipX = false;
            }
        }
        if (plantType == PlantType.Black)
            sprite.Play("Black");
        if (plantType == PlantType.Green)
        {
            if (!moving && !player.JustRespawned)
                Add(new Coroutine (MovingCycle()));
            if (player != null)
            {
                if (player.Center.X > Center.X)
                    sprite.FlipX = true;
                else
                    sprite.FlipX = false;
            }
        }
        if (plantType == PlantType.Red)
        {
            if (player != null)
            {
                if (Math.Abs(player.Center.X - Center.X) <= 32)
                {
                    if (!moving)
                        Add(new Coroutine(MoveUp()));
                }
                else
                {
                    if (moving)
                        Add(new Coroutine(MoveDown()));
                }

                if (player.Center.X > Center.X)
                    sprite.FlipX = true;
                else
                    sprite.FlipX = false;
            }
        }
    }

    private IEnumerator Jump()
    {
        Speed.Y = -80f;
        yield return 0.5f;
        Speed.Y = 80f;
        yield return 0.75;
        yield break;
    }

    private IEnumerator MovingCycle() //For the green plants
    {
        moving = true;
        yield return 1f;

        for (int i = 0; i < 32; i++)
        {
            Y -= 1;
            yield return 0.01f * 1/movingSpeed;
        }
        yield return 1f;

        for (int i = 0; i < 32; i++)
        {
            Y += 1;
            yield return 0.01f * 1/movingSpeed;
        }
        Logger.Debug(nameof(KoseiHelperModule), $"Moving cycle completed.");
        Add(new Coroutine(MovingCycle()));
    }

    private IEnumerator MoveUp() // Red plants
    {
        moving = true;
        for (int i = 0; i < 32; i++)
        {
            Y -= 1;
            yield return 0.01f * 1 / movingSpeed;
        }
    }

    private IEnumerator MoveDown() // Red plants
    {
        moving = false;
        for (int i = 0; i < 32; i++)
        {
            Y += 1;
            yield return 0.01f * 1 / movingSpeed;
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

}