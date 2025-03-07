local NemesisGunSettings = {}

NemesisGunSettings.name = "KoseiHelper/NemesisGunSettings"
NemesisGunSettings.depth = -10500
NemesisGunSettings.placements = {
	{
		name = "NemesisGunSettings",
		data = {
		triggerMode = "OnStay",
		enabled = true,
		gunshotSound = "event:/ashleybl/gunshot",
		dashBehavior = "ReplacesDash",
		cooldown = 8,
		color1 = "4682B4",
		color2 = "FFFF00",
		dustType = "Normal",
		directions = "FourDirections",
		bulletExplosion = true,
		--bulletSound = "event:/none",
		loseGunOnRespawn = true,
		lifetime = 60,
		gunTexture = "objects/KoseiHelper/NemesisGun/Gun",
		speedMultiplier = 1,
		recoil = 80
		}
	}
}

NemesisGunSettings.fieldInformation = 
{
	cooldown = { fieldType = "integer" },
	triggerMode = {
		options =
		{
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	dashBehavior = {
		options =
		{
		"ReplacesDash",
		"ConsumesDash",
		"None"
		},
		editable = false
	},
	color1 = { fieldType = "color" },
	color2 = { fieldType = "color" },
	dustType = {
		options = {
			"Normal",
			"Sparkly",
			"Chimney",
			"Steam",
			"VentDust",
			"None"
		},
		editable = false
	},
	directions = {
		options = {
			"FourDirections",
			"EightDirections",
			"Horizontal"
		},
		editable = false
	}
}

NemesisGunSettings.fieldOrder =
{
	"x",
	"y",
	"width",
	"height",
	"triggerMode",
	"directions",
	"dustType",
	"dashBehavior",
	"gunshotSound",
	"cooldown",
	"lifetime",
	"speedMultiplier",
	"recoil",
	"gunTexture",
	"color1",
	"color2",
	"enabled",
	"loseGunOnRespawn",
	"bulletExplosion"
	
}

return NemesisGunSettings