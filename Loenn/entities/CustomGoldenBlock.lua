local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableSprite = require("structs.drawable_sprite")
local enums = require("consts.celeste_enums")

local CustomGoldenBlock = {}

CustomGoldenBlock.name = "KoseiHelper/CustomGoldenBlock"
CustomGoldenBlock.depth = -10000
CustomGoldenBlock.warnBelowSize = {16, 16}
CustomGoldenBlock.placements = {
    name = "CustomGoldenBlock",
    data = {
        width = 16,
        height = 16,
		sinkOffset = 12,
		startSinkingTimer = 0.1,
		iconTexture = "collectables/goldberry/idle00",
		blockTexture = "objects/goldblock",
		surfaceSoundIndex = 32,
		depth = -10000,
		occludesLight = true,
		allBerryTypes = false,
		drawOutline = true
    }
}

CustomGoldenBlock.fieldInformation = {
	startSinkingTimer = { minimumValue = 0 },
	surfaceSoundIndex = {
        options = enums.tileset_sound_ids,
        fieldType = "integer"
    },
	depth = { fieldType = "integer" }
}

local ninePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    fillMode = "repeat"
}

local blockTexture = "objects/goldblock"
local middleTexture = "collectables/goldberry/idle00"

function CustomGoldenBlock.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, entity.height or 24

    local ninePatch = drawableNinePatch.fromTexture(entity.blockTexture, ninePatchOptions, x, y, width, height)
    local middleSprite = drawableSprite.fromTexture(middleTexture, entity)
    local sprites = ninePatch:getDrawableSprite()

    middleSprite:addPosition(math.floor(width / 2), math.floor(height / 2))
    table.insert(sprites, middleSprite)

    return sprites
end

return CustomGoldenBlock