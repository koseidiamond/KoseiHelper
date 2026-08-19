local drawableRectangle = require("structs.drawable_rectangle")
local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")
local EntityResizer = {}

EntityResizer.name = "KoseiHelper/EntityResizer"
EntityResizer.depth = -15001

EntityResizer.placements = {
	{
		name = "EntityResizer",
		data = {
			affectedEntities = "Bumper",
			entityIDs = "",
			allEntities = true,
			transitionUpdate = false,
			global = false,
			counter = false,
			sliderMode = false,
			sliderCounterName = "KoseiHelper_resizerNumbers",
			sliderCounterMinValue = 0,
			sliderCounterMaxValue = 10,
			flag = "",
			onlyOnce = true,
			scale = 1,
			maxScale = 1,
			everyFrame = false
		}
	},
	{
		name = "EntityResizerSlider",
		data = {
			affectedEntities = "Bumper",
			entityIDs = "",
			allEntities = true,
			transitionUpdate = false,
			global = false,
			counter = false,
			sliderMode = true,
			sliderCounterName = "KoseiHelper_resizerNumbers",
			sliderCounterMinValue = 0,
			sliderCounterMaxValue = 10,
			flag = "",
			onlyOnce = false,
			scale = 1,
			maxScale = 1,
			everyFrame = false
		}
	}
}

EntityResizer.fieldInformation = {
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
			options = {
			"AngryOshiro",
			"BladeRotateSpinner,StarRotateSpinner",
			"BladeTrackSpinner,StarTrackSpinner",
			"Bonfire",
			"Booster",
			"BounceBlock",
			"Bumper",
			"Cassette",
			"ClutterDoor",
			"ClutterSwitch",
			"CrumblePlatform",
			"CrushBlock",
			"DashSwitch",
			"Door",
			"HeartGem,FakeHeart,DreamHeartGem",
			"FlyFeather",
			"FireBall",
			"HangingLamp",
			"ForegroundDebris",
			"FlingBird",
			"FloatingDebris",
			"Flutterbird",
			"Glider",
			"JumpThru",
			"Key",
			"Lamp",
			"LightningBreakerBox",
			"LockBlock",
			"Lookout",
			"MoonCreature",
			"MoveBlock",
			"MovingPlatform",
			"MrOshiroDoor",
			"Payphone",
			"PicoConsole",
			"Plateau",
			"PlaybackBillboard",
			"PowerSourceNumber",
			"Refill",
			"ReflectionHeartStatue",
			"ResortLantern",
			"ResortRoofEnding",
			"RidgeGate",
			"Seeker,SeekerStatue",
			"Spikes",
			"Spring",
			"StarJumpBlock",
			"Strawberry,StrawberryPoints",
			"SummitCloud",
			"TheoCrystal,TheoCrystalPedestal",
			"Torch",
			"Trapdoor",
			"WallBooster",
			"WhiteBlock"
			}
			},
		editable = true
		}
}

EntityResizer.fieldOrder = {
	"x",
	"y",
	"affectedEntities",
	"entityIDs",
	"scale",
	"maxScale",
	"sliderCounterName",
	"sliderCounterMinValue",
	"sliderCounterMaxValue",
	"flag",
	"allEntities",
	"global",
	"transitionUpdate",
	"counter",
	"onlyOnce"
}

function EntityResizer.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"sliderMode",
	"maxScale",
	"counter",
	"sliderCounterName",
	"sliderCounterMinValue",
	"sliderCounterMaxValue",
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
		doNotIgnore("onlyOnce")
	else
		doNotIgnore("maxScale")
		doNotIgnore("counter")
		doNotIgnore("sliderCounterName")
		doNotIgnore("sliderCounterMinValue")
		doNotIgnore("sliderCounterMaxValue")
	end
	return ignored
end

function EntityResizer.draw(room, entity, viewport)
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
	local y = entity.y - #split * lineSpacing - 6
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
	
    local tinterSprite = drawableSprite.fromTexture("objects/KoseiHelper/EntityModifiers/Resizer", entity)
    tinterSprite:draw()
end

function EntityResizer.selection(room, entity)
    return utils.rectangle(entity.x - 5, entity.y - 5, 10, 10)
end

return EntityResizer