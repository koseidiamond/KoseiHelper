local TalkComponentCustomizator = {}

TalkComponentCustomizator.name = "KoseiHelper/TalkComponentCustomizator"
TalkComponentCustomizator.depth = -9500
TalkComponentCustomizator.texture = "objects/KoseiHelper/Controllers/TalkComponentCustomizator"
TalkComponentCustomizator.placements = {
	{
		name = "TalkComponentCustomizator",
		data = {
		highlightTexture = "hover/highlight",
		idleTexture = "hover/idle",
		sfxIn = "event:/ui/game/hotspot_main_in",
		sfxOut = "event:/ui/game/hotspot_main_out",
		floatiness = 1,
		tint = "FFFFFFFF",
		talkTextColor = "FFFFFFFF",
		textStroke = 2,
		animationFrames = 0,
		animationSpeed = 0.1,
		text = "Default",
		textScaleX = 1,
		textScaleY = 1,
		iconScaleX = 1,
		iconScaleY = 1,
		allEntities = false
		}
	}
}

TalkComponentCustomizator.fieldInformation = {
	tint = {
        fieldType = "color",
		useAlpha = true
    },
	talkTextColor = {
        fieldType = "color",
		useAlpha = true
    },
	highlightTexture = {
		options = {
			"hover/highlight",
			"hover/resort",
			"hover/resort_japanese"
		},
		editable = true
	},
	idleTexture = {
		options = {
			"hover/idle"
		},
		editable = true
	},
	textStroke = {
        minimumValue = 0
    },
	animationFrames = {
        minimumValue = 0,
		fieldType = "integer"
    },
	animationSpeed = {
        minimumValue = 0.0000001
    },
	textScaleX = {
        minimumValue = 0
    },
	textScaleY = {
        minimumValue = 0
    },
	iconScaleX = {
        minimumValue = 0
    },
	iconScaleY = {
        minimumValue = 0
    }
}

TalkComponentCustomizator.fieldOrder = {
	"x",
	"y",
	--icon
	"idleTexture",
	"highlightTexture",
	"tint",
	"floatiness",
	"animationFrames",
	"animationSpeed",
	"iconScaleX",
	"iconScaleY",
	--text
	"text",
	"textStroke",
	"talkTextColor",
	"textScaleX",
	"textScaleY",
	--other
	"sfxIn",
	"sfxOut",
	"allEntities"
}

return TalkComponentCustomizator