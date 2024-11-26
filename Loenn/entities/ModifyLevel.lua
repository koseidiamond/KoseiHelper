local ModifyLevel = {}

ModifyLevel.name = "KoseiHelper/ModifyLevel"
ModifyLevel.depth = -9500

ModifyLevel.placements = {
	{
		name = "ModifyLevel",
		data = {
		}
	}
}

function ModifyLevel.texture(room, entity)
    return "objects/KoseiHelper/MaryBlock/idle00"
end

return ModifyLevel