using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/SpawnToFlagController")]
public class SpawnToFlagController : Entity
{
    public string flagPrefix;
    public SpawnToFlagController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        flagPrefix = data.Attr("flagPrefix", "spawnCoordinates_");
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        if (level == null)
            return;
        Session session = level.Session;
        Vector2 spawn = level.Session.RespawnPoint ?? level.GetSpawnPoint(level.Session.LevelData.Spawns[0]);
        string flagName = $"{flagPrefix}{(int)spawn.X}_{(int)spawn.Y}";
        session.SetFlag(flagName, true);
        List<string> flagsToUnset = [.. session.Flags.Where(f => f.StartsWith(flagPrefix))];
        foreach (string flag in flagsToUnset)
        {
            if (flag != flagName)
                session.SetFlag(flag, false);
        }
    }
}