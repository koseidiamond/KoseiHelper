using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/ReplaceSpeedWithStoredSpeedTrigger")]
public class ReplaceSpeedWithStoredSpeedTrigger : Trigger
{
    public bool onlyOnce;
    public bool replacesDirection; // TODO fix this
    public bool addSpeed;
    public string firstSfx, replacementSfx;
    public float factor;

    private bool alteredSpeedOnce;
    private float storedSpeedX, storedSpeedY;

    public enum SpeedAxis
    {
        SpeedX,
        SpeedY,
        Both,
        Swapped
    };

    private SpeedAxis speedAxis;

    public ReplaceSpeedWithStoredSpeedTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        replacesDirection = data.Bool("storesDirection", true);
        onlyOnce = data.Bool("onlyOnce", false);
        speedAxis = data.Enum("speedAxis", SpeedAxis.SpeedX);
        addSpeed = data.Bool("addSpeed", false);
        firstSfx = data.Attr("firstSfx", "event:/none");
        replacementSfx = data.Attr("replacementSfx", "event:/none");
        factor = data.Float("factor", 1f);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        ReplaceSpeed(player);
        StoreSpeed(player);
        if (!string.IsNullOrEmpty(firstSfx))
            Audio.Play(firstSfx);
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (onlyOnce && alteredSpeedOnce)
            RemoveSelf();
    }

    public void StoreSpeed(Player player)
    {
            switch (speedAxis)
            {
                case SpeedAxis.SpeedX:
                    storedSpeedX = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                case SpeedAxis.SpeedY:
                    storedSpeedY = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
                case SpeedAxis.Swapped:
                    storedSpeedX = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    storedSpeedY = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                default: // Both
                    storedSpeedX = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    storedSpeedY = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
        }
    }

    public void ReplaceSpeed(Player player)
    {
        switch (speedAxis)
        {
            case SpeedAxis.SpeedX:
                if (addSpeed)
                    player.Speed.X += storedSpeedX * factor;
                else
                    player.Speed.X = storedSpeedX * factor;
                break;
            case SpeedAxis.SpeedY:
                if (addSpeed)
                    player.Speed.Y += storedSpeedY * factor;
                else
                    player.Speed.Y = storedSpeedY * factor;
                break;
            case SpeedAxis.Swapped:
                if (addSpeed)
                {
                    player.Speed.X += storedSpeedY * factor;
                    player.Speed.Y += storedSpeedX * factor;
                }
                else
                {
                    player.Speed.X = storedSpeedY * factor;
                    player.Speed.Y = storedSpeedX * factor;
                }
                break;
            default: // Both
                if (addSpeed)
                {
                    player.Speed.X += storedSpeedX * factor;
                    player.Speed.Y += storedSpeedY * factor;
                }
                else
                {
                    player.Speed.X = storedSpeedX * factor;
                    player.Speed.Y = storedSpeedY * factor;
                }
                break;
        }
        alteredSpeedOnce = true;
        if (!string.IsNullOrEmpty(replacementSfx))
            Audio.Play(replacementSfx);
    }
}