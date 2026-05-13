local state = require("loaded_state")

local CreateSpawnPointTrigger = {}

CreateSpawnPointTrigger.name = "KoseiHelper/CreateSpawnPointTrigger"
CreateSpawnPointTrigger.nodeLimits = {0, 1}

CreateSpawnPointTrigger.placements = {
	{
		name = "CreateSpawnPointTrigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		flag = "",
		room = ""
		}
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

CreateSpawnPointTrigger.fieldInformation = {
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	room = {
        options = getRoomOptions,
        editable = true
    }
}

return CreateSpawnPointTrigger
