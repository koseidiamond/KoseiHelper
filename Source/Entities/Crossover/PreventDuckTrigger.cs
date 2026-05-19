using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/PreventDuckTrigger")]
public class PreventDuckTrigger : Trigger
{
    private enum TriggerMode
    {
        OnEnter,
        OnLeave,
        OnStay
    };
    private readonly TriggerMode mode;
    private readonly string flag;

    public PreventDuckTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        flag = data.Attr("flag", "");
        mode = data.Enum("mode", TriggerMode.OnStay);
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if ((string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)) && mode == TriggerMode.OnEnter)
        {
            player.Ducking = false;
            player.wasDucking = false;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        Level level = SceneAs<Level>();
        if ((string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)) && mode == TriggerMode.OnLeave)
        {
            player.Ducking = false;
            player.wasDucking = false;
        }
    }

    public override void OnStay(Player player)
    {
        base.Update();
        Level level = SceneAs<Level>();
        if ((string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)) && mode == TriggerMode.OnStay)
        {
            player.Ducking = false;
            player.wasDucking = false;
            if (player.onGround && (float)Input.MoveY == 1f && player.Speed.Y >= 0f)
                player.Sprite.Scale = Vector2.One;
            // todo make sure the stupid "duck" sprite doesn't happen
        }
    }
}