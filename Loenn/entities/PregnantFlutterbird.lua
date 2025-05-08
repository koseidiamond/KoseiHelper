local drawableSpriteStruct = require("structs.drawable_sprite")
local utils = require("utils")

local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local PregnantFlutterbird = {}

PregnantFlutterbird.name = "KoseiHelper/SpecialFlutterbird"
PregnantFlutterbird.depth = -9999
PregnantFlutterbird.placements = {
    name = "PregnantFlutterbird",
	data = {
		childrenCount = 1,
		timeToGiveBirth = 5,
		chaser = false,
		gender = "Nonbinary",
		orientation = "Gay",
		shootLasers = false,
		killOnContact = false,
		bouncy = false,
		flyAway = true,
		flyAwayFlag = "",
		sterilizationFlag = "",
		squishable = true,
		hopSfx = "event:/game/general/birdbaby_hop",
		birthSfx = "event:/game/09_core/frontdoor_heartfill",
		spriteID = "flutterbird",
		emitLight = false,
		depth = -9999,
		coyote = false,
		polyamorous = true,
		partnerID = 0,
		--hoppingDistance = 8,
		color = "ffffff"
		
	}
}

PregnantFlutterbird.fieldOrder = {
	"x",
	"y",
	"gender",
	"orientation",
	"childrenCount",
	"timeToGiveBirth",
	"flyAwayFlag",
	"sterilizationFlag",
	"hopSfx",
	"birthSfx",
	"spriteID",
	"depth",
	"color",
	--"hoppingDistance",
	"partnerID",
	"bouncy",
	"chaser",
	"coyote",
	"flyAway",
	"killOnContact",
	"shootLasers",
	"squishable",
	"emitLight",
	"polyamorous"
}

PregnantFlutterbird.fieldInformation = {
    childrenCount = {
        fieldType = "integer"
    },
    gender = {
        options = {
			"Nonbinary",
			"Male",
			"Female"
		},
		editable = false
	},
	orientation = {
        options = {
			"Gay",
			"Straight",
			"Asexual",
			"Bisexual",
			"Self"
		},
		editable = false
	},
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	color = {
        fieldType = "color"
    },
	partnerID = {
		fieldType = "integer",
		minimumValue = 0
	}
}

function PregnantFlutterbird.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	"partnerID",
	"hoppingDistance"
	}
	
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.polyamorous == false then
		doNotIgnore("partnerID")
	end

	return ignored
end

function PregnantFlutterbird.depth(room,entity)
	return entity.depth
end

local texture = "scenery/flutterbird/idle00"

function PregnantFlutterbird.sprite(room, entity)
    utils.setSimpleCoordinateSeed(entity.x, entity.y)

    local PregnantFlutterbirdSprite = drawableSpriteStruct.fromTexture(texture, entity)

    PregnantFlutterbirdSprite:setJustification(0.5, 1.0)
    PregnantFlutterbirdSprite:setColor(entity.color)

    return PregnantFlutterbirdSprite
end

return PregnantFlutterbird