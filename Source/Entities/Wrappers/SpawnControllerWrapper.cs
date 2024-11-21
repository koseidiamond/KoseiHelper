using Celeste.Mod.Entities;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;


[CustomEntity("KoseiHelper/SpawnControllerWrapper")]
// A wrapper class to store both the entity and its timeToLive value
public class EntityWithTTL
{
    public Entity Entity { get; set; }
    public float TimeToLive { get; set; }

    public EntityWithTTL(Entity entity, float ttl)
    {
        Entity = entity;
        TimeToLive = ttl;
    }
}