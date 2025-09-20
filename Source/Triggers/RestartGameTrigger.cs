using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/RestartGameTrigger")]
public class RestartGameTrigger : Trigger
{
    public bool restartOnlyLevel = false;
    public TriggerMode triggerMode;
    public string flag;
    public bool onlyOnce;
    public float delay; // it crashes the game sometimes so it is always 0 sorry
    private EntityID id;
    private bool triggered = false;

    public RestartGameTrigger(EntityData data, Vector2 offset, EntityID id) : base(data, offset)
    {
        restartOnlyLevel = data.Bool("restartOnlyLevel", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        flag = data.Attr("flag", "");
        onlyOnce = data.Bool("onlyOnce", false);
        delay = data.Float("delay", 0f);
        this.id = id;
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnEnter && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            if (delay > 0f)
                Add(new Coroutine(RestartRoutine()));
            else
                Restart();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnLeave && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            if (delay > 0f)
                Add(new Coroutine(RestartRoutine()));
            else
                Restart();
        }
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnStay && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
        {
            if (delay > 0f)
                Add(new Coroutine(RestartRoutine()));
            else
                if (!AssetReloadHelper.IsReloading)
                AssetReloadHelper.ReloadLevel(); //Restart();
        }
    }

    private void Restart()
    {
        Level level = SceneAs<Level>();

        if (restartOnlyLevel)
        {
            if (!AssetReloadHelper.IsReloading)
                AssetReloadHelper.ReloadLevel();
            if (onlyOnce)
                KoseiHelperModule.Session.SoftDoNotLoad.Add(id);
        }
        else
        {
            if (onlyOnce)
                KoseiHelperModule.Session.SoftDoNotLoad.Add(id);
            Everest.QuickFullRestart();
        }
        RemoveSelf();
    }

    private IEnumerator RestartRoutine()
    {
        if (triggered)
            yield break;
        triggered = true;

        if (delay > 0f)
            yield return delay;
        else
            yield return null;
        Restart();
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (restartOnlyLevel && KoseiHelperModule.Session.SoftDoNotLoad.Contains(id))
        {
            RemoveSelf();
        }
    }
}