using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/ReplaceSpeedWithStoredSpeedTrigger")]
public class ReplaceSpeedWithStoredSpeedTrigger : Trigger
{
    public bool onlyOnce;
    public bool replacesDirection;
    public bool addSpeed;
    public string firstSfx, replacementSfx;
    public float factor;
    public bool storeFirstSpeedOnly;
    public string flag;

    private float storedXA, storedXB, storedYA, storedYB;
    private bool storingCycle, hasStored, hasAltered, dontNeedToStore;

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
        flag = data.Attr("flag", "");
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();

        if (!string.IsNullOrEmpty(firstSfx))
        {
            if (hasStored)
                Audio.Play(replacementSfx);
            else
                Audio.Play(firstSfx);
        }
        if (!dontNeedToStore)
            Store(player, storingCycle);
        if (hasStored && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
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
        if (!storeFirstSpeedOnly)
            storingCycle = !storing;
        else
            dontNeedToStore = true;
    }

    public void Replace(Player player, bool storing)
    {
        if (storing)
        {
            switch (speedAxis)
            {
                case SpeedAxis.SpeedX:
                    if (addSpeed)
                    {
                        if (replacesDirection)
                            player.Speed.X += storedXB * factor;
                        else
                            player.Speed.X += storedXB * factor * Math.Sign(player.Speed.X);
                    }
                    else
                    {
                        if (replacesDirection)
                            player.Speed.X = storedXB * factor;
                        else
                            player.Speed.X = storedXB * factor * Math.Sign(player.Speed.X);
                    }
                    break;
                case SpeedAxis.SpeedY:
                    if (addSpeed)
                    {
                        if (replacesDirection)
                            player.Speed.Y += storedYB * factor;
                        else
                            player.Speed.Y += storedYB * factor * Math.Sign(player.Speed.Y);
                    }
                    else
                    {
                        if (replacesDirection)
                            player.Speed.Y = storedYB * factor;
                        else
                            player.Speed.Y = storedYB * factor * Math.Sign(player.Speed.Y);
                    }
                    break;
                case SpeedAxis.Swapped:
                    if (addSpeed)
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X += storedYB * factor;
                            player.Speed.Y += storedXB * factor;
                        }
                        else
                        {
                            player.Speed.X += storedYB * factor * Math.Sign(player.Speed.Y);
                            player.Speed.Y += storedXB * factor * Math.Sign(player.Speed.X);
                        }
                    }
                    else
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X = storedYB * factor;
                            player.Speed.Y = storedXB * factor;
                        }
                        else
                        {
                            player.Speed.X = storedYB * factor * Math.Sign(player.Speed.Y);
                            player.Speed.Y = storedXB * factor * Math.Sign(player.Speed.X);
                        }
                    }
                    break;
                default: // Both
                    if (addSpeed)
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X += storedXB * factor;
                            player.Speed.Y += storedYB * factor;
                        }
                        else
                        {
                            player.Speed.X += storedXB * factor * Math.Sign(player.Speed.X);
                            player.Speed.Y += storedYB * factor * Math.Sign(player.Speed.Y);
                        }
                    }
                    else
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X = storedXB * factor;
                            player.Speed.Y = storedYB * factor;
                        }
                        else
                        {
                            player.Speed.X = storedXB * factor * Math.Sign(player.Speed.X);
                            player.Speed.Y = storedYB * factor * Math.Sign(player.Speed.Y);
                        }
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
                    {
                        if (replacesDirection)
                            player.Speed.X += storedXA * factor;
                        else
                            player.Speed.X += storedXA * factor * Math.Sign(player.Speed.X);
                    }
                    else
                    {
                        if (replacesDirection)
                            player.Speed.X = storedXA * factor;
                        else
                            player.Speed.X = storedXA * factor * Math.Sign(player.Speed.X);
                    }
                    break;
                case SpeedAxis.SpeedY:
                    if (addSpeed)
                    {
                        if (replacesDirection)
                            player.Speed.Y += storedYA * factor;
                        else
                            player.Speed.Y += storedYA * factor * Math.Sign(player.Speed.Y);
                    }
                    else
                    {
                        if (replacesDirection)
                            player.Speed.Y = storedYA * factor;
                        else
                            player.Speed.Y = storedYA * factor * Math.Sign(player.Speed.Y);
                    }
                    break;
                case SpeedAxis.Swapped:
                    if (addSpeed)
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X += storedYA * factor;
                            player.Speed.Y += storedXA * factor;
                        }
                        else
                        {
                            player.Speed.X += storedYA * factor * Math.Sign(player.Speed.Y);
                            player.Speed.Y += storedXA * factor * Math.Sign(player.Speed.X);
                        }
                    }
                    else
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X = storedYA * factor;
                            player.Speed.Y = storedXA * factor;
                        }
                        else
                        {
                            player.Speed.X = storedYA * factor * Math.Sign(player.Speed.Y);
                            player.Speed.Y = storedXA * factor * Math.Sign(player.Speed.X);
                        }
                    }
                    break;
                default: // Both
                    if (addSpeed)
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X += storedXA * factor;
                            player.Speed.Y += storedYA * factor;
                        }
                        else
                        {
                            player.Speed.X += storedXA * factor * Math.Sign(player.Speed.X);
                            player.Speed.Y += storedYA * factor * Math.Sign(player.Speed.Y);
                        }
                    }
                    else
                    {
                        if (replacesDirection)
                        {
                            player.Speed.X = storedXA * factor;
                            player.Speed.Y = storedYA * factor;
                        }
                        else
                        {
                            player.Speed.X = storedXA * factor * Math.Sign(player.Speed.X);
                            player.Speed.Y = storedYA * factor * Math.Sign(player.Speed.Y);
                        }
                    }
                    break;
            }
        }
        hasAltered = true;
    }
}