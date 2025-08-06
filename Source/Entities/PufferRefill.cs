using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/PufferRefill")]
[Tracked]
public class PufferRefill : Entity
{
    private static Coroutine PufferEndDelayCoroutine;
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
    private bool twoDashes;
    private bool oneUse;
    private float respawnTimer;
    private string texturePath;
    public float respawnTime = 2.5f;
    public PufferRefill(Vector2 position, bool oneUse, float respawnTime, bool twoDashes) : base(position)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
        this.oneUse = oneUse;
        this.respawnTime = respawnTime;
        this.twoDashes = twoDashes;
        texturePath = "objects/KoseiHelper/Refills/PufferRefill/";
        base.Add(this.outline = new Image(GFX.Game[texturePath + "outline"]));
        this.outline.CenterOrigin();
        this.outline.Visible = false;
        base.Add(this.sprite = new Sprite(GFX.Game, texturePath + "idle"));
        this.sprite.AddLoop("idle", "", 0.1f);
        this.sprite.Play("idle", false, false);
        this.sprite.Visible = false;
        this.sprite.CenterOrigin();
        base.Add(this.flash = new Sprite(GFX.Game, texturePath + "flash"));
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
    public PufferRefill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("oneUse", false), data.Float("respawnTime", 2.5f), data.Bool("twoDashes", false))
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
        this.oneUse = data.Bool("oneUse", false);
        this.respawnTime = data.Float("respawnTime", 2.5f);
        this.twoDashes = data.Bool("twoDashes", false);
        texturePath = data.Attr("sprite", "objects/KoseiHelper/Refills/PufferRefill/");
        base.Add(this.outline = new Image(GFX.Game[texturePath + "outline"]));
        this.outline.CenterOrigin();
        this.outline.Visible = false;
        base.Add(this.sprite = new Sprite(GFX.Game, texturePath + "idle"));
        this.sprite.AddLoop("idle", "", 0.1f);
        this.sprite.Play("idle", false, false);
        this.sprite.CenterOrigin();
        base.Add(this.flash = new Sprite(GFX.Game, texturePath + "flash"));
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
        P_Shatter = new ParticleType(Seeker.P_Regen)
        {
            Color = Color.Coral,
            Color2 = Color.Orange
        };
        P_Regen = new ParticleType(BadelineBoost.P_Move)
        {
            Color = Color.DarkOrange,
            Color2 = Color.Salmon
        };
        P_Glow = new ParticleType(BadelineBoost.P_Ambience)
        {
            Color = Color.DarkOrange,
            Color2 = Color.Orange
        };
        base.Added(scene);
    }
    public override void Update()
    {
        base.Update();
        bool flag = this.respawnTimer > 0f;
        if (flag)
        {
            this.respawnTimer -= Engine.DeltaTime;
            bool flag2 = this.respawnTimer <= 0f;
            if (flag2)
            {
                this.Respawn();
            }
        }
        else
        {
            bool flag3 = base.Scene.OnInterval(0.1f);
            if (flag3)
            {
                SceneAs<Level>().ParticlesFG.Emit(P_Glow, 1, this.Position, Vector2.One * 6f);
            }
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
            Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_return" : "event:/game/general/diamond_return", this.Position);
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
    private void OnPlayer(Player player)
    {
        if (KoseiHelperModule.Session.HasPufferDash == false)
        {
            Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", this.Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            this.Collidable = false;
            base.Add(new Coroutine(this.RefillRoutine(player), true));
            player.UseRefill(false);
            KoseiHelperModule.Session.HasPufferDash = true;
            this.respawnTimer = respawnTime;
        }
    }
    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
        yield return null;
        SceneAs<Level>().Shake(0.3f);
        this.sprite.Visible = (this.flash.Visible = false);
        bool flag = !this.oneUse;
        if (flag)
        {
            this.outline.Visible = true;
        }
        this.Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        SceneAs<Level>().ParticlesFG.Emit(P_Shatter, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
        SceneAs<Level>().ParticlesFG.Emit(P_Shatter, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
        SlashFx.Burst(this.Position, angle);
        bool flag2 = this.oneUse;
        if (flag2)
        {
            this.RemoveSelf();
        }
        yield break;
    }
    public static void Load()
    {
        On.Celeste.Player.Die += PufferDash;
        On.Celeste.Player.DashEnd += PufferDashEnd;
        On.Celeste.Player.DashBegin += PufferDashBegin;
        On.Celeste.PlayerHair.GetHairColor += PufferDashHairColor;
    }
    public static void Unload()
    {
        On.Celeste.Player.Die -= PufferDash;
        On.Celeste.Player.DashEnd -= PufferDashEnd;
        On.Celeste.Player.DashBegin -= PufferDashBegin;
        On.Celeste.PlayerHair.GetHairColor -= PufferDashHairColor;
    }

    public static Color PufferDashHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
    {
        if (KoseiHelperModule.Session.HasPufferDash)
        {
            return Color.FromNonPremultiplied(199, 137, 66, 255);
        }
        else
        {
            return orig(self, index);
        }

    }

    private static PlayerDeadBody PufferDash(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
    {
        if (!KoseiHelperModule.Session.PufferDashActive || evenIfInvincible)
        {
            KoseiHelperModule.Session.HasPufferDash = false;
            KoseiHelperModule.Session.PufferDashActive = false;
        }
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }
    private static void PufferDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        if (KoseiHelperModule.Session.HasPufferDash)
        {
            self.Add(new Coroutine(PufferExplode(self)));
            KoseiHelperModule.Session.PufferDashActive = true;
            PufferEndDelayCoroutine?.Cancel();
            PufferEndDelayCoroutine?.RemoveSelf();
        }
        KoseiHelperModule.Session.HasPufferDash = false;
        orig(self);
    }
    private static void PufferDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);
        if (self.StateMachine.State != 2 && KoseiHelperModule.Session.PufferDashActive)
        {
            PufferEndDelayCoroutine = new Coroutine(PufferEndDelay());
            self.Add(PufferEndDelayCoroutine);
        }
    }
    private static IEnumerator PufferEndDelay()
    {
        yield return 0.03f;
        KoseiHelperModule.Session.PufferDashActive = false;
    }

    private static IEnumerator PufferExplode(Player player)
    {
        Level level = player.SceneAs<Level>();
        player.ExplodeLaunch(player.Center - 2 * player.DashDir, false);
        Audio.Play("event:/new_content/game/10_farewell/puffer_splode");
        yield return 0.01f;
        level.Shake();
        level.Displacement.AddBurst(player.Center - 8 * player.DashDir, 0.4f, 12f, 36f, 0.5f);
        level.Displacement.AddBurst(player.Center - 8 * player.DashDir, 0.4f, 24f, 48f, 0.5f);
        level.Displacement.AddBurst(player.Center - 8 * player.DashDir, 0.4f, 36f, 60f, 0.5f);
        for (float num = 0f; num < MathF.PI * 2f; num += 0.17453292f)
        {
            Vector2 position = player.Center - 8 * player.DashDir + Calc.AngleToVector(num + Calc.Random.Range(-MathF.PI / 90f, MathF.PI / 90f), Calc.Random.Range(12, 18));
            level.Particles.Emit(Seeker.P_Regen, position, num);
        }
        yield return null;
    }
}