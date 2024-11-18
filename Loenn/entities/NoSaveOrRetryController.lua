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
		dieOnUnpause = false
		}
	}
}

return CustomPauseController
