local utils = require("utils")

local DebugRenderer = {}
DebugRenderer.name = "KoseiHelper/DebugRenderer"
DebugRenderer.depth = -1
DebugRenderer.nodeLineRenderType = "line"
DebugRenderer.nodeLimits = {0,1}
DebugRenderer.nodeVisibility= "always"
DebugRenderer.fieldInformation = {
    depth = {
        fieldType = "integer"
    },
    color = {
        fieldType = "color"
    },
    alpha = {
        minimumValue = 0.0,
        maximumValue = 1.0
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
		ellipseSegments = 99
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
	"ellipseSegments"
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
			"Text"
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
	"ellipseSegments"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.shape == "Text" then
		doNotIgnore("font")
		doNotIgnore("fontSize")
		doNotIgnore("message")
	end
	if entity.shape == "Ellipse" then
		doNotIgnore("ellipseSegments")
	end
	return ignored
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

return DebugRenderer