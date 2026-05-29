using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/LightReactiveBlock")]
[Tracked]
public class LightReactiveBlock : Solid
{

    private char tileType;
    private float width;
    private float height;
    private bool blendIn;
    private Color tint;
    private bool cutoutEffect; // TODO see whats going on with this

    private float lightLevelForCollidable;

    private TileGrid tiles;

    public LightReactiveBlock(EntityData data, Vector2 offset, EntityID id)
        : base(data.Position + offset, data.Width, data.Height, safe: true)
    {
        Depth = data.Int("depth", -12999);
        width = data.Width;
        height = data.Height;
        blendIn = data.Bool("blendin");
        tileType = data.Char("tiletype", '3');
        tint = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("tint", out object c1) ? c1.ToString() : null, Color.White);
        lightLevelForCollidable = data.Float("lightLevelForCollidable", 0.8f);
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
        cutoutEffect = data.Bool("cutoutEffect", true);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

        if (!blendIn)
        {
            tiles = GFX.FGAutotiler.GenerateBox(tileType, (int)width / 8, (int)height / 8).TileGrid;
            Add(new LightOcclude(0f));
        }
        else
        {
            Level level = SceneAs<Level>();

            Rectangle tileBounds = level.Session.MapData.TileBounds;
            VirtualMap<char> solidsData = level.SolidsData;

            int x = (int)(X / 8f) - tileBounds.Left;
            int y = (int)(Y / 8f) - tileBounds.Top;
            int tilesX = (int)Width / 8;
            int tilesY = (int)Height / 8;

            tiles = GFX.FGAutotiler.GenerateOverlay(tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
            if (cutoutEffect)
                Add(new EffectCutout());
        }

        if (tint != Color.White)
            tiles.Color = tint;
        Add(tiles);
        Add(new TileInterceptor(tiles, highPriority: true));
        UpdateLightState();
    }

    public override void Update()
    {
        base.Update();
        UpdateLightState();
    }

    private void UpdateLightState()
    {
        Level level = SceneAs<Level>();
        // Ambient visibility (0 = dark, 1 = fully lit)
        float visibility = 1f - level.Lighting.Alpha;
        bool ambientLit = level.Lighting.Alpha <= lightLevelForCollidable;

        if (ambientLit)
        {
            if (!Collidable && !CollideCheck<Player>())
                Collidable = true;
        }
        else
            Collidable = false;
        float alpha = Calc.ClampedMap(visibility, 0f, lightLevelForCollidable, 0f, 0.75f); // tweak the final number for max transparency when lit
        tiles.Color = tint * alpha;
    }
}