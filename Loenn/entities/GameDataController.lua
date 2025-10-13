local GameDataController = {}

GameDataController.name = "KoseiHelper/GameDataController"
GameDataController.depth = 8999

GameDataController.placements = {
	{
		name = "GameDataController",
		data = {
			ducking = "KoseiHelper_DuckingFlag",
			facing = "KoseiHelper_FacingFlag",
			stamina = "KoseiHelper_StaminaSlider",
			justRespawned = "KoseiHelper_JustRespawnedFlag",
			speedX = "KoseiHelper_SpeedXSlider",
			speedY = "KoseiHelper_SpeedYSlider",
			onGround = "KoseiHelper_onGroundFlag",
			playerState = "KoseiHelper_playerStateCounter",
			dashAttacking = "KoseiHelper_dashAttackingFlag",
			startedFromGolden = "KoseiHelper_startedFromGoldenFlag",
			carryingGolden = "KoseiHelper_carryingGoldenFlag",
			darknessAlpha = "KoseiHelper_darknessAlphaSlider",
			bloomAmount = "KoseiHelper_bloomAmountSlider",
			deaths = "KoseiHelper_deathCounter",
			timesDashed = "KoseiHelper_timesDashedCounter",
			timesJumped = "KoseiHelper_timesJumpedCounter",
			worldPositionX = "KoseiHelper_worldPositionXSlider",
			worldPositionY = "KoseiHelper_worldPositionYSlider",
			featherTime = "KoseiHelper_featherTimeSlider",
			launched = "KoseiHelper_launchedFlag",
			transitioning = "KoseiHelper_transitioningFlag",
			windLevelX = "KoseiHelper_horizontalWindLevelSlider",
			windLevelY = "KoseiHelper_verticalWindLevelSlider",
			demoDashing = "KoseiHelper_demoDashingFlag",
			forceMoveX = "KoseiHelper_forceMoveXSlider",
			wallSlide = "KoseiHelper_wallSlideSlider",
			gliderBoost = "KoseiHelper_gliderBoostFlag",
			chainedBerries = "KoseiHelper_chainedBerries",
			onTopOfWater = "KoseiHelper_onTopOfWater",
			totalBerriesCollected = "KoseiHelper_totalBerriesCollected",
			timeRate= "KoseiHelper_timeRateSlider",
			anxiety = "KoseiHelper_anxietySlider",
			coyoteFrames = "KoseiHelper_coyoteFrameCounter",
			usingWatchtower = "KoseiHelper_usingWatchtower",
			playerSpriteScaleX = "KoseiHelper_playerSpriteScaleXSlider",
			playerSpriteScaleY = "KoseiHelper_playerSpriteScaleYSlider"
		}
	}
}


function GameDataController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/GameDataController"
end

return GameDataController