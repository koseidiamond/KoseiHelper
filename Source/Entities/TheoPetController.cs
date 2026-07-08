using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/TheoPetController")]
public class TheoPetController : Entity
{
    public float angleIncrement;
    public float speed;
    public float jumpStrength;
    public bool affectAllTheos;

    public TheoPetController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        if (data.Bool("persistent", true))
            base.Tag = Tags.Persistent; // unused
        speed = data.Float("speed", 8f);
        jumpStrength = data.Float("jumpStrength", 1f);
        affectAllTheos = data.Bool("affectAllTheos", false);
    }

    public override void Update()
    {
        base.Update();

        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            if (player.JustRespawned || player.IsIntroState)
                return;

            if (affectAllTheos)
            {
                foreach (TheoCrystal theo in level.Tracker.GetEntities<TheoCrystal>())
                {
                    if (theo != null)
                        UpdateTheo(theo, player);
                }
            }
            else
            {
                TheoCrystal theo = level.Tracker.GetNearestEntity<TheoCrystal>(player.Center);
                if (theo != null)
                    UpdateTheo(theo, player);
            }
        }
    }

    private void UpdateTheo(TheoCrystal theo, Player player)
    {
        if (player.Position.Y <= theo.Position.Y - 150 || player.Position.Y >= theo.Position.Y + 300)
            return;

        if (theo.OnGround() && Math.Abs(theo.CenterX - player.CenterX) > 14f)
        {
            theo.ExplodeLaunch(theo.BottomCenter);
            theo.noGravityTimer = jumpStrength / 20f;
        }

        if (!theo.OnGround())
            theo.MoveTowardsX(player.CenterX, 0.5f + Math.Abs(speed) / 10f * Math.Abs(player.Speed.X) / 100f + Math.Abs(speed) / 10f);
    }
}