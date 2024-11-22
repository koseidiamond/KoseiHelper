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
		removeDash = false,
		removeStamina = false,
		flagValue = true,
		relativeToPlayerFacing = true,
		timeToLive = 0,
		appearSound = "event:/none",
		disappearSound = "event:/KoseiHelper/spawn",
		flag = "",
		spawnCondition = "OnCustomButtonPress",
		persistent = false,
		--Spawn conditions
		--TODO notes: test     OnFlagEnabled, OnDash ,OnJump, OnCustomButtonPress, OnSpeedX, OnInterval
		spawnFlag = "koseiHelper_spawn",
		spawnSpeed = 300,
		spawnInterval = 1,
		spawnLimit = -1,
		-- Entity-specific attributes
		nodeX = 0,
		nodeY = 0,
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
		iceBlockWidth = 16,
		iceBlockHeight = 16,
		moveBlockDirection = "Down",
		moveBlockWidth = 16,
		moveBlockHeight = 16,
		moveBlockCanSteer = false,
		moveBlockFast = true,
		swapBlockWidth = 16,
		swapBlockHeight = 16,
		swapBlockTheme = "Normal",
		zipMoverWidth = 16,
		zipMoverHeight = 16,
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
	timeToLive = {
		minimumValue = 0
	},
	spawnCondition = {
		options = {
		"OnFlagEnabled",
		"OnDash",
		"OnJump",
		"OnCustomButtonPress",
		"OnSpeedX",
		"OnInterval"
		},
		editable = false
	},
	spawnInterval = {
		minimumValue = 0
	},
	spawnLimit = { fieldType = "integer"},
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
	},
	swapBlockTheme = {
		options = {
		"Normal",
		"Moon"
		},
		editable = false
	},
	zipMoverTheme = {
		options = {
		"Normal",
		"Moon"
		},
		editable = false
	}
}

function SpawnController.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	"nodeX",
	"nodeY",
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
	"moveBlockWidth", -- TODO replace widths with a single one
	"moveBlockHeight",
	"moveBlockCanSteer",
	"moveBlockFast",
	"swapBlockWidth",
	"swapBlockHeight",
	"swapBlockTheme", 
	"zipMoverWidth",
	"zipMoverHeight",
	"zipMoverTheme",
	"spawnFlag",
	"spawnSpeed",
	"spawnInterval"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.spawnCondition == "OnFlagEnabled" then
		doNotIgnore("spawnFlag")
	end
	if entity.spawnCondition == "OnSpeedX" then
		doNotIgnore("spawnSpeed")
	end
	if entity.spawnCondition == "OnInterval" then
		doNotIgnore("spawnInterval")
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
		doNotIgnore("swapBlockTheme")
	end
	if entity.entityToSpawn == "ZipMover" then
		doNotIgnore("zipMoverWidth")
		doNotIgnore("zipMoverHeight")
		doNotIgnore("zipMoverTheme")
	end
	if entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "Iceball" then
		doNotIgnore("nodeX")
		doNotIgnore("nodeY")
	end

	return ignored
end

return SpawnController