local drawing = require("utils.drawing")
local utils = require("utils")
local drawableLine = require("structs.drawable_line")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local CustomWire = {}

CustomWire.name = "KoseiHelper/CustomWire"

CustomWire.placements = {
    name = "CustomWire",
    data = {
        depth = 2000,
        color = "6B6A3C",
		thickness = 1,
		affectedByWind = true,
		attachToSolids = true,
		wobbliness = 1,
		alpha = 1,
		flag = ""
    }
}

CustomWire.fieldInformation = {
    color = {
        fieldType = "color"
    },
	thickness = {
        fieldType = "integer"
    },
	wobbliness = {
		minimumValue = 0
	},
	alpha = {
		minimumValue = 0,
		maximumValue = 1
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    }
}

CustomWire.nodeLimits = {1, 1}
CustomWire.nodeVisibility = "never"

function CustomWire.depth(room,entity)
	return entity.depth
end

local defaultColor = {89 / 255, 88 / 255, 102 / 255}

function CustomWire.sprite(room, entity)
    local color = defaultColor

    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)

        if success then
            color = {r, g, b}
        end
    end

    local firstNode = entity.nodes[1]

    local start = {entity.x, entity.y}
    local stop = {firstNode.x, firstNode.y}
    local control = {
        (start[1] + stop[1]) / 2,
        (start[2] + stop[2]) / 2 + 24
    }

    local points = drawing.getSimpleCurve(start, stop, control)

    return drawableLine.fromPoints(points, color, entity.thickness)
end

function CustomWire.selection(room, entity)
    local main = utils.rectangle(entity.x - 2, entity.y - 2, 5, 5)
    local nodes = {}

    if entity.nodes then
        for i, node in ipairs(entity.nodes) do
            nodes[i] = utils.rectangle(node.x - 2, node.y - 2, 5, 5)
        end
    end

    return main, nodes
end

return CustomWire