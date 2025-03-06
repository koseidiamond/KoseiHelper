using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunTrigger")]
public class NemesisGunTrigger : Trigger
{
    private bool enabled;
    private bool replacesDash, canKillPlayer = true;
    private string gunshotSound;
    private int cooldown;
    public NemesisGunTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        enabled = data.Bool("enabled", false);
        gunshotSound = data.Attr("gunshotSound", "event:/ashleybl/gunshot");
        replacesDash = data.Bool("replacesDash", true);
        cooldown = data.Int("cooldown", 8);
        canKillPlayer = data.Bool("canKillPlayer", true);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Extensions.gunshotSound = gunshotSound;
        Extensions.replacesDash = replacesDash;
        Extensions.cooldown = cooldown;
        Extensions.canKillPlayer = canKillPlayer;
        if (enabled)
        {
            KoseiHelperModule.Settings.GunEnabled = true;
            (Scene as Level).Session.SetFlag("EnableNemesisGun", true);
        }
        else
        {
            KoseiHelperModule.Settings.GunEnabled = false;
            (Scene as Level).Session.SetFlag("EnableNemesisGun", false);
        }
    }
}