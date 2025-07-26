local utils = require("utils")
local drawableSprite = require("structs.drawable_sprite")

local DebugMapTile = {}
DebugMapTile.name = "KoseiHelper/DebugMapTile"

DebugMapTile.placements = {
    name = "DebugMapTile",
    data = {
        width = 8,
        height = 8,
		above = true,
        color = "6969ee",
		shape = "Tile",
		thickness = 1,
		resolution = 500,
		hollow = false,
		textSize = 1,
		message = "hello",
		altFont = false,
		texture = "decals/1-forsakencity/flag00",
		scaleX = 1,
		scaleY = 1,
		rotation = 0,
		animationFrames = 0,
		animationSpeed = 10,
		gui = false,
		angle = 0,
		length = 8
    }
}

DebugMapTile.fieldOrder = {
	"x",
	"y",
	"shape",
	"color",
	"width",
	"height"
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
	animationFrames = {
		minimumValue = 0,
		fieldType = "integer"
	},
	animationSpeed = {
		minimumValue = 1
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
	"hollow",
	"textSize",
	"message",
	"altFont",
	"texture",
	"scaleX",
	"scaleY",
	"rotation",
	"animationFrames",
	"animationSpeed",
	"gui",
	"angle",
	"length"
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
	if entity.shape == "Tile" then
		doNotIgnore("hollow")
	end
	if entity.shape == "Text" then
		doNotIgnore("textSize")
		doNotIgnore("message")
		doNotIgnore("altFont")
	end
	if entity.shape == "Decal" then
		doNotIgnore("texture")
		doNotIgnore("scaleX")
		doNotIgnore("scaleY")
		doNotIgnore("rotation")
		doNotIgnore("animationFrames")
		doNotIgnore("animationSpeed")
		doNotIgnore("gui")
	end
	if entity.shape == "Line" then
		doNotIgnore("angle")
		doNotIgnore("length")
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
		local texPath = entity.texture
		if (entity.animationFrames or 0) > 0 then
			texPath = texPath .. "00"
		end
		local debugDecal = drawableSprite.fromTexture(texPath, entity)
		if debugDecal then
			debugDecal:draw()
		end
	else
		if entity.shape == "Tile" then
			if entity.hollow then
				love.graphics.rectangle("line", entity.x, entity.y, entity.width, entity.height)
			else
				love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
			end
		elseif entity.shape == "Circle" then
			love.graphics.circle("line", entity.x + entity.width / 2, entity.y + entity.height / 2, entity.width / 2)
		elseif entity.shape == "Text" then
			if entity.message == "" then
				love.graphics.print("Text", entity.x, entity.y)
			else
				love.graphics.print(entity.message or "Text", entity.x, entity.y)
			end
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

function DebugMapTile.depth(room, entity)
	if entity.above == true then
		return -99999
	else
		return 9999
	end
end

function onResize(room, entity, offsetX, offsetY, directionX, directionY)
	if entity.shape == "Circle" then
		entity.height = entity.width
	end
end

return DebugMapTile