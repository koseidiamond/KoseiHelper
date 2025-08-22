local CreateSpawnPointTrigger = {}

CreateSpawnPointTrigger.name = "KoseiHelper/CreateSpawnPointTrigger"
CreateSpawnPointTrigger.depth = 100

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
