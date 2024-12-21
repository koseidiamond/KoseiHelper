local drawableSpriteStruct = require("structs.drawable_sprite")
local enums = require("consts.celeste_enums")
local utils = require("utils")

local TriggerSpinnerConnectionDistanceSquared = 24 * 24
local dustEdgeColor = {1.0, 0.0, 0.0}

local defaultTriggerSpinnerColor = "blue"
local unknownTriggerSpinnerColor = "blue"
local TriggerSpinnerColors = {
    "blue",
    "red",
    "purple",
    "core",
    "rainbow"
}
local colorOptions = {}

for _, color in ipairs(TriggerSpinnerColors) do
    colorOptions[utils.titleCase(color)] = color
end

-- Doesn't have textures directly, handled by code
local customTriggerSpinnerColors = {
    core = "red",
    rainbow = "white"
}

local TriggerSpinner = {}

TriggerSpinner.name = "KoseiHelper/TriggerSpinner"
TriggerSpinner.fieldInformation = {
    color = {
        options = colorOptions,
        editable = false
    }
}
TriggerSpinner.placements = {
    {
        name = "dust_sprite",
        data = {
            color = "blue",
            dust = true,
            attachToSolid = false
        }
    }
}

for _, color in ipairs(TriggerSpinnerColors) do
    table.insert(TriggerSpinner.placements, {
        name = color,
        data = {
            color = color,
            dust = false,
            attachToSolid = false
        }
    })
end

local function getTriggerSpinnerTexture(entity, color, foreground)
    local prefix = (foreground or foreground == nil) and "fg_" or "bg_"

    return "danger/crystal/" .. prefix .. color .. "00"
end

local function getTriggerSpinnerSprite(entity, foreground)
    -- Prevent color from TriggerSpinner to tint the drawable sprite
    local color = string.lower(entity.color or defaultTriggerSpinnerColor)
    local position = {
        x = entity.x,
        y = entity.y
    }

    if customTriggerSpinnerColors[color] then
        color = customTriggerSpinnerColors[color]
    end

    local texture = getTriggerSpinnerTexture(entity, color, foreground)
    local sprite = drawableSpriteStruct.fromTexture(texture, position)

    -- Check if texture color exists, otherwise use default color
    -- Needed because Rainbow and Core colors doesn't have textures
    if sprite then
        return sprite

    else
        texture = getTriggerSpinnerTexture(entity, unknownTriggerSpinnerColor, foreground)

        return drawableSpriteStruct.fromTexture(texture, position)
    end
end

local function getConnectionSprites(room, entity)
    -- TODO - This can create some overlaps, can be improved later

    local sprites = {}

    for _, target in ipairs(room.entities) do
        if target == entity then
            break
        end

        if entity._name == target._name and not target.dust and entity.attachToSolid == target.attachToSolid then
            if utils.distanceSquared(entity.x, entity.y, target.x, target.y) < TriggerSpinnerConnectionDistanceSquared then
                local connectorData = {
                    x = math.floor((entity.x + target.x) / 2),
                    y = math.floor((entity.y + target.y) / 2),
                    color = entity.color
                }
                local sprite = getTriggerSpinnerSprite(connectorData, false)

                sprite.depth = -8499

                table.insert(sprites, sprite)
            end
        end
    end

    return sprites
end

function TriggerSpinner.depth(room, entity)
    return entity.dusty and -50 or -8500
end

function TriggerSpinner.sprite(room, entity)
    local dusty = entity.dust

    if dusty then
        local position = {
            x = entity.x,
            y = entity.y
        }

        local baseTexture = "danger/dustcreature/base00"
        local baseOutlineTexture = "dust_creature_outlines/base00"
        local baseSprite = drawableSpriteStruct.fromTexture(baseTexture, position)
        local baseOutlineSprite = drawableSpriteStruct.fromInternalTexture(baseOutlineTexture, entity)

        baseOutlineSprite:setColor(dustEdgeColor)

        return {
            baseOutlineSprite,
            baseSprite
        }

    else
        local sprites = getConnectionSprites(room, entity)
        local mainSprite = getTriggerSpinnerSprite(entity)

        table.insert(sprites, mainSprite)

        return sprites
    end
end

function TriggerSpinner.selection(room, entity)
    local dusty = entity.dust

    if dusty then
        local baseSprite = drawableSpriteStruct.fromTexture("danger/dustcreature/base00", entity)

        return baseSprite:getRectangle()

    else
        return utils.rectangle(entity.x - 8, entity.y - 8, 16, 16)
    end
end

return TriggerSpinner