local MaryBlock = {}

MaryBlock.name = "KoseiHelper/MaryBlock"
MaryBlock.depth = -9500

MaryBlock.placements = {
	{
		name = "Mary Block",
		data = {
			outline = false,
			affectTheo = false,
			oneUse = true,
			maryType = "Idle",
		}
	}
}

MaryBlock.fieldInformation = {
	maryType = {
		options = {
			"Potted",
			"Idle",
			"Mary",
			"Bubble"
		},
		editable = false
	}
}

function MaryBlock.selection(room, entity)
    local width, height = 16, 16
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height + 8)
end

function MaryBlock.texture(room, entity)
    local maryType = entity.maryType
    if maryType == "Potted" then
        return "objects/KoseiHelper/MaryBlock/potted00"
    elseif maryType == "Idle" then
        return "objects/KoseiHelper/MaryBlock/idle00"
	elseif maryType == "Mary" then
		return "objects/KoseiHelper/MaryBlock/mary00"
		elseif maryType == "Bubble" then
		return "objects/KoseiHelper/MaryBlock/marybuble00"
    end
end

return MaryBlock
