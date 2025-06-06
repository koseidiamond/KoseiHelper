local drawableRectangle = require("structs.drawable_rectangle")
local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local TilesetOshiroDoor = {}

TilesetOshiroDoor.name = "KoseiHelper/TilesetOshiroDoor"
TilesetOshiroDoor.depth = 0

TilesetOshiroDoor.placements = {
	{
		name = "CustomOshiroDoorTileset",
		data = {
			tiletype = fakeTilesHelper.getPlacementMaterial(),
			bumpSound = "event:/game/03_resort/forcefield_bump",
			singleUse = false,
			debris = false,
			flag = "oshiro_resort_talked_1",
			refillDash = false,
			givesCoyote = false,
			collisionMode = "Vanilla",
			giveFreezeFrames = false,
			destroyAttached = true,
			width = 8,
            height = 8,
			depth = -9000
		}
	}
}

TilesetOshiroDoor.fieldInformation = function(entity)
	local orig = fakeTilesHelper.getFieldInformation("tiletype")(entity)
	orig.collisionMode = {
		options = {
			"Vanilla",
			"Rebound",
			"SideBounce",
			"PointBounce"
			},
		editable = false
	}
	orig.depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    }
	return orig
end

function TilesetOshiroDoor.depth(room,entity)
	return entity.depth
end

TilesetOshiroDoor.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype",false)

return TilesetOshiroDoor
