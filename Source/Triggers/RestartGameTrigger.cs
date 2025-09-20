using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/RestartGameTrigger")]
public class RestartGameTrigger : Trigger
{
    public bool restartOnlyLevel = false;
    public TriggerMode triggerMode;
    public string flag;

    public RestartGameTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        restartOnlyLevel = data.Bool("restartOnlyLevel", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        flag = data.Attr("flag", "");
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnEnter && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            Restart();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnLeave && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            Restart();
        }
    }

    public override void OnStay(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnStay && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            Restart();
        }
    }

    public void Restart()
    {
        Level level = SceneAs<Level>();
        if (restartOnlyLevel)
            AreaData.Areas[level.Session.Area.ID].Mode[(int)level.Session.Area.Mode].MapData.Reload();
        else
            Everest.QuickFullRestart();
        RemoveSelf();
    }
}