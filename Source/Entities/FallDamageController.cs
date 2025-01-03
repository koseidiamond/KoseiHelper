using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FallDamageController")]
public class FallDamageController : Entity
{
    public float terminalSpeed = 160f; //Max normal fall speed
    public float maxFallSpeed = 160f * 1.5f; //Max fast fall speed which is 240 by default
    public float fallTimeThreshold = 2f; // Time before fall damage is triggered
    public float fallDamageThreshold = 100f; // Speed threshold for fall damage
    public float fallDamageAmount = 10f; //Amount of health to remove
    public float playerHealth = 100f;

    private float fallTime;
    private bool isFalling;
    public FallDamageController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        terminalSpeed = data.Float("terminalSpeed", 160f);
        fallTimeThreshold = data.Float("fallTimeThreshold", 0.5f);
        fallDamageThreshold = data.Float("fallDamageThreshold", 100f);
        fallDamageAmount = data.Float("fallDamageAmount", 100f);
        playerHealth = data.Float("playerHealth", 100f);
        if (data.Bool("persistent", true))
            Tag = Tags.Persistent;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        (scene as Level).Session.SetSlider("koseiHelper_health", playerHealth);
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            if (player.wasOnGround || player.Speed.Y <= 0 || player.StateMachine.state != 0)
            {
                if (isFalling)
                    CheckFallDamage(player);
                fallTime = 0f;
                isFalling = false;
            }
            else
            {
                if (!isFalling && player.Speed.Y > 0)
                {
                    isFalling = true;
                    fallTime = 0f;
                }
                if (isFalling && player.Speed.Y > 0 && player.StateMachine.state == 0)
                    fallTime += Engine.DeltaTime;
            }
            if (level.Session.GetSlider("koseiHelper_health") <= 0)
                player.Die(player.Center);
        }
    }

    private void CheckFallDamage(Player player)
    {
        Logger.Debug(nameof(KoseiHelperModule), $"fallTime: {fallTime}, falling speed: {player.Speed.Y}, current health: {player.level.Session.GetSlider("koseiHelper_health")}");
        Logger.Debug(nameof(KoseiHelperModule), $"fallTimeThreshold: {fallTimeThreshold}, fallDamageThreshold: {fallDamageThreshold}");
        if (fallTime >= fallTimeThreshold && player.Speed.Y >= fallDamageThreshold)
        {
            player.level.Session.SetSlider("koseiHelper_health", player.level.Session.GetSlider("koseiHelper_health") - fallDamageAmount);
        }
    }
}