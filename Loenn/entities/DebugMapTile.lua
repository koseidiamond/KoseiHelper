local utils = require("utils")
local drawing = require("utils.drawing")

local DebugMapTile = {}
DebugMapTile.name = "KoseiHelper/DebugMapTile"
DebugMapTile.depth = 1
DebugMapTile.placements = {
    name = "DebugMapTile",
    data = {
        width = 8,
        height = 8,
        color = "6969ee",
		shape = "Tile",
		textSize = 1,
		message = "hello"
    }
}
DebugMapTile.fieldInformation = {
    color = {
        fieldType = "color"
    },
	shape = {
		options = {
			"Tile",
			"Circle",
			"Text",
			"Decal"
		},
		editable = false
	},
	textSize = {
		minimumValue = 0.00001
	}
}

function DebugMapTile.ignoredFields(entity)
	local ignored = {
	"textSize",
	"message"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.shape == "Text" then
		doNotIgnore("textSize")
		doNotIgnore("message")
	end
	return ignored
end

function DebugMapTile.color(room, entity)
    local color = {0.411, 0.411, 0.933}
    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)
        if success then
            color = {r, g, b}
        end
    end
    return color
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function DebugMapTile.draw(room, entity)
    -- Set color from entity
    local colorHex = entity.color or DebugMapTile.color
    local r, g, b = hexToRGB(colorHex)
	
	love.graphics.setColor(r, g, b)
    -- Draw shape based on entity's shape type
    if entity.shape == "Tile" then
        love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
    elseif entity.shape == "Circle" then
        love.graphics.circle("line", entity.x + entity.width / 2, entity.y + entity.height / 2, entity.width / 2)
    elseif entity.shape == "Text" then
        love.graphics.print(entity.message or "Text", entity.x, entity.y)
	end
	 love.graphics.setColor(1, 1, 1)
end

return DebugMapTile