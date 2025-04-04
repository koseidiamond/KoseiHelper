local drawing = require("utils.drawing")
local utils = require("utils")
local drawableLine = require("structs.drawable_line")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local CustomCobweb = {}

CustomCobweb.name = "KoseiHelper/CustomCobweb"
CustomCobweb.nodeLimits = {1, -1}
CustomCobweb.depth = -1

function CustomCobweb.depth(room,entity)
	return entity.depth
end

CustomCobweb.nodeVisibility = "never"

CustomCobweb.fieldInformation = {
    color = {
        fieldType = "color"
    },
	edgeColor = {
        fieldType = "color"
    },
	offshoots = {
		minimumValue = 0,
		maximumValue = 1
	},
	edgeColorAlpha = {
		minimumValue = 0,
		maximumValue = 1
	},
	thickness = {
		minimumValue = 0
	},
	waveMultiplier = {
		minimumValue = 0
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    }
}

CustomCobweb.placements = {
    name = "CustomCobweb",
    placementType = "line",
    data = {
        color = "696A6A",
		edgeColor = "0f0e17",
		edgeColorAlpha = 0.2,
		waveMultiplier = 1,
		offshoots = 0.4,
		removeIfNotColliding = true,
		thickness = 1,
		depth = -1
    }
}

CustomCobweb.fieldOrder = {
	"x",
	"y",
	"color",
	"edgeColor",
	"edgeColorAlpha",
	"depth",
	"thickness",
	"offshoots",
	"waveMultiplier",
	"removeIfNotColliding"
}

local function spritesFromMiddle(middle, target, color)
    local coords = {target.x, target.y}
    local control = {
        (middle[1] + coords[1]) / 2,
        (middle[2] + coords[2]) / 2 + 4
    }
    local points = drawing.getSimpleCurve(coords, middle, control)

    return drawableLine.fromPoints(points, color, 1)
end

local defaultColor = {41 / 255, 42 / 255, 41 / 255}

function CustomCobweb.sprite(room, entity)
    local color = defaultColor

    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)

        if success then
            color = {r, g, b}
        end
    end

    local nodes = entity.nodes or {}
    local firstNode = nodes[1] or entity

    local start = {entity.x, entity.y}
    local stop = {firstNode.x, firstNode.y}
    local control = {
        (start[1] + stop[1]) / 2,
        (start[2] + stop[2]) / 2 + 4
    }
    local middle = {drawing.getCurvePoint(start, stop, control, 0.5)}
    local sprites = {}

    for _, node in ipairs(nodes) do
        table.insert(sprites, spritesFromMiddle(middle, node, color))
    end

    -- Use entity rather than start since drawFromMiddle uses x and y
    table.insert(sprites, spritesFromMiddle(middle, entity, color))

    return table.flatten(sprites)
end

function CustomCobweb.selection(room, entity)
    local main = utils.rectangle(entity.x - 2, entity.y - 2, 5, 5)
    local nodes = {}

    if entity.nodes then
        for i, node in ipairs(entity.nodes) do
            nodes[i] = utils.rectangle(node.x - 2, node.y - 2, 5, 5)
        end
    end

    return main, nodes
end

return CustomCobweb