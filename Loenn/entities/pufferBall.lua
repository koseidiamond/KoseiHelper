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
        spawnSound = "event:/none",
		offset = 0,
		flag = ""
    }
}

function PufferBall.selection(room, entity)
    local width, height = 24, 24
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height - 1)
end

function PufferBall.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
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

function PufferBall.sprite(room, entity)
	local texturePath = "objects/KoseiHelper/Balls/pufferBall"
	local sprite = drawableSprite.fromTexture(texturePath, entity)

    local arrowTexturePath = "objects/KoseiHelper/Balls/pufferArrow"
    local arrowSprite = nil
	
    if not entity.vertical then
        if entity.speed > 0 then
            arrowSprite = drawableSprite.fromTexture(arrowTexturePath, entity)
            arrowSprite.rotation = -math.pi / 2
        elseif entity.speed < 0 then
            arrowSprite = drawableSprite.fromTexture(arrowTexturePath, entity)
            arrowSprite.rotation = math.pi / 2
        end
    elseif entity.vertical then
        if entity.speed > 0 then
            arrowSprite = drawableSprite.fromTexture(arrowTexturePath, entity)
        elseif entity.speed < 0 then
            arrowSprite = drawableSprite.fromTexture(arrowTexturePath, entity)
            arrowSprite.rotation = math.pi
        end
    end
	local sprites = {sprite}
    if arrowSprite then
        table.insert(sprites, arrowSprite)
    end
	return sprites
end

return PufferBall