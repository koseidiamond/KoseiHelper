local SpawnController = {}

SpawnController.name = "KoseiHelper/SpawnController"
SpawnController.depth = -9500
SpawnController.texture = "objects/KoseiHelper/Controllers/SpawnController"
SpawnController.placements = {
	{
		name = "SpawnController",
		data = {
		offsetX = 0,
		offsetY = 8,
		entityName = "Celeste.Puffer",
		entityData = "right = false",
		removeDash = true
		}
	}
}

return SpawnController
