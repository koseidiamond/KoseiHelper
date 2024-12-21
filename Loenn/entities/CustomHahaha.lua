local drawableSprite = require("structs.drawable_sprite")

local CustomHahaha = {}

CustomHahaha.name = "KoseiHelper/CustomHahaha"
CustomHahaha.depth = -10001
CustomHahaha.nodeLineRenderType = "line"
CustomHahaha.nodeLimits = {1, 1}
CustomHahaha.placements = {
    name = "CustomHahaha",
    data = {
        ifset = "",
        triggerLaughSfx = true,
		sprite = "characters/oldlady/KoseiHelper/",
		sound = "event:/KoseiHelper/laugh_oneho"
    }
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