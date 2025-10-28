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
		timeToLive = 9999999,
		flagToLive = "KoseiHelper_despawnEntity",
		counterPositive = "",
		appearSound = "event:/KoseiHelper/spawn",
		disappearSound = "event:/game/general/assist_dash_aim",
		flag = "",
		spawnCondition = "OnCustomButtonPress",
		persistent = false,
		absoluteCoords = false,
		poofWhenDisappearing = true,
		ignoreJustRespawned = false,
		gridAligned = false,
		gridSize = 8,
		randomLocation = false,
		spawnAreaColor = "deb887",
		doNotRepeatSpots = true,
		clustered = false,
		clusteredChance = 80,
		--Spawn conditions
		spawnFlag = "koseiHelper_spawn",
		spawnFlagValue = true,
		spawnSpeed = 300,
		spawnLimit = -1,
		everyXDashes = 1,
		cassetteColor = "Any",
		despawnMethod = "TTL",
		-- Entity-specific attributes
		nodeX = 0,
		nodeY = 0,
		blockWidth = 16,
		blockHeight = 16,
		blockTileType = "3",
		
		dummyFix = true,
		
		boosterRed = false,
		boosterSingleUse = false,
		
		cloudFragile = true,
		
		minCounterCap = 0,
		maxCounterCap = 9999,
		decreaseCounter = false,
		
		dashBlockCanDash = true,
		
		dashSwitchPersistent = false,
		dashSwitchAllGates = false,
		dashSwitchSprite = "Default",
		dashSwitchSide = "Up",
		
		decalTexture = "10-farewell/creature_f00",
		decalDepth = 9000,
		dontFlip = false,
		
		fallingBlockBadeline = false,
		fallingBlockClimbFall = false,
		
		featherShielded = false,
		featherSingleUse = true,
		
		flagCycleAt = 9999,
		flagStop = false,
		
		iceballSpeed = 1,
		iceballAlwaysIce = false,
		
		jellyfishBubble = false,
		
		jumpthruTexture = "wood",
		soundIndex = -1,
		
		crushBlockAxe = "Both",
		crushBlockChillout = false,
		
		moveBlockDirection = "Down",
		moveBlockCanSteer = false,
		moveBlockFast = true,
		
		swapBlockTheme = "Normal",
		
		refillTwoDashes = false,
		refillOneUse = false,
		
		blockSinks = false,
		
		playerSpriteMode = "Madeline",
		
		attachToSolid = false,
		crystalColor = "Blue",
		
		circularMovement = false,
		bladeStar = false,
		bladeStartCenter = false,
		clockwise = false,
		bladeSpeed = "Normal",
		
		orientation = "Floor",
		playerCanUse = true,
		
		winged = false,
		moon = false,
		
		onlyOnSafeGround = true,
		
		templeGateType = "NearestSwitch",
		templeGateSprite = "Default",
		
		hasBottom = false,
		
		zipMoverTheme = "Normal",

		-- Custom Entity (or any entity that can be spawned with EntityData)
		entityPath = "",
		dictKeys = "",
		dictValues = "",
		noNode = false,
		
		-- Mouse attributes
		canDragAround = false,
		mouseWheelMode = false,
		wheelOptions = "",
		wheelIndicatorX = 20,
		wheelIndicatorY = 140,
		wheelIndicatorImage = false,
		oppositeDragButton = false
		
		-- Secret attributes that no one should use unless they have a good reason (if you're lurking here you know what you're doing):
		-- noCollider = false,
		-- transitionUpdate = false,
		-- cooldownOffset = 0
		}
	}
}

SpawnController.fieldInformation = function (entity) return {
	entityToSpawn = {
		options = {
		"BadelineBoost",
		"Blade",
		"Booster",
		"BounceBlock",
		"Bumper",
		"Cloud",
		"Counter",
		"CrumblePlatform",
		"DashBlock",
		"DashSwitch",
		"Decal",
		"DreamBlock",
		"DustBunny",
		"ExitBlock",
		"FallingBlock",
		"Feather",
		"Flag",
		"FloatySpaceBlock",
		"GlassBlock",
		"Heart",
		"Iceball",
		"IceBlock",
		"Jellyfish",
		"JumpthruPlatform",
		"Kevin",
		"MoveBlock",
		"Oshiro",
		"Player",
		"Puffer",
		"Refill",
		"Seeker",
		"SpawnPoint",
		"Spinner",
		"Spring",
		"StarJumpBlock",
		"Strawberry",
		"SwapBlock",
		"TempleGate",
		"TheoCrystal",
		"Water",
		"ZipMover",
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
	despawnMethod = {
		options = {
		"None",
		"TTL",
		"Flag",
		"TTLFlag",
		"OverriddenByNext"
		},
		editable = false
	},
	appearSound = {
		options = {
		"event:/KoseiHelper/spawn",
		"event:/new_content/game/10_farewell/ppt_mouseclick",
		"event:/none"
		},
		editable = true
	},
	disappearSound = {
		options = {
		"event:/game/general/assist_dash_aim",
		"event:/KoseiHelper/nwaps",
		"event:/none"
		},
		editable = true
	},
	spawnCooldown = {
		minimumValue = 0
	},
	timeToLive = {
		minimumValue = 0.00000001
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
		"OnJump",
		"LeftClick",
		"RightClick"
		},
		editable = false
	},
	spawnLimit = {
		fieldType = "integer"
	},
	offsetX = {
		fieldType = "integer"
	},
	offsetY = {
		fieldType = "integer"
	},
	nodeX = {
		fieldType = "integer"
	},
	nodeY = {
		fieldType = "integer"
	},
	blockWidth = {
		fieldType = "integer"
	},
	blockHeight = {
		fieldType = "integer"
	},
	blockTileType = {
		options = fakeTilesHelper.getTilesOptions(entity.fg and "tilesFg"),
		editable = false
	},
	everyXDashes = {
		fieldType = "integer",
		minimumValue = 1
	},
	spawnAreaColor = {
		fieldType = "color"
	},
	clusteredChance = {
		minimumValue = 0,
		maximumValue = 100
	},
	gridSize = {
		fieldType = "integer",
		minimumValue = 1
	},
	-- Specific entities
	bladeSpeed = {
		options = {
		"Slow",
		"Normal",
		"Fast"
		},
		editable = false
	},
	dashSwitchSprite = {
		options = {
		"Default",
		"Mirror"
		},
		editable = true
	},
	dashSwitchSide = {
		options = {
		"Up",
		"Down",
		"Left",
		"Right",
		"MovingDirection"
		},
		editable = false
	},
	minCounterCap = {
		fieldType = "integer"
	},
	maxCounterCap = {
		fieldType = "integer"
	},
	decalDepth = {
		fieldType = "integer"
	},
	flagCycleAt = {
		fieldType = "integer"
	},
	jumpthruTexture = {
        options = textures
    },
	crushBlockAxe = {
		options = {
		"Both",
		"Horizontal",
		"Vertical"
		},
		editable = false
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
	soundIndex = {
        options = enums.tileset_sound_ids,
        fieldType = "integer"
    },
	playerSpriteMode = {
		options = {
		"Madeline",
		"MadelineNoBackPack",
		"Badeline",
		"MadelineAsBadeline",
		"Playback"
		},
		editable = false
	},
	crystalColor = {
		options = {
		"Blue",
		"Red",
		"Purple",
		"Rainbow"
		},
		editable = false
	},
	orientation = {
		options = {
		"Floor",
		"WallLeft",
		"WallRight",
		"PlayerFacing",
		"PlayerFacingOpposite"
		},
		editable = false
	},
	templeGateType = {
		options = {
		"NearestSwitch",
		"CloseBehindPlayer",
		"CloseBehindPlayerAlways",
		"HoldingTheo",
		"TouchSwitches",
		"CloseBehindPlayerAndTheo"
		},
		editable = false
	},
	templeGateSprite = {
		options = {
		"Default",
		"Mirror",
		"Theo"
		},
		editable = true
	},
	zipMoverTheme = {
		options = {
		"Normal",
		"Moon"
		},
		editable = false
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
	"timeToLive",
	"flagToLive",
	"poofWhenDisappearing",
	"disappearSound",
	"everyXDashes",
	"cassetteColor",
	"gridSize",
	"spawnAreaColor",
	"doNotRepeatSpots",
	"clustered",
	"clusteredChance",
	
	"canDragAround",
	"mouseWheelMode",
	"wheelOptions",
	"wheelIndicatorX",
	"wheelIndicatorY",
	"wheelIndicatorImage",
	"oppositeDragButton",
	
	"dummyFix",
	
	"boosterRed",
	"boosterSingleUse",
	
	"cloudFragile",
	
	"crushBlockAxe",
	"crushBlockChillout",
	
	"dashSwitchPersistent",
	"dashSwitchAllGates",
	"dashSwitchSprite",
	"dashSwitchSide",
	
	"decalTexture",
	"decalDepth",
	"dontFlip",
	
	"fallingBlockBadeline",
	"fallingBlockClimbFall",
	
	"featherShielded",
	"featherSingleUse",
	
	"jellyfishBubble",
	
	"dashBlockCanDash",
	
	"iceballSpeed",
	"iceballAlwaysIce",
	
	"jumpthruTexture",
	"soundIndex",
	
	"moveBlockDirection",
	"moveBlockCanSteer",
	"moveBlockFast",
	
	"swapBlockTheme", 
	
	"spawnFlag",
	"spawnFlagValue",
	"spawnSpeed",
	
	"refillTwoDashes",
	"refillOneUse",
	
	"blockSinks",
	
	"attachToSolid",
	"crystalColor",
	"bladeStar",
	"circularMovement",
	"clockwise",
	"bladeStartCenter",
	"bladeSpeed",
	
	"orientation",
	"playerCanUse",
	
	"winged",
	"moon",
	
	"playerSpriteMode",
	
	"flagCycleAt",
	"flagStop",
	
	"minCounterCap",
	"maxCounterCap",
	"decreaseCounter",
	
	"templeGateType",
	"templeGateSprite",
	
	"onlyOnSafeGround",
	
	"hasBottom",
	
	"zipMoverTheme",
	
	"entityPath",
	"dictKeys",
	"dictValues",
	"noNode"
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
	if entity.despawnMethod == "TTL" or entity.despawnMethod == "TTLFlag" then
		doNotIgnore("timeToLive")
	end
	if entity.despawnMethod ~= "None" then
		doNotIgnore("poofWhenDisappearing")
		doNotIgnore("disappearSound")
	end
	if entity.despawnMethod == "Flag" or entity.despawnMethod == "TTLFlag" then
		doNotIgnore("flagToLive")
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
	if entity.gridAligned then
		doNotIgnore("gridSize")
	end
	if entity.randomLocation then
		doNotIgnore("spawnAreaColor")
		doNotIgnore("doNotRepeatSpots")
		doNotIgnore("clustered")
	end
	if entity.clustered then
		doNotIgnore("clusteredChance")
	end
	-- Mouse options
	if entity.spawnCondition == "LeftClick" or entity.spawnCondition == "RightClick" then
		doNotIgnore("canDragAround")
		doNotIgnore("mouseWheelMode")
		if entity.canDragAround == true then
			doNotIgnore("oppositeDragButton")
		end
		if entity.mouseWheelMode == true then
			doNotIgnore("wheelOptions")
			doNotIgnore("wheelIndicatorX")
			doNotIgnore("wheelIndicatorY")
			doNotIgnore("wheelIndicatorImage")
		end
	end
	-- Entity attributes
	if entity.spawnCondition == "BadelineBoost" then
		doNotIgnore("dummyFix")
	end
	if entity.entityToSpawn == "Blade" or entity.entityToSpawn == "DustBunny" then
		doNotIgnore("circularMovement")
		if entity.entityToSpawn == "Blade" then
			doNotIgnore("bladeStar")
		end
		if entity.circularMovement == true then
			doNotIgnore("clockwise")
		else
			doNotIgnore("bladeSpeed")
			doNotIgnore("bladeStartCenter")
		end
	end
	if entity.entityToSpawn == "Booster" then
		doNotIgnore("boosterRed")
		doNotIgnore("boosterSingleUse")
	end
	if entity.entityToSpawn == "Cloud" then
		doNotIgnore("cloudFragile")
	end
	if entity.entityToSpawn == "Counter" then
		doNotIgnore("minCounterCap")
		doNotIgnore("maxCounterCap")
		doNotIgnore("decreaseCounter")
	end
	if entity.entityToSpawn == "CrumblePlatform" then
		doNotIgnore("blockWidth")
	end
	if entity.entityToSpawn == "DashBlock" then
		doNotIgnore("dashBlockCanDash")
	end
	if entity.entityToSpawn == "DashSwitch" then
		doNotIgnore("dashSwitchPersistent")
		doNotIgnore("dashSwitchAllGates")
		doNotIgnore("dashSwitchSprite")
		doNotIgnore("dashSwitchSide")
	end
	if entity.entityToSpawn == "Decal" then
		doNotIgnore("decalTexture")
		doNotIgnore("decalDepth")
		doNotIgnore("dontFlip")
	end
	if entity.entityToSpawn == "FallingBlock" then
		doNotIgnore("fallingBlockBadeline")
		doNotIgnore("fallingBlockClimbFall")
	end
	if entity.entityToSpawn == "Feather" then
		doNotIgnore("featherShielded")
		doNotIgnore("featherSingleUse")
	end
	if entity.entityToSpawn == "Flag" then
		doNotIgnore("flagCycleAt")
		doNotIgnore("flagStop")
	end
	if entity.entityToSpawn == "Iceball" then
		doNotIgnore("iceballSpeed")
		doNotIgnore("iceballAlwaysIce")
	end
	if entity.entityToSpawn == "Jellyfish" then
		doNotIgnore("jellyfishBubble")
	end
	if entity.entityToSpawn == "JumpthruPlatform" then
		doNotIgnore("jumpthruTexture")
		doNotIgnore("blockWidth")
		doNotIgnore("soundIndex")
	end
	if entity.entityToSpawn == "Kevin" then
		doNotIgnore("crushBlockAxe")
		doNotIgnore("crushBlockChillout")
	end
	if entity.entityToSpawn == "MoveBlock" then
		doNotIgnore("moveBlockDirection")
		doNotIgnore("moveBlockCanSteer")
		doNotIgnore("moveBlockFast")
	end
	if entity.entityToSpawn == "Refill" then
		doNotIgnore("refillTwoDashes")
		doNotIgnore("refillOneUse")
	end
	if entity.entityToSpawn == "GlassBlock" or entity.entityToSpawn == "StarJumpBlock" then
		doNotIgnore("blockSinks")
	end
	if entity.entityToSpawn == "Player" then
		doNotIgnore("playerSpriteMode")
	end
	if entity.entityToSpawn == "Spinner" then
		doNotIgnore("attachToSolid")
		doNotIgnore("crystalColor")
	end
	if entity.entityToSpawn == "SpawnPoint" then
		doNotIgnore("onlyOnSafeGround")
	end
	if entity.entityToSpawn == "Spring" then
		doNotIgnore("orientation")
		doNotIgnore("playerCanUse")
	end
	if entity.entityToSpawn == "Strawberry" then
		doNotIgnore("winged")
		doNotIgnore("moon")
	end
	if entity.entityToSpawn == "SwapBlock" then
		doNotIgnore("swapBlockTheme")
	end
	if entity.entityToSpawn == "TempleGate" then
		doNotIgnore("templeGateType")
		doNotIgnore("templeGateSprite")
	end
	if entity.entityToSpawn == "Water" then
		doNotIgnore("hasBottom")
	end
	if entity.entityToSpawn == "ZipMover" then
		doNotIgnore("zipMoverTheme")
	end
	-- Noded entities
	if entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "Iceball" or entity.entityToSpawn == "Blade" or
		(entity.entityToSpawn == "DustBunny" and entity.noNode == false) then
		doNotIgnore("nodeX")
		doNotIgnore("nodeY")
		doNotIgnore("nodeRelativeToPlayerFacing")
	end
	-- Entities with tileset
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" or entity.entityToSpawn == "FloatySpaceBlock" or entity.entityToSpawn == "ExitBlock" then
		doNotIgnore("blockTileType")
	end
	--Entities with size
	if entity.entityToSpawn == "DashBlock" or entity.entityToSpawn == "FallingBlock" or entity.entityToSpawn == "IceBlock" or entity.entityToSpawn == "MoveBlock" or entity.entityToSpawn == "StarJumpBlock"
	or entity.entityToSpawn == "Kevin"	or entity.entityToSpawn == "SwapBlock" or entity.entityToSpawn == "ZipMover" or entity.entityToSpawn == "DreamBlock" or entity.entityToSpawn == "GlassBlock"
	or entity.entityToSpawn == "FloatySpaceBlock" or entity.entityToSpawn == "Water" or entity.entityToSpawn == "ExitBlock" then
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
	if entity.entityToSpawn == "CustomEntity" or entity.entityToSpawn == "DustBunny" then
		doNotIgnore("noNode")
	end
	return ignored
end

SpawnController.fieldOrder =  {
	"x",
	"y",
	"entityToSpawn",
	"spawnCondition",
	"despawnMethod",
	-- spawn conditions
	"spawnSpeed",
	"spawnFlag",
	"everyXDashes",
	"cassetteColor",
	-- general attributes
	"offsetX",
	"offsetY",
	"nodeX",
	"nodeY",
	"appearSound",
	"disappearSound",
	"timeToLive",
	"flagToLive",
	"counterPositive",
	"spawnCooldown",
	"spawnLimit",
	"blockWidth",
	"blockHeight",
	"blockTileType",
	"flag",
	"entityPath",
	"dictKeys",
	"dictValues",
	"wheelOptions",
	"wheelIndicatorX",
	"wheelIndicatorY",
	"gridSize",
	"spawnAreaColor",
	"clusteredChance",
	-- non-boolean entity-specific attributes
	"bladeSpeed",
	
	"minCounterCap",
	"maxCounterCap",
	
	"dashSwitchSprite",
	"dashSwitchSide",
	
	"decalTexture",
	"decalDepth",
	
	"flagCycleAt",
	
	"iceballSpeed",
	
	"crushBlockAxe",
	
	"moveBlockDirection",
	
	"jumpthruTexture",
	"soundIndex",
	
	"crystalColor",
	
	"orientation",
	
	"playerSpriteMode",
	
	"swapBlockTheme",
	
	"templeGateSprite",
	"templeGateType",
	
	"zipMoverTheme",
	
	-- general booleans
	"flagValue",
	"spawnFlagValue",
	"removeDash",
	"removeStamina",
	"relativeToPlayerFacing",
	"nodeRelativeToPlayerFacing",
	"persistent",
	"absoluteCoords",
	"poofWhenDisappearing",
	"ignoreJustRespawned",
	"gridAligned",
	"randomLocation",
	"doNotRepeatSpots",
	"clustered",
	--"transitionUpdate",
	"noNode"
}

function SpawnController.texture(room, entity)
    local entityToSpawn = entity.entityToSpawn
	if entityToSpawn == "BadelineBoost" then
        return "objects/KoseiHelper/Controllers/SpawnController/BadelineBoost"
	elseif entityToSpawn == "Blade" then
		if entity.bladeStar then
			return "objects/KoseiHelper/Controllers/SpawnController/Star"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/Blade"
		end
	elseif entityToSpawn == "Bumper" then
        return "objects/KoseiHelper/Controllers/SpawnController/Bumper"
    elseif entityToSpawn == "Booster" then
		if entity.boosterRed == true then
			return "objects/KoseiHelper/Controllers/SpawnController/Booster"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/BoosterGreen"
		end
	elseif entityToSpawn == "BounceBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/BounceBlock"
	elseif entityToSpawn == "Cloud" then
		if entity.cloudFragile == true then
			return "objects/KoseiHelper/Controllers/SpawnController/CloudFragile"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/Cloud"
		end
	elseif entityToSpawn == "Counter" then
		return "objects/KoseiHelper/Controllers/SpawnController/Counter"
	elseif entityToSpawn == "DashBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/DashBlock"
	elseif entityToSpawn == "DashSwitch" then
		if entity.dashSwitchSprite == "Mirror" then
			if entity.dashSwitchSide == "Left" then
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchLeftMirror"
			elseif entity.dashSwitchSide == "Right" then
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchRightMirror"
			elseif entity.dashSwitchSide == "Down" then
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchDownMirror"
			else
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchUpMirror"
			end
		else
			if entity.dashSwitchSide == "Left" then
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchLeftDefault"
			elseif entity.dashSwitchSide == "Right" then
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchRightDefault"
			elseif entity.dashSwitchSide == "Down" then
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchDownDefault"
			else
				return "objects/KoseiHelper/Controllers/SpawnController/DashSwitchUpDefault"
			end
		end
	elseif entityToSpawn == "Decal" then
		return "objects/KoseiHelper/Controllers/SpawnController/Decal"
	elseif entityToSpawn == "DustBunny" then
		return "objects/KoseiHelper/Controllers/SpawnController/DustBunny"
	elseif entityToSpawn == "ExitBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/ExitBlock"
	elseif entityToSpawn == "FallingBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/FallingBlock"
	elseif entityToSpawn == "Feather" then
		if entity.featherShielded then
			return "objects/KoseiHelper/Controllers/SpawnController/FeatherShielded"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/Feather"
		end
	elseif entityToSpawn == "Flag" then
		return "objects/KoseiHelper/Controllers/SpawnController/Flag"
	elseif entityToSpawn == "FloatySpaceBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/FloatySpaceBlock"
	elseif entityToSpawn == "GlassBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/GlassBlock"
	elseif entityToSpawn == "Heart" then
        return "objects/KoseiHelper/Controllers/SpawnController/Heart"
	elseif entityToSpawn == "Iceball" then
        return "objects/KoseiHelper/Controllers/SpawnController/Iceball"
	elseif entityToSpawn == "IceBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/IceBlock"
	elseif entityToSpawn == "Jellyfish" then
		if entity.jellyfishBubble then
			return "objects/KoseiHelper/Controllers/SpawnController/JellyfishFloating"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/Jellyfish"
		end
	elseif entityToSpawn == "JumpthruPlatform" then
		return "objects/KoseiHelper/Controllers/SpawnController/JumpthruPlatform"
	elseif entityToSpawn == "Kevin" then
		return "objects/KoseiHelper/Controllers/SpawnController/Kevin"
	elseif entityToSpawn == "MoveBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/MoveBlock"
	elseif entityToSpawn == "Seeker" then
        return "objects/KoseiHelper/Controllers/SpawnController/Seeker"
	elseif entityToSpawn == "SwapBlock" then
        return "objects/KoseiHelper/Controllers/SpawnController/SwapBlock"
	elseif entityToSpawn == "CrumblePlatform" then
		return "objects/KoseiHelper/Controllers/SpawnController/CrumblePlatform"
	elseif entityToSpawn == "DreamBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/DreamBlock"
	elseif entityToSpawn == "Oshiro" then
		return "objects/KoseiHelper/Controllers/SpawnController/Oshiro"
	elseif entityToSpawn == "Player" then
		return "objects/KoseiHelper/Controllers/SpawnController/Player"
	elseif entityToSpawn == "Puffer" then
        return "objects/KoseiHelper/Controllers/SpawnController/Puffer"
	elseif entityToSpawn == "Refill" then
	if entity.refillTwoDashes == true then
			return "objects/KoseiHelper/Controllers/SpawnController/RefillPink"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/Refill"
		end
	elseif entityToSpawn == "SpawnPoint" then
		return "objects/KoseiHelper/Controllers/SpawnController/SpawnPoint"
	elseif entityToSpawn == "Spinner" then
		if entity.crystalColor == "Purple" then
			return "objects/KoseiHelper/Controllers/SpawnController/SpinnerPurple"
		elseif entity.crystalColor == "Red" then
			return "objects/KoseiHelper/Controllers/SpawnController/SpinnerRed"
		elseif entity.crystalColor == "Rainbow" then
			return "objects/KoseiHelper/Controllers/SpawnController/SpinnerWhite"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/SpinnerBlue"
		end
	elseif entityToSpawn == "Spring" then
		if entity.orientation == "WallLeft" then
			return "objects/KoseiHelper/Controllers/SpawnController/SpringWallLeft"
		elseif entity.orientation == "WallRight" then
			return "objects/KoseiHelper/Controllers/SpawnController/SpringWallRight"
		elseif entity.orientation == "PlayerFacing" then
			return "objects/KoseiHelper/Controllers/SpawnController/SpringPlayerFacing"
		elseif entity.orientation == "PlayerFacingOpposite" then
			return "objects/KoseiHelper/Controllers/SpawnController/SpringPlayerFacingOpposite"
		else
			return "objects/KoseiHelper/Controllers/SpawnController/SpringFloor"
		end
	elseif entityToSpawn == "StarJumpBlock" then
		return "objects/KoseiHelper/Controllers/SpawnController/StarJumpBlock"
	elseif entityToSpawn == "Strawberry" then
		if entity.moon then
			if entity.winged then
				return "objects/KoseiHelper/Controllers/SpawnController/StrawberryWingedMoon"
			else
				return "objects/KoseiHelper/Controllers/SpawnController/StrawberryMoon"
			end
		else
			if entity.winged then
				return "objects/KoseiHelper/Controllers/SpawnController/StrawberryWinged"
			else
				return "objects/KoseiHelper/Controllers/SpawnController/Strawberry"
			end
		end
	elseif entityToSpawn == "TheoCrystal" then
        return "objects/KoseiHelper/Controllers/SpawnController/TheoCrystal"
	elseif entityToSpawn == "TempleGate" then
		if entity.templeGateSprite == "Mirror" then
			return "objects/KoseiHelper/Controllers/SpawnController/TempleGateMirror"
		elseif entity.templeGateSprite == "Theo" then
			return "objects/KoseiHelper/Controllers/SpawnController/TempleGateTheo"
		else -- Default
			return "objects/KoseiHelper/Controllers/SpawnController/TempleGate"
		end
	elseif entityToSpawn == "Water" then
		return "objects/KoseiHelper/Controllers/SpawnController/Water"
	elseif entityToSpawn == "ZipMover" then
        return "objects/KoseiHelper/Controllers/SpawnController/ZipMover"
	else
		return "objects/KoseiHelper/Controllers/SpawnController/Broken"
    end
end

return SpawnController