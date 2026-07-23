local KillTheoOnTouchController = {}

KillTheoOnTouchController.name = "KoseiHelper/KillTheoOnTouchController"
KillTheoOnTouchController.depth = -12500
KillTheoOnTouchController.texture = "objects/KoseiHelper/Controllers/KillTheoOnTouchController"
KillTheoOnTouchController.placements = {
	{
		name = "KillTheoOnTouchController",
		data = {
		dangerousEntities = "Celeste.CrystalStaticSpinner",
		dieOnSolids = false
		}
	}
}

return KillTheoOnTouchController