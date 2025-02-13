using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste.Editor;
using Monocle;
using System.Collections.Generic;
using Celeste.Mod.Helpers;
using System;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DebugMapController")]
[Tracked]
public class DebugMapController : Entity
{
    private static bool renderKeys, renderBerries, renderSpawns;
    private static bool redBlink;
    private static Color gridColor, jumpthruColor, berryColor, checkpointColor, spawnColor, bgTileColor, levelColor;
    private static bool debugMapModified;

    private static Color[] fgTilesColor = new Color[7] // TODO make these customizable too
    {
        Color.White,
        Calc.HexToColor("f6735e"),
        Calc.HexToColor("85f65e"),
        Calc.HexToColor("37d7e3"),
        Calc.HexToColor("376be3"),
        Calc.HexToColor("c337e3"),
        Calc.HexToColor("e33773")
    };

    public DebugMapController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        renderKeys = !data.Bool("hideKeys", false);
        renderBerries = !data.Bool("hideBerries", false);
        renderSpawns = !data.Bool("hideSpawns", false);
        redBlink = data.Bool("redBlink", true);
        gridColor = data.HexColor("gridColor", new Color(0.1f, 0.1f, 0.1f));
        jumpthruColor = data.HexColor("jumpthruColor", Color.FromNonPremultiplied(255,255,0,255)); // Yellow
        berryColor = data.HexColor("berryColor", Color.FromNonPremultiplied(255, 182, 193, 255)); // LightPink
        checkpointColor = data.HexColor("checkpointColor", Color.FromNonPremultiplied(0, 255, 0, 255)); // Lime
        spawnColor = data.HexColor("spawnColor", Color.FromNonPremultiplied(255, 0, 0, 255)); // Red
        bgTileColor = data.HexColor("bgTileColor", Color.FromNonPremultiplied(47, 79, 79, 255)); // DarkSlateGray
        levelColor = Color.LightGray;

    }

    public static int EditorColorIndex;

    public static void Load()
    {
        On.Celeste.Editor.MapEditor.RenderKeys += RenderKeys;
        On.Celeste.Editor.LevelTemplate.RenderContents += RenderLevels;
    }

    public static void Unload()
    {
        On.Celeste.Editor.MapEditor.RenderKeys -= RenderKeys;
        On.Celeste.Editor.LevelTemplate.RenderContents -= RenderLevels;
    }

    private static void RenderKeys(On.Celeste.Editor.MapEditor.orig_RenderKeys orig, MapEditor self)
    {
        if (renderKeys)
        {
            orig(self);
        }
    }

    private static void RenderLevels(On.Celeste.Editor.LevelTemplate.orig_RenderContents orig, LevelTemplate self, Camera camera, List<LevelTemplate> allLevels)
    {
        if (debugMapModified)
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
                Draw.Rect(self.X, self.Y, self.Width, self.Height, (flag ? Color.Red : Color.Black) * 0.5f);
                foreach (Rectangle back in self.backs)
                {
                    Draw.Rect(self.X + back.X, self.Y + back.Y, back.Width, back.Height, self.Dummy ? bgTileColor * 0.5f : bgTileColor * 0.5f); // todo
                }
                foreach (Rectangle solid in self.solids)
                {
                    Draw.Rect(self.X + solid.X, self.Y + solid.Y, solid.Width, solid.Height, self.Dummy ? Color.LightGray : fgTilesColor[EditorColorIndex]);
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
                {
                    foreach (Rectangle jumpthru in self.Jumpthrus)
                    {
                        Draw.Rect(self.X + jumpthru.X, self.Y + jumpthru.Y, jumpthru.Width, 1f, jumpthruColor);
                    }
                    return;
                }
            }
            Draw.Rect(self.X, self.Y, self.Width, self.Height, levelColor);
            Draw.Rect((float)(self.X + self.Width) - self.resizeHoldSize.X, (float)(self.Y + self.Height) - self.resizeHoldSize.Y, self.resizeHoldSize.X, self.resizeHoldSize.Y, Color.Orange);
        }
        else
            orig(self, camera, allLevels);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        debugMapModified = true;
        DebugMap();
    }

    public void DebugMap()
    {
        MapEditor.gridColor = gridColor;
    }
}