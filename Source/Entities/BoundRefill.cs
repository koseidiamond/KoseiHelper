using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/BoundRefill")]
public class BoundRefill : Entity
{
    public static ParticleType P_Shatter = Refill.P_ShatterTwo;
    public static ParticleType P_Regen = Refill.P_RegenTwo;
    public static ParticleType P_Glow = Refill.P_GlowTwo;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private readonly SineWave sine;
    public Sprite sprite;
    private readonly bool outBound;
    public bool outline;
    public float respawnTimer;
    private Wiggler wiggler;
    private bool oneUse;
    public float respawnTime = 2.5f;

    public BoundRefill (EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        Add(new PlayerCollider(OnPlayer));

        //Read the custom properties from data
        outBound = data.Bool("outBound",true);
        respawnTime = data.Float("respawnTime", 2.5f);
        KoseiHelperModule.Session.oobClimbFix = data.Bool("climbFix", false);
        oneUse = data.Bool("oneUse", true);
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_boundRefill"));
        if (outBound)
            sprite.Play("OutBound");
        else
            sprite.Play("InBound");
        Add(new MirrorReflection());
        Add(bloom = new BloomPoint(0.5f, 16f));
        Add(light = new VertexLight(Color.White, 0.5f, 16, 48));
        Add(sine = new SineWave(0.6f, 0f));
        Add(wiggler = Wiggler.Create(1f, 4f, delegate (float v)
        {
            sprite.Scale = Vector2.One * (1f + v * 0.2f);
        }, false, false));
        sine.Randomize();
        UpdateY();
        Depth = -100;
    }


    public override void Added(Scene scene)
    {
        base.Added(scene);
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
                Respawn();
        }
        else
        {
            if (base.Scene.OnInterval(0.1f))
                level.ParticlesFG.Emit(BadelineBoost.P_Ambience, 1, this.Position, Vector2.One * 5f);
        }
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.8f;
    }

    private void Respawn()
    {
        bool flag = !this.Collidable;
        if (flag)
        {
            Collidable = true;
            sprite.Visible = true;
            outline = false;
            Depth = -100;
            wiggler.Start();
            Audio.Play("event:/new_content/game/10_farewell/pinkdiamond_return", this.Position);
            SceneAs<Level>().ParticlesFG.Emit(BadelineBoost.P_Move, 16, this.Position, Vector2.One * 2f);
            if (outBound)
                sprite.Play("OutBound");
            else
                sprite.Play("InBound");
        }
    }
    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
        yield return null;
        SceneAs<Level>().Shake(0.3f);
        sprite.Visible = false;
        bool flag = !this.oneUse;
        if (flag)
            outline = true;
        this.Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        SceneAs<Level>().ParticlesFG.Emit(Refill.P_ShatterTwo, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
        SceneAs<Level>().ParticlesFG.Emit(Refill.P_ShatterTwo, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
        SlashFx.Burst(Position, angle);
        bool flag2 = oneUse;
        if (flag2)
        {
            this.RemoveSelf();
        }
        yield break;
    }

    private void UpdateY()
    {
        Sprite obj = sprite;
        float num = (bloom.Y = sine.Value * 2f);
        float y = (obj.Y = num);
        obj.Y = y;
    }

    public override void Render()
    {
        if (sprite.Visible)
        {
            sprite.DrawOutline();

        }
        base.Render();
    }

    private void OnPlayer(Player player)
    {
        Collidable = false;
        Audio.Play("event:/new_content/game/10_farewell/pinkdiamond_touch", Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        sprite.Play("NoBound");
        Add(new Coroutine(RefillRoutine(player)));
        if (outBound)
            player.EnforceLevelBounds = false;

        else
        {
            player.EnforceLevelBounds = true;
            //Ideally this should check if the refill is not on the level bounds, and set a new spawn, to make filler rooms possible to transition to
        }
        respawnTimer = respawnTime;
    }
}