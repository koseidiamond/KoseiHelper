using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/ForceFacingDirectionTrigger")]
public class ForceFacingDirectionTrigger : Trigger
{
    private enum TriggerMode
    {
        OnEnter,
        OnLeave,
        OnStay
    };
    private TriggerMode mode;
    private string flag;
    private float speedThreshold;
    private bool noSpeedRequired;
    private bool flagValue;

    public ForceFacingDirectionTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        flag = data.Attr("flag", "");
        mode = data.Enum("mode", TriggerMode.OnStay);
        speedThreshold = data.Float("speedThreshold", 1f);
        noSpeedRequired = data.Bool("noSpeedRequired", false);
        flagValue = data.Bool("flagValue", true);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        SetFacing(player, SceneAs<Level>(), TriggerMode.OnEnter);
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        SetFacing(player, SceneAs<Level>(), TriggerMode.OnLeave);
    }

    public override void OnStay(Player player)
    {
        base.Update();
        SetFacing(player, SceneAs<Level>(), TriggerMode.OnStay);
    }

    private void SetFacing(Player player, Level level, TriggerMode triggerMode)
    {
        if ((string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag) == flagValue) && mode == triggerMode)
        {
            if (player.Speed.X > speedThreshold || noSpeedRequired)
                player.Facing = Facings.Right;
            if (player.Speed.X < -speedThreshold || noSpeedRequired)
                player.Facing = Facings.Left;
        }
    }
}