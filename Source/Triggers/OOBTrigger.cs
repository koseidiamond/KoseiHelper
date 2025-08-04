using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/OOBTrigger")]
public class OOBTrigger : Trigger
{
    public bool onlyOnce;
    public bool onExit;
    public bool inBound;
    public OOBTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        onExit = data.Bool("triggerMode", false);
        inBound = data.Bool("inBound", false);
        KoseiHelperModule.Session.oobClimbFix = data.Bool("climbFix", false);
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && !onExit)
        {
            if (!inBound)
                player.EnforceLevelBounds = false;
            else
                player.EnforceLevelBounds = true;
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && onExit)
        {
            if (!inBound)
                player.EnforceLevelBounds = false;
            else
                player.EnforceLevelBounds = true;
            if (onlyOnce)
                RemoveSelf();
        }
    }
}