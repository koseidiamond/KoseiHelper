local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local FlagRefill = {}

FlagRefill.name = "KoseiHelper/FlagRefill"

function FlagRefill.depth(room,entity)
	return entity.depth
end
FlagRefill.placements = {
	{
		name = "FlagRefill",
		data = {
			oneUse = false,
			sprite = "objects/KoseiHelper/Refills/FlagRefill/",
			respawnTime = 2.5,
			returnSound = "",
			touchSound = "",
			depth = -100
		}
	}
}

FlagRefill.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    }
}

function FlagRefill.texture(room, entity)
    return entity.sprite .. "idle00"
end

return FlagRefill
