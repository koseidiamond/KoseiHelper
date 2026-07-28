using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/KillTheoOnTouchController")]
[Tracked]
public class KillTheoOnTouchController : Entity
{
    private List<Type> dangerousEntities = new();
    public bool dieOnSolids;

    public KillTheoOnTouchController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        dieOnSolids = data.Bool("dieOnSolids", false);
        foreach (string path in data.Attr("affectedEntities", "Celeste.CrystalStaticSpinner")
             .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            Type type = FakeAssembly.GetFakeEntryAssembly().GetType(path);
            if (type != null)
                dangerousEntities.Add(type);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Couldn't find type '{path}'.");
        }
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        foreach (TheoCrystal theo in level.Tracker.GetEntities<TheoCrystal>())
        {
            Rectangle theoBounds = theo.Collider.Bounds;
            bool inSafeArea = level.Tracker.GetEntities<SafeTheoArea>().Any(area => area.CollideRect(theoBounds));
            if (dieOnSolids && !inSafeArea)
            {
                theoBounds.Inflate(1, 1);
                foreach (Solid solid in level.Tracker.GetEntities<Solid>())
                {
                    if (solid.CollideRect(theoBounds))
                    {
                        theo.Die();
                        break;
                    }
                }
            }
            foreach (Entity entity in level.Entities)
            {
                if (!dangerousEntities.Any(t => t.IsInstanceOfType(entity)))
                    continue;
                if (theo.CollideCheck(entity) && !inSafeArea)
                {
                    theo.Die();
                    break;
                }
            }
        }
    }
}

[CustomEntity("KoseiHelper/SafeTheoArea")]
[Tracked]
public class SafeTheoArea : Entity
{
    public SafeTheoArea(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Collider = new Hitbox(data.Width, data.Height);
    }
}