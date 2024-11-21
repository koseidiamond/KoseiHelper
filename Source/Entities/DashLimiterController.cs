using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DashLimiterController")]
public class DashLimiterController : Entity
{
    public int count;
    private float countCooldown = 0f;
    public bool demoDashOnly;

    public DashLimiterController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        count = data.Int("count", 3);
        demoDashOnly = data.Bool("demoDashOnly", false);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
    }

    public override void Update()
    {
        base.Update();
        if (Scene.Tracker.GetEntity<Player>() is not { } player) // Checks that there is a player
            return;
        if ((!demoDashOnly && player.dashCooldownTimer > 0) || // If any kind of dash and dashes, OR //TODO it looks like dash down/downleft/downright doesn't spend a dash
            (demoDashOnly && (player.demoDashed || (player.dashCooldownTimer > 0 && player.DashDir.Y < 0.7 && player.Ducking)))) // Only demo and demodash
        { // If the player demodashed, or started dashing while ducking (manual demodash), then a demodash is spent
            Add(new Coroutine(spentADash()));
        }
        if (countCooldown > 0f)
        {
            countCooldown -= Engine.DeltaTime;
            if (countCooldown < 0)
                countCooldown = 0;
        }
        if (count == 0)
        {
            Add(new Coroutine(killPlayerRoutine()));
        }
    }

    private IEnumerator killPlayerRoutine()
    {
        Logger.Debug(nameof(KoseiHelperModule), $"The player has spent all dashes. Goodbye!");
        yield return 0.01f;
        if (Scene.Tracker.GetEntity<Player>() is not { } player)
            yield break;
        player.Die(Vector2.Zero, true, true);
    }

    private IEnumerator spentADash()
    {
        if (Scene.Tracker.GetEntity<Player>() is not { } player)
            yield break;
        if (countCooldown == 0f)
        {
            count -= 1; // The dash count can only be reduced once countCooldown has reached 0
            Logger.Debug(nameof(KoseiHelperModule), $"A dash was spent. Count: {count}");
            countCooldown = 0.2f;
        }
        yield break;
    }

}