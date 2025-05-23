local FlagRefillController = {}

FlagRefillController.name = "KoseiHelper/FlagRefillController"
FlagRefillController.depth = -9500
FlagRefillController.texture = "objects/KoseiHelper/Controllers/FlagRefillController"
FlagRefillController.placements = {
	{
		name = "FlagRefillController",
		alternativeName = "CounterRefillController",
		data = {
			flagHairColor = "e6001e",
			counterHairColor = "1e00e6",
			flagName = "KoseiHelper_FlagRefill",
			counterName="KoseiHelper_CounterRefill",
			persistent = false,
			decrease = false,
			countWhenUsed = false
		}
	}
}

FlagRefillController.fieldOrder = {
	"x",
	"y",
	"flagHairColor",
	"flagName",
	"counterName",
	"counterHairColor",
	"decrease",
	"countWhenUsed",
	"persistent"
}

FlagRefillController.fieldInformation = {
	flagHairColor = { fieldType = "color" },
	counterHairColor = { fieldType = "color" }
}

return FlagRefillController