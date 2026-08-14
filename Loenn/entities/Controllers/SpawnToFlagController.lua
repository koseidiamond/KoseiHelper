local SpawnToFlagController = {}

SpawnToFlagController.name = "KoseiHelper/SpawnToFlagController"
SpawnToFlagController.depth = -9500
SpawnToFlagController.texture = "objects/KoseiHelper/Controllers/SpawnToFlagController"
SpawnToFlagController.placements = {
	{
		name = "SpawnToFlagController",
		data = {
		flagPrefix = "spawnCoordinates_"
		}
	}
}

return SpawnToFlagController