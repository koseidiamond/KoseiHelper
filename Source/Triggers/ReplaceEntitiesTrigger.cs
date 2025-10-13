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
        public Vector2[] nodes;
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
            nodes = data.NodesOffset(offset);

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
        { // kinda copypasted from SpawnController i should turn this into an Utils method or something (this one supports nodes and has a cool log tho)
            EntityData entityData = new EntityData
            {
                Position = position,
                Level = level.Session.LevelData,
                Values = new Dictionary<string, object>()
            };

            if (nodes.Length > 0)
            {
                entityData.Nodes = nodes.Select(node => entityData.Position + (node - Center)).ToArray();
            }

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
                List<string> expectedParams = new List<string>();
                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo param = parameters[i];
                    expectedParams.Add($"{param.Name} ({param.ParameterType.Name})");

                    object paramValue = null;
                    if (param.ParameterType == typeof(EntityData))
                    {
                        paramValue = entityData;
                    }
                    else if (param.ParameterType == typeof(Vector2[])) // we'll, uhhh, assume this is a node
                    {
                        paramValue = entityData.Nodes;
                    }
                    else if (param.ParameterType == typeof(Vector2))
                    {
                        paramValue = position;
                    }
                    else if (param.ParameterType == typeof(bool))
                    {
                        bool boolValue = entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value is string boolVal && bool.TryParse(boolVal, out bool parsedBool) ? parsedBool : false;
                        paramValue = boolValue;
                    }
                    else if (param.ParameterType.IsEnum)
                    {
                        object enumParsed = Enum.TryParse(param.ParameterType, entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value?.ToString(), out object enumValue) ? enumValue : Enum.GetValues(param.ParameterType).GetValue(0);
                        paramValue = enumParsed;
                    }
                    else if (param.ParameterType == typeof(int))
                    {
                        int intValue = int.TryParse(entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value?.ToString(), out int parsedInt) ? parsedInt : 0;
                        paramValue = intValue;
                    }
                    else if (param.ParameterType == typeof(float))
                    {
                        float floatValue = float.TryParse(entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value?.ToString(), out float parsedFloat) ? parsedFloat : 0f;
                        paramValue = floatValue;
                    }
                    else if (param.ParameterType == typeof(string))
                    {
                        string stringValue = entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value as string ?? "";
                        paramValue = stringValue;
                    }
                    else
                    {
                        paramValue = param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null;
                    }
                    ctorParams.Add(paramValue);
                }
                try
                {
                    return (Entity)ctor.Invoke(ctorParams.ToArray());
                }
                catch (Exception)
                {
                    Logger.Log(LogLevel.Error, "KoseiHelper", $"Failed to replace entity!\nExpected arguments: {string.Join(", ", expectedParams)}\n" +
                        $"Arguments: {string.Join(", ", ctorParams.Select((param, index) => $"{expectedParams[index].Split(' ')[0]}={param}"))}");

                }
            }
            return null;
        }
    }
}