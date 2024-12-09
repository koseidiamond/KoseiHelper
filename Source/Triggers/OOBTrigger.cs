using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/OOBTrigger")]
public class OOBTrigger : Trigger
{
    public bool onlyOnce;
    public bool onExit;
    public bool inBound;
    private static bool climbFix;
    public OOBTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        onExit = data.Bool("triggerMode", false);
        inBound = data.Bool("inBound", false);
        climbFix = data.Bool("climbFix", false);
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && !onExit)
        {
            if (!inBound)
            {
                player.EnforceLevelBounds = false;
            }
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
            {
                player.EnforceLevelBounds = false;
            }
            else
                player.EnforceLevelBounds = true;
        }
        if (onlyOnce)
            RemoveSelf();
    }

    public static void Load()
    {
        On.Celeste.Player.ClimbBoundsCheck += ClimbCheck;
    }

    public static void Unload()
    {
        On.Celeste.Player.ClimbBoundsCheck -= ClimbCheck;
    }

    private static bool ClimbCheck(On.Celeste.Player.orig_ClimbBoundsCheck orig, Player self, int dir)
    {
        if (climbFix)
            return true;
        else
        {
            if (self.Left + (float)(dir * 2) >= (float)self.level.Bounds.Left)
                return self.Right + (float)(dir * 2) < (float)self.level.Bounds.Right;
        }
        return false;
    }
}