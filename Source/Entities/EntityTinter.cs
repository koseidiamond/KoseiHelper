using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/EntityTinter")]
[Tracked]
public class EntityTinter : Entity
{
    private readonly List<Type> affectedTypes = new();
    private Color tint;
    private bool allEntities;
    private bool everyFrame;
    public EntityTinter(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        everyFrame = data.Bool("everyFrame", false);
        allEntities = data.Bool("allEntities", false);
        foreach (string path in data.Attr("affectedEntities", "Celeste.Glider")
             .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            Type type = FakeAssembly.GetFakeEntryAssembly().GetType(path);
            if (type != null)
                affectedTypes.Add(type);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Couldn't find type '{path}'.");
        }
        tint = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("tint", out object tintColor) ? tintColor.ToString() : null, Calc.HexToColor("FFFFFF"));
        if (data.Bool("TransitionUpdate"))
            base.Tag = Tags.TransitionUpdate;
        if (data.Bool("Global"))
            base.Tag = Tags.Global;

    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        TryApplyCustomization();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (everyFrame)
        TryApplyCustomization();
    }

    private void TryApplyCustomization()
    {
        Level level = SceneAs<Level>();
        if (level == null)
            return;

        Entity closest = null;
        float closestDistanceSq = float.MaxValue;

        foreach (Entity entity in level.Entities)
        {
            if (!affectedTypes.Any(t => t.IsInstanceOfType(entity)))
                continue;

            if (allEntities)
            {
                TintEntity(entity);
            }
            else
            {
                float dist = Vector2.DistanceSquared(Position, entity.Position);

                if (dist < closestDistanceSq)
                {
                    closestDistanceSq = dist;
                    closest = entity;
                }
            }
        }

        if (!allEntities && closest != null)
            TintEntity(closest);
    }

    private void TintEntity(Entity entity)
    {
        foreach (Component component in entity.Components)
        {
            switch (component)
            {
                case Sprite sprite:
                    sprite.Color = tint;
                    break;

                case Image image:
                    image.Color = tint;
                    break;
            }
        }
    }
}