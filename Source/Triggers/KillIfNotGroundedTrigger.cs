using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/KillIfNotGroundedTrigger")]
public class KillIfNotGroundedTrigger : Trigger
{
    public bool spareIfClimbing;
    public bool killIfGrounded;
    public KillIfNotGroundedTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        spareIfClimbing = data.Bool("spareIfClimbing", false);
        killIfGrounded = data.Bool("killIfGrounded", false);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (player != null)
        {
            if ((!player.onGround && !killIfGrounded) || (player.onGround && killIfGrounded))
            {
                if (!spareIfClimbing || spareIfClimbing && player.StateMachine.state != 1)
                    KillPlayer(player);
            }
        }
    }

    private void KillPlayer(Player player)
    {
        {
            DeathEffect component = new DeathEffect(Player.NormalHairColor, base.Center - base.Position + new Vector2(-16, -32))
            {
                OnEnd = delegate
                {
                    this.RemoveSelf();
                }
            };
            player.Add(component);
            player.Die(Vector2.Zero);
            RemoveSelf();
        }
    }
}