local MagicInkController = {}

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")
local enums = require("consts.celeste_enums")

MagicInkController.name = "KoseiHelper/MagicInkController"
MagicInkController.depth = -10500
MagicInkController.texture = "objects/KoseiHelper/Controllers/MagicInkController"
MagicInkController.placements = {
	{
		name = "MagicInkController",
		data = {
		timeToLive = 3,
		maxInk = 300,
		regenerationRate = 20,
		thickness = 4,
		flag = "",
		depth = 1,
		surfaceSoundIndex = 32,
		recoverInkUponShattering = true,
		global = false,
		renderCursor = true
		}
	}
}

MagicInkController.fieldInformation = {
    timeToLive = {
		minimumValue = 0.0001
    },
	thickness = {
		fieldType = "integer",
		minimumValue = 1
    },
	regenerationRate = {
		minimumValue = 0
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	surfaceSoundIndex = {
	options = enums.tileset_sound_ids,
        fieldType = "integer"
    }
}

return MagicInkController
