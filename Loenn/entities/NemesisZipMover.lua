local drawableSprite = require("structs.drawable_sprite")
local drawableLine = require("structs.drawable_line")
local drawableNinePatch = require("structs.drawable_nine_patch")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")

local NemesisZipMover = {}

local blockNinePatchOptions = {
    mode = "border",
    borderMode = "repeat"
}

local centerColor = {0, 0, 0}

NemesisZipMover.name = "KoseiHelper/NemesisZipMover"
NemesisZipMover.depth = -9999
NemesisZipMover.nodeVisibility = "never"
NemesisZipMover.nodeLimits = {1, 1}
NemesisZipMover.warnBelowSize = {16, 16}

NemesisZipMover.placements = {
	{
		name = "NemesisZipMover",
		data = {
			width = 16,
            height = 16,
			normalSpeedMultiplier = 1,
			retractionSpeedMultiplier = 1,
			timeBeforeMoving = 0.1,
			timeBeforeRetraction = 0.5,
			timeAfterRetraction = 0.5,
			blockPath = "objects/zipmover/block",
			cogPath = "objects/zipmover/cog",
			lightPath = "objects/zipmover/light",
			innerCogPath = "objects/zipmover/innercog",
			ropeColor = "663931",
			ropeLightColor = "9b6157",
			sfxPath = "event:/game/01_forsaken_city/zip_mover",
			bloomAlpha = 1,
			bloomRadius = 6,
			triggeredByPlayer = false,
			drawBlackBorder = true
		}
	}
}

NemesisZipMover.fieldOrder = {
	"x",
	"y",
	"width",
	"height",
	"normalSpeedMultiplier",
	"retractionSpeedMultiplier",
	"timeBeforeMoving",
	"timeBeforeRetraction",
	"timeAfterRetraction",
	"blockPath",
	"cogPath",
	"lightPath",
	"innerCogPath",
	"sfxPath",
	"ropeColor",
	"ropeLightColor",
	"bloomAlpha",
	"bloomRadius",
	"triggeredByPlayer",
	"drawBlackBorder"
}

NemesisZipMover.fieldInformation = {
	ropeColor = {
		fieldType = "color",
		useAlpha = true
	},
	ropeLightColor = {
		fieldType = "color",
		useAlpha = true
	},
	sfxPath = {
		options = {
			"event:/game/01_forsaken_city/zip_mover",
			"event:/new_content/game/10_farewell/zip_mover"
		},
		editable = true
	},
	blockPath = {
		options = {
			"objects/zipmover/block",
			"objects/zipmover/block2",
			"objects/zipmover/moon/block"
		},
		editable = true
	},
	cogPath = {
		options = {
			"objects/zipmover/cog",
			"objects/zipmover/moon/cog"
		},
		editable = true
	},
	innerCogPath = {
		options = {
			"objects/zipmover/innercog",
			"objects/zipmover/moon/innercog"
		},
		editable = true
	},
	lightPath = {
		options = {
			"objects/zipmover/light",
			"objects/zipmover/moon/light"
		},
		editable = true
	},
	timeBeforeMoving = {
		minimumValue = 0
	},
	timeBeforeRetraction = {
		minimumValue = 0
	},
	timeAfterRetraction = {
		minimumValue = 0
	}
}

local function hexToRGBA(hex)
    if #hex == 6 then
        return
            tonumber(hex:sub(1,2), 16) / 255,
            tonumber(hex:sub(3,4), 16) / 255,
            tonumber(hex:sub(5,6), 16) / 255,
            1
    elseif #hex == 8 then
        return
            tonumber(hex:sub(1,2), 16) / 255,
            tonumber(hex:sub(3,4), 16) / 255,
            tonumber(hex:sub(5,6), 16) / 255,
            tonumber(hex:sub(7,8), 16) / 255
    end
    return 1, 1, 1, 1
end

local function addNodeSprites(sprites, entity, cogTexture, centerX, centerY, centerNodeX, centerNodeY, ropeColor)
    local nodeCogSprite = drawableSprite.fromTexture(cogTexture, entity)

    nodeCogSprite:setPosition(centerNodeX, centerNodeY)
    nodeCogSprite:setJustification(0.5, 0.5)

    local points = {centerX, centerY, centerNodeX, centerNodeY}
    local leftLine = drawableLine.fromPoints(points, ropeColor, 1)
    local rightLine = drawableLine.fromPoints(points, ropeColor, 1)

    leftLine:setOffset(0, 4.5)
    rightLine:setOffset(0, -4.5)
    leftLine.depth = 5000
    rightLine.depth = 5000

    for _, sprite in ipairs(leftLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end
    for _, sprite in ipairs(rightLine:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end

    table.insert(sprites, nodeCogSprite)
end

local function addBlockSprites(sprites, entity, blockTexture, lightsTexture, x, y, width, height)
    local rectangle = drawableRectangle.fromRectangle("fill", x + 2, y + 2, width - 4, height - 4, centerColor)

    local frameNinePatch = drawableNinePatch.fromTexture(blockTexture, blockNinePatchOptions, x, y, width, height)
    local frameSprites = frameNinePatch:getDrawableSprite()
    local lightsSprite = drawableSprite.fromTexture(lightsTexture, entity)

    lightsSprite:addPosition(math.floor(width / 2), 0)
    lightsSprite:setJustification(0.5, 0.0)

    table.insert(sprites, rectangle:getDrawableSprite())

    for _, sprite in ipairs(frameSprites) do
        table.insert(sprites, sprite)
    end
    table.insert(sprites, lightsSprite)
end

function NemesisZipMover.sprite(room, entity)
    local sprites = {}

    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 16, entity.height or 16
    local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)

    local nodes = entity.nodes or {{x = 0, y = 0}}
    local nodeX, nodeY = nodes[1].x, nodes[1].y

    local centerX, centerY = x + halfWidth, y + halfHeight
    local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight

    local blockPath = entity.blockPath or "objects/zipmover/block"
    local cogPath = entity.cogPath or "objects/zipmover/cog"
    local lightPath = entity.lightPath or "objects/zipmover/light"
	
	love.graphics.setColor({0, 0, 0, 0.15})
	love.graphics.rectangle("line", x, y, entity.width, entity.height)
	love.graphics.setColor({1, 1, 1, 1})

	local r, g, b, a = hexToRGBA(entity.ropeColor or "663931")
	local customRopeColor = {r, g, b, a}
    addNodeSprites(sprites, entity, cogPath, centerX, centerY, centerNodeX, centerNodeY, customRopeColor)
    addBlockSprites(sprites, entity, blockPath, lightPath .. "01", x, y, width, height)

    return sprites
end

function NemesisZipMover.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width, height = entity.width or 8, entity.height or 8
    local halfWidth, halfHeight = math.floor(entity.width / 2), math.floor(entity.height / 2)

    local nodes = entity.nodes or {{x = 0, y = 0}}
    local nodeX, nodeY = nodes[1].x, nodes[1].y
    local centerNodeX, centerNodeY = nodeX + halfWidth, nodeY + halfHeight

	local cogPath = entity.cogPath or "objects/zipmover/cog"
    local cogSprite = drawableSprite.fromTexture(cogPath, entity)
    local cogWidth, cogHeight = cogSprite.meta.width, cogSprite.meta.height

    local mainRectangle = utils.rectangle(x, y, width, height)
    local nodeRectangle = utils.rectangle(centerNodeX - math.floor(cogWidth / 2), centerNodeY - math.floor(cogHeight / 2), cogWidth, cogHeight)

    return mainRectangle, {nodeRectangle}
end

return NemesisZipMover