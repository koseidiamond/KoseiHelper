local MetadataTrigger = {}

MetadataTrigger.name = "KoseiHelper/MetadataTrigger"
MetadataTrigger.depth = 100

MetadataTrigger.placements = {
	{
		name = "MetadataTrigger",
		data = {
		onlyOnce = false,
		metadataToAffect = "Both",
		flag = "",
		-- Map metadata
		heartIsEnd = false,
		seekerSlowdown = true,
		theoInBubble = false,
		dreaming = false,
		coreMode = "NoChange",
		inventory = "NoChange",
		-- Room metadata
		isSpace = false,
		isDark = false,
		disableDownTransition = false,
		isUnderwater = false
		}
	}
}

MetadataTrigger.fieldInformation = function (entity) return {
	metadataToAffect = {
		options = {
		"Both",
		"Map",
		"Room"
		},
		editable = false
	},
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	coreMode = {
		options = {
		"NoChange",
		"Cold",
		"Hot",
		"None"
		},
		editable = false
	},
	inventory = {
		options = {
		"NoChange",
		"CH6End",
		"Core",
		"Default",
		"Farewell",
		"OldSite",
		"Prologue",
		"TheSummit"
		},
		editable = false
	}
}
end

MetadataTrigger.fieldOrder= {
	"x",
	"y",
	"width",
	"height",
	"heartIsEnd",
	"seekerSlowdown",
	"theoInBubble",
	"dreaming",
	"coreMode",
	"inventory",
	"isUnderwater",
	"isSpace",
	"disableDownTransition",
	"isDark"
}

return MetadataTrigger
