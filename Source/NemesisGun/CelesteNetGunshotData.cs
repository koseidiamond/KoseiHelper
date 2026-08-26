using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    // Copied from Guneline, see https://github.com/oli-x64/Guneline/blob/master/CelesteNetGunshotData.cs
    public class CelesteNetGunshotData : DataType<CelesteNetGunshotData>
    {
        public DataPlayerInfo Player;
        public Vector2 Velocity;
        public int Facing;

        static CelesteNetGunshotData()
        {
            DataID = "nemesisGunShot";
        }

        protected override void Read(CelesteNetBinaryReader reader)
        {
            Velocity = reader.ReadVector2();
            Facing = reader.ReadInt32();
        }

        protected override void Write(CelesteNetBinaryWriter writer)
        {
            writer.Write(Velocity);
            writer.Write(Facing);
        }

        public override MetaType[] GenerateMeta(DataContext ctx)
        {
            return [new MetaPlayerUpdate(Player)];
        }

        public override void FixupMeta(DataContext ctx)
        {
            Player = Get<MetaPlayerUpdate>(ctx);
        }
    }
}