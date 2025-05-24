local TileRefill = {}

TileRefill.name = "KoseiHelper/TileRefill"
TileRefill.depth = 8999

TileRefill.placements = {
	{
		name = "TileRefill",
		data = {
			collidable = true,
			visible = true,
			oneUse = false,
			respawnTime = 2.5
		}
	}
}


function TileRefill.texture(room, entity)
    return "objects/KoseiHelper/Refills/TileRefill/tiles00"
end

return TileRefill
