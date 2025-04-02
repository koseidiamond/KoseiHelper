using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/ReverseDepthsTrigger")]
public class ReverseDepthsTrigger : Trigger
{
    public bool onlyOnce;
    public bool onLeave;
    public bool reverseTiles, reverseEntities;
    public static bool reverseStylegrounds;
    private bool isReversed = false;
    private bool[,] array;
    private Entity bgSolidTiles;
    public string flag;
    public ReverseDepthsTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        onLeave = data.Bool("onLeave", false);
        reverseStylegrounds = data.Bool("reverseStylegrounds", false);
        reverseTiles = data.Bool("reverseTiles", true);
        reverseEntities = data.Bool("reverseEntities", false);
        flag = data.Attr("flag", "");
        Add(new TransitionListener
        {
            OnOut = delegate
            {
                DisableEffects();
            }
        });
    }

    private void DisableEffects()
    {
        Level level = SceneAs<Level>();
        level.Tracker.GetEntity<Player>().Depth = 0;
        if (bgSolidTiles != null)
            base.Scene.Remove(bgSolidTiles);
        if (level.Session.GetFlag("KoseiHelper_ReversedLevel"))
        {
            if (reverseStylegrounds)
            {
                level.Background.Backdrops.Reverse();
                level.Foreground.Backdrops.Reverse();
            }
            level.Session.SetFlag("KoseiHelper_ReversedLevel", false);
        }
        foreach (Entity entity in Scene.Entities)
        {
            if (entity is SolidTiles foregroundTiles)
                foregroundTiles.Depth = -10000;
            if (entity is BackgroundTiles backgroundTiles)
            {
                backgroundTiles.Depth = 10000;
            }
        }
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && !onLeave)
        {
            if (string.IsNullOrEmpty(flag) || SceneAs<Level>().Session.GetFlag(flag))
                ReverseDepths();
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        if (player.Scene != null && onLeave)
        {
            if (string.IsNullOrEmpty(flag) || SceneAs<Level>().Session.GetFlag(flag))
                ReverseDepths();
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        DisableEffects();
        if (reverseTiles)
        {
            Rectangle rectangle = new Rectangle(level.Bounds.Left / 8, level.Bounds.Y / 8, level.Bounds.Width / 8, level.Bounds.Height / 8);
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            array = new bool[rectangle.Width, rectangle.Height];
            for (int i = 0; i < rectangle.Width; i++)
            {
                for (int j = 0; j < rectangle.Height; j++)
                {
                    array[i, j] = level.BgData[i + rectangle.Left - tileBounds.Left, j + rectangle.Top - tileBounds.Top] != '0';
                }
            }
            bgSolidTiles = new Solid(new Vector2(level.Bounds.Left, level.Bounds.Top), 1f, 1f, safe: true);
            base.Scene.Add(bgSolidTiles);
            bgSolidTiles.Collider = new Grid(8f, 8f, array);
            bgSolidTiles.Collidable = false;
        }
    }

    private void ReverseDepths()
    {
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (reverseStylegrounds)
        {
            level.Background.Backdrops.Reverse();
            level.Foreground.Backdrops.Reverse();
        }
        if (isReversed)
        {
            player.Depth = 10001;
            level.Session.SetFlag("KoseiHelper_ReversedLevel", false);
        }
        else
        {
            player.Depth = 0;
            level.Session.SetFlag("KoseiHelper_ReversedLevel", true);
        }
            foreach (Entity entity in Scene.Entities)
            {
                if (reverseEntities)
                    entity.Depth = -entity.Depth;
                if (reverseTiles)
                {
                    if (entity is SolidTiles foregroundTiles)
                        foregroundTiles.Collidable = isReversed;
                    if (entity is BackgroundTiles backgroundTiles)
                    {
                        backgroundTiles.Collidable = isReversed;
                    }
                }
            }
        if (reverseTiles)
            bgSolidTiles.Collidable = !bgSolidTiles.Collidable;
        isReversed = !isReversed;
    }
}