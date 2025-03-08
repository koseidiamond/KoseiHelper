using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{ // I don't even know what is this for but it's from Guneline
    public struct CursorData
    {
        public uint ID;

        public Vector2 cursorPos;

        public int facing;

        public CursorData(uint ID, Vector2 cursorPos, int facing)
        {
            this.ID = ID;
            this.cursorPos = cursorPos;
            this.facing = facing;
        }
    }
}