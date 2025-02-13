using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste.Mod.Meta;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/MetadataTrigger")]
public class MetadataTrigger : Trigger
{
    public bool onlyOnce;
    public bool theoInBubble, seekerSlowdown, heartIsEnd, Dreaming; // Map metadata
    public bool isDark, isSpace, isUnderwater, disabledDownTransition; // Room metadata

    public MetadataTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);

        theoInBubble = data.Bool("theoInBubble", false);
        seekerSlowdown = data.Bool("seekerSlowdown", true);
        heartIsEnd = data.Bool("heartIsEnd", false);
        Dreaming = data.Bool("dreaming", false);

        isDark = data.Bool("isDark", false);
        isSpace = data.Bool("isSpace", false);
        isUnderwater = data.Bool("isUnderwater", false);
        disabledDownTransition = data.Bool("disableDownTransition", false);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null)
            MetadataChanges();
    }

    public void MetadataChanges()
    {
        MapMetaModeProperties meta = SceneAs<Level>().Session.MapData.Meta;
        LevelData levelData = SceneAs<Level>().Session.LevelData;
        meta.TheoInBubble = theoInBubble;
        meta.SeekerSlowdown = seekerSlowdown;
        meta.HeartIsEnd = heartIsEnd;
        SceneAs<Level>().Session.Dreaming = Dreaming;

        levelData.Underwater = isUnderwater;
        levelData.Space = isSpace;
        levelData.DisableDownTransition = disabledDownTransition;
        levelData.Dark = isDark;

        if (onlyOnce)
            RemoveSelf();
    }
}