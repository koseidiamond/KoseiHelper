using Celeste.Mod.Entities;
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
    private Color tint, maxColor;
    private bool allEntities;
    private bool everyFrame;
    private bool alpha;
    private bool affectSprite, affectImage, affectTiles;
    private readonly List<string> animationIDs = new();
    private readonly Dictionary<Sprite, Color> originalSpriteColors = new();
    private readonly Dictionary<Image, Color> originalImageColors = new();
    private readonly Dictionary<TileGrid, Color> originalTilegridColors = new();
    private bool untintIfAnimChanged;
    private bool counter;
    private bool sliderMode;
    private bool absoluteValue;
    private string sliderCounterName;
    private float sliderCounterMinValue, sliderCounterMaxValue;
    private string flag;
    private bool onlyOnce;
    private bool tintApplied;

    public EntityTinter(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        everyFrame = data.Bool("everyFrame", true);
        allEntities = data.Bool("allEntities", true);
        alpha = data.Bool("alpha", true);
        affectSprite = data.Bool("sprite", true);
        affectImage = data.Bool("image", true);
        affectTiles = data.Bool("tiles", true);
        untintIfAnimChanged = data.Bool("untintIfAnimChanged", true);
        flag = data.Attr("flag", "");
        tint = KoseiHelperUtils.ParseHexColor(
            data.Values.TryGetValue("tint", out object tintColor) ? tintColor.ToString() : null, Calc.HexToColor("FFFFFF"));
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
        maxColor = KoseiHelperUtils.ParseHexColor(
            data.Values.TryGetValue("maxColor", out object tintColor2) ? tintColor2.ToString() : null, Calc.HexToColor("FF0000"));

        // parsing lists
        foreach (string path in data.Attr("affectedEntities", "Celeste.Glider").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
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
        foreach (string anim in data.Attr("animationIDs", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            animationIDs.Add(anim);
        }
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = scene as Level;
        if (KoseiHelperUtils.CheckFlag(level, flag))
            TryApplyCustomization();
        else if (!string.IsNullOrEmpty(flag))
            RestoreCustomization();
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
        if (everyFrame && level != null)
        {
            if (KoseiHelperUtils.CheckFlag(level, flag))
            {
                TryApplyCustomization();
                tintApplied = true;
            }
            else if (!string.IsNullOrEmpty(flag) && tintApplied)
            {
                RestoreCustomization(); // restores original colors when the flag unmatches again
                tintApplied = false; // to ensure that it doesn't remove the tint on every frame unnecessarily
            }
        }
    }

    private void TryApplyCustomization()
    {
        Level level = SceneAs<Level>();
        if (level == null)
            return;
        Color currentTint = GetCurrentTint(level);
        Entity closestEntity = null;
        float closestDistanceSq = float.MaxValue;

        foreach (Entity entity in level.Entities)
        {
            if (!affectedTypes.Any(t => t.IsInstanceOfType(entity)))
                continue; // filters by entity type
            if (affectedIDs.Count > 0 && !affectedIDs.Contains(entity.SourceId.ID))
                continue; // filters by entity id

            if (allEntities || affectedIDs.Count > 1)
                TintEntity(entity, currentTint); // tints entities if multiple IDs are specified regardless of allEntities
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
            TintEntity(closestEntity, currentTint);
    }

    private void TintEntity(Entity entity, Color tintColor)
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

                        Color spriteColor = sprite.Color;

                        spriteColor.R = tintColor.R;
                        spriteColor.G = tintColor.G;
                        spriteColor.B = tintColor.B;
                        if (alpha)
                            spriteColor.A = tintColor.A;
                        sprite.Color = spriteColor;
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
                        if (!originalImageColors.ContainsKey(image))
                            originalImageColors[image] = image.Color;
                        Color imageColor = image.Color;
                        imageColor.R = tintColor.R;
                        imageColor.G = tintColor.G;
                        imageColor.B = tintColor.B;
                        if (alpha)
                            imageColor.A = tintColor.A;
                        image.Color = imageColor;
                    }
                    break;

                case TileGrid tg:
                    if (affectTiles)
                    {
                        if (!originalTilegridColors.ContainsKey(tg))
                            originalTilegridColors[tg] = tg.Color;
                        Color tgColor = tg.Color;

                        tgColor.R = tintColor.R;
                        tgColor.G = tintColor.G;
                        tgColor.B = tintColor.B;
                        if (alpha)
                            tgColor.A = tintColor.A;
                        tg.Color = tgColor;
                    }
                    break;
            }
        }
        if (onlyOnce)
            RemoveSelf();
    }

    private Color GetCurrentTint(Level level)
    {
        if (!sliderMode)
            return tint;
        float value = counter ? level.Session.GetCounter(sliderCounterName) : level.Session.GetSlider(sliderCounterName);
        if (sliderCounterMaxValue - sliderCounterMinValue == 0f) // don't divide by 0
            return maxColor;
        if (absoluteValue)
            value = Math.Abs(value);
        float normalized = MathHelper.Clamp((value - sliderCounterMinValue) / (sliderCounterMaxValue - sliderCounterMinValue), 0f, 1f);
        return Color.Lerp(tint, maxColor, normalized);
    }

    private void RestoreCustomization()
    {
        foreach (var pair in originalSpriteColors)
        {
            if (pair.Key.Entity != null)
                pair.Key.Color = pair.Value;
        }

        originalSpriteColors.Clear();

        foreach (var pair in originalImageColors)
        {
            if (pair.Key.Entity != null)
                pair.Key.Color = pair.Value;
        }

        originalImageColors.Clear();
    }
}