local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")

local ladder = {}

ladder.name = "KoseiHelper/Ladder"
ladder.canResize = {true, true}
ladder.isInvisible = {false, true}
ladder.drainsStamina = {true, false}
ladder.color = "FFFFFF"
ladder.placements = {
    name = "Ladder",
    data = {
        width = 16,
        height = 32,
        climbSpeed = 60,
        depth = 1,
        color = "FFFFFF", -- Requires not isInvisible
        texture = "objects/KoseiHelper/Crossover/Ladder/ladder", -- Requires not isInvisible
        sound = "event:/KoseiHelper/Crossover/Ladder",
        isInvisible = false,
        canClimbHorizontally = true,
        drainsStamina = false,
        requiresGrabButton = false,
        advancedMode = false,
        -- All below require advancedMode
        staminaDrainage = 1, -- Requires drainsStamina
        horizontalSpeedLimit = 330,
        regrabCooldown = 1,
        leaveLaddersToRegrab = false,
        coyoteTime = false,
        isAttached = false,
        dummyMode = false,
        verticalOffset = 6,
		decelerationValues = 1,
		singleUse = false
    }
}

function ladder.fieldOrder(entity)
    local fieldOrder = {
        "x",
        "y",
        "width",
        "height",
        "climbSpeed",
        "depth",
        "sound",
        "isInvisible",
        "canClimbHorizontally",
        "drainsStamina",
        "requiresGrabButton",
        "advancedMode",
		"isAttached"
    }
    if entity.isInvisible == false then
        table.insert(fieldOrder,"color")
    end
    if entity.isInvisible == false then
        table.insert(fieldOrder,"texture")
    end
    if entity.advancedMode == true then
        table.insert(fieldOrder,"staminaDrainage")
        if entity.leaveLaddersToRegrab == false then -- it doesn't make sense to have a regrab cooldown if you can't regrab
            table.insert(fieldOrder,"regrabCooldown")
        end
        if entity.isInvisible == false then
            table.insert(fieldOrder,"verticalOffset")
        end
		if entity.dummyMode == false then
			table.insert(fieldOrder,"decelerationValues")
		end
        table.insert(fieldOrder,"horizontalSpeedLimit")
        table.insert(fieldOrder,"leaveLaddersToRegrab")
        table.insert(fieldOrder,"coyoteTime")
        table.insert(fieldOrder,"dummyMode")
    end
    return fieldOrder
end

function ladder.ignoredFields(entity)
    local ignored = {
        "_name",
        "_id",
        "originX",
        "originY",
        "color",
        "texture",
        "staminaDrainage",
		"canClimbHorizontally",
        "regrabCooldown",
        "leaveLaddersToRegrab",
        "horizontalSpeedLimit",
        "coyoteTime",
        "dummyMode",
        "verticalOffset",
        "regrabCooldown",
		"decelerationValues"
    }
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
    if entity.isInvisible == false then
        doNotIgnore("texture")
        doNotIgnore("color")
    end
    if entity.leaveLaddersToRegrab == false then
        doNotIgnore("regrabCooldown")
    end
    if entity.advancedMode == true then
        doNotIgnore("staminaDrainage")
        doNotIgnore("regrabCooldown")
        doNotIgnore("horizontalSpeedLimit")
        doNotIgnore("leaveLaddersToRegrab")
        doNotIgnore("coyoteTime")
        doNotIgnore("isAttached")
        doNotIgnore("dummyMode")
        doNotIgnore("regrabCooldown")
		doNotIgnore("canClimbHorizontally")
        if entity.isInvisible == false then
            doNotIgnore("verticalOffset")
        end
		if entity.dummyMode == false then
			doNotIgnore("decelerationValues")
		end
    end
    return ignored
end

ladder.depth = function(room, entity)
    return entity.depth or entity.depth or 1
end


local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function ladder.draw(room, entity)

    local colorHex = entity.color or ladder.color
    local r, g, b = hexToRGB(colorHex)

    love.graphics.setColor(r, g, b)
    love.graphics.rectangle("line", entity.x, entity.y, entity.width, entity.height)

    for i = 0, entity.height / 4 do
        local y = entity.y + (i * 4)
        love.graphics.line(entity.x, y, entity.x + entity.width, y)
    end

    love.graphics.setColor(1, 1, 1)
end

ladder.fieldInformation = {
    color = {
        fieldType = "color"
    }
}

return ladder