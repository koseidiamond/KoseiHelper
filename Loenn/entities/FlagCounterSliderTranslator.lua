local FlagCounterSliderTranslator = {}

FlagCounterSliderTranslator.name = "KoseiHelper/FlagCounterSliderTranslator"
FlagCounterSliderTranslator.depth = -9500
FlagCounterSliderTranslator.texture = "objects/KoseiHelper/Controllers/FlagCounterSliderTranslator"
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

return FlagCounterSliderTranslator