local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local MagicInkBox = {}

MagicInkBox.name = "KoseiHelper/MagicInkBox"
MagicInkBox.depth = -1000

function MagicInkBox.depth(room,entity)
	return entity.depth
end

MagicInkBox.fillColor = { 26 / 255, 16 / 255, 56 / 255 }
MagicInkBox.borderColor = { 17 / 255, 10 / 255, 37 / 255 }

MagicInkBox.placements = {
	{
		name = "MagicInkBox",
		data = {
		width = 16,
		height = 16,
			depth = -1000,
			pushSpeed = 200,
			singleUse = false,
			bumpSound = "event:/game/03_resort/forcefield_bump"
		}
	}
}

MagicInkBox.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {
		{"MagicInkBox", -1000}
		}),
        editable = true
    }
}

-- todo
--function MagicInkBox.selection(room, entity)
--    local width, height = 16, 16
--    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height)
--end

-- todo
--function MagicInkBox.texture(room, entity)
--    local type = entity.type
--        return "objects/KoseiHelper/Brick/Ice00"
--end

return MagicInkBox
