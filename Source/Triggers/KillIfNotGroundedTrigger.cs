using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using static Celeste.TrackSpinner;
using static MonoMod.InlineRT.MonoModRule;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/KillIfNotGroundedTrigger")]
public class KillIfNotGroundedTrigger : Trigger
{
    public bool spareIfClimbing;
    public KillIfNotGroundedTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        spareIfClimbing = data.Bool("spareIfClimbing", false);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (player != null)
            if (!player.onGround)
                if (!spareIfClimbing || spareIfClimbing && player.StateMachine.state != 1)
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
}