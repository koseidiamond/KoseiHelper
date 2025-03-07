using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Triggers;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunInteractions")]
public class NemesisGunInteractions : Trigger
{
    private TriggerMode triggerMode;

    public bool canKillPlayer, canGoThroughDreamBlocks, breakBounceBlocks, activateFallingBlocks, harmEnemies, harmTheo,
        breakSpinners, breakMovingBlades, enableKevins, collectables, useRefills, explodeFishes, pressDashSwitches, canBounce = true;

    public Extensions.TheoInteraction theoInteraction;

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
        explodeFishes = data.Bool("explodeFishes", true);
        pressDashSwitches = data.Bool("pressDashSwitches", true);
        canBounce = data.Bool("canBounce", true);
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
        Extensions.explodeFishes = explodeFishes;
        Extensions.pressDashSwitches = pressDashSwitches;
        Extensions.canBounce = canBounce;
    }
}