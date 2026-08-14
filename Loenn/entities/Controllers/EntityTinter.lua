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
			affectedEntities = "Glider",
			entityIDs = "",
			tint = "FFFFFF",
			allEntities = true,
			everyFrame = true,
			transitionUpdate = false,
			global = false,
			alpha = true,
			sprite = true,
			image = true,
			tiles = true,
			animationIDs = "",
			untintIfAnimChanged = true,
			counter = false,
			sliderMode = false,
			sliderCounterName = "KoseiHelper_tinterNumbers",
			sliderCounterMinValue = 0,
			sliderCounterMaxValue = 10,
			maxColor = "FF0000",
			flag = "",
			onlyOnce = false
		}
	},
	{
		name = "EntityTinterSlider",
		data = {
			affectedEntities = "Glider",
			entityIDs = "",
			tint = "FFFFFF",
			allEntities = true,
			everyFrame = true,
			transitionUpdate = false,
			global = false,
			alpha = true,
			sprite = true,
			image = true,
			tiles = true,
			animationIDs = "",
			untintIfAnimChanged = true,
			counter = false,
			sliderMode = true,
			sliderCounterName = "KoseiHelper_tinterNumbers",
			sliderCounterMinValue = 0,
			sliderCounterMaxValue = 10,
			maxColor = "FF0000",
			flag = "",
			onlyOnce = false
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
	"flag",
	"allEntities",
	"everyFrame",
	"global",
	"transitionUpdate",
	"counter",
	"alpha",
	"sprite",
	"image",
	"tiles",
	"onlyOnce",
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
	"maxColor",
	"onlyOnce"
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
		doNotIgnore("onlyOnce")
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
	local r, g, b, a
	
	if entity.sliderMode then -- draws outer rectangle
		r, g, b, a = hexToRGBA(entity.maxColor or "FF0000")
		if entity.alpha then
			love.graphics.setColor(r, g, b, a)
		else
			love.graphics.setColor(r, g, b, 1)
		end
		love.graphics.rectangle("fill", entity.x - 6, entity.y - 6, 12, 12)
	end
	
	r, g, b, a = hexToRGBA(entity.tint or "FFFFFF") -- draws inner rectangle
	if entity.alpha then
		love.graphics.setColor(r, g, b, a)
	else
		love.graphics.setColor(r, g, b, 1)
	end
	love.graphics.rectangle("fill", entity.x - 5, entity.y - 5, 10, 10)
	
	love.graphics.setColor(1, 1, 1, 1) -- reset colors
	
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
	
	if entity.sliderMode then
		if entity.counter then
			love.graphics.print("(Counter: "..entity.sliderCounterName..")", entity.x + 6, entity.y)
		else
			love.graphics.print("(Slider: "..entity.sliderCounterName..")", entity.x + 6, entity.y)
		end
	end
	
	if entity.flag ~= "" then
		if entity.sliderMode then
			love.graphics.print("(Flag: "..entity.flag..")", entity.x + 6, entity.y + 8)
		else
			love.graphics.print("(Flag: "..entity.flag..")", entity.x + 6, entity.y)
		end
	end
	
    local tinterSprite = drawableSprite.fromTexture("objects/KoseiHelper/EntityTinter/Tinter", entity)
    tinterSprite:draw()
end

function EntityTinter.selection(room, entity)
    return utils.rectangle(entity.x - 5, entity.y - 5, 10, 10)
end

return EntityTinter