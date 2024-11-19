local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local PufferBall = {}

PufferBall.name = "KoseiHelper/PufferBall"
PufferBall.depth = -100
PufferBall.placements = {
    name = "Puffer Ball",
    data = {
        speed = 200,
        sineLength = 4,
        sineSpeed = 0.5,
        vertical = false,
        spawnSound = "event:/game/04_cliffside/snowball_spawn"
    }
}

function PufferBall.texture(room, entity)
    return "objects/KoseiHelper/Balls/pufferBall"
end

return PufferBall