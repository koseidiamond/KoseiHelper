using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/CreateSpawnPointTrigger")]
public class CreateSpawnPointTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public string flag;
    private bool current, previous;
    public CreateSpawnPointTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnStay);
        flag = data.Attr("flag", "");
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
            if (string.IsNullOrEmpty(flag) || session.GetFlag(flag) && current != previous)
            {
                AddSpawn(session, player.Position);
                if (onlyOnce)
                    RemoveSelf();
            }
            previous = current;
        }
    }

    public void AddSpawn(Session session, Vector2 newSpawn)
    {
        MapData mapData = AreaData.Areas[session.Area.ID].Mode[(int)session.Area.Mode].MapData;
        mapData.Get(session.Level).Spawns.Add(newSpawn);
        session.HitCheckpoint = true;
        session.RespawnPoint = newSpawn;
    }
}