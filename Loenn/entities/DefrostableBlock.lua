local utils = require("utils")
local mods = require("mods")

local DefrostableBlock = {}

DefrostableBlock.name = "KoseiHelper/DefrostableBlock"
function DefrostableBlock.depth(room,entity)
	return entity.depth
end

DefrostableBlock.placements = {
	{
		name = "Defrostable Block",
		data = {
			big = false,
			sprite = "koseiHelper_DefrostableBlock",
			sound = "event:/none",
			depth = -10050,
		},
	},
}

function DefrostableBlock.selection(room, entity)
	local big = entity.big
	if big then
		return utils.rectangle(entity.x - 32 / 2, entity.y - 32 / 2, 32, 32)
	else
		return utils.rectangle(entity.x - 16 / 2, entity.y - 16 / 2, 16, 16)
	end
end

function DefrostableBlock.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    }
}

function DefrostableBlock.texture(room, entity)
    local big = entity.big
    if big then
        return "objects/KoseiHelper/DefrostableBlock/BigMelt00"
    else
        return "objects/KoseiHelper/DefrostableBlock/SmallMelt00"
    end
end

return DefrostableBlock
