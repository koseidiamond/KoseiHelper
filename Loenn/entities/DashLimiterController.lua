local DashLimiterController = {}

DashLimiterController.name = "KoseiHelper/DashLimiterController"
DashLimiterController.depth = -9500
DashLimiterController.texture = "objects/KoseiHelper/Controllers/IRLController"
DashLimiterController.placements = {
	{
		name = "Dash Limiter Controller",
		data = {
		count = 3
		}
	}
}

DashLimiterController.fieldInformation = {
    count = {
        fieldType = "integer"
    }
}

return DashLimiterController
