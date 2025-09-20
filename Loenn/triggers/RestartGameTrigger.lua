local RestartGameTrigger = {}

RestartGameTrigger.name = "KoseiHelper/RestartGameTrigger"
RestartGameTrigger.placements = {
    name = "RestartGameTrigger",
    data = {
		triggerMode = "OnEnter",
		flag = "",
		restartOnlyLevel = false,
		onlyOnce = false
		--delay = 0
    }
}

RestartGameTrigger.fieldInformation = function (entity) return {
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	delay = {
		minimumValue = 0
	}
}
end

return RestartGameTrigger