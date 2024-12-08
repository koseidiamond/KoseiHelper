local OOBTrigger = {}

OOBTrigger.name = "KoseiHelper/OOBTrigger"
OOBTrigger.depth = 100

OOBTrigger.placements = {
	{
		name = "OOBTrigger",
		data = {
		onlyOnce = false,
		onExit = false,
		inBound = false
		}
	}
}

OOBTrigger.triggerText = "OOB"

return OOBTrigger