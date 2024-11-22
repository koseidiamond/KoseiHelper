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
		nodeRelativeToPlayerFacing = true,
		timeToLive = 0,
		appearSound = "event:/KoseiHelper/spawn",
		disappearSound = "event:/game/general/assist_dash_aim",
		flag = "",
		spawnCondition = "OnCustomButtonPress",
		persistent = false,
		--Spawn conditions
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
		dummyFix = true,
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
		zipMoverTheme = "Normal",
		refillTwoDashes = false
		}
	}
}

SpawnController.fieldInformation = function (entity) return {
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
		"ZipMover",
		"CrumblePlatform",
		"DreamBlock",
		"BounceBlock",
		"Refill"
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
		"OnCassetteBeat",
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
	offsetX = { fieldType = "integer"},
	offsetY = { fieldType = "integer"},
	nodeX = { fieldType = "integer"},
	nodeY = { fieldType = "integer"},
	blockWidth = { fieldType = "integer"},
	blockHeight = { fieldType = "integer"},
	blockTileType = {
		options = fakeTilesHelper.getTilesOptions(entity.fg and "tilesFg")
	},
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
} end


function SpawnController.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	"nodeX",
	"nodeY",
	"blockWidth",
	"blockHeight",
	"blockTileType",
	"nodeRelativeToPlayerFacing",
	"boosterRed",
	"dummyFix",
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
	"fallingBlockClimbFall",
	"refillTwoDashes"
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
	if entity.spawnCondition == "BadelineBoost" then
		doNotIgnore("dummyFix")
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
	if entity.entityToSpawn == "CrumblePlatform" then
		doNotIgnore("blockWidth")
	end
	if entity.entityToSpawn == "Refill" then
		doNotIgnore("refillTwoDashes")
	end
	-- Noded entities
	if entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "Iceball" then
		doNotIgnore("nodeX")
		doNotIgnore("nodeY")
		doNotIgnore("nodeRelativeToPlayerFacing")
	end
	-- Entities with tileset
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" then
		doNotIgnore("blockTileType")
	end
	--Entities with size
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" or entity.entityToSpawn == "IceBlock" or entity.entityToSpawn == "MoveBlock"
	or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "DreamBlock" then
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
	elseif entityToSpawn == "CrumblePlatform" then
		return "objects/KoseiHelper/Controllers/SpawnController/CrumblePlatform"
	elseif entityToSpawn == "DreamBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/DreamBlock"
	elseif entityToSpawn == "BounceBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/BounceBlockBlock"
	elseif entityToSpawn == "Refill" then
		return "objects/KoseiHelper/Controllers/SpawnController/Refill"
	else
		return "objects/KoseiHelper/Controllers/SpawnController/Broken"
    end
end

return SpawnController