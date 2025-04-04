local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableSprite = require("structs.drawable_sprite")
local enums = require("consts.celeste_enums")

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
		safe = false
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
			"BerriesAndKeys",
			"OnlyKeys"
		},
		editable = false
	}
}

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
	"allBerryTypes",
	"drawOutline",
	"safe",
	"occludesLight"
}

function CustomGoldenBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local ninePatch = drawableNinePatch.fromTexture(entity.blockTexture, ninePatchOptions, x, y, width, height)
    local middleSprite = drawableSprite.fromTexture(entity.iconTexture, entity)
    local sprites = ninePatch:getDrawableSprite()

    middleSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
    table.insert(sprites, middleSprite)

    return sprites
end

return CustomGoldenBlock