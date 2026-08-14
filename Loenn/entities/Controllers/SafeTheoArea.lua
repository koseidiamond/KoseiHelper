local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local SafeTheoArea = {}

SafeTheoArea.name = "KoseiHelper/SafeTheoArea"
SafeTheoArea.depth = -12100

SafeTheoArea.placements = {
	{
		name = "SafeTheoArea",
		data = {
		width = 16,
		height = 16
		}
	}
}

function SafeTheoArea.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1,2), 16) / 255
    local g = tonumber(hex:sub(3,4), 16) / 255
    local b = tonumber(hex:sub(5,6), 16) / 255
    return r, g, b
end

SafeTheoArea.fillColor = function(room, entity)
    local colorHex = "873724"
    local r, g, b = hexToRGB(colorHex)
    return { r, g, b, 0.15 }
end
SafeTheoArea.borderColor = function(room, entity)
    local colorHex = "331e16"
    local r, g, b = hexToRGB(colorHex)
    local brighten = function(c) return math.min(c + 0.2, 1) end
    return { brighten(r), brighten(g), brighten(b), 0.35 }
end

return SafeTheoArea