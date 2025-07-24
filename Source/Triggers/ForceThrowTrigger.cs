using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/ForceThrowTrigger")]
public class ForceThrowTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public ForceThrowTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnStay);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnStay)
        {
            player.Throw();
            if (onlyOnce)
                RemoveSelf();
        }
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnEnter)
        {
            player.Throw();
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnLeave)
        {
            player.Throw();
            if (onlyOnce)
                RemoveSelf();
        }
    }
    public override void Update()
    {
        base.Update();
    }
}