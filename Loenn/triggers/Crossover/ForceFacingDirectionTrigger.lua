local ForceFacingDirectionTrigger = {}

ForceFacingDirectionTrigger.name = "KoseiHelper/ForceFacingDirectionTrigger"
ForceFacingDirectionTrigger.depth = 100

ForceFacingDirectionTrigger.placements = {
	{
		name = "ForceFacingDirectionTrigger",
		data = {
		mode = "OnStay",
		flag = "",
		speedThreshold = 1,
		noSpeedRequired = false,
		flagValue = true
		}
	}
}

ForceFacingDirectionTrigger.fieldInformation = {
	mode = {
        options = {
			"OnEnter",
			"OnLeave",
			"OnStay"
		},
        editable = false
    },
	speedThreshold = {
		minimumValue = 0.000001
	}
}

return ForceFacingDirectionTrigger