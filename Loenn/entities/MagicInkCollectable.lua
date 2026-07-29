local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local MagicInkCollectable = {}

MagicInkCollectable.name = "KoseiHelper/MagicInkCollectable"

MagicInkCollectable.texture = "objects/KoseiHelper/Refills/MagicInkCollectable/ink00"

function MagicInkCollectable.texture(room, entity)
	if entity.spriteID == "koseiHelper_inkBottleCollectable" then
		return "objects/KoseiHelper/Refills/MagicInkCollectable/inkBottle00"
	else
		return "objects/KoseiHelper/Refills/MagicInkCollectable/ink00"
	end
end

function MagicInkCollectable.depth(room,entity)
	return entity.depth
end

MagicInkCollectable.placements = {
	{
		name = "MagicInkCollectable",
		data = {
			depth = -100,
			inkGiven = 100,
			canOverfill = true,
			spriteID = "koseiHelper_inkCollectable",
			sound = "event:/game/general/diamond_touch",
			canReappear = true
		}
	}
}

MagicInkCollectable.fieldInformation = {
	inkGiven = {
		fieldType = "integer"
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	spriteID = {
		options = {
		"koseiHelper_inkCollectable",
		"koseiHelper_inkBottleCollectable"
		},
		editable = true
	}
}

return MagicInkCollectable
