local CompassController = {}

CompassController.name = "KoseiHelper/CompassController"
CompassController.texture = "objects/KoseiHelper/Crossover/Controllers/CompassController"
CompassController.depth = -20000
CompassController.placements = {
    name = "CompassController",
    data = {
        roomCoordinatesX = 0,
		roomCoordinatesY = 0,
		berryCount = 0
    }
}

CompassController.fieldInformation = {
	roomCoordinatesX = {
		fieldType = "integer",
		minimumValue = 0,
		maximumValue = 1920
	},
	roomCoordinatesY = {
		fieldType = "integer",
		minimumValue = 0,
		maximumValue = 1080
	},
	berryCount = {
		fieldType = "integer",
		minimumValue = 0
	}
}

return CompassController