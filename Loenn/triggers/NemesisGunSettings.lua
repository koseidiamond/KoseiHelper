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
		color1 = "FFA500", -- orange 
		color2 = "FFFF00", -- yellow
		particleType = "Normal",
		particleAlpha = 1,
		customParticleTexture = "particles/KoseiHelper/star",
		directions = "EightDirections",
		bulletExplosion = true,
		loseGunOnRespawn = false,
		lifetime = 600,
		gunTexture = "objects/KoseiHelper/Guns/NemesisGun",
		bulletTexture = "objects/KoseiHelper/Guns/Bullets/Invisible",
		speedMultiplier = 1,
		recoilStrength = 80,
		recoilCooldown = 16,
		recoilUpwards = false,
		recoilOnInteraction = false,
		canShootInFeather = true,
		freezeFrames = 0,
		horizontalAcceleration = 0,
		verticalAcceleration = 0,
		--bulletHeight = 6,
        --bulletWidth = 6,
		--bulletXOffset = 0,
		--bulletYOffset = 0
		}
	}
}

function NemesisGunSettings.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	"customParticleTexture"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end

	if entity.particleType == "Custom" then
		doNotIgnore("customParticleTexture")
	end
	return ignored
end

NemesisGunSettings.fieldInformation = 
{
	cooldown = { fieldType = "integer" },
	lifetime = { minimumValue = 0},
	recoilCooldown = { fieldType = "integer" },
	freezeFrames = {
		fieldType = "integer",
		minimumValue = 0
		},
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
		"event:/KoseiHelper/Guns/shotMarioFireball",
		"event:/KoseiHelper/Guns/shotMeow",
		"event:/KoseiHelper/Guns/shotArrow",
		"event:/KoseiHelper/Guns/shotBoomerang",
		"event:/KoseiHelper/Guns/shotFirework",
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
		"objects/KoseiHelper/Guns/TowerFallCrossbow"
		},
		editable = true
	},
	bulletTexture = {
		{ fieldType = "string" },
		options =
		{
		"objects/KoseiHelper/Guns/Bullets/Ball",
		"objects/KoseiHelper/Guns/Bullets/BigBall",
		"objects/KoseiHelper/Guns/Bullets/Heart",
		"objects/KoseiHelper/Guns/Bullets/Invisible",
		"objects/KoseiHelper/Guns/Bullets/TerrariaMeow",
		"objects/KoseiHelper/Guns/Bullets/TowerFallArrow",
		"objects/KoseiHelper/Guns/Bullets/Trans"
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
			"Bubble",
			"Custom",
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
	"customParticleTexture",
	"color1",
	"color2",
	"dashBehavior",
	"gunshotSound",
	"cooldown",
	"recoilCooldown",
	"recoilStrength",
	"lifetime",
	"speedMultiplier",
	"freezeFrames",
	"horizontalAcceleration",
	"verticalAcceleration",
	"gunTexture",
	"bulletTexture",
	"enabled",
	"loseGunOnRespawn",
	"recoilUpwards",
	"recoilOnInteraction",
	"canShootInFeather",
	"bulletExplosion"
	
}

return NemesisGunSettings