using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/SetWindowSizeTrigger")]
public class SetWindowSizeTrigger : Trigger
{
    public int windowWidthFrom, windowHeightFrom, windowWidthTo, windowHeightTo;
    public bool fullScreen;
    private PositionModes positionMode;
    private string direction;
    public SetWindowSizeTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        windowWidthFrom = data.Int("widthFrom", 100);
        windowHeightFrom = data.Int("heightFrom", 100);
        windowWidthTo = data.Int("widthTo", 100);
        windowHeightTo = data.Int("heightTo", 100);
        fullScreen = data.Bool("fullScreen", false);
        direction = data.Attr("positionMode");
        if (!string.IsNullOrEmpty(direction) && Enum.TryParse<PositionModes>(direction.ToString(), ignoreCase: true, out var result))
            positionMode = result;

    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Level level = SceneAs<Level>();
        if (fullScreen)
            Celeste.SetFullscreen();
        else
        {
            float positionLerp = GetPositionLerp(player, positionMode);
            int width = (int)MathHelper.Lerp(windowWidthFrom, windowWidthTo, positionLerp);
            int height = (int)MathHelper.Lerp(windowHeightFrom, windowHeightTo, positionLerp);
            Celeste.SetWindowed(width, height);
        }
    }
}