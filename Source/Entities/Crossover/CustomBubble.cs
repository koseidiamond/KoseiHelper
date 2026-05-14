using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/CustomBubble")]
[Tracked]
public class CustomBubble : Entity
{
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private readonly Wiggler wiggler, moveWiggle;
    private readonly Wiggler shieldRadiusWiggle;
    private readonly SineWave sine;
    private readonly Sprite sprite;
    private Vector2 moveWiggleDir;

    public float speedMult;
    public string customSound;
    public float wiggliness, floatiness;
    public bool renderSprite, renderBubble;
    public string spriteID;
    public bool singleUse;
    public Color color;
    public float respawnCooldown, respawnTimer;
    private static ParticleType FeatherCollect = FlyFeather.P_Collect;
    private static ParticleType FeatherRespawn = FlyFeather.P_Respawn;
    private enum BubbleBreakBehavior
    {
        DontBreak,
        AlwaysBreak,
        BreakWithDash
    };
    private BubbleBreakBehavior breakBehavior;
    public bool freezeFrames;
    public bool refillDash, refillStamina, releaseFromBooster;
    public float radius;
    public bool coyote;

    public CustomBubble(Vector2 position) : base(position)
    {
    }

    public CustomBubble(EntityData data, Vector2 offset) : this(data.Position + offset)
    {
        radius = (float) data.Int("radius", 10);
        Collider = new Circle(radius);
        Add(new PlayerCollider(OnPlayer));
        color = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("color", out object c1) ? c1.ToString() : null, Color.White);
        spriteID = data.Attr("spriteID", "flyFeather");
        Add(sprite = GFX.SpriteBank.Create(spriteID));
        sprite.Color = color;
        Add(Wiggler.Create(1f, 4f, delegate (float v) {
            sprite.Scale = Vector2.One * (float)(1.0 + (double)v * 0.2);
        }));
        Add(bloom = new BloomPoint(0.5f, 20f));
        Add(light = new VertexLight(Color.White, 1f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f).Randomize());
        Add(wiggler = Wiggler.Create(1f, 4f, (float v) =>
        {
            sprite.Scale = Vector2.One * (1f + v * 0.2f);
        }));
        shieldRadiusWiggle = Wiggler.Create(0.5f, 4f);
        Add(shieldRadiusWiggle);
        moveWiggle = Wiggler.Create(0.8f, 2f);
        moveWiggle.StartZero = true;
        Add(moveWiggle);
        UpdateY();
        speedMult = data.Float("speedMult", 1.2f);
        customSound = data.Attr("sound", "event:/game/06_reflection/feather_bubble_bounce");
        wiggliness = data.Float("wiggliness", 8f);
        floatiness = data.Float("floatiness", 2f);
        renderSprite = data.Bool("renderSprite", false);
        renderBubble = data.Bool("renderBubble", true);
        breakBehavior = data.Enum("breakBehavior", BubbleBreakBehavior.DontBreak);
        singleUse = data.Bool("singleUse", false);
        freezeFrames = data.Bool("freezeFrames", false);
        respawnCooldown = data.Float("respawnCooldown", 3f);
        refillDash = data.Bool("refillDash", true);
        refillStamina = data.Bool("refillStamina", true);
        releaseFromBooster = data.Bool("releaseFromBooster", true);
        coyote = data.Bool("coyote", false);
        if (!renderSprite)
            sprite.Visible = false;
        FeatherCollect.Color = color;
        FeatherCollect.Color2 = color;
        FeatherRespawn.Color = color;
        FeatherRespawn.Color2 = color;
    }

    public override void Update()
    {
        base.Update();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
            {
                Respawn();
            }
        }
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
    }

    public override void Render()
    {
        base.Render();
        if (!renderBubble || !Visible)
            return;
        // auto-adjusts resolution for performance while avoiding BIG bubbles to look bad (done by eye)
        Draw.Circle(Position + sprite.Position, Math.Max(0f, (float)(radius - shieldRadiusWiggle.Value * 2.0)), color, radius < 18 ? 3 : 6);
    }

    private void UpdateY()
    {
        sprite.X = 0f;
        Sprite obj = sprite;
        float y = bloom.Y = sine.Value * floatiness;
        obj.Y = y;
        sprite.Position += moveWiggleDir * moveWiggle.Value * -wiggliness;
    }

    private void OnPlayer(Player player)
    {
        KoseiHelperUtils.PointBounce(Center, player, refillDash, refillStamina, releaseFromBooster, coyote);
        if (Input.MoveX.Value == Math.Sign(player.Speed.X))
            player.Speed.X *= speedMult;
        moveWiggle.Start();
        shieldRadiusWiggle.Start();
        moveWiggleDir = (Center - player.Center).SafeNormalize(Vector2.UnitY);
        Audio.Play(customSound, Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        SceneAs<Level>().DirectionalShake((player.Center - Center).SafeNormalize(), 0.15f);
        if (breakBehavior == BubbleBreakBehavior.AlwaysBreak || (breakBehavior == BubbleBreakBehavior.BreakWithDash && player.DashAttacking))
        {
            if (freezeFrames)
                Celeste.Freeze(0.05f);
            Collidable = false;
            Add(new Coroutine(CollectRoutine(player, player.Speed, player.level)));
            if (!singleUse)
            {
                Visible = true;
                respawnTimer = respawnCooldown;
            }
        }
    }

    public IEnumerator CollectRoutine(Player player, Vector2 playerSpeed, Level level)
    {
        Visible = false;
        yield return 0.05f;
        float direction = !(playerSpeed != Vector2.Zero) ? (Position - player.Center).Angle() : playerSpeed.Angle();
        level.ParticlesFG.Emit(FeatherCollect, 10, Center, Vector2.One * 6f);
        SlashFx.Burst(Position, direction);
        if (singleUse)
            RemoveSelf();
    }

    public void Respawn()
    {
        if (!Collidable)
        {
            Collidable = true;
            Visible = true;
            wiggler.Start();
            Audio.Play("event:/game/06_reflection/feather_reappear", Position);
            SceneAs<Level>().ParticlesFG.Emit(FeatherRespawn, 16, Center, Vector2.One * 2f);
        }
    }
}