local NemesisGunTrigger = {}

NemesisGunTrigger.name = "KoseiHelper/NemesisGunTrigger"
NemesisGunTrigger.depth = -10500
NemesisGunTrigger.placements = {
	{
		name = "NemesisGunTrigger",
		data = {
		enabled = true,
		gunshotSound = "event:/ashleybl/gunshot",
		replacesDash = true,
		cooldown = 8,
		canKillPlayer = true,
		triggerMode = "OnStay"
		}
	}
}

-- TODO look into BossesHelper/HealthBar as an example of how to add a list field, for the interactions

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