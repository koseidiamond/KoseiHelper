local PreventDuckTrigger = {}

PreventDuckTrigger.name = "KoseiHelper/PreventDuckTrigger"
PreventDuckTrigger.depth = 100

PreventDuckTrigger.placements = {
	{
		name = "PreventDuckTrigger",
		data = {
			mode = "OnStay",
			flag = ""
		}
	}
}

PreventDuckTrigger.fieldInformation = {
	mode = {
        options = {
			"OnEnter",
			"OnLeave",
			"OnStay"
		},
        editable = false
    }
}

return PreventDuckTrigger