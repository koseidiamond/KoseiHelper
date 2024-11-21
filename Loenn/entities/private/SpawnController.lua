local fakeTilesHelper = require("helpers.fake_tiles")

local SpawnController = {}

SpawnController.name = "KoseiHelper/SpawnController"
SpawnController.depth = -9500
SpawnController.texture = "objects/KoseiHelper/Controllers/SpawnController"
SpawnController.placements = {
	{
		name = "SpawnController",
		data = {
		offsetX = 0,
		offsetY = 8,
		entityToSpawn = "Puffer",
		spawnCooldown = 1,
		iceBlockWidth = 16,
		iceBlockHeight = 16,
		removeDash = true,
		removeStamina = false,
		flag = "",
		flagValue = true,
		relativeToPlayerFacing = true,
		timeToLive = -1,
		audio = "",
		-- Entity-specific attributes
		boosterRed = false,
		cloudFragile = true,
		featherShielded = false,
		featherSingleUse = true,
		dashBlockTileType = "3",
		dashBlockWidth = 16,
		dashBlockHeight = 16,
		dashBlockCanDash = true,
		iceballSpeed = 1,
		iceballAlwaysIce = false,
		moveBlockDirection = "Down",
		moveBlockWidth = 16,
		moveBlockHeight = 16,
		moveBlockCanSteer = false,
		moveBlockFast = true,
		swapBlockWidth = 16,
		swapBlockHeight = 16,
		swapBlockNodeX = 0,
		swapBlockNodeY = 0,
		swapBlockTheme = "Normal",
		zipMoverWidth = 16,
		zipMoverHeight = 16,
		zipMoverNodeX = 0,
		zipMoverNodeY = 0,
		zipMoverTheme = "Normal"
		}
	}
}

SpawnController.fieldInformation = {
	entityToSpawn = {
		options = {
		"Puffer",
		"Cloud",
		"BadelineBoost",
		"Booster",
		"Bumper",
		"IceBlock",
		"Heart",
		"DashBlock",
		"FallingBlock",
		"Feather",
		"Iceball",
		"MoveBlock",
		"Seeker",
		"SwapBlock",
		"ZipMover"
		},
		editable = false
	},
	spawnCooldown = {
		minimumValue = 0
	},
	iceBlockWidth = { fieldType = "integer"},
	iceBlockHeight = { fieldType = "integer"},
	dashBlockWidth = { fieldType = "integer"},
	dashBlockHeight = { fieldType = "integer"},
	moveBlockDirection = {
		options = {
		"Left",
		"Right",
		"Up",
		"Down"
		},
		editable = false
	}
}

function SpawnController.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	"iceBlockWidth",
	"iceBlockHeight",
	"boosterRed",
	"cloudFragile",
	"featherShielded",
	"featherSingleUse",
	"dashBlockTileType",
	"dashBlockWidth",
	"dashBlockHeight",
	"dashBlockCanDash",
	"iceballSpeed",
	"iceballAlwaysIce",
	"moveBlockDirection",
	"moveBlockWidth",
	"moveBlockHeight",
	"moveBlockCanSteer",
	"moveBlockFast",
	"swapBlockWidth",
	"swapBlockHeight",
	"swapBlockNodeX",
	"swapBlockNodeY",
	"swapBlockTheme",
	"zipMoverWidth",
	"zipMoverHeight",
	"zipMoverNodeX",
	"zipMoverNodeY",
	"zipMoverTheme"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.entityToSpawn == "Booster" then
		doNotIgnore("boosterRed")
	end
	if entity.entityToSpawn == "IceBlock" then
		doNotIgnore("iceBlockWidth")
		doNotIgnore("iceBlockHeight")
	end
	if entity.entityToSpawn == "Cloud" then
		doNotIgnore("cloudFragile")
	end
	if entity.entityToSpawn == "Feather" then
		doNotIgnore("featherShielded")
		doNotIgnore("featherSingleUse")
	end
	if entity.entityToSpawn == "DashBlock" then
		doNotIgnore("dashBlockTileType")
		doNotIgnore("dashBlockWidth")
		doNotIgnore("dashBlockHeight")
		doNotIgnore("dashBlockCanDash")
	end
	if entity.entityToSpawn == "Iceball" then
		doNotIgnore("iceballSpeed")
		doNotIgnore("iceballAlwaysIce")
	end
	if entity.entityToSpawn == "MoveBlock" then
		doNotIgnore("moveBlockDirection")
		doNotIgnore("moveBlockWidth")
		doNotIgnore("moveBlockHeight")
		doNotIgnore("moveBlockCanSteer")
		doNotIgnore("moveBlockFast")
	end
	if entity.entityToSpawn == "SwapBlock" then
		doNotIgnore("swapBlockWidth")
		doNotIgnore("swapBlockHeight")
		doNotIgnore("swapBlockNodeX")
		doNotIgnore("swapBlockNodeY")
		doNotIgnore("swapBlockTheme")
	end
	if entity.entityToSpawn == "ZipMover" then
		doNotIgnore("zipMoverWidth")
		doNotIgnore("zipMoverHeight")
		doNotIgnore("zipMoverNodeX")
		doNotIgnore("zipMoverNodeY")
		doNotIgnore("zipMoverTheme")
	end
	return ignored
end

return SpawnController
