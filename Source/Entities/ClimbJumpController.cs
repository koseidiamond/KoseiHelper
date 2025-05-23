using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/ClimbJumpController")]
[Tracked]
public class ClimbJumpController : Entity
{
    public static float staminaSpent, wallBoostTimer, wallBoostStaminaRefund;
    public static float horizontalSpeedMultiplier = 1f;
    public static bool spendStaminaOnGround;
    public static bool checkWallBoostDirection, sweat = true;
    public static string climbJumpSound;

    private enum MultiplierCase
    {
        AllCases,
        OnlyNeutrals,
        OnlyNonNeutrals
    };
    public static DustType climbDustType = DustType.Normal;
    private static MultiplierCase multiplierCase = MultiplierCase.AllCases;
    public ClimbJumpController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Add(new PostUpdateHook(() => { }));
        staminaSpent = data.Float("staminaSpent", 27.5f);
        wallBoostStaminaRefund = data.Float("wallBoostStaminaRefund", 27.5f);
        horizontalSpeedMultiplier = data.Float("horizontalSpeedMultiplier", 1f);
        wallBoostTimer = data.Float("wallBoostTimer", 0.2f);
        spendStaminaOnGround = data.Bool("spendStaminaOnGround", false);
        checkWallBoostDirection = data.Bool("checkWallBoostDirection", true);
        climbDustType = data.Enum("dustType", DustType.Normal);
        multiplierCase = data.Enum("speedMultiplierCase", MultiplierCase.AllCases);
        climbJumpSound = data.Attr("climbJumpSound", "event:/char/madeline/jump_climb_");
        sweat = data.Bool("sweat", true);
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
                if (sweat)
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
            if (self.moveX == 0 || !checkWallBoostDirection)
            {
                self.wallBoostDir = 0 - self.Facing;
                self.wallBoostTimer = wallBoostTimer;
                // I HAVE NO CLUE WHY IS THIS NOT WORKING
                /*if (self.wallBoostTimer >= 0f && self.wallBoostDir != self.moveX)
                {
                    //self.Speed.X = Player.WallJumpHSpeed * (float)self.moveX * horizontalSpeedMultiplier;
                    if (self.CheckStamina <= Player.ClimbMaxStamina - wallBoostStaminaRefund) // If current stamina <= 110 by default
                    {

                        Logger.Debug(nameof(KoseiHelperModule), $"Refunded");
                        self.Stamina += wallBoostStaminaRefund; // Refunds 27.5 by default
                    }
                }*/
            }
            switch (multiplierCase)
            {
                case MultiplierCase.OnlyNeutrals:
                    if (self.moveX == 0)
                        self.Speed.X *= horizontalSpeedMultiplier;
                    break;
                case MultiplierCase.OnlyNonNeutrals:
                    if (self.moveX != 0)
                        self.Speed.X *= horizontalSpeedMultiplier;
                    break;
                default: // AllCases
                    self.Speed.X *= horizontalSpeedMultiplier;
                    break;
            }

            // Particles:
            if (self.Facing == Facings.Right)
            {
                Audio.Play(climbJumpSound + "right");
                switch (climbDustType)
                {
                    case DustType.Sparkly:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.SparkyDust);
                        break;
                    case DustType.Chimney:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.Chimney);
                        break;
                    case DustType.Steam:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.Steam);
                        break;
                    case DustType.VentDust:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.VentDust);
                        break;
                    case DustType.None:
                        break;
                    default:
                        Dust.Burst(self.Center + Vector2.UnitX * 2f, MathF.PI * -3f / 4f, 4, ParticleTypes.Dust);
                        break;
                }
            }
            else
            {
                Audio.Play(climbJumpSound + "left");
                switch (climbDustType)
                {
                    case DustType.Sparkly:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.SparkyDust);
                        break;
                    case DustType.Chimney:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.Chimney);
                        break;
                    case DustType.Steam:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.Steam);
                        break;
                    case DustType.VentDust:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.VentDust);
                        break;
                    case DustType.None:
                        break;
                    default:
                        Dust.Burst(self.Center + Vector2.UnitX * -2f, -MathF.PI / 4f, 4, ParticleTypes.Dust);
                        break;
                }
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