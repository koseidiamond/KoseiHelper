local drawableRectangle = require("structs.drawable_rectangle")
local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local TilesetOshiroDoor = {}

TilesetOshiroDoor.name = "KoseiHelper/TilesetOshiroDoor"
TilesetOshiroDoor.depth = 0

TilesetOshiroDoor.placements = {
	{
		name = "Custom Oshiro Door (Tileset)",
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
			width = 8,
            height = 8
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
	return orig
end

TilesetOshiroDoor.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype",false)

return TilesetOshiroDoor
