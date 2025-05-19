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
			horizontalWindLevel = "KoseiHelper_horizontalWindLevelSlider",
			demoDashing = "KoseiHelper_demoDashingFlag",
			forceMoveX = "KoseiHelper_forceMoveXSlider",
			wallSlide = "KoseiHelper_wallSlideSlider",
			gliderBoost = "KoseiHelper_gliderBoostFlag"
		}
	}
}


function GameDataController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/GameDataController"
end

return GameDataController