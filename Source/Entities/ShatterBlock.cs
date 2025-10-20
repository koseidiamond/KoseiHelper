using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/ShatterDashBlock")]
[Tracked]
public class ShatterDashBlock : Solid
{

    private bool permanent;
    private EntityID id;
    private char tileType;
    private float width;
    private float height;
    private bool blendIn;
    private float speedReq;
    private float delay;

    private float speedDecrease;
    private float shakeTime;
    private bool canDash = true;
    private bool givesCoyote = false;
    private bool requireOnlySpeed = false;
    private float previousHSpeed, previousVSpeed;
    private string flagSet;
    private bool destroyStaticMovers;
    private bool verticalSpeed;
    private Color tint;
    private enum SubstractSpeedMode
    {
        Substract,
        Multiply,
        Set
    };
    private SubstractSpeedMode speedAfterShatterMode;
    private enum ShatterBlockDirection
    {
        Horizontal,
        Vertical,
        Both
    };
    private ShatterBlockDirection shatterAxis;
    public ShatterDashBlock(EntityData data, Vector2 offset, EntityID id) : base(data.Position + offset, data.Width, data.Height, true)
    {
        base.Depth = data.Int("depth", -12999);
        this.id = id;
        permanent = data.Bool("permanent");
        width = data.Width;
        height = data.Height;
        blendIn = data.Bool("blendin");
        tileType = data.Char("tiletype", '3');
        canDash = data.Bool("canDash", true);
        verticalSpeed = data.Bool("verticalSpeed", false); // for legacy
        shatterAxis = verticalSpeed ? ShatterBlockDirection.Vertical : data.Enum("shatterAxis", ShatterBlockDirection.Horizontal); // backwards compatible
        givesCoyote = data.Bool("givesCoyote", false);
        requireOnlySpeed = data.Bool("requireOnlySpeed", false);
        delay = MathHelper.Clamp(data.Float("FreezeTime", 0.1f), 0, 0.5f);
        speedReq = Math.Max(0f, data.Float("SpeedRequirement", 0f));
        speedDecrease = Math.Max(0f, data.Float("SpeedDecrease", 0f));
        shakeTime = Math.Max(0f, data.Float("ShakeTime", 0.3f));
        speedAfterShatterMode = data.Enum("substractSpeedMode", SubstractSpeedMode.Substract);
        flagSet = data.Attr("flagSet", "");
        destroyStaticMovers = data.Bool("destroyStaticMovers", false);
        tint = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("tint", out object c1) ? c1.ToString() : null, Color.White);
        OnDashCollide = OnDashed;
        SurfaceSoundIndex = SurfaceIndex.TileToIndex[tileType];
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (permanent && KoseiHelperModule.Session.SoftDoNotLoad.Contains(id))
        {
            DestroyStaticMovers();
            RemoveSelf();
        }
        TileGrid tileGrid;
        if (!blendIn)
        {
            tileGrid = GFX.FGAutotiler.GenerateBox(tileType, (int)width / 8, (int)height / 8).TileGrid;
            Add(new LightOcclude());
        }
        else
        {
            Level level = SceneAs<Level>();
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            VirtualMap<char> solidsData = level.SolidsData;
            int x = (int)(base.X / 8f) - tileBounds.Left;
            int y = (int)(base.Y / 8f) - tileBounds.Top;
            int tilesX = (int)base.Width / 8;
            int tilesY = (int)base.Height / 8;
            tileGrid = GFX.FGAutotiler.GenerateOverlay(tileType, x, y, tilesX, tilesY, solidsData).TileGrid;
            if (tint != Color.White)
                tileGrid.Color = tint;
            Add(new EffectCutout());
            base.Depth = -10501;
        }
        if (tint != Color.White)
            tileGrid.Color = tint;
        Add(tileGrid);
        Add(new TileInterceptor(tileGrid, highPriority: true));
        if (CollideCheck<Player>())
        {
            RemoveSelf();
        }
    }

    public void Break(Player player, Vector2 direction, bool playSound = true, bool playDebrisSound = true)
    {
        if (playSound)
        {
            if (tileType == '1')
            {
                Audio.Play(SFX.game_gen_wallbreak_dirt, Position);
            }
            else if (tileType == '3')
            {
                Audio.Play(SFX.game_gen_wallbreak_ice, Position);
            }
            else if (tileType == '9')
            {
                Audio.Play(SFX.game_gen_wallbreak_wood, Position);
            }
            else
            {
                Audio.Play(SFX.game_gen_wallbreak_stone, Position);
            }
        }
        for (int i = 0; (float)i < base.Width / 8f; i++)
        {
            for (int j = 0; (float)j < base.Height / 8f; j++)
            {
                if (tint != Color.White)
                {
                    CustomDebris debris = Engine.Pooler.Create<CustomDebris>().Init(Position + new Vector2(4 + i * 8, 4 + j * 8), tileType, playDebrisSound).BlastFrom(player.Center).SetTint(tint);
                    base.Scene.Add(debris);
                }
                else
                    base.Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(4 + i * 8, 4 + j * 8), tileType, playDebrisSound).BlastFrom(player.Center));
            }
        }

        Celeste.Freeze(delay);
        if (destroyStaticMovers)
            DestroyStaticMovers();
        Collidable = false;
        SceneAs<Level>().DirectionalShake(direction * -1, shakeTime);
        switch (speedAfterShatterMode)
        {
            case SubstractSpeedMode.Set:
                //player.Speed = Vector2.UnitX.RotateTowards(direction.Angle(), 6.3f) * speedDecrease;
                if (shatterAxis != ShatterBlockDirection.Horizontal)
                    player.Speed = new Vector2(player.Speed.X, Vector2.UnitY.RotateTowards(direction.Angle(), 6.3f).Y * speedDecrease);
                if (shatterAxis != ShatterBlockDirection.Vertical)
                    player.Speed = new Vector2(Vector2.UnitX.RotateTowards(direction.Angle(), 6.3f).X * speedDecrease, player.Speed.Y);
                break;
            case SubstractSpeedMode.Multiply:
                //player.Speed *= new Vector2(Math.Abs(Vector2.UnitX.RotateTowards(direction.Angle(), 6.3f).X), Math.Abs(Vector2.UnitX.RotateTowards(direction.Angle(), 6.3f).Y)) * speedDecrease;
                if (shatterAxis != ShatterBlockDirection.Horizontal)
                    player.Speed = new Vector2(player.Speed.X, player.Speed.Y * Math.Abs(Vector2.UnitY.RotateTowards(direction.Angle(), 6.3f).Y) * speedDecrease);
                if (shatterAxis != ShatterBlockDirection.Vertical)
                    player.Speed = new Vector2(player.Speed.X * Math.Abs(Vector2.UnitX.RotateTowards(direction.Angle(), 6.3f).X) * speedDecrease, player.Speed.Y);
                break;
            default: // Decrease
                if (shatterAxis != ShatterBlockDirection.Horizontal)
                    player.Speed -= Vector2.UnitY.RotateTowards(direction.Angle(), 6.3f) * speedDecrease;
                if (shatterAxis != ShatterBlockDirection.Vertical)
                    player.Speed -= Vector2.UnitX.RotateTowards(direction.Angle(), 6.3f) * speedDecrease;
                break;
        }

        if (givesCoyote)
            player.jumpGraceTimer = 0.084f;
        if (!string.IsNullOrEmpty(flagSet))
            SceneAs<Level>().Session.SetFlag(flagSet, true);

        if (permanent)
        {
            RemoveAndFlagAsGone();
        }
        else
        {
            RemoveSelf();
        }
    }

    public void RemoveAndFlagAsGone()
    {
        RemoveSelf();
        KoseiHelperModule.Session.SoftDoNotLoad.Add(id);
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
        if ((shatterAxis != ShatterBlockDirection.Horizontal && Math.Abs(player.Speed.Y) > speedReq) ||
            (shatterAxis != ShatterBlockDirection.Vertical && Math.Abs(player.Speed.X) > speedReq) &&
            (canDash || (!canDash && player.StateMachine.state != 2)))
        {
            Break(player, direction, true);
            return DashCollisionResults.Ignore;
        }
        else
        {
            return DashCollisionResults.NormalCollision;
        }
    }

    public override void Update()
    {
        Player player = Scene.Tracker.GetEntity<Player>();
        base.Update();
        if (player != null)
        {
            if (PlayerIsTouching() != null && requireOnlySpeed)
                Break(player, player.Center, true);
            previousHSpeed = player.Speed.X;
            previousVSpeed = player.Speed.Y;
        }
    }

    public Player PlayerIsTouching()
    {
        foreach (Player entity in Scene.Tracker.GetEntities<Player>())
        {
            if (shatterAxis != ShatterBlockDirection.Horizontal)
            {
                if (Math.Abs(previousVSpeed) > speedReq && CollideFirst<Player>(Position + Vector2.UnitX * 2f) != null)
                    return entity;
                if (Math.Abs(previousVSpeed) > speedReq && CollideFirst<Player>(Position - Vector2.UnitX * 2f) != null)
                    return entity;
                if (Math.Abs(previousVSpeed) > speedReq && previousVSpeed > 0 && CollideFirst<Player>(Position - Vector2.UnitY) != null)
                    return entity;
                if (Math.Abs(previousVSpeed) > speedReq && previousVSpeed < 0 && CollideFirst<Player>(Position + Vector2.UnitY) != null)
                    return entity;
            }
            if (shatterAxis != ShatterBlockDirection.Vertical)
            {
                if (Math.Abs(previousHSpeed) > speedReq && CollideFirst<Player>(Position + Vector2.UnitX * 2f) != null)
                    return entity;
                if (Math.Abs(previousHSpeed) > speedReq && CollideFirst<Player>(Position - Vector2.UnitX * 2f) != null)
                    return entity;
                if (Math.Abs(previousHSpeed) > speedReq && previousVSpeed > 0 && CollideFirst<Player>(Position - Vector2.UnitY) != null)
                    return entity;
                if (Math.Abs(previousHSpeed) > speedReq && previousVSpeed < 0 && CollideFirst<Player>(Position + Vector2.UnitY) != null)
                    return entity;
            }
        }
        return null;
    }

    public void Break(Player player, Vector2 direction, bool playSound = true)
    {
        Break(player, direction, playSound, playDebrisSound: true);
    }
}