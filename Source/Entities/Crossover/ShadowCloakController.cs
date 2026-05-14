using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/ShadowCloakController")]
[Tracked]
public class ShadowCloakController : Entity
{

    private string flag;
    private static Coroutine shadowEndDelayCoroutine;
    private static readonly ParticleType ShadowParticles = new ParticleType
    {
        Color = Color.FromNonPremultiplied(8, 8, 12, 210),
        Color2 = Color.FromNonPremultiplied(20, 23, 29, 185),
        ColorMode = ParticleType.ColorModes.Fade,
        FadeMode = ParticleType.FadeModes.InAndOut,
        Size = 2f,
        SizeRange = 0.8f,
        LifeMin = 0.2f,
        LifeMax = 0.4f,
        SpeedMin = 38f,
        SpeedMax = 60f,
        Friction = 12f,
        RotationMode = ParticleType.RotationModes.Random,
        ScaleOut = true
    };
    private readonly float cooldown;
    private static float cooldownTimer = 0f;
    private string recoverShadowDashSfx;
    private string useShadowDashSfx;

    private static bool playedRecoverEffect = false;

    public ShadowCloakController(EntityData data, Vector2 levelOffset)
    {
        flag = data.Attr("flagRequired","");
        cooldown = data.Float("cooldown", 1.5f);
        recoverShadowDashSfx = data.Attr("recoverShadowDashSfx", "event:/none");
        useShadowDashSfx = data.Attr("useShadowDashSfx", "event:/KoseiHelper/Crossover/ShadowCloak");
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = Scene.Tracker.GetEntity<Player>();
        KoseiHelperModule.Session.HasShadowDash = level.Session.GetFlag(flag) || string.IsNullOrEmpty(flag);
        if (player != null)
        {
            if (cooldownTimer > 0f)
            {
                if (player.JustRespawned)
                    cooldownTimer = 0f;
                else
                {
                    cooldownTimer -= Engine.DeltaTime;
                    if (!playedRecoverEffect && cooldownTimer <= 0.15f)
                    {
                        playedRecoverEffect = true;
                        EmitShadowParticles(level, player);
                        Audio.Play(recoverShadowDashSfx, player.Center);
                    }
                }
            }
        }
    }

    private void EmitShadowParticles(Level level, Player player)
    {
        for (int i = 0; i < 14; i++)
        {
            float angle = Calc.Random.NextAngle();
            Vector2 offset = Calc.AngleToVector(angle, 18f);
            Vector2 velocityOffset = player.Speed * 0.04f;
            level.ParticlesBG.Emit(ShadowParticles, player, 1, offset + new Vector2(0f,-9f), Vector2.Zero, angle + MathHelper.Pi);
        }
    }

    private static PlayerDeadBody ShadowDash(On.Celeste.Player.orig_Die orig, Player p, Vector2 direction, bool evenIfInvincible = false, bool registerDeathInStats = true)
    {
        if (KoseiHelperModule.Session.HasShadowDash && KoseiHelperModule.Session.ShadowDashActive && !evenIfInvincible)
            return null;
        else
            return orig(p, direction, evenIfInvincible, registerDeathInStats);
    }

    public static Color ShadowDashColor(On.Celeste.PlayerHair.orig_GetHairColor orig, PlayerHair self, int index)
    {
        Color original = orig(self, index);

        if (KoseiHelperModule.Session.HasShadowDash)
        {
            ShadowCloakController controller = Engine.Scene?.Tracker?.GetEntity<ShadowCloakController>();

            float maxCooldown = controller?.cooldown/15 ?? 0.1f;
            float t = Calc.ClampedMap(cooldownTimer, 0f, maxCooldown, 0f, 1f);
            return Color.Lerp(new Color(20, 23, 29), original, t);
        }

        return original;
    }



    private static Color ShadowTrailColor(On.Celeste.Player.orig_GetCurrentTrailColor orig, Player self)
    {
        if (KoseiHelperModule.Session.HasShadowDash)
        {
            Color color = new Color(7, 8, 11);
            return color;
        }
        return orig(self);
    }


    private static void ShadowDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
    {
        KoseiHelperModule.Session.ShadowDashActive = false;
        if (KoseiHelperModule.Session.HasShadowDash && cooldownTimer <= 0f)
        {
            cooldownTimer = (float)(self.Scene.Tracker.GetEntity<ShadowCloakController>()?.cooldown);
            playedRecoverEffect = false;
            Audio.Play(self.Scene.Tracker.GetEntity<ShadowCloakController>()?.useShadowDashSfx, self.Center);
            KoseiHelperModule.Session.ShadowDashActive = true;
            shadowEndDelayCoroutine?.RemoveSelf();
            KoseiHelperModule.ExtendedVariantImports.TriggerBooleanVariant("MadelineIsSilhouette", true, true);
            self.Hair.Color = new Color(20, 23, 29); // I didn't manage to avoid the blue while dashing but at least it starts black
        }
        orig(self);
    }
    private static void ShadowDashEnd(On.Celeste.Player.orig_DashEnd orig, Player self)
    {
        orig(self);
        if (self.StateMachine.State != 2 && KoseiHelperModule.Session.ShadowDashActive)
        {
            shadowEndDelayCoroutine = new Coroutine(ShadowEndDelay());
            self.Add(shadowEndDelayCoroutine);
            self.Hair.Color = new Color(20, 23, 29); // enforce black silhouette for the final frame while dashing
            KoseiHelperModule.ExtendedVariantImports.TriggerBooleanVariant("MadelineIsSilhouette", false, true);
        }
    }

    private static IEnumerator ShadowEndDelay()
    {
        yield return 0.03f;
        KoseiHelperModule.Session.ShadowDashActive = false;
    }

    public static void Load()
    {
        On.Celeste.Player.Die += ShadowDash;
        On.Celeste.PlayerHair.GetHairColor += ShadowDashColor;
        On.Celeste.Player.GetCurrentTrailColor += ShadowTrailColor;
        On.Celeste.Player.DashEnd += ShadowDashEnd;
        On.Celeste.Player.DashBegin += ShadowDashBegin;
    }

    public static void Unload()
    {
        On.Celeste.Player.Die -= ShadowDash;
        On.Celeste.PlayerHair.GetHairColor -= ShadowDashColor;
        On.Celeste.Player.GetCurrentTrailColor -= ShadowTrailColor;
        On.Celeste.Player.DashEnd -= ShadowDashEnd;
        On.Celeste.Player.DashBegin -= ShadowDashBegin;
    }
}