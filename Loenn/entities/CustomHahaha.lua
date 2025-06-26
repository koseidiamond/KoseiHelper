local drawableSprite = require("structs.drawable_sprite")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local CustomHahaha = {}

CustomHahaha.name = "KoseiHelper/CustomHahaha"
CustomHahaha.depth = -10001
CustomHahaha.placements = {
    name = "CustomHahaha",
    data = {
        flag = "VivHelper/IsPlayerAlive",
        synchronizedSfx = false,
		sprite = "characters/oldlady/KoseiHelper/",
		spriteFileName = "ha",
		spriteCount = 2,
		spriteDelay = 0.15,
		sound = "event:/KoseiHelper/laugh_oneho",
		timeForHahaha = 1.5,
		timeForHa = 0.6,
		timeForSfx = 0.4,
		distance = 60,
		sineAmplitude = 1,
		depth = -10001,
		left = false,
		vertical = false,
		groupSize = 3,
		tint = "FFFFFF",
		noDisappearAnim = false
    }
}

function CustomHahaha.depth(room,entity)
	return entity.depth
end

CustomHahaha.fieldInformation = {
	depth = { fieldType = "integer" },
	tint = {
        fieldType = "color"
    },
	spriteCount = { fieldType = "integer" },
	groupSize = { fieldType = "integer" },
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	sound = {
		options = {
			"event:/KoseiHelper/laugh_oneho",
			"event:/char/granny/laugh_oneha",
			"event:/none"
		},
		editable = true
	},
	sprite = {
		options = {
			"characters/oldlady/KoseiHelper/",
			"characters/oldlady/"
		},
		editable = true
	}
}

local spriteOffsets = {
    {-11, -1},
    {0, 0},
    {11, -1}
}

function CustomHahaha.sprite(room, entity)
    local sprites = {}
	local texture = entity.sprite .. entity.spriteFileName .. "00"
    for _, offset in ipairs(spriteOffsets) do
        local sprite = drawableSprite.fromTexture(texture, entity)

        sprite:addPosition(offset[1], offset[2])
        table.insert(sprites, sprite)
    end

    return sprites
end

return CustomHahaha