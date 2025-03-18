using Celeste.Mod.KoseiHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    // This class is mostly used to configure the gun settings that don't have an option in Mod Settings
    public static class Extensions
    {
        public static Vector2 HalfDimensions(this MTexture texture)
            => new Vector2(texture.Width / 2, texture.Height / 2);
        public static float ToRotation(this Vector2 vector) => (float)Math.Atan2(vector.Y, vector.X);
        public static string gunshotSound = "event:/KoseiHelper/Guns/shotDefault";
        public static bool bulletExplosion = true;
        public static bool loseGunOnRespawn = false;
        public static Color color1 = Color.Orange;
        public static Color color2 = Color.Yellow;
        public static float particleAlpha = 1f;
        public static DustType shotDustType = DustType.Normal; // Aka Dust

        public static string gunTexture = "objects/KoseiHelper/Guns/NemesisGun";
        public static string bulletTexture = "objects/KoseiHelper/Guns/Bullets/Invisible";
        public static string customParticleTexture = "particles/KoseiHelper/star";
        public static int bulletWidth = 6, bulletHeight = 6, bulletXOffset, bulletYOffset;

        public static bool shotInput => KoseiHelperModule.Settings.GunSettings.dashBehavior == KoseiHelperModuleSettings.NemesisSettings.DashBehavior.ReplacesDash ? Input.Dash.Pressed || Input.CrouchDash.Pressed : KoseiHelperModule.Settings.NemesisShot.Pressed;
    }
}