local ReplaceSpeedWithStoredSpeedTrigger = {}

ReplaceSpeedWithStoredSpeedTrigger.name = "KoseiHelper/ReplaceSpeedWithStoredSpeedTrigger"
ReplaceSpeedWithStoredSpeedTrigger.depth = 100

ReplaceSpeedWithStoredSpeedTrigger.placements = {
	{
		name = "ReplaceSpeedWithStoredSpeedTrigger",
		data = {
		storesDirection = true,
		onlyOnce = false,
		speedAxis = "SpeedX",
		addSpeed = false,
		firstSfx = "event:/none",
		replacementSfx = "event:/none",
		factor = 1,
		storeFirstSpeedOnly = false,
		flag = ""
		}
	}
}

ReplaceSpeedWithStoredSpeedTrigger.fieldInformation = {
	speedAxis = {
		options = {
			"SpeedX",
			"SpeedY",
			"Both",
			"Swapped"
		},
		editable = false
	}
}

return ReplaceSpeedWithStoredSpeedTrigger