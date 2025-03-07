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
		theoInteraction = "HitSpring",
		breakSpinners = true,
		breakMovingBlades = true,
		enableKevins = true,
		breakDashBlocks = true,
		collectables = true,
		useRefills = true,
		explodeFishes = true,
		pressDashSwitches = true,
		canBounce = true,
		useBoosters = false,
		waterFriction = 0.995,
		scareBirds = true,
		collectTouchSwitches = true
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