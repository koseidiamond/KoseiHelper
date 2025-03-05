using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste.Editor;
using Monocle;
using System.Collections.Generic;
using Celeste.Mod.Helpers;
using MonoMod.Utils;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DebugMapController")]
[Tracked]
public class DebugMapController : Entity
{
    private static bool renderBerries, renderSpawns;
    private static bool renderKeys = true;
    private static bool redBlink, ignoreDummy;
    private static Color gridColor, jumpthruColor, berryColor, checkpointColor, spawnColor, bgTileColor, fgTileColor, levelColor, keyColor, roomBgColor;
    private static bool disallowDebugMap;
    private static string blockDebugMap;
    private static string roomsToAffect;

    private static List<Vector2> keys;
    private static Camera camera;

    private static Color[] fgTilesColor = new Color[8] // TODO make these customizable too
    {
        Color.White,
        Calc.HexToColor("f6735e"),
        Calc.HexToColor("85f65e"),
        Calc.HexToColor("37d7e3"),
        Calc.HexToColor("376be3"),
        Calc.HexToColor("c337e3"),
        Calc.HexToColor("e33773"),
        Calc.HexToColor("")
    };

public DebugMapController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        roomsToAffect = data.Attr("roomsToAffect", "");
        renderKeys = !data.Bool("hideKeys", false);
        ignoreDummy = data.Bool("ignoreDummy", false);
        renderBerries = !data.Bool("hideBerries", false);
        renderSpawns = !data.Bool("hideSpawns", false);
        redBlink = data.Bool("redBlink", true);
        blockDebugMap = data.String("blockDebugMap","");
        gridColor = data.HexColor("gridColor", new Color(0.1f, 0.1f, 0.1f));
        roomBgColor = data.HexColor("roomBgColor", new Color(0f, 0f, 0f));
        jumpthruColor = data.HexColor("jumpthruColor", Color.FromNonPremultiplied(255,255,0,255)); // Yellow
        berryColor = data.HexColor("berryColor", Color.FromNonPremultiplied(255, 182, 193, 255)); // LightPink
        checkpointColor = data.HexColor("checkpointColor", Color.FromNonPremultiplied(0, 255, 0, 255)); // Lime
        spawnColor = data.HexColor("spawnColor", Color.FromNonPremultiplied(255, 0, 0, 255)); // Red
        bgTileColor = data.HexColor("bgTileColor", Color.FromNonPremultiplied(47, 79, 79, 255)); // DarkSlateGray
        fgTileColor = data.HexColor("fgTileColor", Color.FromNonPremultiplied(255, 255, 255, 255)); // White
        keyColor = data.HexColor("keyColor", Color.FromNonPremultiplied(255, 215, 0, 255)); // Gold (not done)
        levelColor = Color.LightGray; // I don't know where is this color used
    }

    public static int EditorColorIndex;

    public static void Load()
    {
        On.Celeste.Editor.MapEditor.RenderKeys += RenderKeys;
        On.Celeste.Editor.LevelTemplate.RenderContents += RenderLevels;
        On.Celeste.Editor.MapEditor.ctor += MapEditorCtor;
    }

    public static void Unload()
    {
        On.Celeste.Editor.MapEditor.RenderKeys -= RenderKeys;
        On.Celeste.Editor.LevelTemplate.RenderContents -= RenderLevels;
        On.Celeste.Editor.MapEditor.ctor -= MapEditorCtor;
    }

    private static void RenderKeys(On.Celeste.Editor.MapEditor.orig_RenderKeys orig, MapEditor self)
    {
        if (renderKeys && !disallowDebugMap && !KoseiHelperModule.Session.DebugMapModified)
        {
            orig(self); // Ideally they should have custom colors but they're annoying to work with
        }
    }

    private static void RenderLevels(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig, LevelTemplate self, Camera camera, List<LevelTemplate> allLevels)
    {
        if (KoseiHelperModule.Session.DebugMapModified && self.Name.StartsWith(roomsToAffect))
        {
            if (!CullHelper.IsRectangleVisible(self.X, self.Y, self.Width, self.Height, 4f, camera))
            {
                return;
            }
            if (self.Type == LevelTemplateType.Level)
            {
                bool flag = false;
                if (Engine.Scene.BetweenInterval(0.1f) && redBlink) // Red blinking if levels are intersecting
                {
                    foreach (LevelTemplate allLevel in allLevels)
                    {
                        if (allLevel != self && allLevel.Rect.Intersects(self.Rect))
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                Draw.Rect(self.X, self.Y, self.Width, self.Height, (flag ? Color.Red : roomBgColor) * 0.5f);
                // Actually render the content of the rooms
                fgTilesColor[7] = fgTileColor;
                foreach (Rectangle back in self.backs)
                {
                    Draw.Rect(self.X + back.X, self.Y + back.Y, back.Width, back.Height, ignoreDummy ? bgTileColor * 0.5f :
                        self.Dummy ? bgTileColor * 0.5f : bgTileColor * 0.5f);
                }
                foreach (Rectangle solid in self.solids)
                {
                    Draw.Rect(self.X + solid.X, self.Y + solid.Y, solid.Width, solid.Height, ignoreDummy ? fgTilesColor[7] :
                        self.Dummy ? Color.LightGray : fgTilesColor[7]);
                }
                if (renderSpawns)
                {
                    foreach (Vector2 spawn in self.Spawns)
                    {
                        Draw.Rect((float)self.X + spawn.X, (float)self.Y + spawn.Y - 1f, 1f, 1f, spawnColor);
                    }
                }
                if (renderBerries)
                {
                    foreach (Vector2 strawberry in self.Strawberries)
                    {
                        Draw.HollowRect((float)self.X + strawberry.X - 1f, (float)self.Y + strawberry.Y - 2f, 3f, 3f, berryColor);
                    }
                }
                foreach (Vector2 checkpoint in self.Checkpoints)
                {
                    Draw.HollowRect((float)self.X + checkpoint.X - 1f, (float)self.Y + checkpoint.Y - 2f, 3f, 3f, checkpointColor);
                }
                foreach (Rectangle jumpthru in self.Jumpthrus)
                {
                    Draw.Rect(self.X + jumpthru.X, self.Y + jumpthru.Y, jumpthru.Width, 1f, jumpthruColor);
                }
                return;
            }
            Draw.Rect(self.X, self.Y, self.Width, self.Height, levelColor);
            Draw.Rect((float)(self.X + self.Width) - self.resizeHoldSize.X, (float)(self.Y + self.Height) - self.resizeHoldSize.Y, self.resizeHoldSize.X, self.resizeHoldSize.Y, Color.Orange);
        }
        else
            orig(self, camera, allLevels);
    }

    private static void MapEditorCtor(On.Celeste.Editor.MapEditor.orig_ctor orig, MapEditor self, AreaKey area, bool reloadMapData)
    {
            orig(self, area, reloadMapData);
            DynData<MapEditor> mapEditorData = new(self);
            if (disallowDebugMap && KoseiHelperModule.Session.DebugMapModified)
            {
                List<LevelTemplate> levels = mapEditorData.Get<List<LevelTemplate>>("levels");
                MapData mapdata = mapEditorData.Get<MapData>("mapData");
                List<LevelTemplate> levelsToHide = new();
                foreach (LevelTemplate template in levels)
                {
                    foreach (LevelData levelData in mapdata.Levels)
                    {
                        levelsToHide.Add(template);
                    }
                }
                foreach (LevelTemplate template in levelsToHide)
                {
                    levels.Remove(template);
                }
            }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        KoseiHelperModule.Session.DebugMapModified = true;
        MapEditor.gridColor = gridColor;
        camera = SceneAs<Level>().Camera;
    }

    public override void Update()
    {
        base.Update();
        if (!string.IsNullOrEmpty(blockDebugMap) && KoseiHelperModule.Session.DebugMapModified)
            disallowDebugMap = SceneAs<Level>().Session.GetFlag(blockDebugMap);
    }
}