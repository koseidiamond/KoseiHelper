local DebugMapController = {}

DebugMapController.name = "KoseiHelper/DebugMapController"
DebugMapController.depth = -9500
DebugMapController.texture = "objects/KoseiHelper/Controllers/DebugMapController"
DebugMapController.placements = {
	{
		name = "DebugMapController",
		data = {
		hideKeys = false,
		hideBerries = false,
		hideSpawns = false,
		redBlink = true,
		gridColor = "1A1A1A", -- grey (0.1,0.1,0.1)
		jumpthruColor = "FFFF00", -- Yellow
		berryColor = "FFB6C1", -- LightPink
		checkpointColor = "00FF00", -- Lime
		spawnColor = "FF0000", -- Red
		bgTileColor = "2F4F4F", -- DarkSlateGray
		}
	}
}

DebugMapController.fieldInformation = {
	gridColor =  { fieldType = "color" },
	jumpthruColor =  { fieldType = "color" },
	berryColor =  { fieldType = "color" },
	checkpointColor =  { fieldType = "color" },
	spawnColor =  { fieldType = "color" },
	bgTileColor = { fieldType = "color" }
}

return DebugMapController