local CustomBubble = {}

CustomBubble.name = "KoseiHelper/CustomBubble"
CustomBubble.depth = 0
CustomBubble.texture = "objects/KoseiHelper/Crossover/Bubble/bubble" -- todo
CustomBubble.placements = {
    name = "CustomBubble",
	data = {
		speedMult = 1.2,
		sound = "event:/game/06_reflection/feather_bubble_bounce",
		wiggliness = 8,
		floatiness = 2,
		renderSprite = false,
		renderBubble = true,
		spriteID = "flyFeather",
		breakBehavior = "DontBreak",
		singleUse = false,
		respawnCooldown = 3,
		color = "ffffffff",
		refillDash = true,
		refillStamina = true,
		releaseFromBooster = true,
		freezeFrames = false,
		radius = 10,
		coyote = false
	}
}

function CustomBubble.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"singleUse",
	"respawnCooldown",
	"freezeFrames",
	"coyote"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.breakBehavior ~= "DontBreak" then
		doNotIgnore("singleUse")
		doNotIgnore("respawnCooldown")
		doNotIgnore("freezeFrames")
		doNotIgnore("coyote")
	end
	return ignored
end

CustomBubble.fieldOrder = {
	"x",
	"y",
	"breakBehavior",
	"respawnCooldown",
	"speedMult",
	"radius",
	"color",
	"sound",
	"spriteID",
	"floatiness",
	"wiggliness",
	"singleUse",
	"freezeFrames",
	"coyote",
	"refillDash",
	"refillStamina",
	"releaseFromBooster",
	"renderBubble",
	"renderSprite"
}

CustomBubble.fieldInformation = {
	respawnCooldown = {
		minimumValue = 0
	},
	radius = {
		fieldType = "integer",
		minimumValue = 1
	},
	color = {
		fieldType = "color",
		useAlpha = true
	},
	breakBehavior = {
		options = {
			"DontBreak",
			"AlwaysBreak",
			"BreakWithDash"
		},
		editable = false
	}
}

return CustomBubble