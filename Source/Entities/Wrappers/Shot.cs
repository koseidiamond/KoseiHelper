using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;
[Pooled]
[Tracked(true)]
public class Shot : Entity
{
    public enum ShotPatterns
    {
        Single,
        Double,
        Triple
    }
    public static ParticleType P_Trail = FinalBossShot.P_Trail;
    private const float MoveSpeed = 100f;
    private const float CantKillTime = 0.15f;
    private const float AppearTime = 0.1f;
    private Plant plant;
    private Vector2 speed = Vector2.Zero;
    private float particleDir;
    private Vector2 anchor;
    private Vector2 perp;
    private Player target;
    private Vector2 targetPt;
    private float angleOffset;
    private bool dead;
    private float cantKillTimer;
    private float appearTimer;
    private bool hasBeenInCamera;
    private SineWave sine;
    private float sineMult;
    private Sprite sprite;
    public Shot()
        : base(Vector2.Zero)
    {
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_Bullet"));
        base.Collider = new Hitbox(4f, 4f, -2f, -2f);
        Add(new PlayerCollider(OnPlayer));
        base.Depth = -99;
        Add(sine = new SineWave(1.4f, 0f));
    }
    public Shot Init(Plant plant, Player target, float angleOffset = 0f) // Fire seeds
    {
        this.plant = plant;
        anchor = (Position = new Vector2(plant.Center.X, plant.Top + 8));
        this.target = target;
        this.angleOffset = angleOffset;
        dead = (hasBeenInCamera = false);
        cantKillTimer = 0.15f;
        appearTimer = 0.1f;
        sine.Reset();
        sineMult = 0f;
        sprite.Play("bulletRed", restart: true);
        InitSpeed();
        return this;
    }
    public Shot Init(Plant plant, Vector2 target, string bulletType, float rot) // Melon seeds
    {
        this.plant = plant;
        anchor = (Position = new Vector2(plant.Center.X, plant.Top + 8));
        this.target = null;
        angleOffset = 0f;
        targetPt = target;
        dead = (hasBeenInCamera = false);
        cantKillTimer = 0.15f;
        appearTimer = 0.1f;
        sine.Reset();
        sineMult = 0f;
        sprite.Play(bulletType, restart: true);
        sprite.Rotation = rot;
        InitSpeed();
        return this;
    }
    private void InitSpeed()
    {
        if (target != null)
        {
            speed = (target.Center - base.Center).SafeNormalize(100f);
        }
        else
        {
            speed = (targetPt - base.Center).SafeNormalize(100f);
        }
        if (angleOffset != 0f)
        {
            speed = speed.Rotate(angleOffset);
        }
        perp = speed.Perpendicular().SafeNormalize();
        particleDir = (-speed).Angle();
    }
    public override void Added(Scene scene)
    {
        base.Added(scene);
    }
    public override void Update()
    {
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        base.Update();
        if (CollideCheck<Solid>() || !level.IsInBounds(this))
            RemoveSelf();
        if (appearTimer > 0f)
        {
            Position = (anchor = plant.ShotOrigin);
            appearTimer -= Engine.DeltaTime;
            return;
        }
        if (cantKillTimer > 0f)
        {
            cantKillTimer -= Engine.DeltaTime;
        }
        anchor += speed * Engine.DeltaTime;
        Position = anchor + perp * sineMult * sine.Value * 3f;
        sineMult = Calc.Approach(sineMult, 1f, 2f * Engine.DeltaTime);
        if (!dead)
        {
            bool flag = level.IsInCamera(Position, 8f);
            if (flag && !hasBeenInCamera)
            {
                hasBeenInCamera = true;
            }
            else if (!flag && hasBeenInCamera)
            {
                Destroy();
            }
            /*if (Scene.OnInterval(0.04f) && fireType)
            {
                level.ParticlesFG.Emit(P_Trail, 1, Center, Vector2.One * 2f, particleDir);
            }*/
        }
    }
    public override void Render()
    {
        Color color = sprite.Color;
        Vector2 position = sprite.Position;
        sprite.Color = Color.Black;
        sprite.Position = position + new Vector2(-1f, 0f);
        sprite.Render();
        sprite.Position = position + new Vector2(1f, 0f);
        sprite.Render();
        sprite.Position = position + new Vector2(0f, -1f);
        sprite.Render();
        sprite.Position = position + new Vector2(0f, 1f);
        sprite.Render();
        sprite.Color = color;
        sprite.Position = position;
        base.Render();
    }

    public void Destroy()
    {
        dead = true;
        RemoveSelf();
    }
    private void OnPlayer(Player player)
    {
        if (!dead)
        {
            if (cantKillTimer > 0f)
            {
                Destroy();
            }
            else
            {
                player.Die((player.Center - Position).SafeNormalize());
            }
        }
    }
}