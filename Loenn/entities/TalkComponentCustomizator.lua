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
		talkTextColor = "FFFFFFFF"
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
	}
}

return TalkComponentCustomizator