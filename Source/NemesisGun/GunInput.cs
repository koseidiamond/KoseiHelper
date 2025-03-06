using Celeste;
using Celeste.Mod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{

    public static class GunInput
    {
        public static VirtualJoystick joystickAim;
        private static Vector2 currentCursorPos;
        private static Vector2 oldMousePos;
        public static bool GunShot => ShotInput;

        public static Vector2 CursorPosition
        {
            get => currentCursorPos;
            set
            {
                currentCursorPos = value;
                oldMousePos = value;
            }
        }
        private static bool ShotInput => Extensions.shotInput;
        private static MouseState State => Mouse.GetState();
        private static Vector2 MouseCursorPos => Vector2.Transform(new Vector2(State.X, State.Y), Matrix.Invert(Engine.ScreenMatrix));

        public static void UpdateInput(Player self)
        {
            currentCursorPos += oldMousePos - currentCursorPos;

            oldMousePos = MouseCursorPos;

            if (joystickAim.Value.LengthSquared() > 0.04f && self.Scene != null)
            {
                currentCursorPos = (self.Center + (joystickAim.Value * 70)) * 6;
            }

            currentCursorPos.X = MathHelper.Clamp(currentCursorPos.X, 0, 1920);
            currentCursorPos.Y = MathHelper.Clamp(currentCursorPos.Y, 0, 1080);
        }

        public static void RegisterInputs()
        {
            joystickAim = new VirtualJoystick(true, new VirtualJoystick.PadRightStick(Input.Gamepad, 0.2f));
        }

        public static void DeregisterInputs()
        {
            joystickAim?.Deregister();
        }
    }
}
