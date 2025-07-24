using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/SwapTilesetTrigger")]
public class SwapTilesetTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public string flag;
    public char fromTileset, toTileset;
    public bool alternate = true;
    private bool lastFlagValue;

    public SwapTilesetTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        flag = data.Attr("flag", "");
        fromTileset = data.Char("fromTileset", '3');
        toTileset = data.Char("toTileset", 'n');
        alternate = data.Bool("alternate", true);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Level level = SceneAs<Level>();
        if (player.Scene != null && triggerMode == TriggerMode.OnStay)
        {
            if (string.IsNullOrEmpty(flag)) return;
            bool now = (Scene as Level).Session.GetFlag(flag);
            if (now != lastFlagValue)
            {
                SwapTilesets(Scene as Level, fromTileset, toTileset);
                lastFlagValue = now;

                if (onlyOnce)
                    RemoveSelf();
            }
        }
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (player.Scene != null && triggerMode == TriggerMode.OnEnter)
        {
            if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag))
            {
                SwapTilesets(level, fromTileset, toTileset);
                if (onlyOnce)
                    RemoveSelf();
            }
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        Level level = SceneAs<Level>();
        if (player.Scene != null && triggerMode == TriggerMode.OnLeave)
        {
            if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag))
            {
                SwapTilesets(level, fromTileset, toTileset);
                if (onlyOnce)
                    RemoveSelf();
            }
        }
    }

    private void SwapTilesets(Level level, char swapFromTileID, char swapToTileID)
    {
        Logger.Debug(nameof(KoseiHelperModule), $"Swapping tilesets!");
        Rectangle tileBounds = level.Session.MapData.TileBounds;
        Rectangle tileBounds2 = level.Session.LevelData.TileBounds;
        Rectangle rectangle = new Rectangle(tileBounds2.X - tileBounds.X, tileBounds2.Y - tileBounds.Y, tileBounds2.Width, tileBounds2.Height);
        Rectangle rectangle2 = rectangle;
        VirtualMap<char> virtualMap = new VirtualMap<char>(rectangle2.Width + 2, rectangle2.Height + 2, '0');
        for (int i = -1; i < rectangle2.Width + 1; i++)
        {
            for (int j = -1; j < rectangle2.Height + 1; j++)
            {
                virtualMap[i, j] = level.SolidsData[i + level.LevelSolidOffset.X, j + level.LevelSolidOffset.Y];
            }
        }
        for (int k = 0; k < rectangle2.Width; k++)
        {
            for (int l = 0; l < rectangle2.Height; l++)
            {
                int x = k + rectangle2.X;
                int y = l + rectangle2.Y;
                if (level.SolidsData[x, y] == swapFromTileID && swapFromTileID != swapToTileID)
                {
                    level.SolidsData[x, y] = swapToTileID;
                    virtualMap[k, l] = swapToTileID;
                }
            }
        }
        Autotiler.Generated generated = GFX.FGAutotiler.GenerateMap(virtualMap, paddingIgnoreOutOfLevel: true);
        VirtualMap<bool> data = ((Grid)level.SolidTiles.Collider).Data;
        for (int m = 0; m <= rectangle2.Width; m++)
        {
            for (int n = 0; n <= rectangle2.Height; n++)
            {
                int x2 = m + rectangle2.X;
                int y2 = n + rectangle2.Y;
                if (level.SolidsData[x2, y2] != swapToTileID)
                    continue;
                MTexture mTexture = generated.TileGrid.Tiles[m, n];
                level.SolidTiles.Tiles.Tiles[x2, y2] = mTexture;
                data[x2, y2] = mTexture != null;
            }
        }
        if (alternate)
            (fromTileset, toTileset) = (toTileset, fromTileset);
    }
}