local SetWindowSizeTrigger = {}

SetWindowSizeTrigger.name = "KoseiHelper/SetWindowSizeTrigger"
SetWindowSizeTrigger.placements = {
    name = "SetWindowSizeTrigger",
    data = {
		windowWidth = 100,
		windowHeight = 100,
		onLeave = false,
		fullScreen = false
    }
}

SetWindowSizeTrigger.fieldInformation = 
{
	windowWidth = { fieldType = "integer" },
	windowHeight = { fieldType = "integer" }
}

return SetWindowSizeTrigger