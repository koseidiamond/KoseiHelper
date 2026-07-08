local drawableRectangle = require("structs.drawable_rectangle")

local EntityTinter = {}

EntityTinter.name = "KoseiHelper/EntityTinter"

EntityTinter.placements = {
    name = "EntityTinter",
	data = {
		affectedEntities = "Celeste.Glider",
		tint = "FFFFFF",
		allEntities = false,
		everyFrame = false,
		transitionUpdate = false,
		global = false
	}
}

EntityTinter.fieldInformation = {
	tint = {
        fieldType = "color",
		useAlpha = true
    }
}

function EntityTinter.texture(room, entity)
    return "objects/KoseiHelper/EntityTinter/Tinter"
end

return EntityTinter