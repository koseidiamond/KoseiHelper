using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/ForceHiccupTrigger")]
public class ForceHiccupTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    private CassetteBlockManager cassetteManager;
    private int cassetteIndex;
    private bool canHiccup;
    public float interval;
    public string flag;
    private bool previousHasHiccuppedFromFlag, currentHasHiccuppedFromFlag;

    public ForceHiccupTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        interval = data.Float("interval", 0.5f);
        flag = data.Attr("flag", "cold");
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        cassetteManager = scene.Tracker.GetEntity<CassetteBlockManager>();
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Level level = SceneAs<Level>();
        if (player.Scene != null && triggerMode == TriggerMode.OnStay)
        {
            if (Scene.OnInterval(interval))
                player.HiccupJump();
        }
        currentHasHiccuppedFromFlag = level.Session.GetFlag(flag);
        if (player.Scene != null && triggerMode == TriggerMode.OnFlagEnabled)
        {
            if (currentHasHiccuppedFromFlag && !previousHasHiccuppedFromFlag)
            {
                player.HiccupJump();
            }
        }
        previousHasHiccuppedFromFlag = currentHasHiccuppedFromFlag;
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnEnter)
        {
            player.HiccupJump();
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (player.Scene != null && triggerMode == TriggerMode.OnLeave)
            player.HiccupJump();
        if (onlyOnce)
            RemoveSelf();
    }

    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        if (cassetteManager != null && triggerMode == TriggerMode.OnCassetteBeat && player != null)
        {
            if (cassetteIndex != cassetteManager.currentIndex)
            {
                cassetteIndex = cassetteManager.currentIndex;
                canHiccup = true;
            }
            if (canHiccup)
                player.HiccupJump();
        }
        canHiccup = false;
    }
}