using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Triggers;
using Celeste.Mod.KoseiHelper.Entities;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunSettings")]
public class NemesisGunSettingsTrigger : Trigger
{
    private bool enabled = true, bulletExplosion = true, loseGunOnRespawn = true;
    private string gunshotSound, gunTexture, bulletTexture;
    private int cooldown, recoilCooldown, lifetime;
    private TriggerMode triggerMode;
    private Color color1, color2;
    private DustType shotDustType;
    private float speedMultiplier;
    private float recoil;
    private Extensions.DashBehavior dashBehavior;
    private float particleAlpha;

    public Extensions.GunDirections gunDirections;

    public NemesisGunSettingsTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        enabled = data.Bool("enabled", true);
        bulletExplosion = data.Bool("bulletExplosion", true);
        gunshotSound = data.Attr("gunshotSound", "event:/KoseiHelper/Guns/shotDefault");
        cooldown = data.Int("cooldown", 8);
        recoilCooldown = data.Int("recoilCooldown", 16);
        color1 = data.HexColor("color1", Color.Orange);
        color2 = data.HexColor("color2", Color.Yellow);
        shotDustType = data.Enum("particleType", DustType.Normal);
        gunDirections = data.Enum("directions", Extensions.GunDirections.FourDirections);
        loseGunOnRespawn = data.Bool("loseGunOnRespawn", true);
        lifetime = data.Int("lifetime", 60)*10;
        gunTexture = data.Attr("gunTexture", "objects/KoseiHelper/Guns/NemesisGun");
        bulletTexture = data.Attr("bulletTexture", "objects/KoseiHelper/Guns/Bullets/Invisible");
        speedMultiplier = data.Float("speedMultiplier", 1f);
        recoil = data.Float("recoilStrength", 80f);
        dashBehavior = data.Enum("dashBehavior", Extensions.DashBehavior.None);
        particleAlpha = data.Float("particleAlpha", 1f);
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
        Extensions.dashBehavior = dashBehavior;
        Extensions.cooldown = cooldown;
        Extensions.color1 = color1;
        Extensions.color2 = color2;
        Extensions.shotDustType = shotDustType;
        Extensions.gunDirections = gunDirections;
        Extensions.bulletExplosion = bulletExplosion;
        Extensions.loseGunOnRespawn = loseGunOnRespawn;
        Extensions.lifetime = lifetime;
        Extensions.gunTexture = gunTexture;
        Extensions.bulletTexture = bulletTexture;
        Extensions.speedMultiplier = speedMultiplier;
        Extensions.recoil = recoil;
        Extensions.particleAlpha = particleAlpha;
        Extensions.recoilCooldown = recoilCooldown;

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