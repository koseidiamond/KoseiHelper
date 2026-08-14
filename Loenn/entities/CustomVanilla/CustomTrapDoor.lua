local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local drawableRectangle = require("structs.drawable_rectangle")

local CustomTrapdoor = {}

CustomTrapdoor.name = "KoseiHelper/CustomTrapdoor"

function CustomTrapdoor.depth(room,entity)
	return entity.depth
end

CustomTrapdoor.placements = {
    name = "CustomTrapdoor",
	data = {
		sfxTop = "event:/game/03_resort/trapdoor_fromtop",
		sfxBottom = "event:/game/03_resort/trapdoor_frombottom",
		occludesLight = true,
		spriteID = "trapdoor",
		depth = 8999,
		tint = "FFFFFF"
	}
}

CustomTrapdoor.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	spriteID = {
        fieldType = "string",
        options = {
			"trapdoor",
			"KoseiHelper_whitetrapdoor"
		},
        editable = true
    },
	tint = {
        fieldType = "color"
    }
}

local color = {22 / 255, 27 / 255, 48 / 255, 1.0}

function CustomTrapdoor.sprite(room, entity)
    return drawableRectangle.fromRectangle("fill", entity.x, entity.y + 6, 24, 4, color)
end

return CustomTrapdoor