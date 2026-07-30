local MagicInkTrigger = {}

MagicInkTrigger.name = "KoseiHelper/MagicInkTrigger"
MagicInkTrigger.depth = 100

MagicInkTrigger.placements = {
	{
		name = "MagicInkTrigger",
		data = {
		triggerMode = "OnStay",
		flag = "",
		inkAmount = 0.5,
		onlyOnce = false
		}
	}
}

MagicInkTrigger.fieldInformation = function (entity) return {
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

return MagicInkTrigger
