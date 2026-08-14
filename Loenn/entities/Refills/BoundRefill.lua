local BoundRefill = {}

BoundRefill.name = "KoseiHelper/BoundRefill"
BoundRefill.depth = 8999

BoundRefill.placements = {
	{
		name = "BoundRefill",
		alternativeName = "OOBRefill",
		data = {
			outBound = true,
			climbFix = true,
			oneUse = true,
			respawnTime = 2.5
		}
	}
}


function BoundRefill.texture(room, entity)
    local outBound = entity.outBound
    if outBound then
        return "objects/KoseiHelper/Refills/OutBoundRefill/unbounded00"
    else
        return "objects/KoseiHelper/Refills/InBoundRefill/bounded00"
    end
end

return BoundRefill
