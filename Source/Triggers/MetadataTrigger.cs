using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste.Mod.Meta;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/MetadataTrigger")]
public class MetadataTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public bool theoInBubble, seekerSlowdown, heartIsEnd; // Map metadata
    public bool isDark, isSpace, isUnderwater, disabledDownTransition; // Room metadata

    public MetadataTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        theoInBubble = data.Bool("theoInBubble", false);
        seekerSlowdown = data.Bool("seekerSlowdown", true);
        heartIsEnd = data.Bool("heartIsEnd", false);
        isDark = data.Bool("isDark", false);
        isSpace = data.Bool("isSpace", false);
        isUnderwater = data.Bool("isUnderwater", false);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnStay)
            MetadataChanges();
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnEnter)
            MetadataChanges();
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnLeave)
            MetadataChanges();
    }

    public void MetadataChanges()
    {
        MapMetaModeProperties meta = SceneAs<Level>().Session.MapData.Meta;
        LevelData levelData = SceneAs<Level>().Session.LevelData;
        meta.TheoInBubble = theoInBubble;
        meta.SeekerSlowdown = seekerSlowdown;
        meta.HeartIsEnd = heartIsEnd;
        levelData.Dark = isDark;
        levelData.Space = isSpace;
        levelData.Underwater = isUnderwater;
        levelData.DisableDownTransition = disabledDownTransition;
        if (onlyOnce)
            RemoveSelf();
    }
}