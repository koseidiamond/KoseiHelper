local NemesisRefill = {}

NemesisRefill.name = "KoseiHelper/NemesisRefill"
NemesisRefill.depth = 8999

NemesisRefill.placements = {
	{
		name = "NemesisRefill",
		data = {
			oneUse = false,
			sprite = "objects/KoseiHelper/Refills/NemesisRefill/",
			respawnTime = 2.5,
			refillAction = "ForceDash"
		}
	}
}



NemesisRefill.fieldInformation = {
	refillAction = {
		options = {
			"RefillDash",
			"RefillStamina",
			"ForceDash",
			"ForceJump",
			"WallJump",
			"SuperJump",
			"SuperBounce",
			"PointBounce",
			"SideBounce",
			"CassetteBubble",
			"Rebound", -- little jump
			"Launch",
			"ExplodeLaunch",
			"GreenBoost",
			"RedBoost",
			"Die"
		},
		editable = false
	},
	sprite = {
		options = {
			"objects/KoseiHelper/Refills/NemesisRefill/",
			"objects/KoseiHelper/Refills/NemesisRefill/Red/",
			"objects/KoseiHelper/Refills/NemesisRefill/Orange/",
			"objects/KoseiHelper/Refills/NemesisRefill/Yellow/",
			"objects/KoseiHelper/Refills/NemesisRefill/Green/",
			"objects/KoseiHelper/Refills/NemesisRefill/Cyan/",
			"objects/KoseiHelper/Refills/NemesisRefill/Blue/",
			"objects/KoseiHelper/Refills/NemesisRefill/Purple/",
			"objects/KoseiHelper/Refills/NemesisRefill/Pink/",
			"objects/KoseiHelper/Refills/NemesisRefill/White/",
			"objects/KoseiHelper/Refills/NemesisRefill/Gray/",
			"objects/KoseiHelper/Refills/NemesisRefill/Black/"
		},
		editable = true
	}
}

function NemesisRefill.texture(room, entity)
    return entity.sprite .. "idle00"
end

return NemesisRefill
