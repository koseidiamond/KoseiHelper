using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CounterRefill")]
[Tracked]
public class CounterRefill : Entity
{
    private static Coroutine CounterEndDelayCoroutine;
    public static ParticleType P_Shatter;
    public static ParticleType P_Regen;
    public static ParticleType P_Glow;
    public static ParticleType P_ShatterTwo;
    public static ParticleType P_RegenTwo;
    public static ParticleType P_GlowTwo;
    private Sprite sprite;
    private Sprite flash;
    private Image outline;
    private Wiggler wiggler;
    private BloomPoint bloom;
    private VertexLight light;
    private Level level;
    private SineWave sine;
    private bool twoDashes;
    private bool oneUse;
    private ParticleType p_shatter;
    private ParticleType p_regen;
    private ParticleType p_glow;
    private float respawnTimer;
    private string str;

    public bool decrease;
    public CounterRefill(Vector2 position, bool oneUse) : base(position)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
        this.oneUse = oneUse;
        str = "objects/KoseiHelper/Refills/CounterRefill/";
        this.p_shatter = Refill.P_Shatter;
        this.p_regen = Refill.P_Regen;
        this.p_glow = Refill.P_Glow;
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
    public CounterRefill(EntityData data, Vector2 offset) : this(data.Position + offset, data.Bool("oneUse", false))
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        base.Add(new PlayerCollider(new Action<Player>(this.OnPlayer), null, null));
        this.oneUse = data.Bool("oneUse", false);
        this.decrease = data.Bool("decrease", false);
        str = data.Attr("sprite", "objects/KoseiHelper/Refills/CounterRefill/");
        this.p_shatter = Refill.P_Shatter;
        this.p_regen = Refill.P_Regen;
        this.p_glow = Refill.P_Glow;
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
        base.Added(scene);
        this.level = base.SceneAs<Level>();
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
                this.level.ParticlesFG.Emit(BadelineBoost.P_Ambience, 1, this.Position, Vector2.One * 5f);
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
            this.level.ParticlesFG.Emit(BadelineBoost.P_Move, 16, this.Position, Vector2.One * 2f);
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
        if (KoseiHelperModule.Session.HasCounterDash == false)
        {
            Audio.Play(this.twoDashes ? "event:/new_content/game/10_farewell/pinkdiamond_touch" : "event:/game/general/diamond_touch", this.Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            this.Collidable = false;
            base.Add(new Coroutine(this.RefillRoutine(player), true));
            player.UseRefill(false);
            KoseiHelperModule.Session.HasCounterDash = true;
            if (!KoseiHelperModule.Session.counterRefillWhenUsed)
            {
                if (!decrease)
                    level.Session.IncrementCounter("KoseiHelper_CounterRefill");
                else
                    level.Session.SetCounter("KoseiHelper_CounterRefill", level.Session.GetCounter("KoseiHelper_CounterRefill") - 1);
            }
            this.respawnTimer = 2.5f;
        }
    }
    private IEnumerator RefillRoutine(Player player)
    {
        Celeste.Freeze(0.05f);
        yield return null;
        this.level.Shake(0.3f);
        this.sprite.Visible = (this.flash.Visible = false);
        bool flag = !this.oneUse;
        if (flag)
        {
            this.outline.Visible = true;
        }
        this.Depth = 8999;
        yield return 0.05f;
        float angle = player.Speed.Angle();
        this.level.ParticlesFG.Emit(Refill.P_ShatterTwo, 5, this.Position, Vector2.One * 4f, angle - 1.5707964f);
        this.level.ParticlesFG.Emit(Refill.P_ShatterTwo, 5, this.Position, Vector2.One * 4f, angle + 1.5707964f);
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
        On.Celeste.Player.Die += CounterDash;
        On.Celeste.Player.DashEnd += CounterDashEnd;
        On.Celeste.Player.DashBegin += CounterDashBegin;
        On.Celeste.PlayerHair.GetHairColor += CounterDashHairColor;
    }
    public static void Unload()
    {
        On.Celeste.Player.Die -= CounterDash;
        On.Celeste.Player.DashEnd -= CounterDashEnd;
        On.Celeste.Player.DashBegin -= CounterDashBegin;
        On.Celeste.PlayerHair.GetHairColor -= CounterDashHairColor;
    }

    public static Color CounterDashHairColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
    {
        if (KoseiHelperModule.Session.HasCounterDash)
            return KoseiHelperModule.Session.CounterDashColor;
        else
            return orig(self, index);
    }

    private static PlayerDeadBody CounterDash(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
    {
        if (!KoseiHelperModule.Session.CounterDashActive || evenIfInvincible)
        {
            KoseiHelperModule.Session.HasCounterDash = false;
            KoseiHelperModule.Session.CounterDashActive = false;
        }
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }
    private static void CounterDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        if (KoseiHelperModule.Session.HasCounterDash)
        {
            KoseiHelperModule.Session.CounterDashActive = true;
            if (KoseiHelperModule.Session.counterRefillWhenUsed)
            {
                if (!KoseiHelperModule.Session.counterRefillDecrease)
                    self.SceneAs<Level>().Session.IncrementCounter("KoseiHelper_CounterRefill");
                else
                    self.SceneAs<Level>().Session.SetCounter("KoseiHelper_CounterRefill", self.SceneAs<Level>().Session.GetCounter("KoseiHelper_CounterRefill")-1);
            }
            CounterEndDelayCoroutine?.Cancel();
            CounterEndDelayCoroutine?.RemoveSelf();
        }
        KoseiHelperModule.Session.HasCounterDash = false;
        orig(self);
    }
    private static void CounterDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);
        if (self.StateMachine.State != 2 && KoseiHelperModule.Session.CounterDashActive)
        {
            CounterEndDelayCoroutine = new Coroutine(CounterEndDelay());
            self.Add(CounterEndDelayCoroutine);
        }
    }
    private static IEnumerator CounterEndDelay()
    {
        yield return 0.03f;
        KoseiHelperModule.Session.CounterDashActive = false;
    }
}