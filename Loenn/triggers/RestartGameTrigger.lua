local RestartGameTrigger = {}

RestartGameTrigger.name = "KoseiHelper/RestartGameTrigger"
RestartGameTrigger.placements = {
    name = "RestartGameTrigger",
    data = {
		triggerMode = "OnEnter",
		flag = "",
		restartOnlyLevel = false
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
	}
}
end

return RestartGameTrigger