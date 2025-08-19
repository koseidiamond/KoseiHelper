using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Triggers;

[CustomEntity("KoseiHelper/KillIfNotGroundedTrigger")]
[Tracked]
public class KillIfNotGroundedTrigger : Trigger
{
    public bool spareIfClimbing;
    public bool killIfGrounded;
    public float cooldown, originalCooldown;
    public int group;
    public KillIfNotGroundedTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        spareIfClimbing = data.Bool("spareIfClimbing", false);
        killIfGrounded = data.Bool("killIfGrounded", false);
        cooldown = originalCooldown = data.Float("delay", 0.01f);
        group = data.Int("group", 0); // Several triggers can share the cooldown if the group is not 0
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        Level level = SceneAs<Level>();
        if (player != null)
        {
            // If death conditions apply
            if (((!player.onGround && !killIfGrounded) || (player.onGround && killIfGrounded)) && (!spareIfClimbing || spareIfClimbing && player.StateMachine.state != 1))
            {
                if (cooldown > 0)
                {
                    cooldown -= Engine.DeltaTime;
                    if (group != 0)
                    {
                        foreach (KillIfNotGroundedTrigger killTrigger in level.Tracker.GetEntities<KillIfNotGroundedTrigger>())
                        {
                            if (killTrigger.group == group)
                                killTrigger.cooldown = cooldown;
                        }
                    }
                }
                if (cooldown <= 0)
                    KillPlayer(player);
            }
            else // If death conditions don't apply, we reset the cooldown(s)
            {
                cooldown = originalCooldown;
                if (group != 0)
                {
                    foreach (KillIfNotGroundedTrigger killTrigger in level.Tracker.GetEntities<KillIfNotGroundedTrigger>())
                    {
                        if (killTrigger.group == group)
                            killTrigger.cooldown = cooldown;
                    }
                }
            }
        }
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Session session = level.Session;
        session.SetSlider("KoseiHelper_KillIfGrounded_cooldown", cooldown);
    }

    public override void OnLeave(Player player) // todo group compat
    {
        base.OnLeave(player);
        Level level = SceneAs<Level>();
        if (group != 0)
        {
            foreach (KillIfNotGroundedTrigger killTrigger in level.Tracker.GetEntities<KillIfNotGroundedTrigger>())
            {
                if (killTrigger != this && killTrigger.group == group && killTrigger.PlayerIsInside)
                    return;
                else
                    killTrigger.cooldown = originalCooldown;
            }
        }
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