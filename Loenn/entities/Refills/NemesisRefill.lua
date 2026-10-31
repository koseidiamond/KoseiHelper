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
			refillAction = "ForceDash",
			collectableByPlayer = false,
			flagName = "NemesisRefillFlag"
		}
	}
}

function NemesisRefill.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"flagName"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.refillAction == "SetFlag" then
		doNotIgnore("flagName")
	end
	return ignored
end

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
			"Die",
			"SetFlag"
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
