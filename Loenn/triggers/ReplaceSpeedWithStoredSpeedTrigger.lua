local ReplaceSpeedWithStoredSpeedTrigger = {}

ReplaceSpeedWithStoredSpeedTrigger.name = "KoseiHelper/ReplaceSpeedWithStoredSpeedTrigger"
ReplaceSpeedWithStoredSpeedTrigger.depth = 100

ReplaceSpeedWithStoredSpeedTrigger.placements = {
	{
		name = "ReplaceSpeedWithStoredSpeedTrigger",
		data = {
		storesDirection = true,
		onlyOnce = false,
		speedAxis = "SpeedX"
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