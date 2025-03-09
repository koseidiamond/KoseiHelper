using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Triggers;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunInteractions")]
public class NemesisGunInteractions : Trigger
{
    private TriggerMode triggerMode;

    public bool canKillPlayer = true, canGoThroughDreamBlocks = true, breakBounceBlocks = true, activateFallingBlocks = true, harmEnemies = true, harmTheo = true,
        breakSpinners = true, breakMovingBlades = true, enableKevins = true, collectables = true, useRefills = true, pressDashSwitches = true, canBounce = true,
        scareBirds = true, collectTouchSwitches = true, collectBadelineOrbs = true;
    public bool useBoosters = false;
    public float waterFriction;

    public Extensions.TheoInteraction theoInteraction;
    public Extensions.PufferInteraction pufferInteraction;

    public NemesisGunInteractions(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);

        canKillPlayer = data.Bool("canKillPlayer", true);
        canGoThroughDreamBlocks = data.Bool("goThroughDreamBlocks", true);
        breakBounceBlocks = data.Bool("breakBounceBlocks", true);
        activateFallingBlocks = data.Bool("activateFallingBlocks", true);
        harmEnemies = data.Bool("harmEnemies", true);
        breakSpinners = data.Bool("breakSpinners", true);
        breakMovingBlades = data.Bool("breakMovingBlades", true);
        theoInteraction = data.Enum("theoInteraction", Extensions.TheoInteraction.HitSpring);
        enableKevins = data.Bool("enableKevins", true);
        collectables = data.Bool("collectables", true);
        useRefills = data.Bool("useRefills", true);
        pufferInteraction = data.Enum("pufferInteraction", Extensions.PufferInteraction.Explode);
        pressDashSwitches = data.Bool("pressDashSwitches", true);
        canBounce = data.Bool("canBounce", true);
        useBoosters = data.Bool("useBoosters", false);
        waterFriction = data.Float("waterFriction", 0.995f);
        scareBirds = data.Bool("scareBirds", true);
        collectTouchSwitches = data.Bool("collectTouchSwitches", true);
        collectBadelineOrbs = data.Bool("collectBadelineOrbs", true);
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
        Extensions.canKillPlayer = canKillPlayer;
        Extensions.canGoThroughDreamBlocks = canGoThroughDreamBlocks;
        Extensions.breakBounceBlocks = breakBounceBlocks;
        Extensions.activateFallingBlocks = activateFallingBlocks;
        Extensions.harmEnemies = harmEnemies;
        Extensions.harmTheo = harmTheo;
        Extensions.breakSpinners = breakSpinners;
        Extensions.breakMovingBlades = breakMovingBlades;
        Extensions.theoInteraction = theoInteraction;
        Extensions.enableKevins = enableKevins;
        Extensions.collectables = collectables;
        Extensions.useRefills = useRefills;
        Extensions.pufferInteraction = pufferInteraction;
        Extensions.pressDashSwitches = pressDashSwitches;
        Extensions.canBounce = canBounce;
        Extensions.useBoosters = useBoosters;
        Extensions.waterFriction = waterFriction;
        Extensions.scareBirds = scareBirds;
        Extensions.collectTouchSwitches = collectTouchSwitches;
        Extensions.collectBadelineOrbs = collectBadelineOrbs;
    }
}