using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

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
    public string coyoteFrames;
    public string usingWatchtower;
    public string playerSpriteScaleX, playerSpriteScaleY;

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
        playerState = data.Attr("playerState", "KoseiHelper_playerStateCounter");
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
        coyoteFrames = data.Attr("coyoteFrames", "KoseiHelper_coyoteFrameCounter");
        usingWatchtower = data.Attr("usingWatchtower", "KoseiHelper_usingWatchtower");
        playerSpriteScaleX = data.Attr("playerSpriteScaleX", "KoseiHelper_playerSpriteScaleXSlider");
        playerSpriteScaleY = data.Attr("playerSpriteScaleY", "KoseiHelper_playerSpriteScaleYSlider");
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
            if (!string.IsNullOrEmpty(timesDashed))
                session.SetCounter(timesDashed, 0);
            if (!string.IsNullOrEmpty(timesJumped))
            {
                session.SetCounter(timesJumped, 0);
                session.SetCounter(timesJumped + "Wall", 0);
                session.SetCounter(timesJumped + "Wallbounce", 0);
                timesJumpedCounter = 0;
                timesWallbounced = 0;
                timesWallJumped = 0;
            }
        }
        if (player != null && !player.Dead)
        {
            if (!string.IsNullOrEmpty(ducking))
                session.SetFlag(ducking, player.Ducking);
            if (!string.IsNullOrEmpty(facing))
            {
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
            }
            if (!string.IsNullOrEmpty(stamina))
                session.SetSlider(stamina, player.Stamina);

            if (!string.IsNullOrEmpty(justRespawned))
                session.SetFlag(justRespawned, player.JustRespawned);

            if (!string.IsNullOrEmpty(speedX))
                session.SetSlider(speedX, player.Speed.X);
            if (!string.IsNullOrEmpty(speedY))
                session.SetSlider(speedY, player.Speed.Y);

            if (!string.IsNullOrEmpty(onGround))
            {
                session.SetFlag(onGround, player.onGround);
                session.SetFlag(onGround + "Safe", player.OnSafeGround);
            }

            if (!string.IsNullOrEmpty(playerState))
                session.SetCounter(playerState, player.StateMachine.state);
            if (!string.IsNullOrEmpty(dashAttacking))
                session.SetFlag(dashAttacking, player.DashAttacking);

            if (!string.IsNullOrEmpty(startedFromGolden))
                session.SetFlag(startedFromGolden, session.RestartedFromGolden);
            if (!string.IsNullOrEmpty(carryingGolden))
                session.SetFlag(carryingGolden, session.GrabbedGolden);

            if (!string.IsNullOrEmpty(darknessAlpha))
                session.SetSlider(darknessAlpha, session.LightingAlphaAdd);
            if (!string.IsNullOrEmpty(bloomAmount))
                session.SetSlider(bloomAmount, session.BloomBaseAdd);

            if (!string.IsNullOrEmpty(death))
                session.SetCounter(death, session.DeathsInCurrentLevel);

            if (player.StartedDashing && !string.IsNullOrEmpty(timesDashed))
                session.IncrementCounter(timesDashed);
            if (!string.IsNullOrEmpty(timesJumped))
            {
                session.SetCounter(timesJumped, timesJumpedCounter);
                session.SetCounter(timesJumped + "Wall", timesWallJumped);
                session.SetCounter(timesJumped + "Wallbounce", timesWallbounced);
            }

            if (!string.IsNullOrEmpty(worldPositionX))
                session.SetSlider(worldPositionX, player.Position.X);
            if (!string.IsNullOrEmpty(worldPositionY))
                session.SetSlider(worldPositionY, player.Position.Y);
            if (!string.IsNullOrEmpty(featherTime))
                session.SetSlider(featherTime, player.starFlyTimer);
            if (!string.IsNullOrEmpty(launched))
                session.SetFlag(launched, player.launched);
            if (!string.IsNullOrEmpty(transitioning))
                session.SetFlag(transitioning, level.Transitioning);

            if (!string.IsNullOrEmpty(WindLevelX))
                session.SetSlider(WindLevelX, level.VisualWind);
            if (!string.IsNullOrEmpty(WindLevelY))
                session.SetSlider(WindLevelY, level.windController.targetSpeed.Y);

            if (!string.IsNullOrEmpty(demoDashing))
                session.SetFlag(demoDashing, player.demoDashed);
            if (!string.IsNullOrEmpty(forceMoveX))
                session.SetSlider(forceMoveX, player.forceMoveXTimer);
            if (!string.IsNullOrEmpty(wallSlide))
                session.SetSlider(wallSlide, player.wallSlideTimer);
            if (!string.IsNullOrEmpty(gliderBoost))
                session.SetFlag(gliderBoost, player.gliderBoostTimer > 0f ? true : false);

            if (!string.IsNullOrEmpty(chainedBerries))
                session.SetCounter(chainedBerries, player.StrawberryCollectIndex);

            if (!string.IsNullOrEmpty(onTopOfWater))
                session.SetFlag(onTopOfWater, player._IsOverWater() && !player.SwimUnderwaterCheck());

            if (!string.IsNullOrEmpty(totalBerriesCollected))
                session.SetCounter(totalBerriesCollected, level.strawberriesDisplay.strawberries.amount);

            if (!string.IsNullOrEmpty(timeRate))
                session.SetSlider(timeRate, Engine.TimeRate);

            if (!string.IsNullOrEmpty(anxiety))
                session.SetSlider(anxiety, Distort.Anxiety);

            if (!string.IsNullOrEmpty(coyoteFrames))
                session.SetCounter(coyoteFrames, (int)(player.jumpGraceTimer * 60) - 1);

            if (!string.IsNullOrEmpty(usingWatchtower))
            {
                Lookout lookout = level.Tracker.GetNearestEntity<Lookout>(player.Center);
                session.SetFlag(usingWatchtower, lookout != null && lookout.interacting);
            }
            if (!string.IsNullOrEmpty(playerSpriteScaleX))
                session.SetSlider(playerSpriteScaleX, player.Sprite.Scale.X);
            if (!string.IsNullOrEmpty(playerSpriteScaleY))
                session.SetSlider(playerSpriteScaleY, player.Sprite.Scale.Y);
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