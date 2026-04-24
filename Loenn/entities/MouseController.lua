local MouseController = {}

MouseController.name = "KoseiHelper/MouseController"
MouseController.depth = -9500
MouseController.texture = "objects/KoseiHelper/Controllers/MouseController"
MouseController.placements = {
	{
		name = "MouseController",
		data = {
		cursorTexture = "dot_outline"
		}
	}
}

MouseController.fieldInformation = {
	cursorTexture = {
		options = {
			"dot_outline",
			"KoseiHelper/mouseCursors/cursor"
		},
		editable = true
	}
}

return MouseController