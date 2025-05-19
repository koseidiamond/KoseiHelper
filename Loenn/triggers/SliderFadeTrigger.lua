local SliderFadeTrigger = {}
local enums = require("consts.celeste_enums")

SliderFadeTrigger.name = "KoseiHelper/SliderFadeTrigger"
SliderFadeTrigger.placements = {
    name = "SliderFadeTrigger",
    data = {
		sliderName = "",
		onlyOnce = false,
		fadeFrom = 0,
		fadeTo = 0.5,
		positionMode = "NoEffect",
		flag = ""
    }
}

SliderFadeTrigger.fieldInformation = 
{
	positionMode = {
        options = enums.trigger_position_modes,
        editable = false
    }
}

SliderFadeTrigger.fieldOrder = 
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

return SliderFadeTrigger