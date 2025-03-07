local NemesisGunTrigger = {}

NemesisGunTrigger.name = "KoseiHelper/NemesisGunTrigger"
NemesisGunTrigger.depth = -10500
NemesisGunTrigger.placements = {
	{
		name = "NemesisGunTrigger",
		data = {
		triggerMode = "OnStay",
		enabled = true,
		gunshotSound = "event:/ashleybl/gunshot",
		replacesDash = true,
		cooldown = 8,
		canKillPlayer = true,
		-- INTERACTIONS
		goThroughDreamBlocks = true,
		breakBounceBlocks = true,
		activateFallingBlocks = true,
		harmEnemies = true,
		harmTheo = true,
		breakSpinners = true
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
	}
}

return NemesisGunTrigger