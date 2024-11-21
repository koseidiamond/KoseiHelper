using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomWhiteBlock")]
public class CustomWhiteBlock : JumpThru
{
    private float duckDuration;
    private float playerDuckTimer;
    private bool enabled = true;
    private bool activated;
    private Image sprite;
    private Entity bgSolidTiles;
    public Color color;
    public string sound;
    public string flag;
    public CustomWhiteBlock(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Width, safe: true)
    {
        duckDuration = data.Float("duckDuration", 3f);
        Add(sprite = new Image(GFX.Game[data.Attr("spritePath", "objects/whiteblock")]));
        base.Depth = 8990;
        SurfaceSoundIndex = data.Int("surfaceSoundIndex", 27);
        color = data.HexColor("color", Color.White);
        sprite.Color = color;
        sound = data.Attr("sound", "event:/game/04_cliffside/whiteblock_fallthru");
        flag = data.Attr("whiteBlockFlag", "whiteBlockFlag");
    }
    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        if (level.Session.GetFlag(flag))
        {
            MakeTransparent();
        }
    }
    private void MakeTransparent()
    {
        Logger.Debug(nameof(KoseiHelperModule), $"The custom white block is transparent!");
        enabled = false;
        sprite.Color = color * 0.25f;
        Collidable = false;
    }
    private void MakeOpaque()
    {
        Logger.Debug(nameof(KoseiHelperModule), $"The custom white block is opaque!");
        enabled = true;
        sprite.Color = color * 1f;
        Collidable = true;
    }
    private void Activate(Player player) //Activate means that the bg tiles become solid
    {
        Logger.Debug(nameof(KoseiHelperModule), $"The custom white block has been enabled!");
        Audio.Play(sound, base.Center);
        activated = true;
        Collidable = false;
        player.Depth = 10001;
        base.Depth = -9000;
        Level level = base.Scene as Level;
        Rectangle rectangle = new Rectangle(level.Bounds.Left / 8, level.Bounds.Y / 8, level.Bounds.Width / 8, level.Bounds.Height / 8);
        Rectangle tileBounds = level.Session.MapData.TileBounds;
        bool[,] array = new bool[rectangle.Width, rectangle.Height];
        for (int i = 0; i < rectangle.Width; i++)
        {
            for (int j = 0; j < rectangle.Height; j++)
            {
                array[i, j] = level.BgData[i + rectangle.Left - tileBounds.Left, j + rectangle.Top - tileBounds.Top] != '0';
            }
        }
        bgSolidTiles = new Solid(new Vector2(level.Bounds.Left, level.Bounds.Top), 1f, 1f, safe: true);
        bgSolidTiles.Collider = new Grid(8f, 8f, array);
        base.Scene.Add(bgSolidTiles);
    }
    public override void Update()
    {
        base.Update();
        if (!enabled)
        {
            return;
        }
        Level level = SceneAs<Level>();
        Player player = base.Scene.Tracker.GetEntity<Player>();
        if (!activated)
        {
            if (HasPlayerRider() && player != null && player.Ducking)
            {
                playerDuckTimer += Engine.DeltaTime;
                if (playerDuckTimer >= duckDuration)
                    Activate(player);
            }
            else
                playerDuckTimer = 0f;
        }
        if (level.Session.GetFlag(flag))
            MakeTransparent();
        else
        {
            {
                MakeOpaque();
                player.Depth = 0;
                base.Scene.Remove(bgSolidTiles);
            }
        }
    }
}