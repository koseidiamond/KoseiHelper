using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.NemesisGun;

[CustomEntity("KoseiHelper/NemesisRefill")]
[Tracked]
public class NemesisRefill : Entity
{
    public static ParticleType P_Shatter;
    public static ParticleType P_Regen;
    public static ParticleType P_Glow;
    private Sprite sprite;
    private Sprite flash;
    private Image outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private SineWave sine;
    private bool oneUse;
    private float respawnTimer;
    private string str;
    public float respawnTime = 2.5f;
    public enum NemesisRefillAction
    {
        RefillDash,
        RefillStamina,
        ForceDash,
        ForceJump,
        WallJump,
        SuperJump,
        SuperBounce,
        PointBounce,
        ReflectBounce,
        SideBounce,
        CassetteBubble,
        Rebound,
        Launch,
        ExplodeLaunch,
        GreenBoost,
        RedBoost,
        Die
    };
    private NemesisRefillAction refillAction;

    public NemesisRefill(Vector2 position, bool oneUse, float respawnTime, NemesisRefillAction refillAction) : base(position)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        this.oneUse = oneUse;
        this.respawnTime = respawnTime;
        this.refillAction = refillAction;
        str = "objects/KoseiHelper/Refills/NemesisRefill/";
        base.Add(this.outline = new Image(GFX.Game[str + "outline"]));
        this.outline.CenterOrigin();
        this.outline.Visible = false;
        base.Add(this.sprite = new Sprite(GFX.Game, str + "idle"));
        this.sprite.AddLoop("idle", "", 0.1f);
        this.sprite.Play("idle", false, false);
        this.sprite.Visible = false;
        this.sprite.CenterOrigin();
        base.Add(this.flash = new Sprite(GFX.Game, str + "flash"));
        this.flash.Add("flash", "", 0.05f);
        this.flash.OnFinish = delegate (string anim)
        {
            this.flash.Visible = false;
        };
        this.flash.CenterOrigin();
        base.Add(this.wiggler = Wiggler.Create(1f, 4f, delegate (float v)
        {
            this.sprite.Scale = (this.flash.Scale = Vector2.One * (1f + v * 0.2f));
        }, false, false));
        base.Add(new MirrorReflection());
        base.Add(this.bloom = new BloomPoint(0.8f, 16f));
        base.Add(this.light = new VertexLight(Color.White, 1f, 16, 48));
        base.Add(this.sine = new SineWave(0.6f, 0f));
        this.sine.Randomize();
        this.UpdateY();
        base.Depth = -100;
    }
    public NemesisRefill(EntityData data, Vector2 offset) : this(
        data.Position + offset,
        data.Bool("oneUse", false),
        data.Float("respawnTime", 2.5f),
        data.Enum("refillAction", NemesisRefillAction.ForceDash))
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        this.oneUse = data.Bool("oneUse", false);
        this.respawnTime = data.Float("respawnTime", 2.5f);
        str = data.Attr("sprite", "objects/KoseiHelper/Refills/NemesisRefill/");
        base.Add(this.outline = new Image(GFX.Game[str + "outline"]));
        this.outline.CenterOrigin();
        this.outline.Visible = false;
        base.Add(this.sprite = new Sprite(GFX.Game, str + "idle"));
        this.sprite.AddLoop("idle", "", 0.1f);
        this.sprite.Play("idle", false, false);
        this.sprite.CenterOrigin();
        base.Add(this.flash = new Sprite(GFX.Game, str + "flash"));
        this.flash.Add("flash", "", 0.05f);
        this.flash.OnFinish = delegate (string anim)
        {
            this.flash.Visible = false;
        };
        this.flash.CenterOrigin();
        base.Add(this.wiggler = Wiggler.Create(1f, 4f, delegate (float v)
        {
            this.sprite.Scale = (this.flash.Scale = Vector2.One * (1f + v * 0.2f));
        }, false, false));
        base.Add(new MirrorReflection());
        base.Add(this.bloom = new BloomPoint(0.8f, 16f));
        base.Add(this.light = new VertexLight(Color.White, 1f, 16, 48));
        base.Add(this.sine = new SineWave(0.6f, 0f));
        this.sine.Randomize();
        this.UpdateY();
        base.Depth = -100;
    }
    public override void Added(Scene scene)
    {
        P_Shatter = new ParticleType(Refill.P_ShatterTwo)
        {
            Color = Extensions.color1,
            Color2 = Extensions.color2
        };
        P_Regen = new ParticleType(Refill.P_RegenTwo)
        {
            Color = Extensions.color1,
            Color2 = Extensions.color2
        };
        P_Glow = new ParticleType(Refill.P_Glow)
        {
            Color = Extensions.color1,
            Color2 = Extensions.color2
        };
        base.Added(scene);
    }
    public override void Update()
    {
        base.Update();

        if (Collidable && respawnTimer <= 0f)
        {
            Bullet bullet = CollideFirst<Bullet>();

            if (bullet != null)
            {
                OnBullet(bullet);
            }
        }
        bool flag = this.respawnTimer > 0f;
        if (flag)
        {
            this.respawnTimer -= Engine.DeltaTime;
            bool flag2 = this.respawnTimer <= 0f;
            if (flag2)
                this.Respawn();
        }
        else
        {
            if (base.Scene.OnInterval(0.1f))
                SceneAs<Level>().ParticlesFG.Emit(P_Glow, 1, this.Position, Vector2.One * 5f);
        }
        this.UpdateY();
        this.light.Alpha = Calc.Approach(this.light.Alpha, this.sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
        this.bloom.Alpha = this.light.Alpha * 0.8f;
        bool flag4 = base.Scene.OnInterval(2f) && this.sprite.Visible;
        if (flag4)
        {
            this.flash.Play("flash", true, false);
            this.flash.Visible = true;
        }
    }
    private void Respawn()
    {
        bool flag = !this.Collidable;
        if (flag)
        {
            this.Collidable = true;
            this.sprite.Visible = true;
            this.outline.Visible = false;
            base.Depth = -100;
            this.wiggler.Start();
            Audio.Play("event:/game/general/diamond_return", this.Position);
            SceneAs<Level>().ParticlesFG.Emit(P_Regen, 16, this.Position, Vector2.One * 2f);
        }
    }
    private void UpdateY()
    {
        this.flash.Y = (this.sprite.Y = (this.bloom.Y = this.sine.Value * 2f));
    }
    public override void Render()
    {
        bool visible = this.sprite.Visible;
        if (visible)
        {
            this.sprite.DrawOutline(1);
        }
        base.Render();
    }

    private void OnBullet(Bullet bullet)
    {
        Player player = Scene.Tracker.GetEntity<Player>();

        if (player == null)
            return;
        bullet.DestroyBullet();
        int playerDir = Math.Sign(player.Position.X - Position.X); // relative to the refill, for certain actions
        // force dash
        switch (refillAction)
        {
            case NemesisRefillAction.ForceDash:
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.State = Player.StDash;
                break;
            case NemesisRefillAction.RefillDash:
                player.UseRefill(false);
                break;
            case NemesisRefillAction.RefillStamina:
                player.RefillStamina();
                break;
            case NemesisRefillAction.ForceJump:
                player.Jump();
                break;
            case NemesisRefillAction.WallJump:
                player.WallJump(playerDir);
                break;
            case NemesisRefillAction.SuperJump:
                player.SuperJump();
                break;
            case NemesisRefillAction.SuperBounce:
                player.SuperBounce(player.Center.Y);
                break;
            case NemesisRefillAction.PointBounce:
                player.PointBounce(Center);
                break;
            case NemesisRefillAction.SideBounce:
                player.SideBounce((int)player.Facing, player.Center.X, player.Center.Y);
                break;
            case NemesisRefillAction.CassetteBubble:
                player.StartCassetteFly(Center + Vector2.UnitY * 6f, Center + Vector2.UnitY * 6f);
                break;
            case NemesisRefillAction.Rebound:
                player.Rebound();
                break;
            case NemesisRefillAction.Launch:
                player.BadelineBoostLaunch(player.Position.Y - 40f);
                break;
            case NemesisRefillAction.ExplodeLaunch:
                player.ExplodeLaunch(Center, true);
                break;
            case NemesisRefillAction.GreenBoost:
                Entities.BoosterNoOutline greenBooster = new Entities.BoosterNoOutline(player.Center, false, true);
                player.level.Add(greenBooster);
                player.Boost(greenBooster);
                break;
            case NemesisRefillAction.RedBoost:
                Entities.BoosterNoOutline redBooster = new Entities.BoosterNoOutline(player.Center, true, true);
                player.level.Add(redBooster);
                player.Boost(redBooster);
                break;
            case NemesisRefillAction.Die:
                player.Die((player.Center - Center).SafeNormalize());
                break;
            default:
                break;
        }

        Audio.Play("event:/game/general/diamond_touch", Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);

        Collidable = false;
        Add(new Coroutine(RefillRoutine(player), true));

        if (!oneUse)
            respawnTimer = respawnTime;
    }

    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
        yield return null;
        SceneAs<Level>().Shake(0.3f);
        this.sprite.Visible = (this.flash.Visible = false);
        bool flag = !this.oneUse;
        if (flag)
            this.outline.Visible = true;
        this.Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        SceneAs<Level>().ParticlesFG.Emit(P_Shatter, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
        SceneAs<Level>().ParticlesFG.Emit(P_Shatter, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
        SlashFx.Burst(this.Position, angle);
        bool flag2 = this.oneUse;
        if (flag2)
            this.RemoveSelf();
        yield break;
    }
}