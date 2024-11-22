local fakeTilesHelper = require("helpers.fake_tiles")

local SpawnController = {}

SpawnController.name = "KoseiHelper/SpawnController"
SpawnController.depth = -9500
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
		blockWidth = 16,
		blockHeight = 16,
		blockTileType = "3",
		boosterRed = false,
		cloudFragile = true,
		featherShielded = false,
		featherSingleUse = true,
		dashBlockCanDash = true,
		iceballSpeed = 1,
		iceballAlwaysIce = false,
		moveBlockDirection = "Down",
		moveBlockCanSteer = false,
		moveBlockFast = true,
		fallingBlockBadeline = false,
		fallingBlockClimbFall = false,
		swapBlockTheme = "Normal",
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
	blockWidth = { fieldType = "integer"},
	blockHeight = { fieldType = "integer"},
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
	"blockWidth",
	"blockHeight",
	"blockTileType",
	"boosterRed",
	"cloudFragile",
	"featherShielded",
	"featherSingleUse",
	"dashBlockCanDash",
	"iceballSpeed",
	"iceballAlwaysIce",
	"moveBlockDirection",
	"moveBlockCanSteer",
	"moveBlockFast",
	"swapBlockTheme", 
	"zipMoverTheme",
	"spawnFlag",
	"spawnSpeed",
	"spawnInterval",
	"fallingBlockBadeline",
	"fallingBlockClimbFall"
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
	if entity.entityToSpawn == "Cloud" then
		doNotIgnore("cloudFragile")
	end
	if entity.entityToSpawn == "Feather" then
		doNotIgnore("featherShielded")
		doNotIgnore("featherSingleUse")
	end
	if entity.entityToSpawn == "DashBlock" then
		doNotIgnore("dashBlockCanDash")
	end
	if entity.entityToSpawn == "Iceball" then
		doNotIgnore("iceballSpeed")
		doNotIgnore("iceballAlwaysIce")
	end
	if entity.entityToSpawn == "MoveBlock" then
		doNotIgnore("moveBlockDirection")
		doNotIgnore("moveBlockCanSteer")
		doNotIgnore("moveBlockFast")
	end
	if entity.entityToSpawn == "SwapBlock" then
		doNotIgnore("swapBlockTheme")
	end
	if entity.entityToSpawn == "ZipMover" then
		doNotIgnore("zipMoverTheme")
	end
	if entity.entityToSpawn == "FallingBlock" then
		doNotIgnore("fallingBlockBadeline")
		doNotIgnore("fallingBlockClimbFall")
	end
	-- Noded entities
	if entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "Iceball" then
		doNotIgnore("nodeX")
		doNotIgnore("nodeY")
	end
	-- Entities with tileset
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" then
		doNotIgnore("blockTileType")
	end
	--Entities with size
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" or entity.entityToSpawn == "IceBlock"
	or entity.entityToSpawn == "MoveBlock"or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "ZipMover" then
		doNotIgnore("blockWidth")
		doNotIgnore("blockHeight")
	end
	return ignored
end

function SpawnController.texture(room, entity)
    local entityToSpawn = entity.entityToSpawn
    if entityToSpawn == "Puffer" then
        return "objects/KoseiHelper/Controllers/SpawnController/Puffer"
    elseif entityToSpawn == "Cloud" then
        return "objects/KoseiHelper/Controllers/SpawnController/Cloud"
    elseif entityToSpawn == "BadelineBoost" then
        return "objects/KoseiHelper/Controllers/SpawnController/BadelineBoost"
    elseif entityToSpawn == "Booster" then
        return "objects/KoseiHelper/Controllers/SpawnController/Booster"
	elseif entityToSpawn == "Bumper" then
        return "objects/KoseiHelper/Controllers/SpawnController/Bumper"
	elseif entityToSpawn == "IceBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/IceBlock"
	elseif entityToSpawn == "Heart" then
        return "objects/KoseiHelper/Controllers/SpawnController/Heart"
	elseif entityToSpawn == "DashBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/DashBlock"
	elseif entityToSpawn == "FallingBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/FallingBlock"
	elseif entityToSpawn == "Feather" then
        return "objects/KoseiHelper/Controllers/SpawnController/Feather"
	elseif entityToSpawn == "Iceball" then
        return "objects/KoseiHelper/Controllers/SpawnController/Iceball"
	elseif entityToSpawn == "MoveBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/MoveBlock"
	elseif entityToSpawn == "Seeker" then
        return "objects/KoseiHelper/Controllers/SpawnController/Seeker"
	elseif entityToSpawn == "SwapBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/SwapBlock"
	elseif entityToSpawn == "ZipMover" then
        return "objects/KoseiHelper/Controllers/SpawnController/ZipMover"
	else
		return "objects/KoseiHelper/Controllers/SpawnController/Broken"
    end
end

return SpawnController