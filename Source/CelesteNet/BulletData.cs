using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.DataTypes;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    public class BulletData : DataType<BulletData>
    {
        public DataPlayerInfo Player;
        public uint HitGhostID;
        public Vector2 BulletDir;
        public Vector2 CursorPos;
        public int Facing;

        static BulletData()
        {
            DataID = "KoseiHelper/BulletData";
        }

        public BulletData()
        {
        }

        public BulletData(DataPlayerInfo player, Vector2 bulletDir)
        {
            Player = player;
            BulletDir = bulletDir;
        }

        protected override void Read(CelesteNetBinaryReader reader)
        {
            Facing = reader.ReadInt32();
            BulletDir = reader.ReadVector2();
            CursorPos = reader.ReadVector2();
        }

        protected override void Write(CelesteNetBinaryWriter writer)
        {
            writer.Write(Facing);
            writer.Write(BulletDir);
            writer.Write(CursorPos);
        }

        public override bool FilterHandle(DataContext ctx)
        {
            return Player != null;
        }

        public override MetaType[] GenerateMeta(DataContext ctx)
            => new MetaType[] { new MetaPlayerUpdate(Player) };

        /*public override MetaType[] GenerateMeta(DataContext ctx)
        {
            return (MetaType[])(object)new MetaType[2]
            {
            (MetaType)new MetaPlayerPrivateState(Player),
            (MetaType)new MetaBoundRef(DataType<DataPlayerInfo>.DataID, (uint)(((int?)Player?.ID) ?? (-1)), true)
            };
        }*/

        public override void FixupMeta(DataContext ctx)
        {
            Player = Get<MetaPlayerUpdate>(ctx);
            //Player = MetaPlayerBaseType<MetaPlayerPrivateState>.
            //    op_Implicit((MetaPlayerBaseType<MetaPlayerPrivateState>)(object)((DataType)this).Get<MetaPlayerPrivateState>(ctx));
            //((DataType)this).Get<MetaBoundRef>(ctx).ID = (uint)(((int?)Player?.ID) ?? (-1));
        }
    }
}