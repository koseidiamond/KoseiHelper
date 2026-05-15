local utils = require("utils")
local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local BalloonTripFish = {}

BalloonTripFish.name = "KoseiHelper/BalloonTripFish"
function BalloonTripFish.depth(room,entity)
	return entity.depth
end
BalloonTripFish.justification = {0.5, 0.5}
BalloonTripFish.placements = {
    {
        name = "BalloonTripFish",
        data = {
            spriteID = "koseiHelper_BalloonTripFish",
			depth = -10500,
			distance = 32,
			flip = true,
			chaseSpeed = 1,
			hideAfterAttack = true,
			idleFlag = ""
        }
    }
}

BalloonTripFish.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	distance = {
		minimumValue = 1,
		fieldType = "integer"
	}
}

function BalloonTripFish.texture(room, entity)
    return "objects/KoseiHelper/Crossover/BalloonTripFish/BalloonTripFish03"
end

function BalloonTripFish.selection(room, entity)
	return utils.rectangle(entity.x - 8, entity.y - 4, 16, 20)
end

return BalloonTripFish