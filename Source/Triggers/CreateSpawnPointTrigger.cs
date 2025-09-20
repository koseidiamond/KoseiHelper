using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/CreateSpawnPointTrigger")]
public class CreateSpawnPointTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public string flag, room;
    private bool current, previous;
    public Vector2[] array;
    public Vector2 Target;
    public int lastSpawnAdded;
    public CreateSpawnPointTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnStay);
        flag = data.Attr("flag", "");
        room = data.Attr("room", "");
        array = data.NodesOffset(offset);
        if (array.Length != 0)
            Target = array[0];
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Session session = SceneAs<Level>().Session;

        if (player.Scene != null && triggerMode == TriggerMode.OnEnter)
        {
            Vector2 newSpawn = player.Position;
            AddSpawn(session, player.Position);
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        Session session = SceneAs<Level>().Session;

        if (player.Scene != null && triggerMode == TriggerMode.OnLeave)
        {
            Vector2 newSpawn = player.Position;
            AddSpawn(session, player.Position);
            if (onlyOnce)
                RemoveSelf();
        }
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Session session = SceneAs<Level>().Session;

        if (player.Scene != null && triggerMode == TriggerMode.OnStay)
        {
            current = session.GetFlag(flag);
            if (session.GetFlag(flag) && current != previous)
            {
                AddSpawn(session, player.Position);
                if (onlyOnce)
                    RemoveSelf();
            }
            if (string.IsNullOrEmpty(flag))
            {
                AddSpawn(session, player.Position, true);
                if (onlyOnce)
                    RemoveSelf();
            }
            previous = current;
        }
    }

    public void AddSpawn(Session session, Vector2 newSpawn, bool removePrevious = false)
    {
        // If the trigger has a node, we override the newSpawn position
        if (array.Length != 0)
            newSpawn = Target;
        MapData mapData = AreaData.Areas[session.Area.ID].Mode[(int)session.Area.Mode].MapData;
        if (removePrevious)
        { // Remove previously created spawn OnStay to prevent spawn spamming. Ensures at least one was created to prevent crashing
            var spawnList = mapData.Get(string.IsNullOrEmpty(room) ? session.Level : room).Spawns;
            if (lastSpawnAdded >= 0 && lastSpawnAdded < spawnList.Count)
                spawnList.RemoveAt(lastSpawnAdded);
        }
        if (string.IsNullOrEmpty(room))
            mapData.Get(session.Level).Spawns.Add(newSpawn);
        else
        {
            mapData.Get(room).Spawns.Add(newSpawn);
            mapData.Get(room).Dummy = false;
        }
        if (removePrevious) // Only need to store last spawn if we're going to remove it
            lastSpawnAdded = mapData.Get(string.IsNullOrEmpty(room) ? session.Level : room).Spawns.IndexOf(newSpawn);
        session.HitCheckpoint = true;
        session.RespawnPoint = newSpawn;
    }
}