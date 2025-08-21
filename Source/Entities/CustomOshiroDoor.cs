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
    public bool dynamicFlagReact;
    public bool tintAttached;
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
        dynamicFlagReact = data.Bool("dynamicFlagReact", false);
        tintAttached = data.Bool("tintAttached", false);
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

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (tintAttached)
        {
            foreach (StaticMover staticMover in staticMovers)
            {
                if (staticMover.Entity is Spikes spikes)
                    spikes.SetSpikeColor(tint);
                if (staticMover.Entity is Spring spring)
                    spring.sprite.SetColor(tint);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (dynamicFlagReact)
        {
            if (level.Session.GetFlag(flag) == true)
                Disintegrate();
            else
            {
                if (!BlockedCheck())
                {
                    Visible = true;
                    Reintegrate();
                }
                else
                    Visible = false;
            }
        }
    }

    public void Disintegrate() // The vanilla method
    {
        if (Collidable)
        {
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Audio.Play("event:/game/03_resort/forcefield_vanish", Center);
            sprite.Play("open");
            Collidable = false;
            if (destroyAttached)
            {
                // Instead of destroying them we just disable so they can reappear later
                //DestroyStaticMovers();
                foreach (StaticMover staticMover in staticMovers)
                {
                    staticMover.Entity.Collidable = false;
                    staticMover.Entity.Visible = false;
                    staticMover.Entity.Active = false;
                    staticMover.Disable();
                }
            }
        }
    }

    public void Reintegrate() // Aka reappears
    {
        if (!Collidable && sprite.CurrentAnimationID != "reappear")
        {
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Audio.Play("event:/KoseiHelper/forcefield_reappear", Center);
            sprite.Play("reappear");
            sprite.OnLastFrame = delegate (string anim)
            {
                if (!BlockedCheck())
                {
                    if (anim == "reappear")
                    {
                        if (destroyAttached)
                        {
                            // staticMovers reappear
                            foreach (StaticMover staticMover in staticMovers)
                            {
                                staticMover.Entity.Collidable = true;
                                staticMover.Entity.Visible = true;
                                staticMover.Entity.Active = true;
                                staticMover.Enable();
                                if (tintAttached)
                                {
                                    if (staticMover.Entity is Spikes spikes)
                                        spikes.SetSpikeColor(tint);
                                    if (staticMover.Entity is Spring spring)
                                        spring.sprite.SetColor(tint);
                                }
                            }
                        }
                        Collidable = true;
                    }
                }
            };
        }
    }

    public bool BlockedCheck()
    {
        if (CollideFirst<TheoCrystal>() != null)
            return true;
        if (CollideFirst<Player>() != null)
            return true;
        return false;
    }

    public DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
        Audio.Play(bumpSound, Center);
        wiggler.Start();
        if (singleUse)
            Disintegrate();
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