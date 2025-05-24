local CounterRefill = {}

CounterRefill.name = "KoseiHelper/CounterRefill"
CounterRefill.depth = 8999

CounterRefill.placements = {
	{
		name = "CounterRefill",
		data = {
			oneUse = false,
			sprite = "objects/KoseiHelper/Refills/CounterRefill/",
			decrease = false,
			respawnTime = 2.5
		}
	}
}


function CounterRefill.texture(room, entity)
    return entity.sprite .. "idle00"
end

return CounterRefill