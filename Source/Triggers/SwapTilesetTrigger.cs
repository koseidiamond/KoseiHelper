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
    private bool bgTiles;
    private bool flash;
    private Color flashColor;
    private string sound;

    public SwapTilesetTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        flag = data.Attr("flag", "");
        fromTileset = data.Char("fromTileset", '3');
        toTileset = data.Char("toTileset", 'n');
        alternate = data.Bool("alternate", true);
        bgTiles = data.Bool("bgTiles", false);
        flash = data.Bool("flash", false);
        sound = data.Attr("sound", "event:/none");
        flashColor = data.HexColor("flashColor", Color.White);
        if (data.Bool("persistent", false))
            base.Tag = Tags.Persistent;
        if (data.Bool("global", false))
            base.Tag = Tags.Global;
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
                SwapTilesets(level);
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
                SwapTilesets(level);
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
                SwapTilesets(level);
                if (onlyOnce)
                    RemoveSelf();
            }
        }
    }

    private void SwapTilesets(Level level)
    {
        if (!bgTiles)
        {
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            Rectangle tileBounds2 = level.Session.LevelData.TileBounds;
            Rectangle rectangle = new Rectangle(tileBounds2.X - tileBounds.X, tileBounds2.Y - tileBounds.Y, tileBounds2.Width, tileBounds2.Height);
            Rectangle rectangle2 = rectangle;
            VirtualMap<char> virtualMap = new VirtualMap<char>(rectangle2.Width + 2, rectangle2.Height + 2, '0');

            for (int i = -1; i < rectangle2.Width + 1; i++) // Copy foreground tiles into virtualMap
            {
                for (int j = -1; j < rectangle2.Height + 1; j++)
                {
                    virtualMap[i, j] = level.SolidsData[i + level.LevelSolidOffset.X, j + level.LevelSolidOffset.Y];
                }
            }

            for (int k = 0; k < rectangle2.Width; k++) // Swap foreground tiles
            {
                for (int l = 0; l < rectangle2.Height; l++)
                {
                    int x = k + rectangle2.X;
                    int y = l + rectangle2.Y;

                    // Only swap if the current tile is not already the target tileset
                    if (level.SolidsData[x, y] == fromTileset && fromTileset != toTileset)
                    {
                        level.SolidsData[x, y] = toTileset;
                        virtualMap[k, l] = toTileset;
                    }
                }
            }

            Autotiler.Generated generated = GFX.FGAutotiler.GenerateMap(virtualMap, paddingIgnoreOutOfLevel: true);
            VirtualMap<bool> data = ((Grid)level.SolidTiles.Collider).Data;

            for (int m = 0; m <= rectangle2.Width; m++) // Apply the generated foreground tiles
            {
                for (int n = 0; n <= rectangle2.Height; n++)
                {
                    int x2 = m + rectangle2.X;
                    int y2 = n + rectangle2.Y;

                    // Only apply if the tile's current texture matches the target tileset
                    if (level.SolidsData[x2, y2] != toTileset)
                        continue;

                    MTexture mTexture = generated.TileGrid.Tiles[m, n];
                    level.SolidTiles.Tiles.Tiles[x2, y2] = mTexture;
                    data[x2, y2] = mTexture != null;
                }
            }
        }
        else
        {
            if (bgTiles)
            {
                VirtualMap<char> bgData = level.BgData;
                VirtualMap<MTexture> bgTexes = level.BgTiles.Tiles.Tiles;
                VirtualMap<char> bgVirtualMap = new VirtualMap<char>(bgData.Columns + 2, bgData.Rows + 2, '0');
                for (int i = 0; i < bgData.Columns; i++) // Copy background tiles into virtualMap
                {
                    for (int j = 0; j < bgData.Rows; j++)
                    {
                        bgVirtualMap[i + 1, j + 1] = bgData[i, j];
                    }
                }

                for (int k = 0; k < bgData.Columns; k++) // Swap background tiles
                {
                    for (int l = 0; l < bgData.Rows; l++)
                    {
                        // Only swap if the current tile is not already the target tileset
                        if (bgData[k, l] == fromTileset && fromTileset != toTileset)
                        {
                            bgData[k, l] = toTileset;
                            bgVirtualMap[k + 1, l + 1] = toTileset;
                        }
                    }
                }

                Autotiler.Generated bgGenerated = GFX.BGAutotiler.GenerateMap(bgVirtualMap, paddingIgnoreOutOfLevel: true);

                for (int m = 0; m < bgData.Columns; m++) // Apply generated background tiles
                {
                    for (int n = 0; n < bgData.Rows; n++)
                    {
                        // Only apply if the tile's current texture matches the target tileset
                        if (bgData[m, n] != toTileset)
                            continue;

                        MTexture bgTexture = bgGenerated.TileGrid.Tiles[m + 1, n + 1];
                        bgTexes[m, n] = bgTexture;
                    }
                }
            }
        }

        if (flash)
            level.Flash(flashColor, false);

        if (alternate)
            (fromTileset, toTileset) = (toTileset, fromTileset);

        Audio.Play(sound);
    }

}