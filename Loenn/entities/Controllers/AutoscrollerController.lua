local AutoscrollerController = {}

AutoscrollerController.name = "KoseiHelper/AutoscrollerController"
AutoscrollerController.depth = -9500
AutoscrollerController.placements = {
	{
		name = "AutoscrollerController",
		data = {
		speed = 60,
		cameraDirection = "Right",
		pushMode = "PushNCrush",
		behindOffset = 0,
		aheadOffset = 0,
		flag = ""
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
		minimumValue = 1,
		allowEmpty = false
	},
	behindOffset = { fieldType = "integer" },
	aheadOffset = { fieldType = "integer" }
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

AutoscrollerController.fieldOrder = {
	"x",
	"y",
	"cameraDirection",
	"pushMode",
	"speed",
	"flag",
	"behindOffset",
	"aheadOffset"
}

return AutoscrollerController