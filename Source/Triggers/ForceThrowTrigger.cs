using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/ForceThrowTrigger")]
public class ForceThrowTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public string flag;
    public ForceThrowTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnStay);
        flag = data.Attr("flag", "");
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Level level = SceneAs<Level>();
        if (player.Scene != null && triggerMode == TriggerMode.OnStay && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            player.Throw();
            if (onlyOnce)
                RemoveSelf();
        }
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (player.Scene != null && triggerMode == TriggerMode.OnEnter && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            player.Throw();
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (player.Scene != null && triggerMode == TriggerMode.OnLeave && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            player.Throw();
            if (onlyOnce)
                RemoveSelf();
        }
    }
    public override void Update()
    {
        base.Update();
    }
}