using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste.Mod.Meta;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/MetadataTrigger")]
public class MetadataTrigger : Trigger
{
    public bool onlyOnce;
    public bool theoInBubble, seekerSlowdown, heartIsEnd, Dreaming; // Map metadata
    public string coreMode, inventory;
    public bool isDark, isSpace, isUnderwater, disabledDownTransition; // Room metadata
    public string flag;
    public enum MetadataType
    {
        Map,
        Room,
        Both
    };
    public MetadataType metadataToAffect;

    public MetadataTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        metadataToAffect = data.Enum("metadataToAffect", MetadataType.Both);

        theoInBubble = data.Bool("theoInBubble", false);
        seekerSlowdown = data.Bool("seekerSlowdown", true);
        heartIsEnd = data.Bool("heartIsEnd", false);
        Dreaming = data.Bool("dreaming", false);
        coreMode = data.Attr("coreMode", "NoChange");
        inventory = data.Attr("inventory", "NoChange");

        isDark = data.Bool("isDark", false);
        isSpace = data.Bool("isSpace", false);
        isUnderwater = data.Bool("isUnderwater", false);
        disabledDownTransition = data.Bool("disableDownTransition", false);
        flag = data.Attr("flag", "");

    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && (string.IsNullOrEmpty(flag) || SceneAs<Level>().Session.GetFlag(flag)))
            MetadataChanges();
    }

    public void MetadataChanges()
    {
        Level level = SceneAs<Level>();
        MapMetaModeProperties meta = SceneAs<Level>().Session.MapData.Meta;
        LevelData levelData = SceneAs<Level>().Session.LevelData;

        if (metadataToAffect != MetadataType.Room)
        {
            meta.TheoInBubble = theoInBubble;
            meta.SeekerSlowdown = seekerSlowdown;
            meta.HeartIsEnd = heartIsEnd;
            level.Session.Dreaming = Dreaming;
            switch (coreMode)
            {
                case "Cold":
                    level.Session.CoreMode = Session.CoreModes.Cold;
                    break;
                case "Hot":
                    level.Session.CoreMode = Session.CoreModes.Hot;
                    break;
                case "None":
                    level.Session.CoreMode = Session.CoreModes.None;
                    break;
                default: // NoChange
                    break;
            }
            switch (inventory)
            {
                case "CH6End":
                    level.Session.Inventory = PlayerInventory.CH6End;
                    break;
                case "Core":
                    level.Session.Inventory = PlayerInventory.Core;
                    break;
                case "Default":
                    level.Session.Inventory = PlayerInventory.Default;
                    break;
                case "Farewell":
                    level.Session.Inventory = PlayerInventory.Farewell;
                    break;
                case "OldSite":
                    level.Session.Inventory = PlayerInventory.OldSite;
                    break;
                case "Prologue":
                    level.Session.Inventory = PlayerInventory.Prologue;
                    break;
                case "TheSummit":
                    level.Session.Inventory = PlayerInventory.TheSummit;
                    break;
                default: // NoChange
                    break;
            }
        }

        if (metadataToAffect != MetadataType.Map)
        {
            levelData.Underwater = isUnderwater;
            levelData.Space = isSpace;
            levelData.DisableDownTransition = disabledDownTransition;
            levelData.Dark = isDark;
        }

        if (onlyOnce)
            RemoveSelf();
    }
}