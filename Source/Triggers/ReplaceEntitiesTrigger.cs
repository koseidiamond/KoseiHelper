using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.KoseiHelper.Triggers
{

    [CustomEntity("KoseiHelper/ReplaceEntitiesTrigger")]
    public class ReplaceEntitiesTrigger : Trigger
    {
        public bool onlyOnce;
        public Type fromEntityType, toEntityType;
        public List<string> dictionaryKeys, dictionaryValues;
        public string flag;
        public TriggerMode triggerMode;
        public ReplaceEntitiesTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            onlyOnce = data.Bool("onlyOnce", false);
            try
            {
                fromEntityType = FakeAssembly.GetFakeEntryAssembly().GetType(data.Attr("fromEntity", "Celeste.CrystalStaticSpinner"));
            }
            catch
            {
                Logger.Log(LogLevel.Error, "KoseiHelper", $"Failed to get entity: Requested type {fromEntityType} does not exist");
            }
            try
            {
                toEntityType = FakeAssembly.GetFakeEntryAssembly().GetType(data.Attr("toEntity", "Celeste.Booster"));
            }
            catch
            {
                Logger.Log(LogLevel.Error, "KoseiHelper", $"Failed to get entity: Requested type {toEntityType} does not exist");
            }
            dictionaryKeys = data.Attr("attributes").Replace(" ", string.Empty).Split(',').ToList();
            dictionaryValues = data.Attr("attributeValues").Split(',').ToList();

            ConstructorInfo[] fromCtors = fromEntityType.GetConstructors();
            ConstructorInfo[] toCtors = toEntityType.GetConstructors();

            Tracker.AddTypeToTracker(fromEntityType);
            Tracker.AddTypeToTracker(toEntityType);
            Tracker.Refresh();
        }
        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            Level level = SceneAs<Level>();
            if (triggerMode == TriggerMode.OnEnter && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
            {
                ReplaceEntities();
                if (onlyOnce)
                    RemoveSelf();
            }
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            Level level = SceneAs<Level>();
            if (triggerMode == TriggerMode.OnLeave && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
            {
                ReplaceEntities();
                if (onlyOnce)
                    RemoveSelf();
            }
        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            Level level = SceneAs<Level>();
            if (triggerMode == TriggerMode.OnStay && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
            {
                ReplaceEntities();
                if (onlyOnce)
                    RemoveSelf();
            }
        }

        private void ReplaceEntities()
        {
            Level level = SceneAs<Level>();
            List<Entity> toReplace = new(level.Entities);

            foreach (Entity entity in toReplace)
            {
                if (entity.GetType() != fromEntityType)
                    continue;
                Vector2 pos = entity.Position;
                level.Remove(entity);
                Entity newEntity = CreateEntityOfType(toEntityType, pos, level);
                if (newEntity != null)
                    level.Add(newEntity);
            }
        }

        private Entity CreateEntityOfType(Type entityType, Vector2 position, Level level)
        { // copypasted from SpawnController i should turn this into an Utils method or something
            EntityData entityData = new EntityData
            {
                Position = position,
                Level = level.Session.LevelData,
                Values = new Dictionary<string, object>()
            };
            for (int i = 0; i < dictionaryKeys.Count; i++)
            {
                if (i < dictionaryValues.Count)
                {
                    string key = dictionaryKeys[i].Trim();
                    string value = dictionaryValues[i].Trim();
                    entityData.Values[key] = value;
                }
            }
            ConstructorInfo[] ctors = entityType.GetConstructors();
            foreach (ConstructorInfo ctor in ctors)
            {
                List<object> ctorParams = new List<object>();
                ParameterInfo[] parameters = ctor.GetParameters();

                foreach (ParameterInfo param in parameters)
                {
                    if (param.ParameterType == typeof(EntityData))
                    {
                        ctorParams.Add(entityData);
                    }
                    else if (param.ParameterType == typeof(Vector2))
                    {
                        ctorParams.Add(position);
                    }
                    else if (param.ParameterType == typeof(bool))
                    {
                        if (entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value is string boolValue)
                            ctorParams.Add(bool.Parse(boolValue));
                        else
                            ctorParams.Add(false);
                    }
                    else if (param.ParameterType.IsEnum)
                    {
                        if (entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value is string enumValue)
                        {
                            Type enumType = param.ParameterType;
                            object enumParsed = Enum.Parse(enumType, enumValue);
                            ctorParams.Add(enumParsed);
                        }
                        else
                            ctorParams.Add(Enum.GetValues(param.ParameterType).GetValue(0));
                    }
                    else if (param.ParameterType == typeof(int))
                    {
                        if (entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value is string intValue)
                            ctorParams.Add(int.Parse(intValue));
                        else
                            ctorParams.Add(0);
                    }
                    else if (param.ParameterType == typeof(float))
                    {
                        if (entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value is string floatValue)
                            ctorParams.Add(float.Parse(floatValue));
                        else
                            ctorParams.Add(0f);
                    }
                    else if (param.ParameterType == typeof(string))
                    {
                        string stringValue = entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value as string;
                        ctorParams.Add(stringValue ?? "");
                    }
                    else
                        ctorParams.Add(param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null);
                }
                try
                {
                    return (Entity)ctor.Invoke(ctorParams.ToArray());
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, "KoseiHelper", $"Failed to instantiate entity: {ex.Message}");
                }
            }
            return null;
        }
    }
}