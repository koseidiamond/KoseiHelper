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
		particleType = "Normal",
		directions = "FourDirections",
		bulletExplosion = true,
		--bulletSound = "event:/none",
		loseGunOnRespawn = true,
		lifetime = 60,
		gunTexture = "objects/KoseiHelper/Guns/NemesisGun",
		bulletTexture = "objects/KoseiHelper/Guns/Bullets/Invisible",
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
	gunTexture = {
		{ fieldType = "string" },
		options =
		{
		"objects/KoseiHelper/Guns/NemesisGun",
		"objects/KoseiHelper/Guns/Handgun",
		"util/glove"
		},
		editable = true
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
	particleType = {
		options = {
			"Normal",
			"Sparkly",
			"Chimney",
			"Steam",
			"VentDust",
			"Fire",
			"Ice",
			"Feather",
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
	"particleType",
	"dashBehavior",
	"gunshotSound",
	"cooldown",
	"lifetime",
	"speedMultiplier",
	"recoil",
	"gunTexture",
	"bulletTexture",
	"color1",
	"color2",
	"enabled",
	"loseGunOnRespawn",
	"bulletExplosion"
	
}

return NemesisGunSettings