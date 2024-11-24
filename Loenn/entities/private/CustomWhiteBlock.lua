local drawableNinePatch = require("structs.drawable_nine_patch")
local enums = require("consts.celeste_enums")

local CustomWhiteBlock = {}

CustomWhiteBlock.name = "KoseiHelper/CustomWhiteBlock"
CustomWhiteBlock.depth = 8990
CustomWhiteBlock.warnBelowSize = {24, 24}
CustomWhiteBlock.placements = {
    {
        name = "Custom White Block",
        data = {
            width = 48,
            duckDuration = 3,
            spritePath = "objects/KoseiHelper/CustomWhiteBlock/whiteblock",
            surfaceSoundIndex = 27,
            color = "FFFFFF",
            sound = "event:/game/04_cliffside/whiteblock_fallthru",
            whiteBlockFlag = "whiteBlockFlag"
        }
    }
}

local ninePatchOptions = {
    mode = "fill",
    borderMode = "repeat",
    fillMode = "repeat"
}

CustomWhiteBlock.fieldInformation = {
	surfaceSoundIndex = {
		options = enums.tileset_sound_ids,
        editable = false
	},
	color = {
		fieldType = "color"
	},
	duckDuration = {
		minimumValue = 0
	}
}

function CustomWhiteBlock.sprite(room, entity)
    local blockTexture = entity.spritePath
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 24, 24

    local ninePatch = drawableNinePatch.fromTexture(blockTexture, ninePatchOptions, x, y, width, height)

    return ninePatch
end

function CustomWhiteBlock.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width, 24)
end

local function getSearchPredicate(entity)
    return function(target)
        return entity._name == target._name and entity.index == target.index
    end
end

return CustomWhiteBlock