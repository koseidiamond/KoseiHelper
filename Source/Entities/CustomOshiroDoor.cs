using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using Celeste.Mod.Entities;
using ExtendedVariants.Variants;

namespace Celeste.Mod.KoseiHelper.Entities;

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
    public bool rebound, refillDash;
    public bool givesCoyote;

    public CustomOshiroDoor(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Width, data.Height, safe: false)
    {
        Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite", "koseiHelper_CustomOshiroDoor")));
        bumpSound = data.Attr("bumpSound", "event:/game/03_resort/forcefield_bump");
        tint = data.HexColor("color", Color.DarkSlateBlue);
        singleUse = data.Bool("singleUse", false);
        rebound = data.Bool("rebound", false);
        wiggleDuration = data.Float("wiggleDuration", 1f) / (5/3);
        wiggleFrequency = data.Float("wiggleFrequency", 1f) * 3;
        wiggleScale = data.Float("wiggleScale", 1f) / 5;
        flag = data.Attr("flag", "oshiro_resort_talked_1");
        givesCoyote = data.Bool("givesCoyote", false);
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
        }
    }

    public void InstantOpen()
    {
        Collidable = (Visible = false);
    }

    private DashCollisionResults OnDashed(Player player, Vector2 direction)
    {
        Audio.Play(bumpSound, Position);
        wiggler.Start();
        if (singleUse)
            Open();
        if (player != null)
        {
            if (refillDash)
                player.RefillDash();
            if (givesCoyote)
                player.jumpGraceTimer = 0.15f;
        }
        if (rebound)
            return DashCollisionResults.Rebound;
        else
            return DashCollisionResults.Bounce;
    }
}