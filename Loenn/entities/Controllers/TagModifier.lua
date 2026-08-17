local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local TagModifier = {}

TagModifier.name = "KoseiHelper/TagModifier"
TagModifier.depth = -15001

TagModifier.placements = {
	{
		name = "TagModifier",
		data = {
			affectedEntities = "Glider",
			entityIDs = "",
			flag = "",
			allEntities = true,
			addFrozenUpdate = false,
			addGlobal = false,
			addHUD = false,
			addPauseUpdate = false,
			addPersistent = false,
			addTransitionUpdate = false,
			addSubHUD = false,
			removeFrozenUpdate = false,
			removeGlobal = false,
			removeHUD = false,
			removePauseUpdate = false,
			removePersistent = false,
			removeTransitionUpdate = false,
			removeSubHUD = false
		}
	}
}

TagModifier.fieldInformation = {
	entityIDs = {
		fieldType = "list",
		elementOptions = {
			fieldType = "integer",
			minimumValue = 0
		}
	},
	affectedEntities = {
		fieldType = "list",
		elementOptions = {
			fieldType = "string",
			minimumValue = 0
		}
	}
}

TagModifier.fieldOrder = {
	"x",
	"y",
	"affectedEntities",
	"entityIDs",
	"flag",
	"addFrozenUpdate",
	"removeFrozenUpdate",
	"addGlobal",
	"removeGlobal",
	"addHUD",
	"removeHUD",
	"addPauseUpdate",
	"removePauseUpdate",
	"addPersistent",
	"removePersistent",
	"addTransitionUpdate",
	"removeTransitionUpdate",
	"addSubHUD",
	"removeSubHUD",
	"allEntities"
}

function TagModifier.draw(room, entity, viewport)
	local r, g, b, a
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
	
	if entity.flag ~= "" then
		love.graphics.print("(Flag: "..entity.flag..")", entity.x + 6, entity.y)
	end
	
    local tinterSprite = drawableSprite.fromTexture("objects/KoseiHelper/EntityModifiers/TagModifier", entity)
    tinterSprite:draw()
end

function TagModifier.selection(room, entity)
    return utils.rectangle(entity.x - 5, entity.y - 5, 10, 10)
end

return TagModifier