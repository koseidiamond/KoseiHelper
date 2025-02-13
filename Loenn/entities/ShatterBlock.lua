local fakeTilesHelper = require("helpers.fake_tiles")

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
			dashRequired = true,
			givesCoyote = false
        }
    }
end

ShatterDashBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")
ShatterDashBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return ShatterDashBlock