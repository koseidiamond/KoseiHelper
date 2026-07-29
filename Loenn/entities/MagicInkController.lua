local MagicInkController = {}

MagicInkController.name = "KoseiHelper/MagicInkController"
MagicInkController.depth = -10500
MagicInkController.texture = "objects/KoseiHelper/Controllers/MagicInkController"
MagicInkController.placements = {
	{
		name = "MagicInkController",
		data = {
		timeToLive = 3,
		maxInk = 300,
		regenerationRate = 60
		}
	}
}

MagicInkController.fieldInformation = {
    timeToLive = {
		minimumValue = 0.0001
    }
}

return MagicInkController
