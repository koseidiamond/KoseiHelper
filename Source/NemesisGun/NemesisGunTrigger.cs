using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Triggers;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunTrigger")]
public class NemesisGunTrigger : Trigger
{
    private bool replacesDash, enabled = true;
    private string gunshotSound;
    private int cooldown;
    private TriggerMode triggerMode;
    // INTERACTIONS
    public bool canKillPlayer, canGoThroughDreamBlocks, breakBounceBlocks, activateFallingBlocks, harmEnemies, harmTheo,
        breakSpinners, breakMovingBlades = true;

    public Extensions.TheoInteraction theoInteraction;

    public NemesisGunTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        enabled = data.Bool("enabled", true);
        gunshotSound = data.Attr("gunshotSound", "event:/ashleybl/gunshot");
        replacesDash = data.Bool("replacesDash", true);
        cooldown = data.Int("cooldown", 8);

        canKillPlayer = data.Bool("canKillPlayer", true);
        canGoThroughDreamBlocks = data.Bool("goThroughDreamBlocks", true);
        breakBounceBlocks = data.Bool("breakBounceBlocks", true);
        activateFallingBlocks = data.Bool("activateFallingBlocks", true);
        harmEnemies = data.Bool("harmEnemies", true);
        breakSpinners = data.Bool("breakSpinners", true);
        breakMovingBlades = data.Bool("breakMovingBlades", true);
        theoInteraction = data.Enum("theoInteraction", Extensions.TheoInteraction.Kill);

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
        Extensions.canKillPlayer = canKillPlayer;

        Extensions.canGoThroughDreamBlocks = canGoThroughDreamBlocks;
        Extensions.breakBounceBlocks = breakBounceBlocks;
        Extensions.activateFallingBlocks = activateFallingBlocks;
        Extensions.harmEnemies = harmEnemies;
        Extensions.harmTheo = harmTheo;
        Extensions.breakSpinners = breakSpinners;
        Extensions.breakMovingBlades = breakMovingBlades;
        Extensions.theoInteraction = theoInteraction;


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