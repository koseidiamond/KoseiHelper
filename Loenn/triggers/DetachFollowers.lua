local DetachFollowerTrigger = {}

DetachFollowerTrigger.name = "KoseiHelper/DetachFollowerTrigger"
DetachFollowerTrigger.nodeLimits = {1, 1}
DetachFollowerTrigger.placements = {
    name = "DetachFollowerTrigger",
    data = {
        global = true,
		affectBerries = true,
		affectKeys = false,
		affectOthers = false,
		sound = "event:/new_content/game/10_farewell/strawberry_gold_detach",
		triggerMode = "OnEnter",
		onlyOnce = false,
		flag = ""
    }
}

DetachFollowerTrigger.fieldInformation = function (entity) return {
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnFlagEnabled"
		},
		editable = false
	}
}
end

return DetachFollowerTrigger