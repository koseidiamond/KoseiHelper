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
    public bool potted;
    private int direction = -1;
    public static ParticleType maryParticle = SwitchGate.P_Behind;
    public bool outline;
    public bool affectTheo;
    public MaryBlock(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = -9500;
        potted = data.Bool("potted", false);
        outline = data.Bool("outline", false);
        affectTheo = data.Bool("affectTheo", false);
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

    public override void Update()
    {
        if (Scene.Tracker.GetEntity<Player>() != null)
            direction = Math.Sign(Scene.Tracker.GetEntity<Player>().Position.X - this.Position.X);
        TheoCrystal theo = SceneAs<Level>().Tracker.GetNearestEntity<TheoCrystal>(Center);
        if (theo != null && affectTheo && CollideCheck(theo))
            OnTheo(theo);
    }

    private void OnPlayer(Player player)
    {
        Level level = SceneAs<Level>();
        if (player.Scene != null)
        {
            Audio.Play("event:/KoseiHelper/mary", Position);
            player.SideBounce(direction, Position.X, Position.Y);
            if (potted)
                player.ExplodeLaunch(Position, false);
            float angle = player.Speed.Angle();
            level.ParticlesFG.Emit(maryParticle, 5, Center + new Vector2(0, -2), Vector2.One * 4f, angle - (float)Math.PI / 2f);
            RemoveSelf();
        }
    }

    private void OnTheo(TheoCrystal theo)
    {
        Level level = SceneAs<Level>();
        Audio.Play("event:/KoseiHelper/mary", Position);
        theo.HitSpinner(this);
        if (potted)
            theo.ExplodeLaunch(Position);
        float angle = theo.Speed.Angle();
        level.ParticlesFG.Emit(maryParticle, 5, Center + new Vector2(0, -2), Vector2.One * 4f, angle - (float)Math.PI / 2f);
        RemoveSelf();
    }

    public override void Render()
    {
        if (outline)
            sprite.DrawOutline();
        base.Render();
    }
}