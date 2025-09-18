local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local EntitySpawnArea = {}

EntitySpawnArea.name = "KoseiHelper/EntitySpawnArea"
EntitySpawnArea.depth = 1

EntitySpawnArea.placements = {
	{
		name = "EntitySpawnArea",
		data = {
		width = 16,
		height = 16,
		color = "deb887"
		}
	}
}

EntitySpawnArea.fieldInformation = {
	color = {
        fieldType = "color"
    }
}

function EntitySpawnArea.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1,2), 16) / 255
    local g = tonumber(hex:sub(3,4), 16) / 255
    local b = tonumber(hex:sub(5,6), 16) / 255
    return r, g, b
end

EntitySpawnArea.fillColor = function(room, entity)
    local colorHex = entity.color or "deb887"
    local r, g, b = hexToRGB(colorHex)
    return { r, g, b, 0.225 }
end
EntitySpawnArea.borderColor = function(room, entity)
    local colorHex = entity.color or "deb887"
    local r, g, b = hexToRGB(colorHex)
    local brighten = function(c) return math.min(c + 0.2, 1) end
    return { brighten(r), brighten(g), brighten(b), 0.45 }
end

return EntitySpawnArea