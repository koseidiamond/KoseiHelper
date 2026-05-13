local state = require("loaded_state")

local ReturnToRoomController = {}

ReturnToRoomController.name = "KoseiHelper/ReturnToRoomController"
ReturnToRoomController.texture = "objects/KoseiHelper/Crossover/Controllers/ReturnToGDMenuController"
ReturnToRoomController.depth = -10500

ReturnToRoomController.placements = {
	name = "ReturnToRoomController",
	data = {
		roomName = "",
		dialogID = "KoseiHelper_ReturnToRoom",
		flagsToUnset = "",
		preventionFlag = "",
		introType = "None",
		closestSpawnX = 0,
		closestSpawnY = 0,
		sound = "event:/none",
		loseFollowers = true,
		menuIndex = 0
	}
}

local function getRoomOptions()
    local rooms = {}
    if state.map and state.map.rooms then
        for _, room in ipairs(state.map.rooms) do
            table.insert(rooms, room.name)
        end
    end
    table.sort(rooms)
    return rooms
end

ReturnToRoomController.fieldInformation = {
	roomName = {
        options = getRoomOptions,
        editable = true
    },
	introType = {
		options = {
			"Transition",
			"Respawn",
			"WalkInRight",
			"WalkInLeft",
			"Jump",
			"WakeUp",
			"Fall",
			"TempleMirrorVoid",
			"None",
			"ThinkForABit"
		},
		editable = false
	},
	menuIndex = {
		fieldType = "integer"
	},
	closestSpawnX = {
		fieldType = "integer"
	},
	closestSpawnY = {
		fieldType = "integer"
	}
}

return ReturnToRoomController