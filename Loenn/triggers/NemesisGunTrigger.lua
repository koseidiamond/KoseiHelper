local NemesisGunTrigger = {}

NemesisGunTrigger.name = "KoseiHelper/NemesisGunTrigger"
NemesisGunTrigger.depth = -10500
NemesisGunTrigger.placements = {
	{
		name = "NemesisGunTrigger",
		data = {
		enabled = false,
		gunshotSound = "event:/ashleybl/gunshot",
		replacesDash = true,
		cooldown = 8,
		canKillPlayer = true
		}
	}
}

-- TODO look into BossesHelper/HealthBar as an example of how to add a list field, for the interactions

NemesisGunTrigger.fieldInformation = 
{
	cooldown = { fieldType = "integer" }
}

return NemesisGunTrigger