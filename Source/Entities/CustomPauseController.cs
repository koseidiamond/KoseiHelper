using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomPauseController")]
public class CustomPauseController : Entity
{
    public bool canPause = true;
    public bool canRetry = true;
    public bool canSaveAndQuit = true;
    public bool timerIsStopped = false;
    public bool pauseTimerWhilePauseMenu = false;
    public bool dieOnUnpause = false;

    public CustomPauseController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        canPause = data.Bool("canPause", true);
        canRetry = data.Bool("canRetry", true);
        canSaveAndQuit = data.Bool("canSaveAndQuit", true);
        timerIsStopped = data.Bool("timerIsStopped", false);
        pauseTimerWhilePauseMenu = data.Bool("pauseTimerWhilePauseMenu", false);
        dieOnUnpause = data.Bool("dieOnUnpause", false);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        level.unpauseTimer = 0;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        level.PauseLock = !canPause;
        level.CanRetry = canRetry;
        level.SaveQuitDisabled = !canSaveAndQuit;
        level.TimerStopped = timerIsStopped;
        level.PauseMainMenuOpen = true;
        if (level.Paused)
            level.TimerStopped = true;
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (Scene.Tracker.GetEntity<Player>() is not { } player) // Checks that there is a player
            return;
        if (level.unpauseTimer < 0 && dieOnUnpause)
            Add(new Coroutine(killPlayerRoutine()));
    }
    private IEnumerator killPlayerRoutine()
    {
        yield return 0.03f;
        if (Scene.Tracker.GetEntity<Player>() is not { } player)
            yield break;
        player.Die(Vector2.Zero, true, true);
    }
}