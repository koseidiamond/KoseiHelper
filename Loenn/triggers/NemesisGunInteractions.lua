local NemesisGunInteractions = {}

NemesisGunInteractions.name = "KoseiHelper/NemesisGunInteractions"
NemesisGunInteractions.depth = -10500
NemesisGunInteractions.placements = {
	{
		name = "NemesisGunInteractions",
		data = {
		triggerMode = "OnStay",
		canKillPlayer = true,
		goThroughDreamBlocks = true,
		breakBounceBlocks = true,
		activateFallingBlocks = true,
		harmEnemies = true,
		theoInteraction = "Kill",
		breakSpinners = true,
		breakMovingBlades = true
		}
	}
}

NemesisGunInteractions.fieldInformation = 
{
	triggerMode =
	{
		options =
		{
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	theoInteraction =
	{
		options =
		{
		"Kill",
		"None",
		"HitSpinner",
		"HitSpring"
		},
		editable = false
	}
}

return NemesisGunInteractions