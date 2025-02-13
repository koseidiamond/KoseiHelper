local MetadataTrigger = {}

MetadataTrigger.name = "KoseiHelper/MetadataTrigger"
MetadataTrigger.depth = 100

MetadataTrigger.placements = {
	{
		name = "MetadataTrigger",
		data = {
		onlyOnce = false,
		-- Map metadata
		heartIsEnd = false,
		seekerSlowdown = true,
		theoInBubble = false,
		dreaming = false,
		-- Room metadata
		isSpace = false,
		isDark = false,
		disableDownTransition = false,
		isUnderwater = false
		}
	}
}

MetadataTrigger.fieldInformation = function (entity) return {
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnStay"
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
	"isUnderwater",
	"isSpace",
	"disableDownTransition",
	"isDark"
}

return MetadataTrigger
