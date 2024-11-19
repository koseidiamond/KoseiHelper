using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/SpawnController")]
public class SpawnController : Entity
{

    private Level level;
    private Player player;
    public int offsetY;
    public int offsetX;
    public string entityName;
    public string entityData;
    public bool removeDash;
    public SpawnController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        offsetX = data.Int("offsetX", 0);
        offsetY = data.Int("offsetY", 8);
        entityName = data.Attr("entityName");
        entityData = data.Attr("entityData");
        removeDash = data.Bool("removeDash", true);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();


    }

    public override void Update()
    {
        base.Update();
        player = Scene.Tracker.GetEntity<Player>();
        if (KoseiHelperModule.Settings.SpawnButton.Pressed)
        {
            if (removeDash)
                player.Dashes -= 1;
            var parsedAttributes = ParseEntityData(entityData);
            if (!string.IsNullOrEmpty(entityName))
            {
                var spawnPosition = new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY);
                Type entityType = Type.GetType(entityName);
                    //var constructor = entityType.GetConstructor(new Type[] { typeof(Vector2), typeof(Dictionary<string, object>) });
                    //object entity = constructor.Invoke(new object[] { spawnPosition, parsedAttributes });
                    this.Scene.Add(new Puffer(new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY), false));
                    //this.Scene.Add((Entity)entity);
                Audio.Play("event:/KoseiHelper/spawn", player.Position);
            }
        }
    }

    private Dictionary<string, object> ParseEntityData(string data)
    {
        var parsedAttributes = new Dictionary<string, object>();
        if (string.IsNullOrEmpty(data))
            return parsedAttributes;

        string[] attributes = data.Split(',');

        foreach (var attribute in attributes)
        {
            var keyValue = attribute.Split('=');
            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();
                object parsedValue = ParseValue(value);
                parsedAttributes[key] = parsedValue;
            }
        }

        return parsedAttributes;
    }

    private object ParseValue(string value)
    {
        if (bool.TryParse(value, out bool boolValue))
            return boolValue;

        if (int.TryParse(value, out int intValue))
            return intValue;

        if (float.TryParse(value, out float floatValue))
            return floatValue;

        return value;
    }
}