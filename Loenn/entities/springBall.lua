local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local SpringBall = {}

SpringBall.name = "KoseiHelper/SpringBall"
SpringBall.depth = -100
SpringBall.placements = {
    name = "Spring Ball",
    data = {
        speed = 200,
        orientation = "WallRight",
        sineLength = 4,
        sineSpeed = 0.5,
        vertical = false,
        color = "FFFFFF",
        visible = true,
        spawnSound = "event:/game/04_cliffside/snowball_spawn",
		offset = 0,
        trackTheo = false,
		flag = ""
    }
}

SpringBall.fieldInformation = {
    orientation = {
        options = {"Floor", "WallLeft", "WallRight" },
        editable = false
    },
    color = {
        fieldType = "color"
    }
}

function SpringBall.selection(room, entity)
    local width, height = 24, 24
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height - 1)
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function SpringBall.sprite(room, entity)
    local orientation = entity.orientation
    local colorHex = entity.color or SpringBall.color
    local r, g, b = hexToRGB(colorHex)
    local texturePath

    if orientation == "WallRight" then
        texturePath = "objects/KoseiHelper/Balls/ballControllerLeft"
    elseif orientation == "WallLeft" then
        texturePath = "objects/KoseiHelper/Balls/ballControllerRight"
    else
        texturePath = "objects/KoseiHelper/Balls/ballController"
    end

    local sprite = drawableSprite.fromTexture(texturePath, entity)
    sprite:setColor(r, g, b, 1)

    local arrowTexturePath = "objects/KoseiHelper/Balls/ballArrow00"
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

    if arrowSprite then
        arrowSprite:setColor(r, g, b, 1)
    end

    local sprites = {sprite}
    if arrowSprite then
        table.insert(sprites, arrowSprite)
    end

    if entity.trackTheo then
        local theoTexturePath = "objects/KoseiHelper/Balls/ballControllerTheo"
        local theoSprite = drawableSprite.fromTexture(theoTexturePath, entity)
        theoSprite:setColor(r, g, b, 1)
        table.insert(sprites, theoSprite)
    end

    return sprites
end

return SpringBall
