local MapController = {}

MapController.name = "KoseiHelper/MapController"
MapController.depth = -20000
MapController.texture = "objects/KoseiHelper/Crossover/Controllers/MapController"
MapController.justification = {0.0, 0.0}
MapController.placements = {
    name = "MapController",
    data = {
		pinMadeline = "KoseiHelper/map/pins/pin_SingleDash",
		pinBerry = "collectables/strawberry",
		mapLayers = "filenames,separated,by,commas",
		rootPath = "",
		onlyOnSafeGround = false,
		mapDisabledFlag = "KoseiHelper_CantOpenMap"
	}
}

MapController.fieldInformation = {
	pinMadeline = {
		options = {
			"KoseiHelper/map/pins/pin_SingleDash",
			"KoseiHelper/map/pins/pin_DoubleDash",
			"KoseiHelper/map/pins/pin_Black_SingleDash",
			"KoseiHelper/map/pins/pin_Black_DoubleDash",
		},
		editable = true
	}
}

return MapController