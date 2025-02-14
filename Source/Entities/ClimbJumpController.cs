using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using static MonoMod.InlineRT.MonoModRule;
using ExtendedVariants.Variants;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/ClimbJumpController")]
[Tracked]
public class ClimbJumpController : Entity
{
    public static float staminaSpent, wallBoostTimer, wallBoostStaminaRefund, horizontalSpeedMultiplier;
    public static bool spendStaminaOnGround, checkWallBoostDirection;
    public static string climbJumpSound;
    public enum ClimbDustType
    {
        Normal,
        Sparkly,
        Chimney,
        Steam,
        VentDust,
        None
    };
    public static ClimbDustType climbDustType;
    public ClimbJumpController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Add(new PostUpdateHook(() => { }));
        staminaSpent = data.Float("staminaSpent", 27.5f);
        wallBoostStaminaRefund = data.Float("wallBoostStaminaRefund", 27.5f);
        horizontalSpeedMultiplier = data.Float("horizontalSpeedMultiplier", 1f);
        wallBoostTimer = data.Float("wallBoostTimer", 0.2f);
        spendStaminaOnGround = data.Bool("spendStaminaOnGround", false);
        checkWallBoostDirection = data.Bool("checkWallBoostDirection", true); // TODO idk how to do this one
        climbDustType = data.Enum("dustType", ClimbDustType.Normal);
        climbJumpSound = data.Attr("climbJumpSound", "event:/char/madeline/jump_climb_");
    }

    public static void Load()
    {
        On.Celeste.Player.ClimbJump += Player_ClimbJump;
    }

    public static void Unload()
    {
        On.Celeste.Player.ClimbJump -= Player_ClimbJump;
    }

    private static void Player_ClimbJump(On.Celeste.Player.orig_ClimbJump orig, Player self)
    {
        if (KoseiHelperModule.Session.ClimbJumpHook)
        {
            if (!self.onGround)
            {
                self.Stamina -= staminaSpent;
                self.sweatSprite.Play("jump", restart: true);
                Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            }
            else
            {
                if (spendStaminaOnGround)
                    self.Stamina -= staminaSpent;
            }
            self.dreamJump = false;
            self.Jump(particles: false, playSfx: false);
            if (self.moveX == 0)
            {
                self.wallBoostDir = 0 - self.Facing;
                self.wallBoostTimer = wallBoostTimer;
            }
            // This is in the original code, it is unused because I can't easily get the platforms on a static method, so the dust types are manual/custom:

            /*Platform platformByPriority = SurfaceIndex.GetPlatformByPriority(CollideAll<Platform>(self.Position - Vector2.UnitX * (float)self.Facing * 4f, self.temp));
            if (platformByPriority != null)
            {
                index = platformByPriority.GetWallSoundIndex(self, (int)self.Facing);
            }*/
            if (self.Facing == Facings.Right)
            {
                Audio.Play(climbJumpSound+"right");
                switch (climbDustType)
                {
                    case ClimbDustType.Sparkly:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.SparkyDust);
                        break;
                    case ClimbDustType.Chimney:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.Chimney);
                        break;
                    case ClimbDustType.Steam:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.Steam);
                        break;
                    case ClimbDustType.VentDust:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.VentDust);
                        break;
                    case ClimbDustType.None:
                        break;
                    default:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.Dust);
                        break;
                }
            }
            else
            {
                Audio.Play(climbJumpSound+"left");
                switch (climbDustType)
                {
                    case ClimbDustType.Sparkly:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.SparkyDust);
                        break;
                    case ClimbDustType.Chimney:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.Chimney);
                        break;
                    case ClimbDustType.Steam:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.Steam);
                        break;
                    case ClimbDustType.VentDust:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.VentDust);
                        break;
                    case ClimbDustType.None:
                        break;
                    default:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.Dust);
                        break;
                }
            }
            // test:
            if (wallBoostTimer > 0f)
            {
                Logger.Debug(nameof(KoseiHelperModule), $"self.moveX={self.moveX}, self.wallBoostDir={self.wallBoostDir}");
                    self.Speed.X = Player.WallJumpHSpeed * (float)self.moveX * horizontalSpeedMultiplier;
                    if (self.Stamina <= Player.ClimbMaxStamina + wallBoostStaminaRefund)
                        self.Stamina += wallBoostStaminaRefund;
            }
        }
        else
            orig(self);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        KoseiHelperModule.Session.ClimbJumpHook = true;
    }

    public override void Update()
    {
        base.Update();
    }
}