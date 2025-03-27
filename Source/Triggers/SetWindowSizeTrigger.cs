using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/SetWindowSizeTrigger")]
public class SetWindowSizeTrigger : Trigger
{
    public int windowWidth, windowHeight;
    public bool onLeave, fullScreen;
    public SetWindowSizeTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        windowWidth = data.Int("windowWidth", 100);
        windowHeight = data.Int("windowHeight", 100);
        onLeave = data.Bool("onLeave", false);
        fullScreen = data.Bool("fullScreen", false);

    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (!onLeave)
        {
            if (fullScreen)
                Celeste.SetFullscreen();
            else
                Celeste.SetWindowed(windowWidth, windowHeight);
        }
    }
    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (onLeave)
        {
            if (fullScreen)
                Celeste.SetFullscreen();
            else
                Celeste.SetWindowed(windowWidth, windowHeight);
        }
    }
}