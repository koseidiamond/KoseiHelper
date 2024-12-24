local PufferRefill = {}

PufferRefill.name = "KoseiHelper/PufferRefill"
PufferRefill.depth = 8999

PufferRefill.placements = {
	{
		name = "PufferRefill",
		data = {
			oneUse = false,
			sprite = "objects/KoseiHelper/Refills/PufferRefill/"
		}
	}
}


function PufferRefill.texture(room, entity)
    return entity.sprite .. "idle00"
end

return PufferRefill
