local drawableNinePatch = require("structs.drawable_nine_patch")
local utils = require("utils")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local CustomTempleCrackedBlock = {}

CustomTempleCrackedBlock.name = "KoseiHelper/CustomTempleCrackedBlock"
CustomTempleCrackedBlock.warnBelowSize = {24, 24}

function CustomTempleCrackedBlock.depth(room,entity)
	return entity.depth
end

CustomTempleCrackedBlock.placements = {
    name = "CustomTempleCrackedBlock",
    data = {
        width = 24,
        height = 24,
        persistent = false,
        texture = "objects/KoseiHelper/CustomTempleCrackedBlock/breakBlock",
        debris = "1",
        tint = "d42c19",
        breakSound = "event:/game/05_mirror_temple/crackedwall_vanish",
		prebreakSound = "event:/KoseiHelper/crackedwall_prebreak",
        health = 1,
		destroyStaticMovers = false,
		depth = -9000
    }
}

CustomTempleCrackedBlock.fieldInformation = {
    tint = {
        fieldType = "color",
		useAlpha = true
    },
	health = {
		fieldType = "integer",
		minimumValue = 1
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    }
}

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

local ninePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    fillMode = "repeat"
}


function CustomTempleCrackedBlock.sprite(room, entity)
    local color = {0.831, 0.173, 0.098}
	local blockTexture = entity.texture.."00"
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24
    local ninePatch = drawableNinePatch.fromTexture(blockTexture, ninePatchOptions, x, y, width, height)
    if entity.tint then
        local success, r, g, b = utils.parseHexColor(entity.tint)
        if success then
            color = {r, g, b}
        end
    end
    ninePatch:setColor(color)

    return ninePatch
end

function CustomTempleCrackedBlock.color(room, entity)
    return {0.831, 0.173, 0.098}
end

return CustomTempleCrackedBlock
