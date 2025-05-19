using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/ReplaceSpeedWithStoredSpeedTrigger")]
public class ReplaceSpeedWithStoredSpeedTrigger : Trigger
{
    public bool onlyOnce;
    public bool storesDirection;

    private bool alteredSpeedOnce, hasStoredSpeed;
    private float storedFirstSpeedX, storedFirstSpeedY, storedSpeedX, storedSpeedY;

    private bool notFirstTime;
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
        storesDirection = data.Bool("storesDirection", true);
        onlyOnce = data.Bool("onlyOnce", false);
        speedAxis = data.Enum("speedAxis", SpeedAxis.SpeedX);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (hasStoredSpeed)
        {
            ReplaceSpeed(player);
            StoreSpeed(player, true);
        }
        else
        {
            StoreSpeed(player, false);
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (onlyOnce && alteredSpeedOnce)
            RemoveSelf();
    }

    public void StoreSpeed(Player player, bool notFirstTime)
    {
        if (notFirstTime)
        {
        }
        else
        {
        }
            switch (speedAxis)
            {
                case SpeedAxis.SpeedX:
                    storedSpeedX = storesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                case SpeedAxis.SpeedY:
                    storedSpeedY = storesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
                case SpeedAxis.Swapped:
                    storedSpeedX = storesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    storedSpeedY = storesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    break;
                default: // Both
                    storedSpeedX = storesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
                    storedSpeedY = storesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
                    break;
            }
        Logger.Debug(nameof(KoseiHelperModule), $"storedSpeedX={storedSpeedX}, storedSpeedY={storedSpeedY}");
        hasStoredSpeed = true;
    }

    public void ReplaceSpeed(Player player)
    {
        switch (speedAxis)
        {
            case SpeedAxis.SpeedX:
                player.Speed.X = storedSpeedX;
                break;
            case SpeedAxis.SpeedY:
                player.Speed.Y = storedSpeedY;
                break;
            case SpeedAxis.Swapped:
                player.Speed.Y = storedSpeedX;
                player.Speed.X = storedSpeedY;
                break;
            default: // Both
                player.Speed.X = storedSpeedX;
                player.Speed.Y = storedSpeedY;
                break;
        }
        alteredSpeedOnce = true;
    }
}