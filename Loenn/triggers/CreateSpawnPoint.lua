local CreateSpawnPointTrigger = {}

CreateSpawnPointTrigger.name = "KoseiHelper/CreateSpawnPointTrigger"
CreateSpawnPointTrigger.nodeLimits = {0, 1}

CreateSpawnPointTrigger.placements = {
	{
		name = "CreateSpawnPointTrigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		flag = "",
		room = ""
		}
	}
}

CreateSpawnPointTrigger.fieldInformation = function (entity) return {
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

return CreateSpawnPointTrigger
