local SetWindowSizeTrigger = {}
local enums = require("consts.celeste_enums")

SetWindowSizeTrigger.name = "KoseiHelper/SetWindowSizeTrigger"
SetWindowSizeTrigger.placements = {
    name = "SetWindowSizeTrigger",
    data = {
		widthFrom = 100,
		heightFrom = 100,
		widthTo = 100,
		heightTo = 100,
		fullScreen = false,
		positionMode = "NoEffect"
    }
}

SetWindowSizeTrigger.fieldInformation = 
{
	widthFrom = { fieldType = "integer" },
	heightFrom = { fieldType = "integer" },
	widthTo = { fieldType = "integer" },
	heightTo = { fieldType = "integer" },
	positionMode = {
        options = enums.trigger_position_modes,
        editable = false
    }
}

SetWindowSizeTrigger.fieldOrder = 
{
	"x",
	"y",
	"width",
	"height",
	"positionMode",
	"widthFrom",
	"widthTo",
	"heightFrom",
	"heightTo",
	"fullScreen"
}

return SetWindowSizeTrigger