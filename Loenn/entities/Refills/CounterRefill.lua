local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local CounterRefill = {}

CounterRefill.name = "KoseiHelper/CounterRefill"

function CounterRefill.depth(room,entity)
	return entity.depth
end

CounterRefill.placements = {
	{
		name = "CounterRefill",
		data = {
			oneUse = false,
			sprite = "objects/KoseiHelper/Refills/CounterRefill/",
			decrease = false,
			respawnTime = 2.5,
			returnSound = "",
			touchSound = "",
			depth = -100
		}
	}
}

CounterRefill.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    }
}


function CounterRefill.texture(room, entity)
    return entity.sprite .. "idle00"
end

return CounterRefill