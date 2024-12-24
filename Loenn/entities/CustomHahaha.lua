local drawableSprite = require("structs.drawable_sprite")

local CustomHahaha = {}

CustomHahaha.name = "KoseiHelper/CustomHahaha"
CustomHahaha.depth = -10001
CustomHahaha.nodeLineRenderType = "line"
CustomHahaha.nodeLimits = {1, 1}
CustomHahaha.placements = {
    name = "CustomHahaha",
    data = {
        flag = "VivHelper/IsPlayerAlive",
        synchronizedSfx = false,
		sprite = "characters/oldlady/KoseiHelper/",
		sound = "event:/KoseiHelper/laugh_oneho",
		timeForHahaha = 1.5,
		timeForHa = 0.6,
		timeForSfx = 0.4,
		depth = -10001,
		left = false,
		groupSize = 3
    }
}

CustomHahaha.fieldInformation = {
	depth = { fieldType = "integer" },
	groupSize = { fieldType = "integer" }
}

local texture = "characters/oldlady/ha00"
local spriteOffsets = {
    {-11, -1},
    {0, 0},
    {11, -1}
}

function CustomHahaha.sprite(room, entity)
    local sprites = {}

    for _, offset in ipairs(spriteOffsets) do
        local sprite = drawableSprite.fromTexture(texture, entity)

        sprite:addPosition(offset[1], offset[2])
        table.insert(sprites, sprite)
    end

    return sprites
end

return CustomHahaha