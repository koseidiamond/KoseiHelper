local drawableText = require("structs.drawable_text")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local DashLimiterController = {}

DashLimiterController.name = "KoseiHelper/DashLimiterController"
DashLimiterController.depth = -10500
DashLimiterController.texture = "objects/KoseiHelper/Controllers/DashLimiterController"
DashLimiterController.placements = {
	{
		name = "Dash Limiter Controller",
		data = {
			count = 3,
			demoDashOnly = false
		}
	}
}

DashLimiterController.fieldInformation = {
    count = {
        fieldType = "integer",
		minimumValue = 1
    }
}

function DashLimiterController.sprite(room, entity)
    local texturePath
    if entity.demoDashOnly then
        texturePath = "objects/KoseiHelper/Controllers/DemoLimiterController"
    else
        texturePath = "objects/KoseiHelper/Controllers/DashLimiterController"
    end
    local sprite = drawableSprite.fromTexture(texturePath, entity)
    local sprites = {sprite}
    local countText = tostring(entity.count)
    local textColor = {1, 1, 1}
    local textSprite = drawableText.fromText(countText, entity.x-1, entity.y-20, entity.width, entity.height, nil, 1, textColor)
    table.insert(sprites, textSprite)
    return sprites
end

return DashLimiterController