local NemesisGunTrigger = {}

NemesisGunTrigger.name = "KoseiHelper/NemesisGunTrigger"
NemesisGunTrigger.depth = -10500
NemesisGunTrigger.placements = {
	{
		name = "NemesisGunSettings",
		data = {
		triggerMode = "OnStay",
		enabled = true,
		gunshotSound = "event:/ashleybl/gunshot",
		replacesDash = true,
		cooldown = 8
		}
	},
	{
		name = "NemesisGunInteractions",
		data = {
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

NemesisGunTrigger.fieldInformation = 
{
	cooldown =
	{
	fieldType = "integer"
	},
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
		"HitSpinner"
		},
		editable = false
	}
}

return NemesisGunTrigger