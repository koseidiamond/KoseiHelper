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

        public static bool replacesDash = true;
        public static int cooldown = 8;
        public static bool shotInput => replacesDash ? Input.Dash.Pressed || Input.CrouchDash.Pressed : KoseiHelperModule.Settings.NemesisShot.Pressed;

        // INTERACTIONS
        public static bool canKillPlayer, canGoThroughDreamBlocks, breakBounceBlocks, activateFallingBlocks, harmEnemies, harmTheo, breakSpinners = true;

    }
}