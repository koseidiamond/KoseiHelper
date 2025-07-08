local SetFlagOnBerryCollectionController = {}

SetFlagOnBerryCollectionController.name = "KoseiHelper/SetFlagOnBerryCollectionController"
SetFlagOnBerryCollectionController.depth = -9500
SetFlagOnBerryCollectionController.texture = "objects/KoseiHelper/Controllers/SetFlagOnBerryCollectionController"
SetFlagOnBerryCollectionController.placements = {
	{
		name = "SetFlagOnBerryCollectionController",
		data = {
		flag = "KoseiHelper_BerryCollected",
		counter = "KoseiHelper_BerriesCollected",
		reactToGoldens = "Default",
		reactToReds = "Default",
		reactToMoons = "Default",
		reactToWingeds = "Default",
		reactToGhosts = "Default",
		global = false
		}
	}
}

SetFlagOnBerryCollectionController.fieldInformation = {
	reactToGoldens = {
		options = {
			"Default",
			"React",
			"Ignore",
		},
		editable = false
	},
	reactToReds = {
		options = {
			"Default",
			"React",
			"Ignore",
		},
		editable = false
	},
	reactToMoons = {
		options = {
			"Default",
			"React",
			"Ignore",
		},
		editable = false
	},
	reactToWingeds = {
		options = {
			"Default",
			"React",
			"Ignore",
		},
		editable = false
	},
	reactToGhosts = {
		options = {
			"Default",
			"React",
			"Ignore",
		},
		editable = false
	}
}

return SetFlagOnBerryCollectionController