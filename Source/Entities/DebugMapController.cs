using Celeste.Editor;
using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections.Generic;

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

    //private static List<Vector2> keys;
    //private static Camera camera;

    private static Color[] fgTilesColor = new Color[8]
    {
        Color.White,
        Calc.HexToColor("f6735e"),
        Calc.HexToColor("85f65e"),
        Calc.HexToColor("37d7e3"),
        Calc.HexToColor("376be3"),
        Calc.HexToColor("c337e3"),
        Calc.HexToColor("e33773"),
        Calc.HexToColor("") // Custom color
    };

    public DebugMapController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        roomsToAffect = data.Attr("roomsToAffect", "");
        renderKeys = !data.Bool("hideKeys", false);
        ignoreDummy = data.Bool("ignoreDummy", false);
        renderBerries = !data.Bool("hideBerries", false);
        renderSpawns = !data.Bool("hideSpawns", false);
        redBlink = data.Bool("redBlink", true);
        blockDebugMap = data.String("blockDebugMap", "");
        gridColor = data.HexColor("gridColor", new Color(0.1f, 0.1f, 0.1f));
        roomBgColor = data.HexColor("roomBgColor", new Color(0f, 0f, 0f));
        jumpthruColor = data.HexColor("jumpthruColor", Color.FromNonPremultiplied(255, 255, 0, 255)); // Yellow
        berryColor = data.HexColor("berryColor", Color.FromNonPremultiplied(255, 182, 193, 255)); // LightPink
        checkpointColor = data.HexColor("checkpointColor", Color.FromNonPremultiplied(0, 255, 0, 255)); // Lime
        spawnColor = data.HexColor("spawnColor", Color.FromNonPremultiplied(255, 0, 0, 255)); // Red
        bgTileColor = data.HexColor("bgTileColor", Color.FromNonPremultiplied(47, 79, 79, 255)); // DarkSlateGray
        fgTileColor = data.HexColor("fgTileColor", Color.FromNonPremultiplied(255, 255, 255, 255)); // White
        keyColor = data.HexColor("keyColor", Color.FromNonPremultiplied(255, 215, 0, 255)); // Gold (not done)
        levelColor = Color.LightGray; // I don't know where is this color used
    }

    public static int EditorColorIndex;
    private static Dictionary<LevelTemplate, List<CustomShape>> dict_levels = new Dictionary<LevelTemplate, List<CustomShape>>();
    public struct CustomShape
    {
        public DebugMapTile.Shape shape;
        public Rectangle rect { get; }
        public Color color { get; }
        public float thickness { get; }
        public int resolution { get; }
        public float textSize { get; }
        public string message { get; }
        public string texture { get; }
        public float scaleX { get; }
        public float scaleY { get; }
        public float rotation { get; }

        public CustomShape(DebugMapTile.Shape shape, Rectangle rect, Color color,
            float thickness, int resolution,
            float textSize, string message,
            string texture, float scaleX, float scaleY, float rotation)
        {
            this.shape = shape;
            this.rect = rect;
            this.color = color;

            this.thickness = thickness;
            this.resolution = resolution;

            this.textSize = textSize;
            this.message = message;

            this.texture = texture;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.rotation = rotation;
        }
    }

    public static void Load()
    {
        On.Celeste.Editor.MapEditor.RenderKeys += RenderKeys;
        On.Celeste.Editor.LevelTemplate.RenderContents += RenderLevels;
        On.Celeste.Editor.MapEditor.ctor += MapEditorCtor;

        dict_levels = new Dictionary<LevelTemplate, List<CustomShape>>();
        On.Celeste.Editor.LevelTemplate.ctor_LevelData += OnLevelTemplate_ctor;
        On.Celeste.Editor.LevelTemplate.RenderContents += OnLevelTemplate_RenderContents;
        //On.Celeste.Editor.LevelTemplate.ctor_LevelData += LevelTemplate_ctor;
    }

    public static void Unload()
    {
        On.Celeste.Editor.MapEditor.RenderKeys -= RenderKeys;
        On.Celeste.Editor.LevelTemplate.RenderContents -= RenderLevels;
        On.Celeste.Editor.MapEditor.ctor -= MapEditorCtor;

        On.Celeste.Editor.LevelTemplate.ctor_LevelData -= OnLevelTemplate_ctor;
        On.Celeste.Editor.LevelTemplate.RenderContents -= OnLevelTemplate_RenderContents;

        //On.Celeste.Editor.LevelTemplate.ctor_LevelData -= LevelTemplate_ctor;
    }

    private static void OnLevelTemplate_ctor(On.Celeste.Editor.LevelTemplate.orig_ctor_LevelData orig, LevelTemplate self, LevelData data)
    {
        orig(self, data);
        if (!dict_levels.ContainsKey(self))
        {
            List<EntityData> list_entities = data.Entities.FindAll(t => t.Name.StartsWith("KoseiHelper/DebugMapTile"));
            if (list_entities.Count > 0)
            {
                List<CustomShape> list_coloredRectangles = new List<CustomShape>();
                List<Rectangle> list_rectangles = [];
                foreach (EntityData entity_data in list_entities)
                {
                    int x = (int)entity_data.Position.X / 8;
                    int y = (int)entity_data.Position.Y / 8;
                    int w = entity_data.Width / 8;
                    int h = entity_data.Height / 8;
                    if (x < 0)
                    {
                        w += x;
                        x = 0;
                    }
                    if (y < 0)
                    {
                        h += y;
                        y = 0;
                    }

                    if (w > self.Width - x)
                        w = self.Width - x;
                    if (h > self.Height - y)
                        h = self.Height - y;

                    if (w > 0 && h > 0)
                    {
                        DebugMapTile debugTile = new DebugMapTile(entity_data, Vector2.Zero);
                        Rectangle rect = new Rectangle(self.X + x, self.Y + y, w, h);
                        CustomShape customShape = new CustomShape(debugTile.shape, rect, debugTile.color, debugTile.thickness, debugTile.resolution,
                            debugTile.textSize, debugTile.message, debugTile.texture, debugTile.scaleX, debugTile.scaleY, debugTile.rotation);
                        list_coloredRectangles.Add(customShape);
                    }
                }
                dict_levels[self] = list_coloredRectangles;
            }
        }
    }

    private static void OnLevelTemplate_RenderContents(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig,
            LevelTemplate self, Camera camera, List<LevelTemplate> allLevels)
    {
        orig(self, camera, allLevels);
        if (dict_levels.TryGetValue(self, out List<CustomShape> coloredRectangles))
        {
            
            foreach (CustomShape debugShape in coloredRectangles)
            {
                DebugMapTile debugTile = new DebugMapTile(new Vector2(debugShape.rect.X, debugShape.rect.Y),
                    debugShape.color, debugShape.rect.Width, debugShape.rect.Height, debugShape.shape,
                    debugShape.thickness, debugShape.resolution,
                    debugShape.textSize, debugShape.message,
                    debugShape.texture, debugShape.scaleX, debugShape.scaleY, debugShape.rotation);
                switch (debugTile.shape)
                {
                    case DebugMapTile.Shape.Circle:
                        Draw.Circle(debugShape.rect.Center.X, debugShape.rect.Center.Y, debugShape.rect.Width / 2f, debugShape.color,
                            debugShape.thickness / 10, debugShape.resolution);
                        break;
                    case DebugMapTile.Shape.Decal:
                        Image image = new Image(GFX.Game[debugShape.texture]);
                        image.Color = debugShape.color;
                        // Idk why I'm dividing by 16 but if it works it works
                        image.Position = new Vector2(debugShape.rect.X - (image.Width * debugShape.scaleX) /16,
                            debugShape.rect.Y - (image.Height *debugShape.scaleY)/16);
                        image.Scale = new Vector2(debugShape.scaleX/8, debugShape.scaleY/8);
                        // TODO this line needs fixing idk why it's not taking the correct angles
                        //image.Rotation = debugShape.rotation;
                        image.Render();
                        break;
                    // TODO (optional if we want to support Lines): Make angle and length attributes and render properly the plugin
                    case DebugMapTile.Shape.Line:
                        Logger.Debug(nameof(KoseiHelperModule), $"The debug lines are not fully implemented yet!");
                        Draw.LineAngle(new Vector2(debugShape.rect.X, debugShape.rect.Y),
                            debugShape.rect.Width, debugShape.rect.Height, debugShape.color);
                        break;
                    case DebugMapTile.Shape.Text:
                        ActiveFont.Draw(debugShape.message, new Vector2(debugShape.rect.X, debugShape.rect.Y), Vector2.Zero,
                            new Vector2(debugShape.textSize / 10, debugShape.textSize / 10), debugShape.color);
                        break;
                    default: //Tile (rectangle)
                        Draw.Rect(debugShape.rect.X, debugShape.rect.Y, debugShape.rect.Width, debugShape.rect.Height, debugShape.color);
                        break;
                }


            }
        }
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
                fgTilesColor[7] = fgTileColor; // Assigns the custom color
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
        //camera = SceneAs<Level>().Camera;
    }

    public override void Update()
    {
        base.Update();
        if (!string.IsNullOrEmpty(blockDebugMap) && KoseiHelperModule.Session.DebugMapModified)
            disallowDebugMap = SceneAs<Level>().Session.GetFlag(blockDebugMap);
    }

    /*private static void LevelTemplate_ctor(On.Celeste.Editor.LevelTemplate.orig_ctor_LevelData orig, LevelTemplate self, LevelData data)
{
    orig(self, data);
    bool controllerFound = false;
    foreach (EntityData entity in data.Entities)
    {
        if (entity.Name != "KoseiHelper/DebugMapController")
            continue;

        if (controllerFound)
            throw new InvalidOperationException($"Found more than one Debug Map Controller in room {data.Name}.");

        // "and just... do your magic" - Snip
        controllerFound = true;
    }
}*/
}