local MaryBlock = {}

MaryBlock.name = "KoseiHelper/MaryBlock"
MaryBlock.depth = -9500

MaryBlock.placements = {
	{
		name = "Mary Block",
		data = {
			potted = false
		},
	},
}

function MaryBlock.texture(room, entity)
    local potted = entity.potted
    if potted then
        return "objects/KoseiHelper/MaryBlock/potted00"
    else
        return "objects/KoseiHelper/MaryBlock/idle00"
    end
end

return MaryBlock
