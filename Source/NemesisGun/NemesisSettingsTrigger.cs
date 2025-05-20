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
    private string gunshotSound, gunTexture, bulletTexture, customParticleTexture;
    private int cooldown, recoilCooldown, lifetime;
    private bool recoilUpwards;
    private TriggerMode triggerMode;
    private Color color1, color2;
    private DustType shotDustType;
    private float speedMultiplier;
    private float recoil;
    private KoseiHelperModuleSettings.NemesisSettings.DashBehavior dashBehavior;
    private float particleAlpha;
    public bool canShootInFeather = true;
    private int freezeFrames;
    private float horizontalAcceleration = 0f, verticalAcceleration = 0f;
    private int bulletWidth = 6, bulletHeight = 6, bulletXOffset, bulletYOffset;
    private bool particleDoesntRotate;

    public KoseiHelperModuleSettings.NemesisSettings.GunDirections gunDirections;

    public NemesisGunSettingsTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        enabled = data.Bool("enabled", true);
        bulletExplosion = data.Bool("bulletExplosion", true);
        gunshotSound = data.Attr("gunshotSound", "event:/KoseiHelper/Guns/shotDefault");
        cooldown = data.Int("cooldown", 8);
        color1 = data.HexColor("color1", Color.Orange);
        color2 = data.HexColor("color2", Color.Yellow);
        shotDustType = data.Enum("particleType", DustType.Normal);
        gunDirections = data.Enum("directions", KoseiHelperModuleSettings.NemesisSettings.GunDirections.EightDirections);
        loseGunOnRespawn = data.Bool("loseGunOnRespawn", true);
        gunTexture = data.Attr("gunTexture", "objects/KoseiHelper/Guns/NemesisGun");
        bulletTexture = data.Attr("bulletTexture", "objects/KoseiHelper/Guns/Bullets/Invisible");
        customParticleTexture = data.Attr("customParticleTexture", "particles/KoseiHelper/star");
        lifetime = data.Int("lifetime", 600);
        speedMultiplier = data.Float("speedMultiplier", 1f);
        recoil = data.Float("recoilStrength", 80f);
        recoilUpwards = data.Bool("recoilUpwards", false);
        recoilCooldown = data.Int("recoilCooldown", 16);
        dashBehavior = data.Enum("dashBehavior", KoseiHelperModuleSettings.NemesisSettings.DashBehavior.None);
        particleAlpha = data.Float("particleAlpha", 1f);
        canShootInFeather = data.Bool("canShootInFeather", true);
        freezeFrames = data.Int("freezeFrames", 0);
        horizontalAcceleration = data.Float("horizontalAcceleration", 0f);
        verticalAcceleration = data.Float("verticalAcceleration", 0f);
        bulletWidth = data.Int("bulletWidth", 6);
        bulletHeight = data.Int("bulletHeight", 6);
        bulletXOffset = data.Int("bulletXOffset", 0);
        bulletYOffset = data.Int("bulletYOffset", 0);
        particleDoesntRotate = data.Bool("particleDoesntRotate", false);
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
        KoseiHelperModule.Settings.GunSettings.dashBehavior = dashBehavior;
        KoseiHelperModule.Settings.GunSettings.Cooldown = cooldown;
        KoseiHelperModule.Settings.GunSettings.gunDirections = gunDirections;
        Extensions.gunshotSound = gunshotSound;
        KoseiHelperModule.Settings.GunSettings.Lifetime = lifetime;
        KoseiHelperModule.Settings.GunSettings.SpeedMultiplier = speedMultiplier;
        KoseiHelperModule.Settings.GunSettings.Recoil = recoil;
        KoseiHelperModule.Settings.GunSettings.RecoilUpwards = recoilUpwards;
        KoseiHelperModule.Settings.GunSettings.RecoilCooldown = recoilCooldown;
        KoseiHelperModule.Settings.GunSettings.CanShootInFeather = canShootInFeather;
        KoseiHelperModule.Settings.GunSettings.FreezeFrames = freezeFrames;
        KoseiHelperModule.Settings.GunSettings.HorizontalAcceleration = horizontalAcceleration;
        KoseiHelperModule.Settings.GunSettings.VerticalAcceleration = verticalAcceleration;
        Extensions.color1 = color1;
        Extensions.color2 = color2;
        Extensions.shotDustType = shotDustType;
        Extensions.bulletExplosion = bulletExplosion;
        Extensions.loseGunOnRespawn = loseGunOnRespawn;
        Extensions.gunTexture = gunTexture;
        Extensions.bulletTexture = bulletTexture;
        Extensions.particleAlpha = particleAlpha;
        Extensions.customParticleTexture = customParticleTexture;
        Extensions.bulletWidth = bulletWidth;
        Extensions.bulletHeight = bulletHeight;
        Extensions.bulletXOffset = bulletXOffset;
        Extensions.bulletYOffset = bulletYOffset;
        Extensions.particleDoesntRotate = particleDoesntRotate;

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