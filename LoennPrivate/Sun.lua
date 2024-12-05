local Sun = {}

Sun.name = "KoseiHelper/Sun"
Sun.depth = 100000

Sun.placements = {
	{
		name = "Angry Sun",
		data = {
			onlyWhileHot = false
		}
	}
}

function Sun.selection(room, entity)
    return utils.rectangle(entity.x - 8, entity.y - 8, 16, 16)
end

function Sun.texture(room, entity)
    return "objects/KoseiHelper/Sun/Chill"
end

return Sun