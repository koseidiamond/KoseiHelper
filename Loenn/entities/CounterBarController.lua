local CounterBarController = {}

CounterBarController.name = "KoseiHelper/CounterBarController"
CounterBarController.depth = -10500
CounterBarController.placements = {
	{
		name = "CounterBarController",
		alternativeName = "BarDisplay",
		data = {
		color = "32cd32",
		xPosition = 80,
		yPosition = 1000,
		barWidth = 1760,
		barHeight = 16,
		maxValue = 10,
		countName = "koseiHelper_counterBar",
		framePath = "",
		innerTexturePath = "",
		vertical = false,
		canOverflow = false,
		slider = false,
		initialValue = 1,
		persistent = false,
		flag = "",
		outline = true
		}
	}
}

CounterBarController.fieldInformation = {
	color = { fieldType = "color" },
	xPosition = {
		minimumValue = 0,
		fieldType = "integer"
	},
	yPosition = {
		minimumValue = 0,
		fieldType = "integer"
	},
	barWidth = {
		minimumValue = 0,
		fieldType = "integer"
	},
	barHeight = {
		minimumValue = 0,
		fieldType = "integer"
	},
	maxValue = {
		minimumValue = 0.00000000000000000000000000000000000000000001
	}
}

function CounterBarController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/BarController"
end

return CounterBarController