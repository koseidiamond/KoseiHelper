local NemesisRefill = {}

NemesisRefill.name = "KoseiHelper/NemesisRefill"
NemesisRefill.depth = 8999

NemesisRefill.placements = {
	{
		name = "NemesisRefill",
		data = {
			oneUse = false,
			sprite = "objects/KoseiHelper/Refills/NemesisRefill/",
			respawnTime = 2.5
		}
	}
}


function NemesisRefill.texture(room, entity)
    return entity.sprite .. "idle00"
end

return NemesisRefill
