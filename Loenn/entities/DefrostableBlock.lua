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
    local size = 16
	if big then
		size = 32
	end
    return utils.rectangle(entity.x - size / 2, entity.y - size / 2, size, size)
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
