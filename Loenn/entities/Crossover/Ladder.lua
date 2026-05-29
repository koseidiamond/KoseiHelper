local drawableSprite = require("structs.drawable_sprite")
local drawing = require("utils.drawing")
local utils = require("utils")
local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local ladder = {}

ladder.name = "KoseiHelper/Ladder"
ladder.canResize = {true, true}
ladder.isInvisible = {false, true}
ladder.drainsStamina = {true, false}
ladder.color = "FFFFFF"
ladder.placements = {
	{
		name = "Ladder",
		data = {
			width = 16,
			height = 32,
			climbSpeed = 60,
			depth = 1,
			color = "FFFFFF", -- Requires not isInvisible
			texture = "objects/KoseiHelper/Crossover/Ladder/ladder_wood", -- Requires not isInvisible
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
			verticalOffset = 0,
			decelerationValues = 1,
			singleUse = false,
			debrisTexture = "debris/KoseiHelper/tintableDebris"
		}
	},
	{
		name = "LadderAdvanced",
		data = {
			width = 16,
			height = 32,
			climbSpeed = 60,
			depth = 1,
			color = "FFFFFF", -- Requires not isInvisible
			texture = "objects/KoseiHelper/Crossover/Ladder/ladder_wood", -- Requires not isInvisible
			sound = "event:/KoseiHelper/Crossover/Ladder",
			isInvisible = false,
			canClimbHorizontally = true,
			drainsStamina = false,
			requiresGrabButton = false,
			advancedMode = true,
			-- All below require advancedMode
			staminaDrainage = 1, -- Requires drainsStamina
			horizontalSpeedLimit = 330,
			regrabCooldown = 1,
			leaveLaddersToRegrab = false,
			coyoteTime = false,
			isAttached = false,
			dummyMode = false,
			verticalOffset = 0,
			decelerationValues = 1,
			singleUse = false,
			debrisTexture = "debris/KoseiHelper/tintableDebris"
		}
	}
	
}

ladder.fieldOrder = {
        "x",
        "y",
        "width",
        "height",
        "climbSpeed",
        "depth",
        "sound",
		"color",
		"texture",
		"verticalOffset",
		"staminaDrainage",
		"regrabCooldown",
		"horizontalSpeedLimit",
		"decelerationValues",
		"debrisTexture",
        "isInvisible",
        "canClimbHorizontally",
        "drainsStamina",
        "requiresGrabButton",
		"isAttached",
		"advancedMode"
}

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
		"decelerationValues",
		"debrisTexture",
		"advancedMode"
    }
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.singleUse == true then
		doNotIgnore("debrisTexture")
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

function ladder.depth(room,entity)
	return entity.depth
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
        fieldType = "color",
		useAlpha = true
    },
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	texture = {
		options = {
			"objects/KoseiHelper/Crossover/Ladder/ladder_dirt",
			"objects/KoseiHelper/Crossover/Ladder/ladder_cement",
			"objects/KoseiHelper/Crossover/Ladder/ladder_girder",
            "objects/KoseiHelper/Crossover/Ladder/ladder_wood",
			"objects/KoseiHelper/Crossover/Ladder/ladder_temple",
            "objects/KoseiHelper/Crossover/Ladder/ladder_metal",
			"objects/KoseiHelper/Crossover/Ladder/ladder_ice",
			"objects/KoseiHelper/Crossover/Ladder/ladder_scifi",
			"objects/KoseiHelper/Crossover/Ladder/ladder_grayscale",
			"objects/KoseiHelper/Crossover/Ladder/ladder_kevin",
			"objects/KoseiHelper/Crossover/Ladder/ladder_net",
			"objects/KoseiHelper/Crossover/Ladder/ladder_cobweb",
			"objects/KoseiHelper/Crossover/Ladder/ladder_railing",
			"objects/KoseiHelper/Crossover/Ladder/ladder_template"
        },
        editable = true
	},
	sound = {
		options = {
			"event:/KoseiHelper/Crossover/Ladder",
			"event:/char/madeline/handhold",
			"event:/none"
		},
		editable = true
	},
	verticalOffset = {
		fieldType = "integer"
	}
}

return ladder