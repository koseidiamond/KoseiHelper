local TileRefill = {}

TileRefill.name = "KoseiHelper/TileRefill"
TileRefill.depth = 8999

TileRefill.placements = {
	{
		name = "Tile Refill",
		data = {
			collidable = true,
			visible = true,
			oneUse = false
		}
	}
}


function TileRefill.texture(room, entity)
    return "objects/KoseiHelper/Refills/TileRefill/tiles00"
end

return TileRefill
