local fakeTilesHelper = require("helpers.fake_tiles")

local SwapTilesetTrigger = {}

SwapTilesetTrigger.name = "KoseiHelper/SwapTilesetTrigger"
SwapTilesetTrigger.depth = 100
SwapTilesetTrigger.category = "visual"

SwapTilesetTrigger.placements = {
	{
		name = "SwapTilesetTrigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		fromTileset = "3",
		toTileset = "n",
		flag = "",
		bgTiles = false,
		alternate = true,
		flash = false,
		flashColor = "FFFFFF",
		sound = "event:/none"
		}
	}
}

function SwapTilesetTrigger.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"flashColor"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.flash == true then
		doNotIgnore("flashColor")
	end
	return ignored
end

SwapTilesetTrigger.fieldInformation = function(entity)
    local orig = fakeTilesHelper.getFieldInformation("toTileset", false, "tilesFg")(entity)

    if entity.bgTiles then
        orig = fakeTilesHelper.getFieldInformation("toTileset", true, "tilesBg")(entity)
    end
    for k,v in pairs(orig) do
        orig["fromTileset"] = v
    end
    orig["triggerMode"] = {
        options = {
			"OnEnter",
			"OnLeave",
			"OnStay"
		},
        editable = false
    }
	orig["flashColor"] = {
		fieldType = "color"
	}
	orig["sound"] = {
        options = {
		"event:/none",
		"event:/new_content/game/10_farewell/glitch_long",
		"event:/new_content/game/10_farewell/glitch_medium",
		"event:/new_content/game/10_farewell/glitch_short"
		},
        editable = true
    }
    return orig
end

return SwapTilesetTrigger