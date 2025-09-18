using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/EntitySpawnArea")]
[Tracked]
public class EntitySpawnArea : Entity
{
    public Color color;
    public EntitySpawnArea(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Collider = new Hitbox(data.Width, data.Height);
        color = data.HexColor("color", Color.BurlyWood);
    }
}