using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomPauseController")]
public class CustomPauseController : Entity
{
    public bool canPause = true;
    public bool canRetry = true;
    public bool canSaveAndQuit = true;
    public bool timerIsStopped, timerHidden = false;
    public bool pauseTimerWhilePauseMenu = false;
    public bool dieOnUnpause = false;
    public string flagWhilePaused = "KoseiHelper_GameIsPaused";

    public CustomPauseController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        canPause = data.Bool("canPause", true);
        canRetry = data.Bool("canRetry", true);
        canSaveAndQuit = data.Bool("canSaveAndQuit", true);
        timerIsStopped = data.Bool("timerIsStopped", false);
        timerHidden = data.Bool("timerHidden", false);
        pauseTimerWhilePauseMenu = data.Bool("pauseTimerWhilePauseMenu", false);
        dieOnUnpause = data.Bool("dieOnUnpause", false);
        flagWhilePaused = data.Attr("flagWhilePaused", "KoseiHelper_GameIsPaused");
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        level.unpauseTimer = 0;
        level.Session.SetFlag(flagWhilePaused, false);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        level.PauseLock = !canPause;
        level.CanRetry = canRetry;
        level.SaveQuitDisabled = !canSaveAndQuit;
        level.TimerStopped = timerIsStopped;
        level.TimerHidden = timerHidden;
        level.PauseMainMenuOpen = true;
        level.Session.SetFlag(flagWhilePaused, false);
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

    public override void Render() // The entity doesn't update while the game is paused so we're using this
    {
        Level level = SceneAs<Level>();
        if (level.PauseMainMenuOpen && level.Paused)
            level.Session.SetFlag(flagWhilePaused, true);
        else
            level.Session.SetFlag(flagWhilePaused, false);
    }
}