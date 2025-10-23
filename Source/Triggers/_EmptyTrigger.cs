using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Triggers;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/_EmptyTrigger")]
[Tracked]
public class _EmptyTrigger : Trigger
{
    public TriggerMode triggerMode;
    public bool onlyOnce;
    public string flag;
    public _EmptyTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        flag = data.Attr("flag", "");
        onlyOnce = data.Bool("onlyOnce", false);
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerMode == TriggerMode.OnEnter)
            SomeMethod();
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (triggerMode == TriggerMode.OnLeave)
            SomeMethod();
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (triggerMode == TriggerMode.OnFlagEnabled && !string.IsNullOrEmpty(flag) && SceneAs<Level>().Session.GetFlag(flag))
            SomeMethod();
    }

    public void SomeMethod()
    {
        if (onlyOnce)
            RemoveSelf();
    }
}