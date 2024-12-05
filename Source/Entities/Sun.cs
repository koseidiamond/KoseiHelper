using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;
using static Celeste.Session;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/Sun")]
[Tracked]
public class Sun : Entity
{
    public float speed;
    public bool onlyWhileHot;
    private CoreModes coreMode;
    private bool attacking;
    private Sprite sprite;
    private Vector2 targetPosition;
    private Vector2 startPosition;
    private float attackProgress;
    private bool attackingRight;
    private bool isInAttackPhase;
    private bool attackCompleted;

    public Sun(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = 100000;
        onlyWhileHot = data.Bool("onlyWhileHot", false);
        Collider = new Hitbox(14, 14, -7, -7);
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_Sun"));
        sprite.Play("Normal");
        Add(new PlayerCollider(OnPlayer));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        Position = level.Camera.Position + new Vector2(24, 16);
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();
        if (!isInAttackPhase)
        {
            Add(new Coroutine(Attack()));
        }
        if (isInAttackPhase)
        {
            attackProgress += Engine.DeltaTime;
            MoveAlongParabola();
        }

        base.Update();
    }

    private IEnumerator Attack()
    {
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            isInAttackPhase = true;
            targetPosition = player.Center;
            startPosition = Position;
            attackingRight = Position.X < targetPosition.X;
        }
        yield return 0.2f;

        attackCompleted = false;

        yield break;
    }

    private void MoveAlongParabola()
    {
        Level level = SceneAs<Level>();
        float parabolaHeight = 16f;
        float progress = attackProgress;
        Vector2 middlePoint = targetPosition;
        Vector2 endPoint = level.Camera.Position + new Vector2(attackingRight ? 320 - 32 : 24, 16);
        float xPos = MathHelper.Lerp(startPosition.X, endPoint.X, progress);
        float yPos = MathHelper.Lerp(startPosition.Y, endPoint.Y, progress) + parabolaHeight * (float)Math.Pow(progress - 0.5f, 2);
        if (progress >= 1f)
        {
            Position = endPoint;
            attackCompleted = true;
            StartNewAttackCycle();
        }
        else
        {
            Position = new Vector2(xPos, yPos);
        }
    }

    private void StartNewAttackCycle()
    {
        Add(new Coroutine(WaitBeforeNextAttack()));
    }

    private IEnumerator WaitBeforeNextAttack()
    {
        yield return 1f;
        attackProgress = 0f;
        isInAttackPhase = false;
    }

    private void OnPlayer(Player player)
    {
        if (player.Scene != null)
            player.Die(player.Center);
    }
}
