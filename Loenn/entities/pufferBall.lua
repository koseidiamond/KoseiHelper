local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local PufferBall = {}

PufferBall.name = "KoseiHelper/PufferBall"
PufferBall.depth = -100
PufferBall.placements = {
    name = "Puffer Ball",
    data = {
        speed = 100,
        sineLength = 4,
        sineSpeed = 0.5,
        vertical = false,
		horizontalFix = true,
        spawnSound = "event:/game/04_cliffside/snowball_spawn"
    }
}

function PufferBall.ignoredFields(entity)
	local ignored = {
	"sineLength",
	"sineSpeed"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.horizontalFix == false or entity.vertical == true then
		doNotIgnore("sineLength")
		doNotIgnore("sineSpeed")
	end
	return ignored
end

function PufferBall.texture(room, entity)
    return "objects/KoseiHelper/Balls/pufferBall"
end

return PufferBall