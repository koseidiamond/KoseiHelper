local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local LightReactiveBlock = {}

LightReactiveBlock.name = "KoseiHelper/LightReactiveBlock"
LightReactiveBlock.depth = 0

function LightReactiveBlock.depth(room,entity)
	return entity.depth
end

function LightReactiveBlock.placements()
    return {
        name = "LightReactiveBlock",
        data = {
            tiletype = fakeTilesHelper.getPlacementMaterial(),
            blendin = false,
            width = 8,
            height = 8,
			depth = -12999,
			tint = "FFFFFF",
			lightLevelForCollidable = 0.8,
			cutoutEffect = true
        }
    }
end

LightReactiveBlock.fieldInformation = function(entity)
	local orig = fakeTilesHelper.getFieldInformation("tiletype")(entity)
	
	orig.tint = {
        fieldType = "color",
        useAlpha = true
    }
	orig.depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {
		{"LightReactiveBlocks", -12999}
		}),
        editable = true
    }
	return orig
end

LightReactiveBlock.fieldOrder= {
	"x",
	"y",
	"width",
	"height",
	"tiletype",
	"tint",
	"depth",
	"lightLevelForCollidable",
	"blendin",
	"cutoutEffect"
}

function LightReactiveBlock.sprite(room, entity)
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

return LightReactiveBlock