local drawableSpriteStruct = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local enums = require("consts.celeste_enums")

local textures = {"wood", "dream", "temple", "templeB", "cliffside", "reflection", "core", "moon"}

local function getTexture(entity)
    return entity.texture and entity.texture ~= "default" and entity.texture or "wood"
end

local FallingPlatform = {}

FallingPlatform.name = "KoseiHelper/FallingPlatform"
FallingPlatform.depth = -9000
FallingPlatform.canResize = {true, false}

FallingPlatform.fieldInformation = {
    texture = {
        options = textures
    },
    surfaceIndex = {
        options = enums.tileset_sound_ids,
        fieldType = "integer"
    },
	fallDelay = { minimumValue = 0 },
	fallSpeed = { minimumValue = 0 }
}
FallingPlatform.placements = {
	{
		name = "FallingPlatform",
		data = {
			width = 8,
            texture = "wood",
            surfaceIndex = -1,
			fallDelay = 0.2,
			fallSpeed = 150,
			blockImage = false,
			fallingSound = "event:/none"
		}
	},
	{
		name = "FallingDonut",
		data = {
			width = 16,
            texture = "KoseiHelper/DonutBlock",
            surfaceIndex = -1,
			fallDelay = 0.2,
			fallSpeed = 150,
			blockImage = true,
			fallingSound = "event:/none"
		}
	}
}

function FallingPlatform.ignoredFields(entity)
	local ignored = {
		"_name",
		"_id",
		"blockImage"
		}
	return ignored
end

function FallingPlatform.sprite(room, entity)
    local textureRaw = getTexture(entity)
    local texture = "objects/jumpthru/" .. textureRaw

    local x, y = entity.x or 0, entity.y or 0
    local width = entity.width or 8
    local blockImage = entity.blockImage or false

    -- For blockImage true, use 16x16 blocks instead of 8x8 tiles
    local spriteWidth = blockImage and 16 or 8
    local spriteHeight = blockImage and 16 or 8

    local startX, startY = math.floor(x / 8) + 1, math.floor(y / 8) + 1
    local stopX = startX + math.floor(width / spriteWidth) - 1
    local len = stopX - startX

    local sprites = {}

    for i = 0, len do
        local quadX, quadY
        if blockImage then
            quadX = 0
            quadY = 0
        else
            if i == 0 then
                quadX = 0
                quadY = room.tilesFg.matrix:get(startX - 1, startY, "0") ~= "0" and 0 or 8
            elseif i == len then
                quadY = room.tilesFg.matrix:get(stopX + 1, startY, "0") ~= "0" and 0 or 8
                quadX = 16
            else
                quadX = 8
                quadY = 8
            end
        end
        local sprite = drawableSpriteStruct.fromTexture(texture, entity)
        sprite:setJustification(0, 0)
        sprite:addPosition(i * spriteWidth, 0)
        if blockImage then
            sprite:useRelativeQuad(0, 0, spriteWidth, spriteHeight)
        else
            sprite:useRelativeQuad(quadX, quadY, 8, 8)
        end

        table.insert(sprites, sprite)
    end

    return sprites
end


function FallingPlatform.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width, 8)
end

return FallingPlatform