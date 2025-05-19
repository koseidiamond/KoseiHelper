using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/SliderFadeTrigger")]
public class SliderFadeTrigger : Trigger
{
    public float fadeFrom, fadeTo;
    private PositionModes positionMode;
    public bool onlyOnce;
    public string direction;
    public string sliderName;
    public SliderFadeTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        sliderName = data.Attr("sliderName", "");
        onlyOnce = data.Bool("onlyOnce", false);
        direction = data.Attr("positionMode");
        fadeFrom = data.Float("fadeFrom", 0f);
        fadeTo = data.Float("fadeTo", 0.5f);
        if (!string.IsNullOrEmpty(direction) && Enum.TryParse<PositionModes>(direction.ToString(), ignoreCase: true, out var result))
            positionMode = result;
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Level level = SceneAs<Level>();
        float positionLerp = GetPositionLerp(player, positionMode);
        level.Session.SetSlider(sliderName, MathHelper.Lerp(fadeFrom, fadeTo, positionLerp));
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (onlyOnce)
            RemoveSelf();
    }
}