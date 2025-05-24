local FlagRefill = {}

FlagRefill.name = "KoseiHelper/FlagRefill"
FlagRefill.depth = 8999

FlagRefill.placements = {
	{
		name = "FlagRefill",
		data = {
			oneUse = false,
			sprite = "objects/KoseiHelper/Refills/FlagRefill/",
			respawnTime = 2.5
		}
	}
}


function FlagRefill.texture(room, entity)
    return entity.sprite .. "idle00"
end

return FlagRefill
