using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/StoreSpeedTrigger")]
public class StoreSpeedTrigger : Trigger
{
    public bool onlyOnce;
    public bool addSpeed;
    public string storeSfx;
    public float factor;
    public string flag;
    public bool storesDirection;

    public static float? storedSpeedX = null;
    public static float? storedSpeedY = null;

    public bool hasStored = false;

    public StoreSpeedTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        addSpeed = data.Bool("addSpeed", false);
        storeSfx = data.Attr("storeSfx", "event:/none");
        factor = data.Float("factor", 1f);
        flag = data.Attr("flag", "");
        storesDirection = data.Bool("storesDirection", true);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();

        if (hasStored)
            return;
        if (!string.IsNullOrEmpty(storeSfx))
            Audio.Play(storeSfx);

        if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag))
        {
            storedSpeedX = storesDirection ? player.Speed.X : Math.Abs(player.Speed.X);
            storedSpeedY = storesDirection ? player.Speed.Y : Math.Abs(player.Speed.Y);
        }
        hasStored = true;

        if (onlyOnce)
            RemoveSelf();
    }

    public static bool HasStoredSpeed()
    {
        return storedSpeedX.HasValue && storedSpeedY.HasValue;
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        storedSpeedX = null;
        storedSpeedY = null;
    }
}