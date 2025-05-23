using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/GameDataController")]
public class GameDataController : Entity
{
    public string ducking;
    public string facing;
    public string stamina;
    public string justRespawned;
    public string speedX, speedY;
    public string onGround;
    public string onSafeGround;
    public string playerState;
    public string dashAttacking;
    public string startedFromGolden, carryingGolden;
    public string darknessAlpha;
    public string bloomAmount;
    public string death;
    public string timesDashed;
    public string timesJumped;
    public string worldPositionX, worldPositionY;
    public string featherTime;
    public string launched;
    public string transitioning;
    public string WindLevelX, WindLevelY;
    public string demoDashing;
    public string forceMoveX;
    public string wallSlide;
    public string gliderBoost;
    public string chainedBerries;
    public string onTopOfWater;
    public string totalBerriesCollected;
    public string timeRate;
    public string anxiety;

    private static int timesJumpedCounter, timesWallJumped, timesWallbounced;

    public GameDataController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        ducking = data.Attr("ducking", "KoseiHelper_DuckingFlag");
        facing = data.Attr("facing", "KoseiHelper_FacingFlag");
        stamina = data.Attr("stamina", "KoseiHelper_StaminaSlider");
        justRespawned = data.Attr("justRespawned", "KoseiHelper_JustRespawnedFlag");
        speedX = data.Attr("speedX", "KoseiHelper_SpeedXSlider");
        speedY = data.Attr("speedY", "KoseiHelper_SpeedYSlider");
        onGround = data.Attr("onGround", "KoseiHelper_onGroundFlag");
        playerState = data.Attr("playerState","KoseiHelper_playerStateCounter");
        dashAttacking = data.Attr("dashAttacking", "KoseiHelper_dashAttackingFlag");
        startedFromGolden = data.Attr("startedFromGolden", "KoseiHelper_startedFromGoldenFlag");
        carryingGolden = data.Attr("carryingGolden", "KoseiHelper_carryingGoldenFlag");
        darknessAlpha = data.Attr("darknessAlpha", "KoseiHelper_darknessAlphaSlider");
        bloomAmount = data.Attr("bloomAmount", "KoseiHelper_bloomAmountSlider");
        death = data.Attr("deaths", "KoseiHelper_deathCounter");
        timesDashed = data.Attr("timesDashed", "KoseiHelper_timesDashedCounter");
        timesJumped = data.Attr("timesJumped", "KoseiHelper_timesJumpedCounter");
        worldPositionX = data.Attr("worldPositionX", "KoseiHelper_worldPositionXSlider");
        worldPositionY = data.Attr("worldPositionY", "KoseiHelper_worldPositionYSlider");
        featherTime = data.Attr("featherTime", "KoseiHelper_featherTimeSlider");
        launched = data.Attr("launched", "KoseiHelper_launchedFlag");
        transitioning = data.Attr("transitioning", "KoseiHelper_transitioningFlag");
        WindLevelX = data.Attr("WindLevelX", "KoseiHelper_horizontalWindLevelSlider");
        WindLevelY = data.Attr("WindLevelY", "KoseiHelper_verticalWindLevelSlider");
        demoDashing = data.Attr("demoDashing", "KoseiHelper_demoDashingFlag");
        forceMoveX = data.Attr("forceMoveX", "KoseiHelper_forceMoveXSlider");
        wallSlide = data.Attr("wallSlide", "KoseiHelper_wallSlideSlider");
        gliderBoost = data.Attr("gliderBoost", "KoseiHelper_gliderBoostFlag");
        chainedBerries = data.Attr("chainedBerries", "KoseiHelper_chainedBerries");
        onTopOfWater = data.Attr("onTopOfWater", "KoseiHelper_onTopOfWater");
        totalBerriesCollected = data.Attr("totalBerriesCollected", "KoseiHelper_totalBerriesCollected");
        timeRate = data.Attr("timeRate", "KoseiHelper_timeRateSlider");
        anxiety = data.Attr("anxiety", "KoseiHelper_anxietySlider");
        base.Tag = Tags.PauseUpdate;
        base.Tag = Tags.TransitionUpdate;
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Session session = level.Session;
        Player player = level.Tracker.GetEntity<Player>();
        if (player?.Dead ?? true)
        {
            session.SetCounter(timesDashed, 0);
            session.SetCounter(timesJumped, 0);
            session.SetCounter(timesJumped + "Wall", 0);
            session.SetCounter(timesJumped + "Wallbounce", 0);
            timesJumpedCounter = 0;
            timesWallbounced = 0;
            timesWallJumped = 0;
        }
        if (player != null && !player.Dead)
        {
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

            session.SetSlider(darknessAlpha, session.LightingAlphaAdd);
            session.SetSlider(bloomAmount, session.BloomBaseAdd);

            session.SetCounter(death, session.DeathsInCurrentLevel);

            if (player.StartedDashing)
                session.IncrementCounter(timesDashed);
            session.SetCounter(timesJumped, timesJumpedCounter);
            session.SetCounter(timesJumped + "Wall", timesWallJumped);
            session.SetCounter(timesJumped + "Wallbounce", timesWallbounced);

            session.SetSlider(worldPositionX, player.Position.X);
            session.SetSlider(worldPositionY, player.Position.Y);
            session.SetSlider(featherTime, player.starFlyTimer);
            session.SetFlag(launched, player.launched);
            session.SetFlag(transitioning, level.Transitioning);
            session.SetSlider(WindLevelX, level.VisualWind);
            session.SetFlag(demoDashing,player.demoDashed);
            session.SetSlider(forceMoveX,player.forceMoveXTimer);
            session.SetSlider(wallSlide, player.wallSlideTimer);
            session.SetFlag(gliderBoost, player.gliderBoostTimer > 0f ? true : false);

            session.SetCounter(chainedBerries, player.StrawberryCollectIndex);
            session.SetFlag(onTopOfWater, player._IsOverWater() && !player.SwimUnderwaterCheck());
            session.SetCounter(totalBerriesCollected, level.strawberriesDisplay.strawberries.amount);
            session.SetSlider(WindLevelY, level.windController.targetSpeed.Y);
            session.SetSlider(timeRate, Engine.TimeRate);
            session.SetSlider(anxiety, Distort.Anxiety);
        }
    }

    private static void jumpCounterIncrease(Player player)
    {
        timesJumpedCounter++;
    }
    private static void wallJumpCounterIncrease(Player player)
    {
        timesWallJumped++;
    }

    private static void wallBounceCounterIncrease(Player player)
    {
        timesWallbounced++;
    }

    private static void onPlayerJump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
    {
        jumpCounterIncrease(self);
        orig(self, particles, playSfx);
    }

    private static void onPlayerSuperJump(On.Celeste.Player.orig_SuperJump orig, Player self)
    {
        jumpCounterIncrease(self);
        orig(self);
    }

    private static void onPlayerWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
    {
        jumpCounterIncrease(self);
        wallJumpCounterIncrease(self);
        orig(self, dir);
    }

    private static void onPlayerSuperWallJump(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
    {
        jumpCounterIncrease(self);
        wallBounceCounterIncrease(self);
        orig(self, dir);
    }

    public static void Load()
    {
        On.Celeste.Player.Jump += onPlayerJump;
        On.Celeste.Player.SuperJump += onPlayerSuperJump;
        On.Celeste.Player.WallJump += onPlayerWallJump;
        On.Celeste.Player.SuperWallJump += onPlayerSuperWallJump;
    }

    public static void Unload()
    {
        On.Celeste.Player.Jump -= onPlayerJump;
        On.Celeste.Player.SuperJump -= onPlayerSuperJump;
        On.Celeste.Player.WallJump -= onPlayerWallJump;
        On.Celeste.Player.SuperWallJump -= onPlayerSuperWallJump;
    }
}