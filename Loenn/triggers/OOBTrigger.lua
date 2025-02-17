local OOBTrigger = {}

OOBTrigger.name = "KoseiHelper/OOBTrigger"
OOBTrigger.depth = 100

OOBTrigger.placements = {
	{
		name = "OOBTrigger",
		alternativeName = "BoundTrigger",
		data = {
		onlyOnce = false,
		onExit = false,
		inBound = false,
		climbFix = true
		}
	}
}

OOBTrigger.triggerText = "OOB"

return OOBTrigger