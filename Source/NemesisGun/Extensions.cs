using Celeste.Mod.KoseiHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    // This class is mostly used to configure the gun settings
    public static class Extensions
    {
        public static Vector2 HalfDimensions(this MTexture texture)
            => new Vector2(texture.Width / 2, texture.Height / 2);
        public static float ToRotation(this Vector2 vector) => (float)Math.Atan2(vector.Y, vector.X);
        public static string gunshotSound = "event:/KoseiHelper/Guns/shotDefault";
        public static bool bulletExplosion = true;
        public static bool loseGunOnRespawn = false;
        public static bool canShootInFeather = true;
        public static int cooldown = 8, lifetime = 600, recoilCooldown = 16;
        public static Color color1 = Color.Orange;
        public static Color color2 = Color.Yellow;
        public static float particleAlpha = 1f;
        public static DustType shotDustType = DustType.Normal; // Aka Dust
        public static float speedMultiplier = 1f, recoil = 80f;
        public static int freezeFrames;
        public enum DashBehavior
        {
            ReplacesDash,
            ConsumesDash,
            None
        }
        public static DashBehavior dashBehavior = DashBehavior.None;
        public enum GunDirections
        {
            Horizontal,
            FourDirections,
            EightDirections
        };
        public static GunDirections gunDirections = GunDirections.FourDirections;
        public enum DreamBlockBehavior
        {
            GoThrough,
            Destroy,
            None
        };
        public static DreamBlockBehavior dreamBlockBehavior;

        // INTERACTIONS
        public static bool canKillPlayer = true, breakBounceBlocks = true, activateFallingBlocks = true,
        harmEnemies = true, harmTheo = true, breakSpinners = true, breakMovingBlades = true, enableKevins = true, useFeathers = true,
        collectables = true, useRefills = true, pressDashSwitches = true, canBounce = true, scareBirds = true, collectTouchSwitches = true,
        collectBadelineOrbs = true, coreModeToggles = true;
        public static bool useBoosters = false;
        public static float waterFriction = 0.995f;
        public static string gunTexture = "objects/KoseiHelper/Guns/NemesisGun";
        public static string bulletTexture = "objects/KoseiHelper/Guns/Bullets/Invisible";
        public static string customParticleTexture = "particles/KoseiHelper/star";
        public enum TheoInteraction
        {
            None,
            Kill,
            HitSpinner,
            HitSpring
        }
        public static TheoInteraction theoInteraction = TheoInteraction.HitSpring;
        public enum PufferInteraction
        {
            None,
            Explode,
            HitSpring
        }
        public static PufferInteraction pufferInteraction = PufferInteraction.Explode;

        public static bool shotInput => dashBehavior == DashBehavior.ReplacesDash ? Input.Dash.Pressed || Input.CrouchDash.Pressed : KoseiHelperModule.Settings.NemesisShot.Pressed;
    }
}