local FlagRefillController = {}

FlagRefillController.name = "KoseiHelper/FlagRefillController"
FlagRefillController.depth = -9500
FlagRefillController.texture = "objects/KoseiHelper/Controllers/FlagRefillController"
FlagRefillController.placements = {
	{
		name = "FlagRefillController",
		data = {
			hairColor = "e6001e",
			flagName = "KoseiHelper_FlagRefill",
			persistent = false
		}
	}
}

FlagRefillController.fieldInformation = {
	hairColor = { fieldType = "color" }
}

return FlagRefillController