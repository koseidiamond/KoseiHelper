local state = require("loaded_state")

local CustomPlayerSeeker = {}

CustomPlayerSeeker.name = "KoseiHelper/CustomPlayerSeeker"
CustomPlayerSeeker.depth = -100

CustomPlayerSeeker.placements = {
	{
		name = "CustomPlayerSeeker",
		data = {
			nextRoom = "c-00",
			colorgrade = "templevoid",
			sprite = "seeker",
			seekerDashSound = "event:/game/05_mirror_temple/seeker_dash",
			vanillaEffects = false,
			onlyBreakAnimation = false,
			canSwitchCharacters = false,
			speedMultiplier = 1,
			isFriendly = false,
			flagToSwap = "",
			flagToHatch = "",
			canShatterSpinners = false,
			freezeFramesWhenDashing = 0
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

function CustomPlayerSeeker.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"flagToSwap",
	"onlyBreakAnimation"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.canSwitchCharacters then
		doNotIgnore("flagToSwap")
	end
	if entity.vanillaEffects then
		doNotIgnore("onlyBreakAnimation")
	end
	return ignored
end

CustomPlayerSeeker.fieldInformation = {
	freezeFramesWhenDashing = {
		minimumValue = 0
	},
	nextRoom = {
        options = getRoomOptions,
        editable = true
    },
	colorgrade = {
        options = {
			"none",
			"oldsite",
			"panicattack",
			"templevoid",
			"reflection",
			"credits",
			"cold",
			"hot",
			"feelingdown",
			"golden"
		},
        editable = true
    },
	freezeFramesWhenDashing = {
		fieldType = "integer"
	}
}

CustomPlayerSeeker.texture = "decals/5-temple/statue_e"

return CustomPlayerSeeker