local fakeTilesHelper = require("helpers.fake_tiles")
local enums = require("consts.celeste_enums")

local textures = {"wood", "dream", "temple", "templeB", "cliffside", "reflection", "core", "moon"}

local SpawnController = {}

SpawnController.name = "KoseiHelper/SpawnController"
SpawnController.depth = -10250
SpawnController.placements = {
	{
		name = "SpawnController",
		alternativeName = "Spawner",
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
		absoluteCoords = false,
		poofWhenDisappearing = true,
		ignoreJustRespawned = false,
		--Spawn conditions
		spawnFlag = "koseiHelper_spawn",
		spawnFlagValue = true,
		spawnSpeed = 300,
		spawnLimit = -1,
		everyXDashes = 1,
		cassetteColor = "Any",
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
		hasBottom = false,
		winged = false,
		moon = false,
		decalTexture = "10-farewell/creature_f00",
		decalDepth = 9000,
		flagCycleAt = 9999,
		flagStop = false,
		minCounterCap = 0,
		maxCounterCap = 9999,
		decreaseCounter = false,

		-- Custom Entity (or any entity that can be spawned with EntityData)
		entityPath = "",
		dictKeys = "",
		dictValues = "",
		noNode = false
		
		-- Secret attributes
		-- noCollider
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
		"Water",
		"Strawberry",
		"Decal",
		"Flag",
		"Counter",
		"CustomEntity"
		},
		editable = false
	},
	cassetteColor = {
		options = {
		"Any",
		"Blue",
		"Rose",
		"BrightSun",
		"Malachite"
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
		"OnInterval",
		"OnClimb",
		"OnJump"
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
	flagCycleAt = { fieldType = "integer" },
	minCounterCap = { fieldType = "integer" },
	maxCounterCap = { fieldType = "integer" },
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
	"spawnFlagValue",
	"spawnSpeed",
	"fallingBlockBadeline",
	"fallingBlockClimbFall",
	"refillTwoDashes",
	"jumpthruTexture",
	"soundIndex",
	"blockSinks",
	"crushBlockAxe",
	"crushBlockChillout",
	"hasBottom",
	"winged",
	"moon",
	"everyXDashes",
	"cassetteColor",
	"decalTexture",
	"decalDepth",
	"entityPath",
	"dictKeys",
	"dictValues",
	"noNode",
	"flagCycleAt",
	"flagStop",
	"minCounterCap",
	"maxCounterCap",
	"decreaseCounter"
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
		doNotIgnore("spawnFlagValue")
	end
	if entity.spawnCondition == "OnSpeedX" then
		doNotIgnore("spawnSpeed")
	end
	if entity.spawnCondition == "OnCassetteBeat" then
		doNotIgnore("cassetteColor")
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
	if entity.entityToSpawn == "Water" then
		doNotIgnore("hasBottom")
	end
	if entity.entityToSpawn == "Strawberry" then
		doNotIgnore("winged")
		doNotIgnore("moon")
	end
	if entity.entityToSpawn == "Decal" then
		doNotIgnore("decalTexture")
		doNotIgnore("decalDepth")
	end
	if entity.entityToSpawn == "Flag" then
		doNotIgnore("flagCycleAt")
		doNotIgnore("flagStop")
	end
	if entity.entityToSpawn == "Counter" then
		doNotIgnore("minCounterCap")
		doNotIgnore("maxCounterCap")
		doNotIgnore("decreaseCounter")
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
	or entity.entityToSpawn == "FloatySpaceBlock" or entity.entityToSpawn == "Water" then
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
		doNotIgnore("noNode")
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
	"spawnFlagValue",
	"removeDash",
	"removeStamina",
	"relativeToPlayerFacing",
	"nodeRelativeToPlayerFacing",
	"persistent",
	"absoluteCoords",
	"entityPath",
	"dictKeys",
	"dictValues",
	"poofWhenDisappearing",
	"ignoreJustRespawned"
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
	elseif entityToSpawn == "Water" then
		return "objects/KoseiHelper/Controllers/SpawnController/Water"
	elseif entityToSpawn == "Strawberry" then
		return "objects/KoseiHelper/Controllers/SpawnController/Strawberry"
	elseif entityToSpawn == "Decal" then
		return "objects/KoseiHelper/Controllers/SpawnController/Decal"
	elseif entityToSpawn == "Flag" then
		return "objects/KoseiHelper/Controllers/SpawnController/Flag"
	elseif entityToSpawn == "Counter" then
		return "objects/KoseiHelper/Controllers/SpawnController/Counter"
	else
		return "objects/KoseiHelper/Controllers/SpawnController/Broken"
    end
end

return SpawnController