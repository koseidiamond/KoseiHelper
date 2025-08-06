using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FlagRefill")]
[Tracked]
public class FlagRefill : Entity
{
    public static ParticleType P_Shatter;
    public static ParticleType P_Regen;
    public static ParticleType P_Glow;
    private static Coroutine FlagEndDelayCoroutine;
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
    private string str;
    public float respawnTime = 2.5f;
    public FlagRefill(Vector2 position, bool oneUse, float respawnTime, bool twoDashes) : base(position)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
        this.oneUse = oneUse;
        this.respawnTime = respawnTime;
        this.twoDashes = twoDashes;
        str = "objects/KoseiHelper/Refills/FlagRefill/";
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
    public FlagRefill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("oneUse", false), data.Float("respawnTime", 2.5f), data.Bool("twoDashes", false))
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
        this.oneUse = data.Bool("oneUse", false);
        this.respawnTime = data.Float("respawnTime", 2.5f);
        this.twoDashes = data.Bool("twoDashes", false);
        str = data.Attr("sprite", "objects/KoseiHelper/Refills/FlagRefill/");
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
            Color = Color.Red,
            Color2 = Color.HotPink
        };
        P_Regen = new ParticleType(Refill.P_RegenTwo)
        {
            Color = Color.Red,
            Color2 = Color.MediumVioletRed
        };
        P_Glow = new ParticleType(Refill.P_Glow)
        {
            Color = Color.IndianRed,
            Color2 = Color.Magenta
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
            if (base.Scene.OnInterval(0.1f))
            {
                SceneAs<Level>().ParticlesFG.Emit(P_Glow, 1, this.Position, Vector2.One * 5f);
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

    //Here's where the actual functionality from this refill starts
    private void OnPlayer(Player player)
    {
        if (KoseiHelperModule.Session.HasFlagDash == false)
        {
            Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", this.Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            this.Collidable = false;
            base.Add(new Coroutine(this.RefillRoutine(player), true));
            player.UseRefill(false);
            KoseiHelperModule.Session.HasFlagDash = true;
            SceneAs<Level>().Session.SetFlag(KoseiHelperModule.Session.flagRefillFlag, true);
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
        On.Celeste.Player.Die += FlagDash;
        On.Celeste.Player.DashEnd += FlagDashEnd;
        On.Celeste.Player.DashBegin += FlagDashBegin;
        On.Celeste.PlayerHair.GetHairColor += FlagDashHairColor;
    }
    public static void Unload()
    {
        On.Celeste.Player.Die -= FlagDash;
        On.Celeste.Player.DashEnd -= FlagDashEnd;
        On.Celeste.Player.DashBegin -= FlagDashBegin;
        On.Celeste.PlayerHair.GetHairColor -= FlagDashHairColor;
    }

    public static Color FlagDashHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
    {
        if (KoseiHelperModule.Session.HasFlagDash)
            return KoseiHelperModule.Session.FlagDashColor;
        else
            return orig(self, index);
    }

    private static PlayerDeadBody FlagDash(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
    {
        if (!KoseiHelperModule.Session.FlagDashActive || evenIfInvincible)
        {
            KoseiHelperModule.Session.HasFlagDash = false;
            KoseiHelperModule.Session.FlagDashActive = false;
            self.SceneAs<Level>().Session.SetFlag(KoseiHelperModule.Session.flagRefillFlag, false);
        }
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }
    private static void FlagDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        if (KoseiHelperModule.Session.HasFlagDash)
        {
            KoseiHelperModule.Session.FlagDashActive = true;
            self.SceneAs<Level>().Session.SetFlag(KoseiHelperModule.Session.flagRefillFlag, false);
            FlagEndDelayCoroutine?.Cancel();
            FlagEndDelayCoroutine?.RemoveSelf();
        }
        KoseiHelperModule.Session.HasFlagDash = false;
        orig(self);
    }
    private static void FlagDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);
        if (self.StateMachine.State != 2 && KoseiHelperModule.Session.FlagDashActive)
        {
            FlagEndDelayCoroutine = new Coroutine(FlagEndDelay());
            self.Add(FlagEndDelayCoroutine);
        }
    }
    private static IEnumerator FlagEndDelay()
    {
        yield return 0.03f;
        KoseiHelperModule.Session.FlagDashActive = false;
    }
}