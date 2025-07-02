local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableSprite = require("structs.drawable_sprite")
local enums = require("consts.celeste_enums")
local utils = require("utils")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local CustomGoldenBlock = {}

CustomGoldenBlock.name = "KoseiHelper/CustomGoldenBlock"

function CustomGoldenBlock.depth(room,entity)
	return entity.depth
end

CustomGoldenBlock.warnBelowSize = {16, 16}
CustomGoldenBlock.placements = {
    name = "CustomGoldenBlock",
    data = {
        width = 16,
        height = 16,
		sinkOffset = 12,
		goBackTimer = 0.1,
		iconTexture = "collectables/goldberry/idle00",
		blockTexture = "objects/goldblock",
		surfaceSoundIndex = 32,
		depth = -10000,
		occludesLight = true,
		appearMode = "GoldenBerry",
		drawOutline = true,
		appearDistance = 80,
		safe = false,
		blockTint = "FFFFFF",
		iconTint = "FFFFFF",
		flag = "KoseiHelper_GoldenBlock"
    }
}

CustomGoldenBlock.fieldInformation = {
	goBackTimer = { minimumValue = 0 },
	surfaceSoundIndex = {
        options = enums.tileset_sound_ids,
        fieldType = "integer"
    },
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	appearDistance = {
		fieldType = "integer",
		minimumValue = "1"
	},
	appearMode = {
		options = {
			"GoldenBerry",
			"AllBerries",
			"NonGoldenBerries",
			"BerriesAndKeys",
			"OnlyKeys",
			"Flag"
		},
		editable = false
	},
	blockTint = {
        fieldType = "color"
    },
	iconTint = {
        fieldType = "color"
    },
	blockTexture = {
		{
			fieldType = "string"
		},
		options =
		{
		"objects/goldblock",
		"objects/KoseiHelper/goldblock/greyscaleblock"
		},
		editable = true
	},
	iconTexture = {
		{
			fieldType = "string"
		},
		options =
		{
		"collectables/goldberry/idle00",
		"collectables/strawberry/normal00",
		"collectables/key/idle00",
		"collectables/moonBerry/normal00"
		},
		editable = true
	}
}

function CustomGoldenBlock.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"flag"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.appearMode == "Flag" then
		doNotIgnore("flag")
	end
	return ignored
end

local ninePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    fillMode = "repeat"
}

CustomGoldenBlock.fieldOrder = {
	"x",
	"y",
	"width",
	"height",
	"blockTexture",
	"iconTexture",
	"sinkOffset",
	"goBackTimer",
	"appearDistance",
	"depth",
	"surfaceSoundIndex",
	"appearMode",
	"flag",
	"blockTint",
	"iconTint",
	"allBerryTypes",
	"drawOutline",
	"safe",
	"occludesLight"
}

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function CustomGoldenBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24
	
	local colorHex = entity.blockTint or CustomGoldenBlock.blockTint
	local r, g, b = hexToRGB(colorHex)

    local ninePatch = drawableNinePatch.fromTexture(entity.blockTexture, ninePatchOptions, x, y, width, height)
    local middleSprite = drawableSprite.fromTexture(entity.iconTexture, entity)
	
	if entity.blockTint then
        local success, r, g, b = utils.parseHexColor(entity.blockTint)
        if success then
            color = {r, g, b}
        end
    end
	if entity.iconTint then
        local success, r, g, b = utils.parseHexColor(entity.iconTint)
        if success then
            colorIcon = {r, g, b}
        end
    end
	
    ninePatch:setColor(color)
	middleSprite:setColor(colorIcon)
	
    local sprites = ninePatch:getDrawableSprite()

    middleSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
    table.insert(sprites, middleSprite)

    return sprites
end

return CustomGoldenBlock