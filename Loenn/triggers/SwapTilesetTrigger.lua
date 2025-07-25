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
		flag = "",
		bgTiles = false
		}
	}
}

SwapTilesetTrigger.fieldInformation = function(entity)
    local orig = fakeTilesHelper.getFieldInformation("toTileset", false, "tilesFg")(entity)

    if entity.bgTiles then
        orig = fakeTilesHelper.getFieldInformation("toTileset", true, "tilesBg")(entity)
    end
    for k,v in pairs(orig) do
        orig["fromTileset"] = v
    end
    orig["triggerMode"] = {
        options = { "OnEnter", "OnLeave", "OnStay" },
        editable = false
    }
    return orig
end

return SwapTilesetTrigger