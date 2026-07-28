using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/SetFlagPrefixOnSpawnController")]
[Tracked]
public class SetFlagPrefixOnSpawnController : Entity
{
    private static bool isRespawning = false;

    public static void Load()
    {
        On.Celeste.Level.LoadLevel += onLoadLevel;
    }

    public static void Unload()
    {
        On.Celeste.Level.LoadLevel -= onLoadLevel;
    }

    private static void onLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
    {
        isRespawning = (playerIntro != Player.IntroTypes.Transition);
        orig(self, playerIntro, isFromLoader);
        isRespawning = false;
    }

    public SetFlagPrefixOnSpawnController(EntityData data, Vector2 offset) : base()
    {
        // Copied from Maddie's Helping Hand!
        Level level = Engine.Scene as Level;
        if (level == null)
        {
            level = (Engine.Scene as LevelLoader)?.Level;
        }

        if (!data.Bool("onlyOnRespawn", defaultValue: false) || isRespawning)
        {
            string otherFlag = data.Attr("ifFlag");
            if (string.IsNullOrEmpty(otherFlag) || level.Session.GetFlag(otherFlag))
            {
                foreach (string flag in data.Attr("flag").Split(','))
                {
                    level?.Session.Flags.RemoveWhere(f => f.StartsWith(flag));
                }
            }
        }
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        RemoveSelf();
    }
}