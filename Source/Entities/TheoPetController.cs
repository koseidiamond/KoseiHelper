using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/TheoPetController")]
public class TheoPetController : Entity
{
    public float angleIncrement;
    TheoCrystal theo;
    public float speed;
    public float jumpStrength;

    public TheoPetController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        if (data.Bool("persistent", true))
            base.Tag = Tags.Persistent;
        speed = data.Float("speed", 8f);
        jumpStrength = data.Float("jumpStrength", 1f);
    }

    public override void Update()
    {
        base.Update();

        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            theo = SceneAs<Level>().Tracker.GetNearestEntity<TheoCrystal>(player.Center);
            if (theo != null && !player.JustRespawned && !player.IsIntroState)
            {
                if (player.Position.Y > theo.Position.Y - 150 && player.Position.Y < theo.Position.Y + 300)
                {
                    if (theo.OnGround() && Math.Abs(theo.CenterX - player.CenterX) > 14f)
                    {
                        theo.ExplodeLaunch(theo.BottomCenter);
                        theo.noGravityTimer = jumpStrength / 20;
                    }
                    if (!theo.OnGround())
                        theo.MoveTowardsX(player.CenterX, 0.5f + Math.Abs(speed) / 10 * Math.Abs(player.Speed.X) / 100 + Math.Abs(speed) / 10);
                }
            }
        }
    }
}