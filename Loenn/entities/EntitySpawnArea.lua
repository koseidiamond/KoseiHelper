local utils = require("utils")

local EntitySpawnArea = {}

EntitySpawnArea.name = "KoseiHelper/EntitySpawnArea"
EntitySpawnArea.depth = 1

EntitySpawnArea.placements = {
	{
		name = "EntitySpawnArea",
		data = {
		width = 16,
		height = 16
		}
	}
}

function EntitySpawnArea.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

EntitySpawnArea.fillColor = {238 / 255, 195 / 255, 154 / 255, 0.2}
EntitySpawnArea.borderColor = {217 / 255, 160 / 255, 102 / 255, 0.4}

return EntitySpawnArea