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
		idleAnimationFrames = 0,
		highlightAnimationFrames = 0,
		idleAnimationSpeed = 0.1,
		highlightAnimationSpeed = 0.1,
		text = "Default",
		textScaleX = 1,
		textScaleY = 1,
		iconScaleX = 1,
		iconScaleY = 1,
		allEntities = false,
		flag = ""
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
	idleAnimationFrames = {
        minimumValue = 0,
		fieldType = "integer"
    },
	highlightAnimationFrames = {
        minimumValue = 0,
		fieldType = "integer"
    },
	idleAnimationSpeed = {
        minimumValue = 0.0000001
    },
	highlightAnimationSpeed = {
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
	"idleAnimationFrames",
	"idleAnimationSpeed",
	"highlightAnimationFrames",
	"highlightAnimationSpeed",
	"iconScaleX",
	"iconScaleY",
	--text
	"text",
	"textStroke",
	"textScaleX",
	"textScaleY",
	"talkTextColor",
	--other
	"sfxIn",
	"sfxOut",
	"flag",
	"allEntities"
}

return TalkComponentCustomizator