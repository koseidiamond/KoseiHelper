local FlagCounterSliderTranslator = {}

FlagCounterSliderTranslator.name = "KoseiHelper/FlagCounterSliderTranslator"
FlagCounterSliderTranslator.depth = -9500
FlagCounterSliderTranslator.texture = "objects/KoseiHelper/Controllers/FlagCounterSliderTranslator"
FlagCounterSliderTranslator.placements = {
	{
		name = "FlagCounterSliderTranslator",
		data = {
			mode = "CounterToSlider",
			flagName = "",
			counterName = "",
			sliderName = "",
			
			valueWhileFalse = 0,
			valueWhileTrue = 1,
			
			minValueForFalse = 0,
			maxValueForTrue = 1,
			
			absoluteValue = false
		}
	}
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
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.FlagToCounter == true then
		doNotIgnore("minisAmount")
		doNotIgnore("timeToSpawnMinis")
	end
	if entity.slowdown == true then
		doNotIgnore("slowdownDistanceMax")
		doNotIgnore("slowdownDistanceMin")
	end
	return ignored
end

return FlagCounterSliderTranslator