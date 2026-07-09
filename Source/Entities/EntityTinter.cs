using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/EntityTinter")]
[Tracked]
public class EntityTinter : Entity
{
    private readonly List<Type> affectedTypes = new();
    private readonly HashSet<int> affectedIDs = new();
    private Color tint;
    private bool allEntities;
    private bool everyFrame;
    private bool red, green, blue, alpha;
    private bool affectSprite, affectImage;
    private readonly List<string> animationIDs = new();
    private readonly Dictionary<Sprite, Color> originalSpriteColors = new();
    private bool untintIfAnimChanged;
    public EntityTinter(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        everyFrame = data.Bool("everyFrame", true);
        allEntities = data.Bool("allEntities", true);
        red = data.Bool("red", true);
        green = data.Bool("green", true);
        blue = data.Bool("blue", true);
        alpha = data.Bool("alpha", true);
        affectSprite = data.Bool("sprite", true);
        affectImage = data.Bool("image", true);
        untintIfAnimChanged = data.Bool("untintIfAnimChanged", true);
        foreach (string path in data.Attr("affectedEntities", "Celeste.Glider")
             .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            Type type = FakeAssembly.GetFakeEntryAssembly().GetType(path);
            if (type != null)
                affectedTypes.Add(type);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Couldn't find type '{path}'.");
        }
        foreach (string id in data.Attr("entityIDs", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(id, out int parsed))
                affectedIDs.Add(parsed);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Invalid entity ID '{id}'.");
        }
        foreach (string anim in data.Attr("animationIDs", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            animationIDs.Add(anim);
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

        Entity closestEntity = null;
        float closestDistanceSq = float.MaxValue;

        foreach (Entity entity in level.Entities)
        {
            if (!affectedTypes.Any(t => t.IsInstanceOfType(entity)))
                continue; // filters by entity type

            if (affectedIDs.Count > 0 && !affectedIDs.Contains(entity.SourceId.ID))
                continue; // filters by entity id

            if (allEntities || affectedIDs.Count > 1)
                TintEntity(entity); // tints entities if multiple IDs are specified regardless of allEntities
            else
            {
                float dist = Vector2.DistanceSquared(Position, entity.Position);

                if (dist < closestDistanceSq)
                {
                    closestDistanceSq = dist;
                    closestEntity = entity;
                }
            }
        }

        if (!allEntities && closestEntity != null)
            TintEntity(closestEntity);
    }

    private void TintEntity(Entity entity)
    {
        foreach (Component component in entity.Components)
        {
            switch (component)
            {
                case Sprite sprite:
                    if (!affectSprite)
                        break;

                    if (animationIDs.Count == 0 || animationIDs.Contains(sprite.CurrentAnimationID))
                    {
                        if (!originalSpriteColors.ContainsKey(sprite))
                            originalSpriteColors[sprite] = sprite.Color;

                        Color color = sprite.Color;

                        if (red)
                            color.R = tint.R;
                        if (green)
                            color.G = tint.G;
                        if (blue)
                            color.B = tint.B;
                        if (alpha)
                            color.A = tint.A;

                        sprite.Color = color;
                    }
                    else if (untintIfAnimChanged && originalSpriteColors.TryGetValue(sprite, out Color original))
                    {
                        sprite.Color = original;
                        originalSpriteColors.Remove(sprite);
                    }
                    break;

                case Image image:
                    if (affectImage)
                    {
                        if (red)
                            image.Color.R = tint.R;
                        if (green)
                            image.Color.G = tint.G;
                        if (blue)
                            image.Color.B = tint.B;
                        if (alpha)
                            image.Color.A = tint.A;
                    }
                    break;
            }
        }
    }
}