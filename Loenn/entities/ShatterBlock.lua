local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local ShatterDashBlock = {}

ShatterDashBlock.name = "KoseiHelper/ShatterDashBlock"
ShatterDashBlock.depth = 0

function ShatterDashBlock.depth(room,entity)
	return entity.depth
end

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
			--verticalSpeed = false,
			shatterAxis = "Horizontal",
			depth = -12999,
			tint = "FFFFFF"
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
	orig.shatterAxis = {
		options = {
			"Horizontal",
			"Vertical",
			"Both"
		},
		editable = false
	}
	orig.FreezeTime = {
		minimumValue = 0
	}
	orig.ShakeTime = {
		minimumValue = 0
	}
	orig.tint = {
        fieldType = "color",
        useAlpha = true
    }
	orig.SpeedDecrease = {
		minimumValue = 0
	}
	orig.depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {
		{"ShatterDashBlocks", -12999}
		}),
        editable = true
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
	"shatterAxis",
	"flagSet",
	"FreezeTime",
	"ShakeTime",
	"SpeedDecrease",
	"SpeedRequirement",
	"tint",
	"depth",
	"blendin",
	"verticalSpeed",
	"canDash",
	"givesCoyote",
	"requireOnlySpeed",
	"permanent",
	"destroyStaticMovers"
}

--ShatterDashBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")

function ShatterDashBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 16, entity.height or 16

    local tint = {1, 1, 1}
    if entity.tint then
        local success, r, g, b = utils.parseHexColor(entity.tint)
        if success then
            tint = {r, g, b}
        end
    end
    return fakeTilesHelper.getEntitySpriteFunction("tiletype", false, "tilesFg", tint, x, y)(room, entity)
end

return ShatterDashBlock