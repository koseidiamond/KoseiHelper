using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Triggers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MagicInkTrigger")]
[Tracked]
public class MagicInkTrigger : Trigger
{
    public TriggerMode triggerMode;
    public bool onlyOnce;
    public string flag;
    public float inkAmount;
    public MagicInkTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnStay);
        flag = data.Attr("flag", "");
        onlyOnce = data.Bool("onlyOnce", false);
        inkAmount = data.Float("inkAmount", 0.5f);
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerMode == TriggerMode.OnEnter)
            GiveInk();
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (triggerMode == TriggerMode.OnLeave)
            GiveInk();
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (triggerMode == TriggerMode.OnStay && KoseiHelperUtils.CheckFlag(SceneAs<Level>(), flag))
            GiveInk();
    }

    public void GiveInk()
    {
        MagicInkController inkController = SceneAs<Level>().Tracker.GetEntity<MagicInkController>();
        inkController?.AddInk(inkAmount);
        if (onlyOnce)
            RemoveSelf();
    }
}