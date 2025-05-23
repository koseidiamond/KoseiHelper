local CustomPauseController = {}

CustomPauseController.name = "KoseiHelper/CustomPauseController"
CustomPauseController.depth = -9500
CustomPauseController.texture = "objects/KoseiHelper/Controllers/CustomPauseController"
CustomPauseController.placements = {
	{
		name = "CustomPauseController",
		data = {
		canPause = true,
		canRetry = true,
		canSaveAndQuit = true,
		timerIsStopped = false,
		timerHidden = false,
		dieOnUnpause = false,
		flagWhilePaused = "KoseiHelper_GameIsPaused"
		}
	}
}

return CustomPauseController
