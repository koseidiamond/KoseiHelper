using Celeste.Mod.KoseiHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    public static class Extensions
    {
        public static Vector2 HalfDimensions(this MTexture texture)
            => new Vector2(texture.Width / 2, texture.Height / 2);
        public static float ToRotation(this Vector2 vector) => (float)Math.Atan2(vector.Y, vector.X);
        public static string gunshotSound = "event:/ashleybl/gunshot";
        public static string bulletSound = "event:/none";
        public static bool replacesDash, bulletExplosion = true;
        public static int cooldown = 8, lifetime = 600;
        public static Color color1 = Color.SteelBlue;
        public static Color color2 = Color.Yellow;
        public static DustType shotDustType = DustType.Normal; // Aka Dust
        public enum GunDirections
        {
            Horizontal,
            FourDirections,
            EightDirections
        };
        public static GunDirections gunDirections;

        public static bool shotInput => replacesDash ? Input.Dash.Pressed || Input.CrouchDash.Pressed : KoseiHelperModule.Settings.NemesisShot.Pressed;

        // INTERACTIONS
        public static bool canKillPlayer, canGoThroughDreamBlocks, breakBounceBlocks, activateFallingBlocks, harmEnemies, harmTheo,
            breakSpinners, breakMovingBlades, enableKevins, loseGunOnRespawn, collectables, useRefills, explodeFishes, pressDashSwitches, canBounce = true;
        public enum TheoInteraction
        {
            None,
            Kill,
            HitSpinner,
            HitSpring
        }
        public static TheoInteraction theoInteraction = TheoInteraction.Kill;
    }
}