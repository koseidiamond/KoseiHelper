local FallDamageController = {}

FallDamageController.name = "KoseiHelper/FallDamageController"
FallDamageController.depth = -10500
FallDamageController.placements = {
	{
		name = "FallDamageController",
		alternativeName = "BarDisplay",
		data = {
		terminalSpeed = 160,
		fallTimeThreshold = 0.5,
		fallDamageThreshold = 100,
		fallDamageAmount = 100,
		playerHealth = 100,
		persistent = true
		}
	}
}

function FallDamageController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/FallDamageController"
end

return FallDamageController