local drawableText = require("structs.drawable_text")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local FlagCounterSliderTranslator = {}

FlagCounterSliderTranslator.name = "KoseiHelper/FlagCounterSliderTranslator"
FlagCounterSliderTranslator.depth = -9500
FlagCounterSliderTranslator.placements = {
	{
		name = "FlagCounterSliderTranslator",
		alternativeName = "FCSTranslator",
		data = {
			mode = "CounterToSlider",
			flagName = "",
			counterName = "",
			sliderName = "",
			
			flagSuffix = false,
			
			valueWhileFalse = 0,
			valueWhileTrue = 1,
			minValueForFalse = 0,
			maxValueForTrue = 1,
			
			absoluteValue = false,
			reverseValue = false,
			multiplierFactor = 1
		}
	}
}

FlagCounterSliderTranslator.fieldOrder = {
	"x",
	"y",
	"mode",
	"flagName",
	"counterName",
	"sliderName",
	"valueWhileFalse",
	"valueWhileTrue",
	"minValueForFalse",
	"maxValueForTrue",
	"multiplierFactor",
	"flagSuffix",
	"absoluteValue",
	"reverseValue"
}

FlagCounterSliderTranslator.fieldInformation = {
	mode = {
		options = {
			"FlagToCounter",
			"FlagToSlider",
			"CounterToFlag",
			"CounterToSlider",
			"SliderToFlag",
			"SliderToCounter"
		},
		editable = false
	}
}

function FlagCounterSliderTranslator.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"flagName",
	"counterName",
	"sliderName",
	"valueWhileFalse",
	"valueWhileTrue",
	"minValueForFalse",
	"maxValueForTrue",
	"flagSuffix",	
	"absoluteValue",
	"multiplierFactor"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.mode == "FlagToCounter" then
		doNotIgnore("flagName")
		doNotIgnore("counterName")
		doNotIgnore("valueWhileTrue")
		doNotIgnore("valueWhileFalse")
		doNotIgnore("multiplierFactor")
	end
	if entity.mode == "FlagToSlider" then
		doNotIgnore("flagName")
		doNotIgnore("sliderName")
		doNotIgnore("valueWhileTrue")
		doNotIgnore("valueWhileFalse")
		doNotIgnore("multiplierFactor")
	end
	if entity.mode == "CounterToFlag" then
		doNotIgnore("counterName")
		doNotIgnore("flagName")
		doNotIgnore("minValueForFalse")
		doNotIgnore("maxValueForTrue")
		doNotIgnore("flagSuffix")
	end
	if entity.mode == "CounterToSlider" then
		doNotIgnore("counterName")
		doNotIgnore("sliderName")
		doNotIgnore("multiplierFactor")
	end
	if entity.mode == "SliderToFlag" then
		doNotIgnore("sliderName")
		doNotIgnore("flagName")
		doNotIgnore("minValueForFalse")
		doNotIgnore("maxValueForTrue")
		doNotIgnore("flagSuffix")
	end
	if entity.mode == "SliderToCounter" then
		doNotIgnore("sliderName")
		doNotIgnore("counterName")
		doNotIgnore("multiplierFactor")
	end
	if entity.mode == "CounterToFlag" or entity.mode == "SliderToFlag" or entity.mode == "CounterToSlider" or entity.mode == "SliderToCounter" then
		doNotIgnore("absoluteValue")
	end
	return ignored
end

function FlagCounterSliderTranslator.sprite(room, entity)
	local texturePath
	local mode = entity.mode
	if mode == "FlagToCounter" then
		texturePath = "objects/KoseiHelper/Controllers/TranslatorFlagCounter"
	elseif mode == "FlagToSlider" then
		texturePath = "objects/KoseiHelper/Controllers/TranslatorFlagSlider"
	elseif mode == "CounterToFlag" then
		texturePath = "objects/KoseiHelper/Controllers/TranslatorCounterFlag"
	elseif mode == "CounterToSlider" then
		texturePath = "objects/KoseiHelper/Controllers/TranslatorCounterSlider"
	elseif mode == "SliderToFlag" then
		texturePath = "objects/KoseiHelper/Controllers/TranslatorSliderFlag"
	elseif mode == "SliderToCounter" then
		texturePath = "objects/KoseiHelper/Controllers/TranslatorSliderCounter"
	else
		texturePath = "objects/KoseiHelper/Controllers/TranslatorFlagCounterSlider"
	end
	local sprite = drawableSprite.fromTexture(texturePath, entity)
	local sprites = {sprite}
	local flagText = tostring(entity.flagName)
	local counterText = tostring(entity.counterName)
	local sliderText = tostring(entity.sliderName)
	if mode == "FlagToCounter" or mode == "FlagToSlider" then
		local text1 = drawableText.fromText(flagText, entity.x - 8, entity.y-28, entity.width, entity.height, nil, 1, {0.92549, 0.12941, 0.32157})
		table.insert(sprites, text1)
	end
	if mode == "CounterToFlag" or mode == "CounterToSlider" then
		local text1 = drawableText.fromText(counterText, entity.x - 8, entity.y-28, entity.width, entity.height, nil, 1, {0.2902, 0.0902, 0.9492})
		table.insert(sprites, text1)
	end
	if mode == "SliderToFlag" or mode == "SliderToCounter" then
		local text1 = drawableText.fromText(sliderText, entity.x - 8, entity.y-28, entity.width, entity.height, nil, 1, {0.19608, 0.80392, 0.19608})
		table.insert(sprites, text1)
	end
	if mode == "CounterToFlag" or mode == "SliderToFlag" then
		local text2 = drawableText.fromText(flagText, entity.x - 8, entity.y-20, entity.width, entity.height, nil, 1, {0.92549, 0.12941, 0.32157})
		table.insert(sprites, text2)
	end
	if mode == "FlagToCounter" or mode == "SliderToCounter" then
		local text2 = drawableText.fromText(counterText, entity.x - 8, entity.y-20, entity.width, entity.height, nil, 1, {0.2902, 0.0902, 0.9492})
		table.insert(sprites, text2)
	end
	if mode == "FlagToSlider" or mode == "CounterToSlider" then
		local text2 = drawableText.fromText(sliderText, entity.x - 8, entity.y-20, entity.width, entity.height, nil, 1, {0.19608, 0.80392, 0.19608})
		table.insert(sprites, text2)
	end
	return sprites
end	

return FlagCounterSliderTranslator