using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using Celeste.Mod.Entities;
using ExtendedVariants.Variants;
using Iced.Intel;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata;

namespace Celeste.Mod.KoseiHelper.Entities;

public enum TilesetOshiroCollisionMode
{
    Vanilla,
    Rebound,
    SideBounce,
    PointBounce
}

[CustomEntity("KoseiHelper/TilesetOshiroDoor")]
public class TilesetOshiroDoor : Solid
{
    public string flag;
    public string bumpSound;
    public bool singleUse;
    public bool refillDash;
    public bool givesCoyote;
    public TilesetOshiroCollisionMode collisionMode;

    private char tileType;
    public bool giveFreezeFrames;
    public bool debris;
    public bool destroyAttached = true;

    public TilesetOshiroDoor(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Width, data.Height, safe: false)
    {
        bumpSound = data.Attr("bumpSound", "event:/game/03_resort/forcefield_bump");
        singleUse = data.Bool("singleUse", false);
        collisionMode = data.Enum("collisionMode", TilesetOshiroCollisionMode.Vanilla);
        flag = data.Attr("flag", "oshiro_resort_talked_1");
        givesCoyote = data.Bool("givesCoyote", false);
        destroyAttached = data.Bool("destroyAttached", true);
        tileType = data.Char("tiletype", 'a');
        refillDash = data.Bool("refillDash", false);
        giveFreezeFrames = data.Bool("giveFreezeFrames", false);
        debris = data.Bool("debris", false);
        OnDashCollide = OnDashed;
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        TileGrid tileGrid;
        tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int)Width / 8, (int)Height / 8).TileGrid;
        Add(new LightOcclude());
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
        if (CollideCheck<Player>())
            RemoveSelf();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        if (level.Session.GetFlag(flag) == true)
            Visible = Collidable = false;
    }

    public void Open()
    {
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (Collidable)
        {
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Audio.Play("event:/game/03_resort/forcefield_vanish", Position);
            if (debris)
            {
                for (int i = 0; (float)i < base.Width / 8f; i++)
                {
                    for (int j = 0; (float)j < base.Height / 8f; j++)
                    {
                        base.Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(4 + i * 8, 4 + j * 8), tileType).BlastFrom(Center));
                    }
                }
            }
            Collidable = false;
            if (giveFreezeFrames)
                Celeste.Freeze(0.05f);
            if (destroyAttached)
                DestroyStaticMovers();
            RemoveSelf();
        }
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
        Audio.Play(bumpSound, Position);
        if (singleUse)
            Open();
        if (player != null)
        {
            if (refillDash)
                player.RefillDash();
            if (collisionMode == TilesetOshiroCollisionMode.SideBounce)
                player.SideBounce((int)direction.X, player.Position.X, player.Position.Y);
            if (collisionMode == TilesetOshiroCollisionMode.PointBounce)
                player.PointBounce(Center);
            if (givesCoyote)
                player.jumpGraceTimer = 0.15f;
        }
        if (collisionMode == TilesetOshiroCollisionMode.Rebound)
            return DashCollisionResults.Rebound;
        if (collisionMode == TilesetOshiroCollisionMode.Vanilla)
            return DashCollisionResults.Bounce;
        return DashCollisionResults.Ignore;
    }
}