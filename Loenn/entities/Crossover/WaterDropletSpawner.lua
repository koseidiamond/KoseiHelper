local utils = require("utils")
local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local droplets = {}

droplets.name = "KoseiHelper/WaterDropletSpawner"
droplets.placements = {
    name = "droplet",
    data = {
        width = 8,
        height = 1,
        interval = 1,
		dropletColor = "1C4FA1F2",
		depth = -50000,
		sound = "",
		maxSpeed = 10,
		ignoreSolids = false,
		lifetime = 5
    }
}

function droplets.depth(room,entity)
	return entity.depth
end

droplets.fieldInformation = {
	dropletColor = {
		fieldType = "color",
		useAlpha = true
		},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	lifetime = {
		minimumValue = 0
	}
}

droplets.warnBelowSize = { 8, 1 }

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function droplets.depth (room, entity)
	return entity.depth or -100
end

function droplets.borderColor (room, entity)
	local color = {0.0, 0.0, 1.0, 0.3}
	if entity.dropletColor then
		local success, r, g, b = utils.parseHexColor(entity.dropletColor)
		if success then
			color = {r, g, b}
		end
	end
	return color
end

function droplets.fillColor (room, entity)
	local color = {0.0, 0.0, 1.0, 0.3}
	if entity.dropletColor then
		local success, r, g, b = utils.parseHexColor(entity.dropletColor)
		if success then
			color = {r, g, b}
		end
	end
	return color
end

droplets.canResize = {true, false}

return droplets