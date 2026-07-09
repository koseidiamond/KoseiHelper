local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local EntityTinter = {}

EntityTinter.name = "KoseiHelper/EntityTinter"
EntityTinter.depth = -15001

EntityTinter.placements = {
	{
		name = "EntityTinter",
		data = {
			affectedEntities = "Celeste.Glider",
			entityIDs = "",
			tint = "FFFFFF",
			allEntities = true,
			everyFrame = true,
			transitionUpdate = false,
			global = false,
			red = true,
			green = true,
			blue = true,
			alpha = true,
			sprite = true,
			image = true,
			animationIDs = "",
			untintIfAnimChanged = true,
			counter = false,
			sliderMode = false,
			sliderCounterName = "KoseiHelper_tinterNumbers",
			sliderCounterMinValue = 0,
			sliderCounterMaxValue = 10,
			maxColor = "FF0000"
		}
	},
	{
		name = "EntityTinterSlider",
		data = {
			affectedEntities = "Celeste.Glider",
			entityIDs = "",
			tint = "FFFFFF",
			allEntities = true,
			everyFrame = true,
			transitionUpdate = false,
			global = false,
			red = true,
			green = true,
			blue = true,
			alpha = true,
			sprite = true,
			image = true,
			animationIDs = "",
			untintIfAnimChanged = true,
			counter = false,
			sliderMode = true,
			sliderCounterName = "KoseiHelper_tinterNumbers",
			sliderCounterMinValue = 0,
			sliderCounterMaxValue = 10,
			maxColor = "FF0000"
		}
	}
}

EntityTinter.fieldInformation = {
	tint = {
        fieldType = "color",
		useAlpha = true
    },
	entityIDs = {
		fieldType = "list",
		elementOptions = {
			fieldType = "integer",
			minimumValue = 0
		}
	},
	animationIDs = {
		fieldType = "list",
		elementOptions = {
			fieldType = "string"
		}
	},
	maxColor = {
        fieldType = "color",
		useAlpha = true
    }
}

EntityTinter.fieldOrder = {
	"x",
	"y",
	"affectedEntities",
	"entityIDs",
	"animationIDs",
	"sliderCounterName",
	"tint",
	"maxColor",
	"sliderCounterMinValue",
	"sliderCounterMaxValue",
	"allEntities",
	"everyFrame",
	"global",
	"transitionUpdate",
	"counter",
	"red",
	"green",
	"blue",
	"alpha",
	"sprite",
	"image",
	"untintIfAnimChanged"
}

function EntityTinter.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"everyFrame",
	"spriteAnimations",
	"untintIfAnimChanged",
	"sliderMode",
	"counter",
	"sliderCounterName",
	"sliderCounterMinValue",
	"sliderCounterMaxValue",
	"maxColor"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.sliderMode == false then
		doNotIgnore("everyFrame")
	else
		doNotIgnore("counter")
		doNotIgnore("sliderCounterName")
		doNotIgnore("sliderCounterMinValue")
		doNotIgnore("sliderCounterMaxValue")
		doNotIgnore("maxColor")
	end
	if entity.sprite == true then
		doNotIgnore("spriteAnimations")
		if entity.animationIDs ~= "" then
			doNotIgnore("untintIfAnimChanged")
		end
	end
	return ignored
end

local function hexToRGBA(hex)
    if #hex == 6 then
        return
            tonumber(hex:sub(1,2), 16) / 255,
            tonumber(hex:sub(3,4), 16) / 255,
            tonumber(hex:sub(5,6), 16) / 255,
            1
    elseif #hex == 8 then
        return
            tonumber(hex:sub(1,2), 16) / 255,
            tonumber(hex:sub(3,4), 16) / 255,
            tonumber(hex:sub(5,6), 16) / 255,
            tonumber(hex:sub(7,8), 16) / 255
    end
    return 1, 1, 1, 1
end

function EntityTinter.draw(room, entity, viewport)
	
	local r, g, b, a = hexToRGBA(entity.tint or "FFFFFF")
	love.graphics.setColor(r, g, b, a)
	love.graphics.rectangle("fill", entity.x - 5, entity.y - 5, 10, 10)
	love.graphics.setColor(1, 1, 1, 1)
	
	-- print entity list
	local text = (entity.affectedEntities or "?"):gsub("%s*,%s*", ",\n")
	local font = love.graphics.getFont()
	local lineSpacing = font:getHeight() * font:getLineHeight()
	local split = {}
	local maxWidth = 0
	for line in text:gmatch("[^\n]+") do
		table.insert(split, line)
		maxWidth = math.max(maxWidth, font:getWidth(line))
	end
	local y = entity.y - #split * lineSpacing - 5
	for i, line in ipairs(split) do
		local w = font:getWidth(line)
		love.graphics.print(line, entity.x - w / 2, y + (i - 1) * lineSpacing)
	end
	
	-- silly triangles
	if entity.red == true then
		love.graphics.setColor(1, 0, 0, 1)
		love.graphics.polygon("fill", {entity.x + 6, entity.y - 5, entity.x + 6, entity.y - 1, entity.x + 8, entity.y - 3})
	end
	if entity.green == true then
		love.graphics.setColor(0, 1, 0, 1)
		love.graphics.polygon("fill", {entity.x + 6, entity.y - 5, entity.x + 8, entity.y - 3, entity.x + 10, entity.y - 5})
	end
	if entity.blue == true then
		love.graphics.setColor(0, 0, 1, 1)
		love.graphics.polygon("fill", {entity.x + 10, entity.y - 1, entity.x + 6, entity.y - 1, entity.x + 8, entity.y - 3})
	end
	love.graphics.setColor(1, 1, 1, 1)
	if entity.alpha == true then
		love.graphics.polygon("fill", {entity.x + 10, entity.y - 5, entity.x + 8, entity.y - 3, entity.x + 10, entity.y - 1})
	end
	
	if entity.sliderMode then
		if entity.counter then
			love.graphics.print("(counter)", entity.x + 6, entity.y)
		else
			love.graphics.print("(slider)", entity.x + 6, entity.y)
		end
	end
	
    local tinterSprite = drawableSprite.fromTexture("objects/KoseiHelper/EntityTinter/Tinter", entity)
    tinterSprite:draw()
end

function EntityTinter.selection(room, entity)
    return utils.rectangle(entity.x - 5, entity.y - 5, 10, 10)
end

return EntityTinter