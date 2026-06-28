local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local conveyor = {}

conveyor.name = "KoseiHelper/Conveyor"
conveyor.canResize = {true, false}
conveyor.minimumSize = {8, 8}

conveyor.placements = {
    {
        name = "conveyor",
        data = {
            width = 16,
			moveSpeed = 40,
			reverseFlag = "KoseiHelper_conveyor_right",
			spriteRoot = "objects/KoseiHelper/Conveyor/blue",
			conveyorSfx = "event:/env/local/09_core/conveyor_idle"
        }
    }
}

conveyor.fieldInformation = {
	spriteRoot = {
		options = {
			"objects/KoseiHelper/Conveyor/blue",
			"objects/KoseiHelper/Conveyor/green",
			"objects/KoseiHelper/Conveyor/pink",
			"objects/KoseiHelper/Conveyor/red",
			"objects/KoseiHelper/Conveyor/yellow",
			"objects/KoseiHelper/Conveyor/special"
		},
		editable = true
	}
}

function conveyor.sprite(room, entity)
    local sprites = {}
	
	local bodyEdgeTexture = entity.spriteRoot .. "_edge00"
	local bodyMidTexture = entity.spriteRoot .. "_middle00"

    local width = entity.width or 16
	local hasEdges = drawableSprite.fromTexture(bodyEdgeTexture, entity) ~= nil
	
	local middleCount = math.max(1, math.floor(width / 8))
    for i = 0, middleCount - 1 do
        local mid = drawableSprite.fromTexture(bodyMidTexture, entity)
        mid:addPosition(i * 8, 0)
        mid:setJustification(0, 0)
        table.insert(sprites, mid)
    end
    if hasEdges then
        local leftEdge = drawableSprite.fromTexture(bodyEdgeTexture, entity)
        leftEdge:setJustification(0, 0)
        table.insert(sprites, leftEdge)

        local rightEdge = drawableSprite.fromTexture(bodyEdgeTexture, entity)
        rightEdge:addPosition(width, 0)
        rightEdge:setScale(-1, 1)
        rightEdge:setJustification(0, 0)
        table.insert(sprites, rightEdge)
    end

    return sprites
end

function conveyor.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local w = entity.width or 16
    return utils.rectangle(x, y, w, 8)
end

return conveyor