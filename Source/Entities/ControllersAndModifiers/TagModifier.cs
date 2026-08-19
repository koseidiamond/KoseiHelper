using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/TagModifier")]
[Tracked]
public class TagModifier : Entity
{
    private readonly List<Type> affectedTypes = new();
    private readonly HashSet<int> affectedIDs = new();
    private bool allEntities;
    private string flag;
    private bool addFrozenUpdate, addGlobal, addHUD, addPauseUpdate, addPersistent, addTransitionUpdate, addSubHUD;
    private bool removeFrozenUpdate, removeGlobal, removeHUD, removePauseUpdate, removePersistent, removeTransitionUpdate, removeSubHUD;
    private bool previousFlagState;

    public TagModifier(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        allEntities = data.Bool("allEntities", true);
        flag = data.Attr("flag", "");
        addFrozenUpdate = data.Bool("addFrozenUpdate", false);
        addGlobal = data.Bool("addGlobal", false);
        addHUD = data.Bool("addHUD", false);
        addPauseUpdate = data.Bool("addPauseUpdate", false);
        addPersistent = data.Bool("addPersistent", false);
        addTransitionUpdate = data.Bool("addTransitionUpdate", false);
        addSubHUD = data.Bool("addSubHUD", false);
        removeFrozenUpdate = data.Bool("removeFrozenUpdate", false);
        removeGlobal = data.Bool("removeGlobal", false);
        removeHUD = data.Bool("removeHUD", false);
        removePauseUpdate = data.Bool("removePauseUpdate", false);
        removePersistent = data.Bool("removePersistent", false);
        removeTransitionUpdate = data.Bool("removeTransitionUpdate", false);
        removeSubHUD = data.Bool("removeSubHUD", false);

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
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = scene as Level;
        if (level == null)
            return;

        previousFlagState = KoseiHelperUtils.CheckFlag(level, flag);
        if (previousFlagState)
            TryApplyTag();
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (level == null)
            return;

        bool flagState = KoseiHelperUtils.CheckFlag(level, flag);
        if (flagState == previousFlagState)
            return;
        previousFlagState = flagState;
        if (flagState)
            TryApplyTag();
    }

    private void TryApplyTag()
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
                ModifyEntityTags(entity); // modifies tags of multiple entities if multiple IDs are specified regardless of allEntities
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
            ModifyEntityTags(closestEntity);
    }

    private void ModifyEntityTags(Entity entity)
    {
        if (addFrozenUpdate)
        {
            if (removeFrozenUpdate)
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add and remove the tag FrozenUpdate to {entity.GetType()} at the same time! The tag will not be modified.");
            }
            else if (!entity.TagCheck(Tags.FrozenUpdate))
            {
                entity.AddTag(Tags.FrozenUpdate);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add the tag FrozenUpdate to {entity.GetType()}, but the tag already exists!");
            }
        }
        if (addGlobal)
        {
            if (removeGlobal)
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add and remove the tag Global to {entity.GetType()} at the same time! The tag will not be modified.");
            }
            else if (!entity.TagCheck(Tags.Global))
            {
                entity.AddTag(Tags.Global);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add the tag Global to {entity.GetType()}, but the tag already exists!");
            }
        }
        if (addHUD)
        {
            if (removeHUD)
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add and remove the tag HUD to {entity.GetType()} at the same time! The tag will not be modified.");
            }
            else if (!entity.TagCheck(Tags.HUD))
            {
                entity.AddTag(Tags.HUD);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add the tag HUD to {entity.GetType()}, but the tag already exists!");
            }
        }
        if (addPauseUpdate)
        {
            if (removePauseUpdate)
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add and remove the tag PauseUpdate to {entity.GetType()} at the same time! The tag will not be modified.");
            }
            else if (!entity.TagCheck(Tags.PauseUpdate))
            {
                entity.AddTag(Tags.PauseUpdate);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add the tag PauseUpdate to {entity.GetType()}, but the tag already exists!");
            }
        }
        if (addPersistent)
        {
            if (removePersistent)
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add and remove the tag Persistent to {entity.GetType()} at the same time! The tag will not be modified.");
            }
            else if (!entity.TagCheck(Tags.Persistent))
            {
                entity.AddTag(Tags.Persistent);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add the tag Persistent to {entity.GetType()}, but the tag already exists!");
            }
        }
        if (addTransitionUpdate)
        {
            if (removeTransitionUpdate)
            {
                Logger.Log(
                    LogLevel.Warn,
                    "KoseiHelper",
                    $"The Tag Modifier tried to add and remove the tag TransitionUpdate to {entity.GetType()} at the same time! The tag will not be modified."
                );
            }
            else if (!entity.TagCheck(Tags.TransitionUpdate))
            {
                entity.AddTag(Tags.TransitionUpdate);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add the tag TransitionUpdate to {entity.GetType()}, but the tag already exists!");
            }
        }
        if (addSubHUD)
        {
            if (removeSubHUD)
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add and remove the tag SubHUD to {entity.GetType()} at the same time! The tag will not be modified.");
            }
            else if (!entity.TagCheck(TagsExt.SubHUD))
            {
                entity.AddTag(TagsExt.SubHUD);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to add the tag SubHUD to {entity.GetType()}, but the tag already exists!");
            }
        }

        if (removeFrozenUpdate && !addFrozenUpdate)
        {
            if (entity.TagCheck(Tags.FrozenUpdate))
                entity.RemoveTag(Tags.FrozenUpdate);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to remove the tag FrozenUpdate from {entity.GetType()}, but the tag does not exist!");
        }
        if (removeGlobal && !addGlobal)
        {
            if (entity.TagCheck(Tags.Global))
                entity.RemoveTag(Tags.Global);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to remove the tag Global from {entity.GetType()}, but the tag does not exist!");
        }
        if (removeHUD && !addHUD)
        {
            if (entity.TagCheck(Tags.HUD))
                entity.RemoveTag(Tags.HUD);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to remove the tag HUD from {entity.GetType()}, but the tag does not exist!");
        }
        if (removePauseUpdate && !addPauseUpdate)
        {
            if (entity.TagCheck(Tags.PauseUpdate))
                entity.RemoveTag(Tags.PauseUpdate);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to remove the tag PauseUpdate from {entity.GetType()}, but the tag does not exist!");
        }
        if (removePersistent && !addPersistent)
        {
            if (entity.TagCheck(Tags.Persistent))
                entity.RemoveTag(Tags.Persistent);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to remove the tag Persistent from {entity.GetType()}, but the tag does not exist!");
        }
        if (removeTransitionUpdate && !addTransitionUpdate)
        {
            if (entity.TagCheck(Tags.TransitionUpdate))
                entity.RemoveTag(Tags.TransitionUpdate);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to remove the tag TransitionUpdate from {entity.GetType()}, but the tag does not exist!");
        }
        if (removeSubHUD && !addSubHUD)
        {
            if (entity.TagCheck(TagsExt.SubHUD))
                entity.RemoveTag(TagsExt.SubHUD);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"The Tag Modifier tried to remove the tag SubHUD from {entity.GetType()}, but the tag does not exist!");
        }
    }
}