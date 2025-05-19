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
    public bool storeFirstSpeedOnly;

    private float storedXA, storedXB, storedYA, storedYB;
    private bool storingCycle, hasStored, hasAltered;
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
        storeFirstSpeedOnly = data.Bool("storeFirstSpeedOnly", false);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);

        if (!string.IsNullOrEmpty(firstSfx))
        {
            if (hasStored)
                Audio.Play(replacementSfx);
            else
                Audio.Play(firstSfx);
        }
        Store(player, storingCycle);
        if (hasStored)
            Replace(player, storingCycle);
        hasStored = true;
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (onlyOnce && hasAltered)
            RemoveSelf();
    }

    public void Store(Player player, bool storing)
    {
        if (!storing)
        {
            switch (speedAxis)
            {
                case SpeedAxis.SpeedX:
                    storedXA = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                case SpeedAxis.SpeedY:
                    storedYA = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
                case SpeedAxis.Swapped:
                    storedXA = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    storedYA = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                default: // Both
                    storedXA = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    storedYA = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
            }
        }
        else
        {
            switch (speedAxis)
            {
                case SpeedAxis.SpeedX:
                    storedXB = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                case SpeedAxis.SpeedY:
                    storedYB = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
                case SpeedAxis.Swapped:
                    storedXB = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    storedYB = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                default: // Both
                    storedXB = replacesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    storedYB = replacesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
            }
        }
            storingCycle = !storing;
    }

    public void Replace(Player player, bool storing)
    {
        if (storing)
        {
            switch (speedAxis)
            {
                case SpeedAxis.SpeedX:
                    if (addSpeed)
                        player.Speed.X += storedXB * factor;
                    else
                        player.Speed.X = storedXB * factor;
                    break;
                case SpeedAxis.SpeedY:
                    if (addSpeed)
                        player.Speed.Y += storedYB * factor;
                    else
                        player.Speed.Y = storedYB * factor;
                    break;
                case SpeedAxis.Swapped:
                    if (addSpeed)
                    {
                        player.Speed.X += storedYB * factor;
                        player.Speed.Y += storedXB * factor;
                    }
                    else
                    {
                        player.Speed.X = storedYB * factor;
                        player.Speed.Y = storedXB * factor;
                    }
                    break;
                default: // Both
                    if (addSpeed)
                    {
                        player.Speed.X += storedXB * factor;
                        player.Speed.Y += storedYB * factor;
                    }
                    else
                    {
                        player.Speed.X = storedXB * factor;
                        player.Speed.Y = storedYB * factor;
                    }
                    break;
            }
        }
        else
        {
            switch (speedAxis)
            {
                case SpeedAxis.SpeedX:
                    if (addSpeed)
                        player.Speed.X += storedXA * factor;
                    else
                        player.Speed.X = storedXA * factor;
                    break;
                case SpeedAxis.SpeedY:
                    if (addSpeed)
                        player.Speed.Y += storedYA * factor;
                    else
                        player.Speed.Y = storedYA * factor;
                    break;
                case SpeedAxis.Swapped:
                    if (addSpeed)
                    {
                        player.Speed.X += storedYA * factor;
                        player.Speed.Y += storedXA * factor;
                    }
                    else
                    {
                        player.Speed.X = storedYA * factor;
                        player.Speed.Y = storedXA * factor;
                    }
                    break;
                default: // Both
                    if (addSpeed)
                    {
                        player.Speed.X += storedXA * factor;
                        player.Speed.Y += storedYA * factor;
                    }
                    else
                    {
                        player.Speed.X = storedXA * factor;
                        player.Speed.Y = storedYA * factor;
                    }
                    break;
            }
        }
        hasAltered = true;
    }
}