local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local CustomOshiroDoor = {}

CustomOshiroDoor.name = "KoseiHelper/CustomOshiroDoor"
CustomOshiroDoor.depth = 0

CustomOshiroDoor.placements = {
	{
		name = "Custom Oshiro Door",
		data = {
			sprite = "koseiHelper_CustomOshiroDoor",
			bumpSound = "event:/game/03_resort/forcefield_bump",
			color = "483D8B",
			singleUse = false,
			wiggleDuration = 1,
			wiggleFrequency = 1,
			wiggleScale = 1,
			flag = "oshiro_resort_talked_1",
			width = 32,
			height = 32,
			rebound = false,
			refillDash = false,
			givesCoyote = false
		}
	}
}

CustomOshiroDoor.canResize = {false, false}

CustomOshiroDoor.fieldInformation = {
    wiggleDuration = { minimumValue = 0 },
	wiggleFrequency = { minimumValue = 0 },
	wiggleScale = { minimumValue = 0 },
    color = { fieldType = "color" }
}

function CustomOshiroDoor.ignoredFields(entity)
	local ignored = {
		"_name",
		"_id"
	}
	return ignored
end

function CustomOshiroDoor.selection(room, entity)
    return utils.rectangle(entity.x - width, entity.y - height, width, height)
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function CustomOshiroDoor.sprite(room, entity)
    local texturePath = "objects/KoseiHelper/door/ghost_door00"
	local colorHex = entity.color or CustomOshiroDoor.color
    local r, g, b = hexToRGB(colorHex)
	local texture = drawableSprite.fromTexture(texturePath, entity)
	texture:setColor(r, g, b, 1)
	texture:addPosition(entity.width / 2, entity.height / 2)
	
    return texture
end

return CustomOshiroDoor
