using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities
{
    [CustomEntity("KoseiHelper/SetFlagOnBerryCollectionController")]
    public class SetFlagOnBerryCollectionController : Entity
    {
        private string flag;
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

        public SetFlagOnBerryCollectionController(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            flag = data.Attr("flag", "KoseiHelper_BerryCollected");
            counter = data.Attr("counter", "KoseiHelper_BerriesCollected");
            reactToGoldens = data.Enum("reactToGoldens", BerryType.Default);
            reactToReds = data.Enum("reactToReds", BerryType.Default);
            reactToMoons = data.Enum("reactToMoons", BerryType.Default);
            reactToWingeds = data.Enum("reactToWingeds", BerryType.Default);
            reactToGhosts = data.Enum("reactToGhosts", BerryType.Default);
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

                    if (golden && red && moon && winged && ghost)
                    {
                        if (!string.IsNullOrEmpty(flag))
                            session.SetFlag(flag, true);
                        if (!string.IsNullOrEmpty(counter))
                            session.IncrementCounter(counter);
                        break;
                    }
                }
            }
            base.Update();
        }

        public void SetFlag(Session session)
        {
            // random funny stuff
            //level.strawberriesDisplay.strawberries.Color = Color.Green;
            //level.Tracker.GetEntity<Player>().Leader.Followers.Count
            //Logger.Debug(nameof(KoseiHelperModule), $"total berries: {session.Strawberries.Count}");
        }
    }
}
