using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/RestartGameTrigger")]
public class RestartGameTrigger : Trigger
{
    public RestartGameTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Everest.QuickFullRestart();
        RemoveSelf();
    }
}