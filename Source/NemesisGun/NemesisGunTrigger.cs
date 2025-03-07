using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Triggers;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisGunTrigger")]
public class NemesisGunTrigger : Trigger
{
    private bool replacesDash, canKillPlayer, enabled = true;
    private string gunshotSound;
    private int cooldown;
    private TriggerMode triggerMode;
    public NemesisGunTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        enabled = data.Bool("enabled", true);
        gunshotSound = data.Attr("gunshotSound", "event:/ashleybl/gunshot");
        replacesDash = data.Bool("replacesDash", true);
        cooldown = data.Int("cooldown", 8);
        canKillPlayer = data.Bool("canKillPlayer", true);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerMode == TriggerMode.OnEnter)
        {
            Extensions.gunshotSound = gunshotSound;
            Extensions.replacesDash = replacesDash;
            Extensions.cooldown = cooldown;
            Extensions.canKillPlayer = canKillPlayer;
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

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (triggerMode == TriggerMode.OnLeave)
        {
            Extensions.gunshotSound = gunshotSound;
            Extensions.replacesDash = replacesDash;
            Extensions.cooldown = cooldown;
            Extensions.canKillPlayer = canKillPlayer;
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

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (triggerMode == TriggerMode.OnStay && !player.JustRespawned && !player.IsIntroState)
        {
            Extensions.gunshotSound = gunshotSound;
            Extensions.replacesDash = replacesDash;
            Extensions.cooldown = cooldown;
            Extensions.canKillPlayer = canKillPlayer;
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
}