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
		height = 16,
		drainInkWhileInside = false,
		drainRate = 1
		}
	}
}

function MagicPreventionArea.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"drainRate",
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.drainInkWhileInside == true then
		doNotIgnore("drainRate")
	end
	return ignored
end

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