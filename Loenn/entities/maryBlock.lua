local MaryBlock = {}

MaryBlock.name = "KoseiHelper/MaryBlock"
MaryBlock.depth = -9500

MaryBlock.placements = {
	{
		name = "Mary Block",
		data = {
			potted = false,
			outline = false,
			affectTheo = false
		}
	}
}

function MaryBlock.selection(room, entity)
    local width, height = 16, 16
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height + 8)
end

function MaryBlock.texture(room, entity)
    local potted = entity.potted
    if potted then
        return "objects/KoseiHelper/MaryBlock/potted00"
    else
        return "objects/KoseiHelper/MaryBlock/idle00"
    end
end

return MaryBlock
