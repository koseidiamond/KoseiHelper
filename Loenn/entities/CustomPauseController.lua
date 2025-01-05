local CustomPauseController = {}

CustomPauseController.name = "KoseiHelper/CustomPauseController"
CustomPauseController.depth = -9500
CustomPauseController.texture = "objects/KoseiHelper/Controllers/CustomPauseController"
CustomPauseController.placements = {
	{
		name = "Custom Pause Controller",
		data = {
		canPause = true,
		canRetry = true,
		canSaveAndQuit = true,
		timerIsStopped = false,
		timerHidden = false,
		dieOnUnpause = false,
		persistent = false
		}
	}
}

return CustomPauseController
