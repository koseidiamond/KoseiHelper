local fakeTilesHelper = require("helpers.fake_tiles")
local enums = require("consts.celeste_enums")

local textures = {"wood", "dream", "temple", "templeB", "cliffside", "reflection", "core", "moon"}

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
		spawnLimit = -1,
		everyXDashes = 1,
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
		refillTwoDashes = false,
		jumpthruTexture = "wood",
		soundIndex = -1,
		blockSinks = false,
		crushBlockAxe = "Both",
		crushBlockChillout = false,
		decalTexture = "10-farewell/creature_f00",
		decalDepth = 9000,

		-- Custom Entity (or any entity that can be spawned with EntityData)
		entityPath = "",
		dictKeys = "",
		dictValues = ""
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
		"Refill",
		"GlassBlock",
		"StarJumpBlock",
		"JumpthruPlatform",
		"FloatySpaceBlock",
		"Kevin",
		"Decal",
		"Flag",
		"CustomEntity"
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
	spawnLimit = { fieldType = "integer"},
	offsetX = { fieldType = "integer"},
	offsetY = { fieldType = "integer"},
	nodeX = { fieldType = "integer"},
	nodeY = { fieldType = "integer"},
	blockWidth = { fieldType = "integer"},
	blockHeight = { fieldType = "integer"},
	blockTileType = {
		options = fakeTilesHelper.getTilesOptions(entity.fg and "tilesFg"),
		editable = false
	},
	everyXDashes = {
		fieldType = "integer",
		minimumValue = 1
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
	},
	crushBlockAxe = {
		options = {
		"Both",
		"Horizontal",
		"Vertical"
		},
		editable = false
	},
	soundIndex = {
        options = enums.tileset_sound_ids,
        fieldType = "integer"
    },
	jumpthruTexture = {
        options = textures
    },
	dictKeys = {
		fieldType = "list",
		elementOptions = {
			fieldType = "string"
		}
	},
	dictValues = {
		fieldType = "list",
		elementOptions = {
			fieldType = "string"
		}
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
	"fallingBlockBadeline",
	"fallingBlockClimbFall",
	"refillTwoDashes",
	"jumpthruTexture",
	"soundIndex",
	"blockSinks",
	"crushBlockAxe",
	"crushBlockChillout",
	"everyXDashes",
	"decalTexture",
	"decalDepth",
	"entityPath",
	"dictKeys",
	"dictValues"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	-- Spawn mode attributes
	if entity.spawnCondition == "OnFlagEnabled" then
		doNotIgnore("spawnFlag")
	end
	if entity.spawnCondition == "OnSpeedX" then
		doNotIgnore("spawnSpeed")
	end
	if entity.spawnCondition == "OnDash" then
		doNotIgnore("everyXDashes")
	end
	-- Entity attributes
	if entity.spawnCondition == "BadelineBoost" then
		doNotIgnore("dummyFix")
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
	if entity.entityToSpawn == "JumpthruPlatform" then
		doNotIgnore("jumpthruTexture")
		doNotIgnore("blockWidth")
		doNotIgnore("soundIndex")
	end
	if entity.entityToSpawn == "GlassBlock" or entity.entityToSpawn == "StarJumpBlock" then
		doNotIgnore("blockSinks")
	end
	if entity.entityToSpawn == "Kevin" then
		doNotIgnore("crushBlockAxe")
		doNotIgnore("crushBlockChillout")
	end
	if entity.entityToSpawn == "Decal" then
		doNotIgnore("decalTexture")
		doNotIgnore("decalDepth")
	end
	-- Noded entities
	if entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "Iceball" then
		doNotIgnore("nodeX")
		doNotIgnore("nodeY")
		doNotIgnore("nodeRelativeToPlayerFacing")
	end
	-- Entities with tileset
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" or entity.entityToSpawn == "FloatySpaceBlock" then
		doNotIgnore("blockTileType")
	end
	--Entities with size
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" or entity.entityToSpawn == "IceBlock" or entity.entityToSpawn == "MoveBlock" or entity.entityToSpawn == "StarJumpBlock"
	or entity.entityToSpawn == "Kevin"	or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "DreamBlock" or entity.entityToSpawn == "GlassBlock"
	or entity.entityToSpawn == "FloatySpaceBlock" then
		doNotIgnore("blockWidth")
		doNotIgnore("blockHeight")
	end
	--Custom Entities
	if entity.entityToSpawn == "CustomEntity" then
		doNotIgnore("blockWidth")
		doNotIgnore("blockHeight")
		doNotIgnore("nodeX")
		doNotIgnore("nodeY")
		doNotIgnore("nodeRelativeToPlayerFacing")
		doNotIgnore("entityPath")
		doNotIgnore("dictKeys")
		doNotIgnore("dictValues")
	end
	return ignored
end

SpawnController.fieldOrder =  {
	"x",
	"y",
	"entityToSpawn",
	"spawnCondition",
	"offsetX",
	"offsetY",
	"nodeX",
	"nodeY",
	"appearSound",
	"disappearSound",
	"timeToLive",
	"spawnCooldown",
	"spawnLimit",
	"spawnSpeed",
	"spawnFlag",
	"blockWidth",
	"blockHeight",
	"blockTileType",
	"flag",
	"flagValue",
	"removeDash",
	"removeStamina",
	"relativeToPlayerFacing",
	"nodeRelativeToPlayerFacing",
	"entityPath",
	"dictKeys",
	"dictValues"
}

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
		return "objects/KoseiHelper/Controllers/SpawnController/BounceBlock"
	elseif entityToSpawn == "Refill" then
		return "objects/KoseiHelper/Controllers/SpawnController/Refill"
	elseif entityToSpawn == "GlassBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/GlassBlock"
	elseif entityToSpawn == "JumpthruPlatform" then
		return "objects/KoseiHelper/Controllers/SpawnController/JumpthruPlatform"
	elseif entityToSpawn == "FloatySpaceBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/FloatySpaceBlock"
	elseif entityToSpawn == "StarJumpBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/StarJumpBlock"
	elseif entityToSpawn == "Kevin" then
		return "objects/KoseiHelper/Controllers/SpawnController/CrushBlock"
	elseif entityToSpawn == "Decal" then
		return "objects/KoseiHelper/Controllers/SpawnController/Decal"
	elseif entityToSpawn == "Flag" then
		return "objects/KoseiHelper/Controllers/SpawnController/Flag"
	else
		return "objects/KoseiHelper/Controllers/SpawnController/Broken"
    end
end

return SpawnController