using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomBubble")]
[Tracked]
public class CustomBubble : Entity
{
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private readonly Wiggler moveWiggle;
    private readonly Wiggler shieldRadiusWiggle;
    private readonly SineWave sine;
    private readonly Sprite sprite;
    private Vector2 moveWiggleDir;

    public float speedMult;
    public string customSound;
    public float wiggliness, floatiness;
    public bool renderSprite, renderBubble;
    public string spriteID;
    public CustomBubble(Vector2 position) : base(position)
    {
    }

    public CustomBubble(EntityData data, Vector2 offset) : this(data.Position + offset)
    {
        Collider = new Circle(10f);
        Add(new PlayerCollider(OnPlayer));
        spriteID = data.Attr("spriteID", "flyFeather");
        Add(sprite = GFX.SpriteBank.Create(spriteID));
        Add(Wiggler.Create(1f, 4f, delegate (float v) {
            sprite.Scale = Vector2.One * (float)(1.0 + (double)v * 0.2);
        }));
        Add(bloom = new BloomPoint(0.5f, 20f));
        Add(light = new VertexLight(Color.White, 1f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f).Randomize());
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
        if (!renderSprite)
            sprite.Visible = false;
    }

    public override void Update()
    {
        base.Update();
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
    }

    public override void Render()
    {
        base.Render();
        if (renderBubble)
            Draw.Circle(Position + sprite.Position, (float)(10.0 - (double)shieldRadiusWiggle.Value * 2.0), Color.White, 3);
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
        player.PointBounce(Center);
        if (Input.MoveX.Value == Math.Sign(player.Speed.X))
            player.Speed.X *= speedMult;
        moveWiggle.Start();
        shieldRadiusWiggle.Start();
        moveWiggleDir = (Center - player.Center).SafeNormalize(Vector2.UnitY);
        Audio.Play(customSound, Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        SceneAs<Level>().DirectionalShake((player.Center - Center).SafeNormalize(), 0.15f);
    }
}