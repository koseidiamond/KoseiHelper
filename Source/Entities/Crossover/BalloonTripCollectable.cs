using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/BalloonTripCollectable")]
[Tracked]
public class BalloonTripCollectable : Entity
{

    public string counterName;
    public int pointsGiven;
    public string sound;
    public bool outline;

    private Sprite sprite;
    private EntityID id;
    private bool canReappear = true;
    private bool collectionEffects = false;
    public static ParticleType balloonParticle = Player.P_Split;
    public bool multiplicative;
    public BalloonTripCollectable(EntityData data, Vector2 offset, EntityID id)
    {
        Depth = data.Int("depth", -100);
        Position = data.Position + offset;
        this.id = id;
        counterName = data.Attr("counterName", "koseiHelper_balloonPoints");
        pointsGiven = data.Int("pointsGiven", 100);
        outline = data.Bool("outline", false);
        sound = data.Attr("sound", "event:/KoseiHelper/Crossover/BalloonExplode");
        canReappear = data.Bool("canReappear", true);
        collectionEffects = data.Bool("collectionEffects", false);
        multiplicative = data.Bool("multiplicative", false);
        Add(sprite = GFX.SpriteBank.Create(data.Attr("spriteID", "koseiHelper_balloonTripCollectable")));
        sprite.CenterOrigin();
        balloonParticle.Color = balloonParticle.Color2 = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("particleColor", out object c1) ? c1.ToString() : null,
            Color.FromNonPremultiplied(231, 0, 91, 255));
        sprite.Play("balloon");
        base.Collider = new Hitbox(data.Float("hitboxWidth",12f), data.Float("hitboxHeight",12f), data.Float("hitboxXOffset",-6f), data.Float("hitboxYOffset",-8f));
        Add(new PlayerCollider(OnPlayer));
    }

    public void OnPlayer(Player player)
    {
        Audio.Play(sound);
        if (multiplicative)
            player.level.Session.SetCounter(counterName, player.level.Session.GetCounter(counterName) * pointsGiven);
        else
            player.level.Session.SetCounter(counterName, player.level.Session.GetCounter(counterName) + pointsGiven);
        if (collectionEffects)
        {
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Celeste.Freeze(0.05f);
            SceneAs<Level>().ParticlesBG.Emit(balloonParticle, 4, Center, Vector2.One * 4f);
        }
        RemoveSelf();
        if (!canReappear)
            SceneAs<Level>().Session.DoNotLoad.Add(id);
    }

    public override void Render()
    {
        if (outline)
            sprite.DrawOutline();
        base.Render();
    }
}