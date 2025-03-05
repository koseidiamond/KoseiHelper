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
		blockDebugMap = "",
		gridColor = "1A1A1A", -- grey (0.1,0.1,0.1)
		jumpthruColor = "FFFF00", -- Yellow
		berryColor = "FFB6C1", -- LightPink
		--keyColor = "FFD700", -- Gold
		checkpointColor = "00FF00", -- Lime
		spawnColor = "FF0000", -- Red
		bgTileColor = "2F4F4F", -- DarkSlateGray
		fgTileColor = "FFFFFF",
		roomBgColor = "000000",
		ignoreDummy = false,
		roomsToAffect = ""
		}
	}
}

DebugMapController.fieldInformation = {
	gridColor =  { fieldType = "color" },
	jumpthruColor =  { fieldType = "color" },
	berryColor =  { fieldType = "color" },
	keyColor = { fieldType = "color" },
	checkpointColor =  { fieldType = "color" },
	spawnColor =  { fieldType = "color" },
	bgTileColor = { fieldType = "color" },
	fgTileColor = { fieldType = "color" },
	roomBgColor = { fieldType = "color" }
}

DebugMapController.fieldOrder = {
	"x",
	"y",
	"blockDebugMap",
	"berryColor",
	--"keyColor",
	"spawnColor",
	"checkpointColor",
	"jumpthruColor",
	"bgTileColor",
	"fgTileColor",
	"roomBgColor",
	"gridColor",
	"redBlink",
	"ignoreDummy",
	"hideBerries",
	"hideKeys",
	"hideSpawns"
}

return DebugMapController