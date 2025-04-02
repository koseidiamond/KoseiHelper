local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local ShatterDashBlock = {}

ShatterDashBlock.name = "KoseiHelper/ShatterDashBlock"
ShatterDashBlock.depth = 0

function ShatterDashBlock.placements()
    return {
        name = "ShatterDashBlock",
        data = {
            tiletype = fakeTilesHelper.getPlacementMaterial(),
            blendin = false,
            canDash = true,
            permanent = false,
			FreezeTime = 0.05,
			SpeedRequirement = 300,
			SpeedDecrease = 0,
			ShakeTime = 0.3,
            width = 8,
            height = 8,
			givesCoyote = false,
			requireOnlySpeed = false,
			flagSet = "",
			substractSpeedMode = "Substract",
			destroyStaticMovers = false,
			verticalSpeed = false
        }
    }
end

ShatterDashBlock.fieldInformation = function(entity)
	local orig = fakeTilesHelper.getFieldInformation("tiletype")(entity)
	orig.substractSpeedMode = {
		options = {
			"Substract",
			"Multiply",
			"Set"
		},
		editable = false
	}
	orig.FreezeTime = {
		minimumValue = 0
	}
	orig.ShakeTime = {
		minimumValue = 0
	}
	orig.SpeedDecrease = {
		minimumValue = 0
	}
	return orig
end

-- ShatterDashBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

ShatterDashBlock.fieldOrder= {
	"x",
	"y",
	"width",
	"height",
	"tiletype",
	"substractSpeedMode",
	"flagSet",
	"FreezeTime",
	"ShakeTime",
	"SpeedDecrease",
	"SpeedRequirement",
	"blendin",
	"verticalSpeed",
	"canDash",
	"givesCoyote",
	"requireOnlySpeed",
	"permanent",
	"destroyStaticMovers"
}

ShatterDashBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")

return ShatterDashBlock