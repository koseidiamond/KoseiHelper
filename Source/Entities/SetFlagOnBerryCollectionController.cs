using Celeste.Mod.CollabUtils2.Entities;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/SetFlagOnBerryCollectionController")]
    public class SetFlagOnBerryCollectionController : Entity
    {
        private string flag;
        private bool flagValue;
        private string counter;
        private enum BerryType
        {
            Default,
            React,
            Ignore
        };

        private BerryType reactToGoldens;
        private BerryType reactToReds;
        private BerryType reactToMoons;
        private BerryType reactToWingeds;
        private BerryType reactToGhosts;
        private BerryType reactToSilvers;

        public SetFlagOnBerryCollectionController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            flag = data.Attr("flag", "KoseiHelper_BerryCollected");
            flagValue = data.Bool("flagValue", true);
            counter = data.Attr("counter", "KoseiHelper_BerriesCollected");
            reactToGoldens = data.Enum("reactToGoldens", BerryType.Default);
            reactToReds = data.Enum("reactToReds", BerryType.Default);
            reactToMoons = data.Enum("reactToMoons", BerryType.Default);
            reactToWingeds = data.Enum("reactToWingeds", BerryType.Default);
            reactToGhosts = data.Enum("reactToGhosts", BerryType.Default);
            reactToSilvers = data.Enum("reactToSilvers", BerryType.Default);
            if (data.Bool("global", false))
                Tag = Tags.Global;
        }

        public override void Update()
        {
            Level level = SceneAs<Level>();
            Session session = level.Session;
            foreach (Strawberry berry in level.Entities.FindAll<Strawberry>())
            {
                if (berry.sprite.CurrentAnimationID == "collect")
                {
                    bool golden = ((reactToGoldens == BerryType.Default) ||
                        (reactToGoldens == BerryType.React && berry.Golden) ||
                        (reactToGoldens == BerryType.Ignore && !berry.Golden));

                    bool red = ((reactToReds == BerryType.Default) ||
                        (reactToReds == BerryType.React && (!berry.Golden && !berry.Moon)) ||
                        (reactToReds == BerryType.Ignore && (berry.Golden && berry.Moon)));

                    bool moon = ((reactToMoons == BerryType.Default) ||
                        (reactToMoons == BerryType.React && berry.Moon) ||
                        (reactToMoons == BerryType.Ignore && !berry.Moon));

                    bool winged = ((reactToWingeds == BerryType.Default) ||
                        (reactToWingeds == BerryType.React && berry.Winged) ||
                        (reactToWingeds == BerryType.Ignore && !berry.Winged));

                    bool ghost = ((reactToGhosts == BerryType.Default) ||
                        (reactToGhosts == BerryType.React && berry.isGhostBerry) ||
                        (reactToGhosts == BerryType.Ignore && !berry.isGhostBerry));

                    bool silver = true;
                    if (KoseiHelperModule.Instance.collabUtils2Loaded)
                    {
                        silver = CollabUtils2_Silver(berry);
                    }

                    if (golden && red && moon && winged && ghost && silver)
                    {
                        if (!string.IsNullOrEmpty(flag))
                            session.SetFlag(flag, flagValue);
                        if (!string.IsNullOrEmpty(counter))
                            session.SetCounter(counter, session.Strawberries.Count);
                        break;
                    }
                }
            }
            base.Update();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool CollabUtils2_Silver(Entity berry)
        {
            return ((reactToSilvers == BerryType.Default) ||
                (reactToSilvers == BerryType.React && berry is SilverBerry) ||
                (reactToSilvers == BerryType.Ignore && berry is not SilverBerry));
        }
    }
}
