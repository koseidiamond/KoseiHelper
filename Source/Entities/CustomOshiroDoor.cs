using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

public enum OshiroCollisionMode
{
    Vanilla,
    Rebound,
    SideBounce,
    PointBounce
}

[CustomEntity("KoseiHelper/CustomOshiroDoor")]
public class CustomOshiroDoor : Solid
{
    private Sprite sprite;
    private Wiggler wiggler;
    public Color tint;
    public string flag;
    public string bumpSound;
    public bool singleUse;
    public float wiggleDuration, wiggleFrequency, wiggleScale;
    public bool refillDash;
    public bool givesCoyote;
    public bool destroyAttached;
    public OshiroCollisionMode collisionMode;

    public CustomOshiroDoor(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Width, data.Height, safe: false)
    {
        base.Depth = data.Int("depth", -9000);
        Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite", "koseiHelper_CustomOshiroDoor")));
        bumpSound = data.Attr("bumpSound", "event:/game/03_resort/forcefield_bump");
        tint = data.HexColor("color", Color.DarkSlateBlue);
        singleUse = data.Bool("singleUse", false);
        collisionMode = data.Enum("collisionMode", OshiroCollisionMode.Vanilla);
        wiggleDuration = data.Float("wiggleDuration", 1f) / (5 / 3);
        wiggleFrequency = data.Float("wiggleFrequency", 1f) * 3;
        wiggleScale = data.Float("wiggleScale", 1f) / 5;
        flag = data.Attr("flag", "oshiro_resort_talked_1");
        givesCoyote = data.Bool("givesCoyote", false);
        destroyAttached = data.Bool("destroyAttached", false);
        sprite.Position = new Vector2(base.Width, base.Height) / 2f;
        sprite.Color = tint;
        sprite.Play("idle");
        refillDash = data.Bool("refillDash", false);
        OnDashCollide = OnDashed;
        Add(wiggler = Wiggler.Create(wiggleDuration, wiggleFrequency, (float f) =>
        {
            sprite.Scale = Vector2.One * (1f - f * wiggleScale);
        }));
        SurfaceSoundIndex = 20;
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
        if (Collidable)
        {
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Audio.Play("event:/game/03_resort/forcefield_vanish", Position);
            sprite.Play("open");
            Collidable = false;
            if (destroyAttached)
                DestroyStaticMovers();
        }
    }

    public DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
        Audio.Play(bumpSound, Position);
        wiggler.Start();
        if (singleUse)
            Open();
        if (player != null)
        {
            if (refillDash)
                player.RefillDash();
            if (collisionMode == OshiroCollisionMode.SideBounce)
                player.SideBounce((int)direction.X, player.Position.X, player.Position.Y);
            if (collisionMode == OshiroCollisionMode.PointBounce)
                player.PointBounce(Center);
            if (givesCoyote)
                player.jumpGraceTimer = 0.15f;
        }
        if (collisionMode == OshiroCollisionMode.Rebound)
            return DashCollisionResults.Rebound;
        if (collisionMode == OshiroCollisionMode.Vanilla)
            return DashCollisionResults.Bounce;
        return DashCollisionResults.Ignore;
    }
}