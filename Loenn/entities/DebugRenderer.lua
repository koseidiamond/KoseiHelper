local utils = require("utils")
local drawing = require("utils.drawing")

local DebugRenderer = {}
DebugRenderer.name = "KoseiHelper/DebugRenderer"
DebugRenderer.depth = -1
DebugRenderer.nodeLineRenderType = "line"
DebugRenderer.nodeLimits = {0,1}
DebugRenderer.nodeVisibility= "selected"
DebugRenderer.fieldInformation = {
    depth = {
        fieldType = "integer"
    },
    color = {
        fieldType = "color"
    }
}
DebugRenderer.placements = {
    name = "DebugRenderer",
    data = {
        width = 8,
        height = 8,
        color = "000000",
		shape = "HollowRectangle",
		flag = "",
		message = "text",
		font = "Consolas12",
		fontSize = 1,
		ellipseSegments = 99,
		imagePath = "characters/bird/Recover03",
		scaled = true,
		nonDebug = false
    }
}

DebugRenderer.fieldOrder = {
	"x",
	"y",
	"width",
	"height",
	"color",
	"shape",
	"flag",
	"message",
	"font",
	"fontSize",
	"ellipseSegments",
	"imagePath",
	"scaled",
	"nonDebug"
}

DebugRenderer.fieldInformation = {
	shape = {
		options = {
			"HollowRectangle",
			"FilledRectangle",
			"Circle",
			"Ellipse",
			"Point",
			"Line",
			"Text",
			"Image"
		},
		editable = false
	},
	font = {
		options = {
			"Consolas12",
			"Renogare"
		},
		editable = false
	},
	color = { fieldType = "color" },
	ellipseSegments = {
		fieldType = "integer",
		minimumValue = 3
	}
}

function DebugRenderer.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"font",
	"fontSize",
	"message",
	"ellipseSegments",
	"imagePath",
	"scaled",
	"color"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.shape ~= "Image" then
		doNotIgnore("color")
	end
	if entity.shape == "Text" then
		doNotIgnore("font")
		doNotIgnore("fontSize")
		doNotIgnore("message")
	end
	if entity.shape == "Ellipse" then
		doNotIgnore("ellipseSegments")
	end
	if entity.shape == "Image" then
		doNotIgnore("imagePath")
		doNotIgnore("scaled")
	end
	return ignored
end

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function DebugRenderer.color(room, entity)
    local color = {0, 0, 0}
    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)
        if success then
            color = {r, g, b}
        end
    end
    return color
end

function DebugRenderer.draw(room, entity)
    -- Set color from entity
    local colorHex = entity.color or DebugRenderer.color
    local r, g, b = hexToRGB(colorHex)

 love.graphics.setColor(r, g, b)
    -- Draw shape based on entity's shape type
    if entity.shape == "HollowRectangle" then
        love.graphics.rectangle("line", entity.x, entity.y, entity.width, entity.height)
    elseif entity.shape == "FilledRectangle" then
        love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
    elseif entity.shape == "Circle" then
        love.graphics.circle("line", entity.x + entity.width / 2, entity.y + entity.height / 2, entity.width / 2)
    elseif entity.shape == "Ellipse" then
        love.graphics.ellipse("line", entity.x + entity.width / 2, entity.y + entity.height / 2, entity.width / 2, entity.height / 2)
    elseif entity.shape == "Point" then
        love.graphics.rectangle("fill", entity.x, entity.y, 1, 1)
    elseif entity.shape == "Line" then
        if entity.node and entity.node.x and entity.node.y then
			love.graphics.rectangle("line", entity.x, entity.y, entity.node.x, entity.node.y)
        end
    elseif entity.shape == "Text" then
        love.graphics.print(entity.message or "Text", entity.x, entity.y)
    elseif entity.shape == "Image" then
		love.graphics.print("?", entity.x , entity.y, 0, 4, 4)
	end
	 love.graphics.setColor(1, 1, 1)
end

return DebugRenderer