local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local MagicPreventionArea = {}

MagicPreventionArea.name = "KoseiHelper/MagicPreventionArea"
MagicPreventionArea.depth = 1

MagicPreventionArea.placements = {
	{
		name = "MagicPreventionArea",
		data = {
		width = 16,
		height = 16
		}
	}
}

function MagicPreventionArea.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1,2), 16) / 255
    local g = tonumber(hex:sub(3,4), 16) / 255
    local b = tonumber(hex:sub(5,6), 16) / 255
    return r, g, b
end

MagicPreventionArea.fillColor = function(room, entity)
    local colorHex = "deb887"
    local r, g, b = hexToRGB(colorHex)
    return { r, g, b, 0.225 }
end
MagicPreventionArea.borderColor = function(room, entity)
    local colorHex = "deb887"
    local r, g, b = hexToRGB(colorHex)
    local brighten = function(c) return math.min(c + 0.2, 1) end
    return { brighten(r), brighten(g), brighten(b), 0.45 }
end

return MagicPreventionArea