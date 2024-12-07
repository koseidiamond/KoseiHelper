local AutoscrollerController = {}

AutoscrollerController.name = "KoseiHelper/AutoscrollerController"
AutoscrollerController.depth = -9500
AutoscrollerController.placements = {
	{
		name = "Autoscroller Controller",
		data = {
		speed = 60,
		cameraDirection = "Right",
		pushMode = "PushNCrush"
		}
	}
}

AutoscrollerController.fieldInformation = {
	cameraDirection = {
		options = {"Right", "Left", "Up", "Down"},
		editable = false
	},
	pushMode = {
		options = {"PushNCrush", "ImmediateDeath", "SafePush"},
		editable = false
	},
	speed = {
		fieldType = "integer",
		minimumValue = 1
	}
}

function AutoscrollerController.texture(room, entity)
    local cameraDirection = entity.cameraDirection
    if cameraDirection == "Up" then
        return "objects/KoseiHelper/Controllers/AutoscrollerControllerUp"
    elseif cameraDirection == "Down" then
        return "objects/KoseiHelper/Controllers/AutoscrollerControllerDown"
	elseif cameraDirection == "Left" then
        return "objects/KoseiHelper/Controllers/AutoscrollerControllerLeft"
	else
		return "objects/KoseiHelper/Controllers/AutoscrollerControllerRight"
    end
end

return AutoscrollerController