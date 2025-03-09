local NemesisGunSettings = {}

NemesisGunSettings.name = "KoseiHelper/NemesisGunSettings"
NemesisGunSettings.depth = -10500
NemesisGunSettings.placements = {
	{
		name = "NemesisGunSettings",
		data = {
		triggerMode = "OnStay",
		enabled = true,
		gunshotSound = "event:/KoseiHelper/Guns/shotDefault",
		dashBehavior = "None",
		cooldown = 8,
		color1 = "4682B4",
		color2 = "FFFF00",
		particleType = "Normal",
		particleAlpha = 1,
		directions = "FourDirections",
		bulletExplosion = true,
		--bulletSound = "event:/none",
		loseGunOnRespawn = false,
		lifetime = 60,
		gunTexture = "objects/KoseiHelper/Guns/NemesisGun",
		bulletTexture = "objects/KoseiHelper/Guns/Bullets/Invisible",
		speedMultiplier = 1,
		recoilStrength = 80,
		recoilCooldown = 16
		}
	}
}

NemesisGunSettings.fieldInformation = 
{
	cooldown = { fieldType = "integer" },
	recoilCooldown = { fieldType = "integer" },
	particleAlpha = { minimumValue = 0, maximumValue = 1 },
	triggerMode = {
		options =
		{
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	gunshotSound = {
		{ fieldType = "string" },
		options =
		{
		"event:/KoseiHelper/Guns/shotDefault",
		"event:/KoseiHelper/bullet",
		"event:/KoseiHelper/bulletB",
		},
		editable = true
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
	"particleAlpha",
	"color1",
	"color2",
	"dashBehavior",
	"gunshotSound",
	"cooldown",
	"recoilCooldown",
	"lifetime",
	"speedMultiplier",
	"recoilStrength",
	"gunTexture",
	"bulletTexture",
	"enabled",
	"loseGunOnRespawn",
	"bulletExplosion"
	
}

return NemesisGunSettings