using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MagicBooster")]
[Tracked]
public class MagicBooster : Booster
{
    private Vector2 trailPosition;
    private bool wasBoosting;
    private const float TrailSpacing = 4f;
    private const float TrailBehindPlayer = 10f;

    public MagicBooster(EntityData data, Vector2 offset) : base(data.Position + offset, data.Bool("red", true))
    {
        sprite.RemoveSelf();
        Add(sprite = GFX.SpriteBank.Create(red ? "koseiHelper_grayBoosterRed" : "koseiHelper_grayBooster"));
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        //sprite.Color = Calc.HsvToColor(((scene as Level).TimeActive * 0.25f) % 1f, 1f, 1f);
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (level == null)
            return;
        MagicInkController controller = level.Tracker.GetEntity<MagicInkController>();
        if (controller == null)
            return;

        //sprite.Color = Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f);
        sprite.Color = red ? Color.MediumPurple : Color.Pink;
        //sprite.Color *= red ? 0.65f : 0.9f;

        Player player = level.Tracker.GetEntity<Player>();
        if (BoostingPlayer && !wasBoosting)
        {
            wasBoosting = true;
            if (player != null)
            {
                Vector2 direction = player.Speed.SafeNormalize();
                trailPosition = player.Center - direction * TrailBehindPlayer;
            }
            else
            {
                trailPosition = Position;
            }
        }

        if (BoostingPlayer && player != null && !player.Dead && (player.StateMachine.State == Player.StBoost || player.StateMachine.State == Player.StRedDash))
        {
            Vector2 target = player.Center - player.Speed.SafeNormalize() * TrailBehindPlayer;
            Vector2 delta = target - trailPosition;
            float distance = delta.Length();
            if (distance >= TrailSpacing)
            {
                Vector2 trailDirection = delta / distance;
                while (distance >= TrailSpacing)
                {
                    Vector2 next = trailPosition + trailDirection * TrailSpacing;
                    if (!controller.TrySpawnPaint(trailPosition, next, this, controller.killIfNoInk))
                    {
                        BoostingPlayer = false;
                        player.StateMachine.State = Player.StNormal;
                        break;
                    }

                    trailPosition = next;
                    delta = target - trailPosition;
                    distance = delta.Length();
                }
            }
        }
        if (!BoostingPlayer)
        {
            wasBoosting = false;
        }
    }

    public override void Render()
    {
        sprite.DrawOutline(Color.Lerp(Calc.HsvToColor((SceneAs<Level>().TimeActive * 0.25f) % 1f, 1f, 1f), red ? Color.MediumPurple : Color.Pink, 0.6f), 2);
        base.Render();
    }
}