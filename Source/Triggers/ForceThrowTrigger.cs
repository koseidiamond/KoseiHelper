using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/ForceThrowTrigger")]
public class ForceThrowTrigger : Trigger
{

    public ForceThrowTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Add(new PlayerCollider(OnPlayer));
    }

    private void OnPlayer(Player player)
    {
        if (player.Scene != null)
            player.Throw();
    }
}