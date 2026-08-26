using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Apps
{
    public class CelesteNetMagicInkData : DataType<CelesteNetMagicInkData>
    {
        public DataPlayerInfo Player;
        public Vector2 From;
        public Vector2 To;

        static CelesteNetMagicInkData()
        {
            DataID = "magicInkStroke";
        }

        protected override void Read(CelesteNetBinaryReader reader)
        {
            From = reader.ReadVector2();
            To = reader.ReadVector2();
        }

        protected override void Write(CelesteNetBinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
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
