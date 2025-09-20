local ForceThrowTrigger = {}

ForceThrowTrigger.name = "KoseiHelper/ForceThrowTrigger"
ForceThrowTrigger.depth = 100

ForceThrowTrigger.placements = {
	{
		name = "Force Throw Trigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		flag = ""
		}
	}
}

ForceThrowTrigger.fieldInformation = function (entity) return {
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

return ForceThrowTrigger
