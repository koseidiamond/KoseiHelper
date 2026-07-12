local utils = require("utils")
local drawing = require("utils.drawing")
local drawableSprite = require("structs.drawable_sprite")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local DebugRenderer = {}
DebugRenderer.name = "KoseiHelper/DebugRenderer"

function DebugRenderer.depth(room,entity)
	return entity.depth
end

DebugRenderer.nodeLineRenderType = "line"
DebugRenderer.nodeLimits = {0,1}
DebugRenderer.nodeVisibility= "always"

DebugRenderer.placements = {
    name = "ShapeRenderer",
	alternativeName = "DebugRenderer",
    data = {
        width = 8,
        height = 8,
        color = "ffffffff",
		shape = "HollowRectangle",
		flag = "",
		message = "text",
		font = "Consolas12",
		fontSize = 1,
		ellipseSegments = 99,
		imagePath = "characters/bird/Recover03",
		scaled = true,
		nonDebug = true,
		depth = -999999,
		gui = false
    }
}

DebugRenderer.fieldOrder = {
	"x",
	"y",
	"width",
	"height",
	"color",
	"alpha",
	"depth",
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
			"DottedRectangle",
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
	color = {
		fieldType = "color",
		useAlpha = true
		},
	alpha = {
		minimumValue = 0,
		maximumValue = 1
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
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
	"scaled"
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
	if entity.shape == "Image" then
		doNotIgnore("imagePath")
		doNotIgnore("scaled")
	end
	return ignored
end

local function hexToRGBA(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
	local a = (tonumber(hex:sub(7, 8), 16) or 255) / 255
    return r, g, b, a
end

function DebugRenderer.color(room, entity)
    local color = {0, 0, 0, 1}
    if entity.color then
        local success, r, g, b, a = utils.parseHexColor(entity.color)
        if success then
            color = {r, g, b, a}
        end
    end
    return color
end

function DebugRenderer.draw(room, entity)
    -- Set color from entity
    local colorHex = entity.color or DebugRenderer.color
    local r, g, b, a = hexToRGBA(colorHex)
	local nodes = entity.nodes or {{x = 0, y = 0}}
	local debugImage
	
	love.graphics.setColor(r, g, b, a)
    -- Draw shape based on entity's shape type
    if entity.shape == "HollowRectangle" then
        love.graphics.rectangle("line", entity.x, entity.y, entity.width, entity.height)
	elseif entity.shape == "DottedRectangle" then
		love.graphics.rectangle("line", entity.x, entity.y, entity.width, entity.height)
		love.graphics.rectangle("fill", entity.x, entity.y, 2, 2)
		love.graphics.rectangle("fill", entity.x + entity.width -2, entity.y, 2, 2)
		love.graphics.rectangle("fill", entity.x, entity.y + entity.height - 2, 2, 2)
		love.graphics.rectangle("fill", entity.x + entity.width - 2, entity.y + entity.height - 2, 2, 2)
    elseif entity.shape == "FilledRectangle" then
        love.graphics.rectangle("fill", entity.x, entity.y, entity.width, entity.height)
    elseif entity.shape == "Circle" then
        love.graphics.circle("line", entity.x + entity.width / 2, entity.y + entity.height / 2, entity.width / 2)
    elseif entity.shape == "Ellipse" then
        love.graphics.ellipse("line", entity.x + entity.width / 2, entity.y + entity.height / 2, entity.width / 2, entity.height / 2)
    elseif entity.shape == "Point" then
        love.graphics.rectangle("fill", entity.x, entity.y, 1, 1)
    elseif entity.shape == "Line" then
			love.graphics.rectangle("fill", entity.x - 1.5, entity.y - 1.5, 3, 3) -- todo offset
			if nodes and nodes[1] then
				love.graphics.rectangle("fill", nodes[1].x - 1.5, nodes[1].y - 1.5, 3, 3)
				love.graphics.line(entity.x, entity.y, nodes[1].x, nodes[1].y)
			end
    elseif entity.shape == "Text" then
        love.graphics.print(entity.message or "Text", entity.x, entity.y)
    elseif entity.shape == "Image" then
		local extraData = utils.deepcopy(entity)
		extraData["atlas"] = "Gui"
		if entity.gui then
			debugImage = drawableSprite.fromTexture(entity.imagePath, extraData)
		else
			debugImage = drawableSprite.fromTexture(entity.imagePath, entity)
		end
		if debugImage == nil then -- todo: test
			debugImage = drawableSprite.fromTexture("objects/KoseiHelper/Other/__fallback", entity)
		end
		if entity.scaled then
			local scaleX = entity.width / debugImage.meta.width or 1
			local scaleY = entity.height / debugImage.meta.height or 1
			debugImage:setScale(scaleX, scaleY)
			debugImage:setJustification(0, 0)
		else
			if entity.gui then
				debugImage:setScale(0.1666667, 0.1666667)
				debugImage:setJustification(0,0)
			else
				debugImage:setScale(1, 1)
				debugImage:setJustification(0.5, 0.5)
			end
		end
		
		debugImage:draw(a)
	end
	 love.graphics.setColor(1, 1, 1, 1)
end

function DebugRenderer.nodeSprite() end

function DebugRenderer.nodeRectangle(room, entity, node, nodeIndex, viewport)
    return utils.rectangle(node.x, node.y, entity.width, entity.height)
end

return DebugRenderer