local TheoPetController = {}

TheoPetController.name = "KoseiHelper/TheoPetController"
TheoPetController.depth = -10500
TheoPetController.placements = {
	{
		name = "TheoPetController",
		data = {
		speed = 8,
		jumpStrength = 1
		}
	}
}

function TheoPetController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/TheoPetController"
end

return TheoPetController