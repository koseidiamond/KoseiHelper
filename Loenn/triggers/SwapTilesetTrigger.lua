local fakeTilesHelper = require("helpers.fake_tiles")

local SwapTilesetTrigger = {}

SwapTilesetTrigger.name = "KoseiHelper/SwapTilesetTrigger"
SwapTilesetTrigger.depth = 100

SwapTilesetTrigger.placements = {
	{
		name = "SwapTilesetTrigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		fromTileset = "3",
		toTileset = "n",
		flag = ""
		}
	}
}

SwapTilesetTrigger.fieldInformation = function (entity) return {
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	fromTileset = {
		options = fakeTilesHelper.getTilesOptions(entity.fg and "tilesFg"),
		editable = true
	},
	toTileset = {
		options = fakeTilesHelper.getTilesOptions(entity.fg and "tilesFg"),
		editable = true
	}
}
end

return SwapTilesetTrigger
