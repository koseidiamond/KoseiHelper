using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/GameDataController")]
public class GameDataController : Entity
{
    public string ducking;
    public string facing;
    public string stamina;
    public string alive;
    public string justRespawned;
    public string speedX;
    public string speedY;
    public string onGround;
    public string onSafeGround;
    public string playerState;
    public string dashAttacking;
    public string startedFromGolden, carryingGolden;
    public string forceMoveX;

    public GameDataController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        alive = data.Attr("alive", "KoseiHelper_AliveFlag");
        ducking = data.Attr("ducking", "KoseiHelper_DuckingFlag");
        facing = data.Attr("facing", "KoseiHelper_FacingFlag");
        stamina = data.Attr("stamina", "KoseiHelper_StaminaSlider");
        justRespawned = data.Attr("justRespawned", "KoseiHelper_JustRespawnedFlag");
        speedX = data.Attr("speedX", "KoseiHelper_SpeedXSlider");
        speedY = data.Attr("speedY", "KoseiHelper_SpeedYSlider");
        onGround = data.Attr("onGround", "KoseiHelper_onGroundFlag");
        playerState = data.Attr("playerState","KoseiHelper_playerStateCounter");
        dashAttacking = data.Attr("dashing", "KoseiHelper_dashAttackingFlag");
        startedFromGolden = data.Attr("startedFromGolden", "KoseiHelper_startedFromGoldenFlag");
        carryingGolden = data.Attr("carryingGolden", "KoseiHelper_carryingGoldenFlag");
        forceMoveX = data.Attr("forceMoveX", "KoseiHelper_forceMoveXCounter");
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Session session = level.Session;
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            session.SetFlag(alive, true);
            session.SetFlag(ducking, player.Ducking);
            if (player.Facing == Facings.Left)
            {
                session.SetFlag(facing + "Left", true);
                session.SetFlag(facing + "Right", false);
            }
            else
            {
                session.SetFlag(facing + "Left", false);
                session.SetFlag(facing + "Right", true);
            }
            session.SetSlider(stamina, player.Stamina);
            session.SetFlag(justRespawned, player.JustRespawned);
            session.SetSlider(speedX, player.Speed.X);
            session.SetSlider(speedY, player.Speed.Y);
            session.SetFlag(onGround, player.onGround);
            session.SetFlag(onGround + "Safe", player.OnSafeGround);
            session.SetCounter(playerState, player.StateMachine.state);
            session.SetFlag(dashAttacking, player.DashAttacking);
            session.SetFlag(startedFromGolden, session.RestartedFromGolden);
            session.SetFlag(carryingGolden, session.GrabbedGolden);
            session.SetCounter(forceMoveX, player.forceMoveX);
        }
        else
        {
            session.SetFlag(alive, false);
        }
    }
}