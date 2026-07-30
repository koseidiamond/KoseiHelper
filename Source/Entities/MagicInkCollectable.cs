using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/MagicInkCollectable")]
[Tracked]
public class MagicInkCollectable : Entity
{
    private EntityID id;
    private bool canReappear = true;
    private Sprite sprite;
    private string sound;
    public static readonly ParticleType inkParticle = new(Player.P_Split);
    public bool canOverfill;
    public float inkGiven;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private SineWave sine;
    private float respawnTime = 3f;
    private bool collected;
    public MagicInkCollectable(EntityData data, Vector2 offset, EntityID id)
    {
        Depth = data.Int("depth", -100);
        inkGiven = data.Float("inkGiven", 60f);
        canOverfill = data.Bool("canOverfill", true);
        Position = data.Position + offset;
        this.id = id;
        sound = data.Attr("sound", "event:/game/general/diamond_touch");
        canReappear = data.Bool("canReappear", true);
        Add(sprite = GFX.SpriteBank.Create(data.Attr("spriteID", "koseiHelper_inkCollectable")));
        sprite.CenterOrigin();
        sprite.Play("ink");
        base.Collider = new Hitbox(data.Float("hitboxWidth", 12f), data.Float("hitboxHeight", 12f), data.Float("hitboxXOffset", -6f), data.Float("hitboxYOffset", -6f));
        base.Add(this.wiggler = Wiggler.Create(1f, 4f, delegate (float v) { this.sprite.Scale = Vector2.One * (1f + v * 0.2f); }, false, false));
        base.Add(new MirrorReflection());
        base.Add(this.bloom = new BloomPoint(0.3f, 16f));
        base.Add(this.sine = new SineWave(0.6f, 0f));
        Add(new PlayerCollider(OnPlayer));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        MagicInkController controller = Scene.Tracker.GetEntity<MagicInkController>();
        if (controller == null)
            RemoveSelf();
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Color rainbow = Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f);
        inkParticle.Color = inkParticle.Color2 = rainbow;
        sprite.Y = (bloom.Y = sine.Value * 1f);
    }

    public void OnPlayer(Player player)
    {
        MagicInkController controller = Scene.Tracker.GetEntity<MagicInkController>();
        if (controller == null)
            return;

        if ((canOverfill && controller.currentInk > controller.maxInk) || (!canOverfill && controller.currentInk >= controller.maxInk))
            return;

        controller.AddInk(inkGiven, canOverfill);
        //controller.investedInk -= inkGiven;

        wiggler.Start();
        Audio.Play(sound, Center);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        Celeste.Freeze(0.05f);
        SceneAs<Level>().ParticlesBG.Emit(inkParticle, 6, Center, Vector2.One * 4f);

        if (canReappear)
        {
            collected = true;
            sprite.Play("outline");
            Collidable = false;

            Add(new Coroutine(RespawnRoutine()));
        }
        else
        {
            RemoveSelf();
        }
    }

    private IEnumerator RespawnRoutine()
    {
        yield return 3f;

        collected = false;
        sprite.Play("ink");
        Collidable = true;

        Audio.Play("event:/game/general/diamond_return", Center);
        wiggler.Start();
    }

    /*public override void Render()
    {
        sprite.DrawOutline();
        base.Render();
    }*/
}