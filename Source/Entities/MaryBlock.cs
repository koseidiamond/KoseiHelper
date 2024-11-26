using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MaryBlock")]
[Tracked]
public class MaryBlock : Entity
{
    private Sprite sprite;
    private Wiggler rotateWiggler;
    public bool potted;
    private int direction = -1;
    public static ParticleType maryParticle = SwitchGate.P_Behind;
    public bool outline;
    public MaryBlock(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = -9500;
        potted = data.Bool("potted", false);
        outline = data.Bool("outline", false);
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_maryBlock"));
        if (potted)
        {
            sprite.Play("potted");
            Collider = new Hitbox(8, 16, -4, 0);
        }
        else
        {
            sprite.Play("idle");
            Collider = new Hitbox(8, 12, -4, 4);
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
        level = SceneAs<Level>();
        Add(rotateWiggler = Wiggler.Create(0.5f, 3f, (float v) =>
        {
            sprite.Rotation = v * 15f * (MathF.PI / 180f);
        }));
    }

    public override void Update()
    {
        if (Scene.Tracker.GetEntity<Player>() != null)
            direction = Math.Sign(Scene.Tracker.GetEntity<Player>().Position.X - this.Position.X);
    }

    private void OnPlayer(Player player)
    {
        Level level = SceneAs<Level>();
        if (player.Scene != null)
        {
            Audio.Play("event:/KoseiHelper/mary", Position);
            player.SideBounce(direction, Position.X, Position.Y);
            //player.AttractBegin();
            if (potted)
                player.ExplodeLaunch(Position, false);
            float angle = player.Speed.Angle();
            level.ParticlesFG.Emit(maryParticle, 5, Position, Vector2.One * 4f, angle - (float)Math.PI / 2f);
            RemoveSelf();
        }
    }

    public override void Render()
    {
        if (outline)
            sprite.DrawOutline();
        base.Render();
    }


}