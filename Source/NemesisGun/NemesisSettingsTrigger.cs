using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Triggers;
using Celeste.Mod.KoseiHelper.Entities;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunSettings")]
public class NemesisGunSettingsTrigger : Trigger
{
    private bool replacesDash, enabled, bulletExplosion, loseGunOnRespawn = true;
    private string gunshotSound, bulletSound;
    private int cooldown, lifetime;
    private TriggerMode triggerMode;
    private Color color1, color2;
    private DustType shotDustType;
    
    public Extensions.GunDirections gunDirections;

    public NemesisGunSettingsTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        enabled = data.Bool("enabled", true);
        bulletExplosion = data.Bool("bulletExplosion", true);
        bulletSound = data.Attr("bulletSound", "event:/none");
        gunshotSound = data.Attr("gunshotSound", "event:/ashleybl/gunshot");
        replacesDash = data.Bool("replacesDash", true);
        cooldown = data.Int("cooldown", 8);
        color1 = data.HexColor("color1", Color.SteelBlue);
        color2 = data.HexColor("color2", Color.Yellow);
        shotDustType = data.Enum("dustType", DustType.Normal);
        gunDirections = data.Enum("directions", Extensions.GunDirections.FourDirections);
        loseGunOnRespawn = data.Bool("loseGunOnRespawn", true);
        lifetime = data.Int("lifetime", 60)*10;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerMode == TriggerMode.OnEnter)
            ChangeSettings();
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (triggerMode == TriggerMode.OnLeave)
            ChangeSettings();
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (triggerMode == TriggerMode.OnStay && !player.JustRespawned && !player.IsIntroState)
            ChangeSettings();
    }

    public void ChangeSettings()
    {
        Extensions.gunshotSound = gunshotSound;
        Extensions.replacesDash = replacesDash;
        Extensions.cooldown = cooldown;
        Extensions.color1 = color1;
        Extensions.color2 = color2;
        Extensions.shotDustType = shotDustType;
        Extensions.gunDirections = gunDirections;
        Extensions.bulletSound = bulletSound;
        Extensions.bulletExplosion = bulletExplosion;
        Extensions.loseGunOnRespawn = loseGunOnRespawn;
        Extensions.lifetime = lifetime;

        if (enabled)
        {
            (Scene as Level).Session.SetFlag("EnableNemesisGun", true);
        }
        else
        {
            (Scene as Level).Session.SetFlag("EnableNemesisGun", false);
        }
    }
}