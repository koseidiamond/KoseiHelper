using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/KillIfNotGroundedTrigger")]
public class KillIfNotGroundedTrigger : Trigger
{
    public bool spareIfClimbing;
    public bool killIfGrounded;
    public float cooldown, originalCooldown;
    public KillIfNotGroundedTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        spareIfClimbing = data.Bool("spareIfClimbing", false);
        killIfGrounded = data.Bool("killIfGrounded", false);
        cooldown = originalCooldown = data.Float("delay", 0.01f);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (player != null)
        {
            if (((!player.onGround && !killIfGrounded) || (player.onGround && killIfGrounded)) && (!spareIfClimbing || spareIfClimbing && player.StateMachine.state != 1))
            {
                if (cooldown > 0)
                    cooldown -= Engine.DeltaTime;
                if (cooldown <= 0)
                    KillPlayer(player);
            }
            else
                cooldown = originalCooldown;
        }
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        cooldown = originalCooldown;
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