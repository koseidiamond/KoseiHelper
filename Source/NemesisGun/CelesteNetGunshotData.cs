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

        static CelesteNetGunshotData()
        {
            DataID = "nemesisGunShot";
        }

        protected override void Read(CelesteNetBinaryReader reader)
        {
            Velocity = reader.ReadVector2();
        }

        protected override void Write(CelesteNetBinaryWriter writer)
        {
            writer.Write(Velocity);
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