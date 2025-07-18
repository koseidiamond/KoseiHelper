local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")

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
		thickness = 1,
		resolution = 500,
		textSize = 1,
		message = "hello",
		texture = "characters/bird/Recover03",
		scaleX = 1,
		scaleY = 1,
		rotation = 0
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
	},
	thickness = {
		minimumValue = 0.00001
	},
	resolution = {
		fieldType = "integer",
		minimumValue = 1,
		maximumValue = 99999
	}
}

function DebugMapTile.ignoredFields(entity)
	local ignored = {
	"thickness",
	"resolution",
	"textSize",
	"message",
	"texture",
	"scaleX",
	"scaleY",
	"rotation"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.shape == "Circle" then
		doNotIgnore("thickness")
		doNotIgnore("resolution")
	end
	if entity.shape == "Text" then
		doNotIgnore("textSize")
		doNotIgnore("message")
	end
	if entity.shape == "Decal" then
		doNotIgnore("texture")
		doNotIgnore("scaleX")
		doNotIgnore("scaleY")
		doNotIgnore("rotation")
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

function DebugMapTile.draw(room, entity, viewport)

	local colorHex = entity.color or DebugMapTile.color
    local r, g, b = hexToRGB(colorHex)
	love.graphics.setColor(r, g, b)
	
	if entity.shape == "Decal" then
		local debugDecal = drawableSprite.fromTexture(entity.texture, entity)
		if debugDecal ~= nil then
			debugDecal:draw()
		end
	else
		if entity.shape == "Tile" then
			love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
		elseif entity.shape == "Circle" then
			love.graphics.circle("line", entity.x + entity.width / 2, entity.y + entity.height / 2, entity.width / 2)
		elseif entity.shape == "Text" then
			love.graphics.print(entity.message or "Text", entity.x, entity.y)
		end
	end
	love.graphics.setColor(1, 1, 1)
end

function DebugMapTile.scale(room, entity)
    return { entity.scaleX or 1, entity.scaleY or 1 }
end
function DebugMapTile.rotation(room, entity)
    return (entity.rotation or 0) * math.pi / 180
end

function DebugMapTile.canResize(room, entity)
	if entity.shape == "Text" or entity.shape == "Decal" then
		return {false,false}
	elseif entity.shape == "Circle" then
		return {true,false}
	elseif entity.shape == "Tile" then
		return {true,true}
	end
end

return DebugMapTile