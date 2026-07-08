local TheoPetController = {}

TheoPetController.name = "KoseiHelper/TheoPetController"
TheoPetController.depth = -10500
TheoPetController.placements = {
	{
		name = "TheoPetController",
		data = {
		speed = 8,
		jumpStrength = 1,
		affectAllTheos = false,
		global = false,
		minDistanceX = 14,
		minDistanceY = 150
		}
	}
}

TheoPetController.fieldInformation = {
	minDistanceX = {
        fieldType = "integer",
		minimumValue = 0
    },
	minDistanceY = {
        fieldType = "integer",
		minimumValue = 0
    },
	jumpStrength = {
		fieldType = "integer",
		minimumValue = 0
	}
}

function TheoPetController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/TheoPetController"
end

return TheoPetController