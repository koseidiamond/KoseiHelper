local utils = require("utils")

local DefrostableBlock = {}

DefrostableBlock.name = "KoseiHelper/DefrostableBlock"
DefrostableBlock.depth = -10050

DefrostableBlock.placements = {
	{
		name = "Defrostable Block",
		data = {
			big = false,
			sprite = "koseiHelper_DefrostableBlock",
			sound = "event:/none"
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

function DefrostableBlock.texture(room, entity)
    local big = entity.big
    if big then
        return "objects/KoseiHelper/DefrostableBlock/BigMelt00"
    else
        return "objects/KoseiHelper/DefrostableBlock/SmallMelt00"
    end
end

return DefrostableBlock
