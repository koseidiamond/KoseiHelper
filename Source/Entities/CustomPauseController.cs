using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomPauseController")]
public class CustomPauseController : Entity
{
    private Level level;
    private LevelExit levelExit;
    public bool canPause = true;
    public bool canRetry = true;
    public bool canSaveAndQuit = true;
    public bool timerIsStopped = false;
    public bool pauseTimerWhilePauseMenu = false;
    private Player player;
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
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        level.PauseLock = !canPause;
        level.CanRetry = canRetry;
        level.SaveQuitDisabled = canSaveAndQuit;
        level.TimerStopped = timerIsStopped;
        if (level.Paused && pauseTimerWhilePauseMenu)
            level.TimerStopped = true;
    }

    public override void Update()
    {
        player = Scene.Tracker.GetEntity<Player>();
        //if (level.wasPaused)
        if (Scene.OnInterval(2f))
            Add(new Coroutine(killPlayerRoutine(player)));
        if (Scene.OnInterval(1f)) Logger.Log(LogLevel.Info, "hello", "update");
    }

    private IEnumerator killPlayerRoutine(Player player)
    {
        Logger.Log(LogLevel.Info, "hello", "please kill me");
        player.Die(Vector2.Zero, true, true);
        Logger.Log(LogLevel.Info, "hello", "i died???");
        yield return 0.15f;
    }
}