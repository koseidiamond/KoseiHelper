local GameDataController = {}

GameDataController.name = "KoseiHelper/GameDataController"
GameDataController.depth = 8999

GameDataController.placements = {
	{
		name = "GameDataController",
		data = {
			alive = "KoseiHelper_AliveFlag",
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
			forceMoveX = "KoseiHelper_forceMoveXCounter"
		}
	}
}


function GameDataController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/GameDataController"
end

return GameDataController