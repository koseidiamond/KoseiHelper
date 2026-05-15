local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local BalloonTripCollectable = {}

BalloonTripCollectable.name = "KoseiHelper/BalloonTripCollectable"
BalloonTripCollectable.texture ="objects/KoseiHelper/Crossover/BalloonTripCollectable/balloon00"

function BalloonTripCollectable.depth(room,entity)
	return entity.depth
end

BalloonTripCollectable.placements = {
	{
		name = "BalloonTripCollectable",
		data = {
			depth = -100,
			pointsGiven = 100,
			counterName = "koseiHelper_balloonPoints",
			spriteID = "koseiHelper_balloonTripCollectable",
			sound = "event:/KoseiHelper/Crossover/BalloonExplode",
			outline = false,
			canReappear = true,
			collectionEffects = false, 
			multiplicative = false,
			hitboxWidth = 12,
			hitboxHeight = 12,
			hitboxXOffset = -6,
			hitboxYOffset = -8,
			particleColor = "e7005bff",
		}
	}
}

BalloonTripCollectable.fieldInformation = {
	pointsGiven = {
		fieldType = "integer"
	},
	hitboxWidth = {
		fieldType = "integer"
	},
	hitboxHeight = {
		fieldType = "integer"
	},
	hitboxXOffset = {
		fieldType = "integer"
	},
	hitboxYOffset = {
		fieldType = "integer"
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	particleColor = {
		fieldType = "color",
		useAlpha = true
	}
}

--function BalloonTripCollectable.selection(room, entity)
--    local width, height = 16, 16
--    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height)
--end

return BalloonTripCollectable
