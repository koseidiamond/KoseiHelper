local MetadataTrigger = {}

MetadataTrigger.name = "KoseiHelper/MetadataTrigger"
MetadataTrigger.depth = 100

MetadataTrigger.placements = {
	{
		name = "MetadataTrigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		theoInBubble = false,
		seekerSlowdown = true,
		heartIsEnd = false,
		isDark = false,
		isSpace = false,
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

return MetadataTrigger
