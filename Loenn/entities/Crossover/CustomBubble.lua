local CustomBubble = {}

CustomBubble.name = "KoseiHelper/CustomBubble"
CustomBubble.depth = 0
CustomBubble.texture = "objects/KoseiHelper/Crossover/Bubble/bubble"
CustomBubble.placements = {
    name = "CustomBubble",
	data = {
		speedMult = 1.2,
		sound = "event:/game/06_reflection/feather_bubble_bounce",
		wiggliness = 8,
		floatiness = 2,
		renderSprite = false,
		renderBubble = true,
		spriteID = "flyFeather"
	}
}

return CustomBubble