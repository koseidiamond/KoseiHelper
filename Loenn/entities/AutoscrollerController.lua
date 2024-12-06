local AutoscrollerController = {}

AutoscrollerController.name = "KoseiHelper/AutoscrollerController"
AutoscrollerController.depth = -9500
AutoscrollerController.texture = "objects/KoseiHelper/Controllers/AutoscrollerController"
AutoscrollerController.placements = {
	{
		name = "Autoscroller Controller",
		data = {
		speed = 60,
		cameraDirection = "Right",
		pushMode = "PushNCrush"
		}
	}
}

AutoscrollerController.fieldInformation = {
	cameraDirection = {
		options = {"Right", "Left", "Up", "Down"},
		editable = false
	},
	pushMode = {
		options = {"PushNCrush", "ImmediateDeath"},
		editable = false
	},
	speed = {
		fieldType = "integer",
		minimumValue = 1
	}
}

return AutoscrollerController