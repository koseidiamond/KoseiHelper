using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/EntityResizer")]
[Tracked]
// Note: A lot of this code has been recycled from the Entity Tinters
public class EntityResizer : Entity
{
    private readonly List<Type> affectedTypes = new();
    private readonly HashSet<int> affectedIDs = new();
    private bool allEntities;
    private bool counter;
    private bool sliderMode;
    private bool absoluteValue;
    private string sliderCounterName;
    private float sliderCounterMinValue, sliderCounterMaxValue;
    private string flag;
    private bool onlyOnce;
    private bool resized;
    private float scale, maxScale;
    private bool everyFrame;
    private readonly Dictionary<Sprite, Vector2> originalSpriteScales = new();
    private readonly Dictionary<Sprite, Vector2> originalSpritePositions = new();
    private readonly Dictionary<Image, Vector2> originalImageScales = new();
    private readonly Dictionary<Image, Vector2> originalImagePositions = new();
    private readonly Dictionary<Hitbox, (float Width, float Height, float Left, float Top)> originalHitboxes = new();
    private readonly Dictionary<Circle, (float Radius, Vector2 Position)> originalCircles = new();
    public EntityResizer(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        allEntities = data.Bool("allEntities", true);
        scale = data.Float("scale", 1f);
        maxScale = data.Float("maxScale", 1f);
        everyFrame = data.Bool("everyFrame", false);
        flag = data.Attr("flag", "");
        if (data.Bool("TransitionUpdate"))
            base.AddTag(Tags.TransitionUpdate);
        if (data.Bool("Global"))
            base.AddTag(Tags.Global);
        onlyOnce = data.Bool("onlyOnce", false);
        // data for the slider placement
        counter = data.Bool("counter", false);
        absoluteValue = data.Bool("absoluteValue", false);
        sliderMode = data.Bool("sliderMode", false);
        sliderCounterName = data.Attr("sliderCounterName", "");
        sliderCounterMinValue = data.Float("sliderCounterMinValue", 0f);
        sliderCounterMaxValue = data.Float("sliderCounterMaxValue", 10f);
        // parsing lists
        foreach (string path in data.Attr("affectedEntities", "Celeste.Bumper").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            Type type = KoseiHelperUtils.GetTypeFromString(path);
            if (type != null)
                affectedTypes.Add(type);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Couldn't find type '{path}'.");
        }
        foreach (string id in data.Attr("entityIDs", "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (int.TryParse(id, out int parsed))
                affectedIDs.Add(parsed);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Invalid entity ID '{id}'.");
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = scene as Level;
        if (level != null && KoseiHelperUtils.CheckFlag(level, flag))
            TryApplyCustomization();
        else if (level != null && !string.IsNullOrEmpty(flag))
            RestoreCustomization();
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (level != null)
        {
            if (KoseiHelperUtils.CheckFlag(level, flag))
            {
                if (everyFrame || !resized)
                    TryApplyCustomization();
                resized = true;
            }
            else if (!string.IsNullOrEmpty(flag) && resized)
            {
                RestoreCustomization(); // restores original scale when the flag unmatches again
                resized = false; // to ensure that it doesn't remove the scale on every frame unnecessarily
            }
        }
    }

    private void TryApplyCustomization()
    {
        Level level = SceneAs<Level>();
        if (level == null)
            return;
        float currentScale = GetCurrentScale(level);
        Entity closestEntity = null;
        float closestDistanceSq = float.MaxValue;

        foreach (Entity entity in level.Entities)
        {
            if (!affectedTypes.Any(t => t.IsInstanceOfType(entity)))
                continue; // filters by entity type
            if (affectedIDs.Count > 0 && !affectedIDs.Contains(entity.SourceId.ID))
                continue; // filters by entity id

            if (allEntities || affectedIDs.Count > 1)
                ResizeEntity(entity, currentScale); // resizes entities if multiple IDs are specified regardless of allEntities
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
            ResizeEntity(closestEntity, currentScale);
    }

    private void ResizeEntity(Entity entity, float scale)
    {
        foreach (Component component in entity.Components)
        {
            switch (component)
            {
                case Sprite sprite:
                    {
                        if (!originalSpriteScales.TryGetValue(sprite, out Vector2 baseScale))
                        {
                            baseScale = sprite.Scale;
                            originalSpriteScales[sprite] = baseScale;
                        }
                        if (!originalSpritePositions.TryGetValue(sprite, out Vector2 basePosition))
                        {
                            basePosition = sprite.Position;
                            originalSpritePositions[sprite] = basePosition;
                        }
                        sprite.Scale = baseScale * scale;
                        sprite.Position = basePosition * scale;
                        break;
                    }
                case Image image:
                    {
                        if (!originalImageScales.TryGetValue(image, out Vector2 baseScale))
                        {
                            baseScale = image.Scale;
                            originalImageScales[image] = baseScale;
                        }
                        if (!originalImagePositions.TryGetValue(image, out Vector2 basePosition))
                        {
                            basePosition = image.Position;
                            originalImagePositions[image] = basePosition;
                        }
                        image.Scale = baseScale * scale;
                        image.Position = basePosition * scale;
                        break;
                    }
                case PlayerCollider playerCollider:
                    ResizeCollider(playerCollider.Collider, scale);
                    break;
            }
        }
        if (entity.Collider != null)
            ResizeCollider(entity.Collider, scale);
        if (onlyOnce)
            RemoveSelf();
    }

    private void ResizeCollider(Collider collider, float scale)
    {
        switch (collider)
        {
            case Hitbox hitbox:
                if (!originalHitboxes.ContainsKey(hitbox))
                {
                    originalHitboxes[hitbox] = (hitbox.Width, hitbox.Height, hitbox.Left, hitbox.Top);
                }
                var original = originalHitboxes[hitbox];
                hitbox.Width = original.Width * scale;
                hitbox.Height = original.Height * scale;
                hitbox.Left = original.Left * scale;
                hitbox.Top = original.Top * scale;
                break;

            case ColliderList list:
                foreach (Collider colliderListed in list.colliders)
                {
                    if (colliderListed != null)
                        ResizeCollider(colliderListed, scale);
                }
                break;
            case Circle circle:
                if (!originalCircles.ContainsKey(circle))
                {
                    originalCircles[circle] = (circle.Radius, circle.Position);
                }
                var originalCircle = originalCircles[circle];
                circle.Radius = originalCircle.Radius * scale;
                circle.Position = originalCircle.Position * scale;
                break;
        }
    }

    private float GetCurrentScale(Level level)
    {
        if (!sliderMode)
            return scale;
        float value = counter ? level.Session.GetCounter(sliderCounterName) : level.Session.GetSlider(sliderCounterName);
        if (absoluteValue)
            value = Math.Abs(value);
        if (sliderCounterMaxValue - sliderCounterMinValue == 0f)
            return maxScale;
        float normalized = MathHelper.Clamp((value - sliderCounterMinValue) / (sliderCounterMaxValue - sliderCounterMinValue), 0f, 1f);
        return MathHelper.Lerp(scale, maxScale, normalized);
    }

    private void RestoreCustomization()
    {
        foreach (var pair in originalSpritePositions)
        {
            if (pair.Key.Entity != null)
                pair.Key.Position = pair.Value;
        }
        originalSpritePositions.Clear();
        originalSpriteScales.Clear();
        foreach (var pair in originalImageScales)
        {
            if (pair.Key.Entity != null)
                pair.Key.Scale = pair.Value;
        }
        originalImageScales.Clear();
        foreach (var pair in originalImagePositions)
        {
            if (pair.Key.Entity != null)
                pair.Key.Position = pair.Value;
        }

        originalImagePositions.Clear();

        foreach (var pair in originalHitboxes)
        {
            Hitbox hitbox = pair.Key;

            if (hitbox.Entity != null)
            {
                hitbox.Width = pair.Value.Width;
                hitbox.Height = pair.Value.Height;
                hitbox.Left = pair.Value.Left;
                hitbox.Top = pair.Value.Top;
            }
        }
        foreach (var pair in originalCircles)
        {
            if (pair.Key.Entity != null)
            {
                pair.Key.Radius = pair.Value.Radius;
                pair.Key.Position = pair.Value.Position;
            }
        }
        originalCircles.Clear();
        originalHitboxes.Clear();
    }
}