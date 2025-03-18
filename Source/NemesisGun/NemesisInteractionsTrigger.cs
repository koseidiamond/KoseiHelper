using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Triggers;
using static Celeste.Mod.KoseiHelper.KoseiHelperModuleSettings.NemesisInteractions;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunInteractions")]
public class NemesisGunInteractions : Trigger
{
    private TriggerMode triggerMode;

    public bool canKillPlayer = false, breakBounceBlocks = true, activateFallingBlocks = true, harmEnemies = true, moveSwapBlocks = true,
        breakSpinners = true, breakMovingBlades = true, collectables = true, useRefills = true, pressDashSwitches = true, canBounce = true,
        scareBirds = true, collectTouchSwitches = true, collectBadelineOrbs = true, useFeathers = true, coreModeToggles = true, collideWithPlatforms = true,
        moveMovingBlocks = false;
    public bool useBoosters = false;
    public float waterFriction;

    public KoseiHelperModuleSettings.NemesisInteractions.JellyfishBehavior jellyfishBehavior;
    public KoseiHelperModuleSettings.NemesisInteractions.TheoBehavior theoBehavior;
    public KoseiHelperModuleSettings.NemesisInteractions.PufferBehavior pufferBehavior;
    public KoseiHelperModuleSettings.NemesisInteractions.DreamBlockBehavior dreamBlockBehavior;
    public KoseiHelperModuleSettings.NemesisInteractions.DashBlockBehavior dashBlockBehavior;

    public NemesisGunInteractions(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);

        canKillPlayer = data.Bool("canKillPlayer", false);
        dreamBlockBehavior = data.Enum("dreamBlockBehavior", KoseiHelperModuleSettings.NemesisInteractions.DreamBlockBehavior.GoThrough);
        breakBounceBlocks = data.Bool("breakBounceBlocks", true);
        activateFallingBlocks = data.Bool("activateFallingBlocks", true);
        harmEnemies = data.Bool("harmEnemies", true);
        breakSpinners = data.Bool("breakSpinners", true);
        breakMovingBlades = data.Bool("breakMovingBlades", true);
        theoBehavior = data.Enum("theoInteraction", KoseiHelperModuleSettings.NemesisInteractions.TheoBehavior.HitSpring);
        jellyfishBehavior = data.Enum("jellyfishInteraction", KoseiHelperModuleSettings.NemesisInteractions.JellyfishBehavior.HitSpring);
        collectables = data.Bool("collectables", true);
        useRefills = data.Bool("useRefills", true);
        pufferBehavior = data.Enum("pufferInteraction", KoseiHelperModuleSettings.NemesisInteractions.PufferBehavior.Explode);
        pressDashSwitches = data.Bool("pressDashSwitches", true);
        canBounce = data.Bool("canBounce", true);
        useBoosters = data.Bool("useBoosters", false);
        waterFriction = data.Float("waterFriction", 0.995f);
        scareBirds = data.Bool("scareBirds", true);
        collectTouchSwitches = data.Bool("collectTouchSwitches", true);
        collectBadelineOrbs = data.Bool("collectBadelineOrbs", true);
        useFeathers = data.Bool("useFeathers", true);
        coreModeToggles = data.Bool("coreModeToggles", true);
        moveSwapBlocks = data.Bool("moveSwapBlocks", true);
        dashBlockBehavior = data.Enum("dashBlockBehavior", KoseiHelperModuleSettings.NemesisInteractions.DashBlockBehavior.BreakAll);
        collideWithPlatforms = data.Bool("collideWithPlatforms", true);
        moveMovingBlocks = data.Bool("moveMovingBlocks", false);
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
        KoseiHelperModule.Settings.GunInteractions.CanKillPlayer = canKillPlayer;
        KoseiHelperModule.Settings.GunInteractions.BreakBounceBlocks = breakBounceBlocks;
        KoseiHelperModule.Settings.GunInteractions.ActivateFallingBlocks = activateFallingBlocks;
        KoseiHelperModule.Settings.GunInteractions.HarmEnemies = harmEnemies;
        KoseiHelperModule.Settings.GunInteractions.BreakSpinners = breakSpinners;
        KoseiHelperModule.Settings.GunInteractions.BreakMovingBlades = breakMovingBlades;
        KoseiHelperModule.Settings.GunInteractions.Collectables = collectables;
        KoseiHelperModule.Settings.GunInteractions.UseRefills = useRefills;
        KoseiHelperModule.Settings.GunInteractions.PressDashSwitches = pressDashSwitches;
        KoseiHelperModule.Settings.GunInteractions.CanBounce = canBounce;
        KoseiHelperModule.Settings.GunInteractions.UseBoosters = useBoosters;
        KoseiHelperModule.Settings.GunInteractions.WaterFriction = waterFriction;
        KoseiHelperModule.Settings.GunInteractions.ScareBirds = scareBirds;
        KoseiHelperModule.Settings.GunInteractions.CollectTouchSwitches = collectTouchSwitches;
        KoseiHelperModule.Settings.GunInteractions.CollectBadelineOrbs = collectBadelineOrbs;
        KoseiHelperModule.Settings.GunInteractions.UseFeathers = useFeathers;
        KoseiHelperModule.Settings.GunInteractions.CoreModeToggles = coreModeToggles;
        KoseiHelperModule.Settings.GunInteractions.theoBehavior = theoBehavior;
        KoseiHelperModule.Settings.GunInteractions.jellyfishBehavior = jellyfishBehavior;
        KoseiHelperModule.Settings.GunInteractions.dreamBlockBehavior = dreamBlockBehavior;
        KoseiHelperModule.Settings.GunInteractions.pufferBehavior = pufferBehavior;
        KoseiHelperModule.Settings.GunInteractions.dashBlockBehavior = dashBlockBehavior;
        KoseiHelperModule.Settings.GunInteractions.MoveSwapBlocks = moveSwapBlocks;
        KoseiHelperModule.Settings.GunInteractions.CollideWithPlatforms = collideWithPlatforms;
        KoseiHelperModule.Settings.GunInteractions.MoveMovingBlocks = moveMovingBlocks;
    }
}