local Goomba = {}

Goomba.name = "KoseiHelper/Goomba"
Goomba.depth = -1

Goomba.placements = {
	{
		name = "Goomba",
		data = {
			speed = 50,
			outline = false,
			isWide = false,
			isWinged = false,
			canBeBounced = true,
			spawnMinis = false,
			timeToSpawnMinis = 1,
			flyAway = false,
			behavior = "Chaser",
			gravityMultiplier = 1,
			canEnableTouchSwitches = false,
			minisAmount = 10,
			slowdown = false,
			deathSound = "event:/KoseiHelper/goomba",
			spriteID = "koseiHelper_goomba",
			deathAnimation = false,
			flagOnDeath = "",
			color = "FFFFFF",
			particleColor = "ff6def", -- Player.P_Split.Color
			slowdownDistanceMax = 40,
			slowdownDistanceMin = 16,
			instantFlagOnDeath = false
		}
	}
}

function Goomba.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"minisAmount",
	"timeToSpawnMinis",
	"slowdownDistanceMax",
	"slowdownDistanceMin",
	"instantFlagOnDeath"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.spawnMinis == true then
		doNotIgnore("minisAmount")
		doNotIgnore("timeToSpawnMinis")
	end
	if entity.slowdown == true then
		doNotIgnore("slowdownDistanceMax")
		doNotIgnore("slowdownDistanceMin")
	end
	if entity.flagOnDeath ~= nil and entity.flagOnDeath ~= '' then
		doNotIgnore("instantFlagOnDeath")
	end
	return ignored
end

Goomba.fieldInformation = {
	color = {
        fieldType = "color",
		useAlpha = true
    },
	particleColor = {
        fieldType = "color",
		useAlpha = true
    },
	timeToSpawnMinis = {
		minimumValue = 0.001
	},
	gravityMultiplier = {
		minimumValue = 1.0
	},
	slowdownDistanceMax = {
		minimumValue = 0
	},
	behavior = {
		options = {
			"Chaser",
			"Dumb",
			"Smart",
			"SuperSmart"
		},
		editable = false
	}
}

local function hexToRGB(hex)
    local r = tonumber(hex:sub(1, 2), 16) / 255
    local g = tonumber(hex:sub(3, 4), 16) / 255
    local b = tonumber(hex:sub(5, 6), 16) / 255
    return r, g, b
end

function Goomba.texture(room, entity)
    local wide = entity.isWide
	local winged = entity.isWinged
    if wide and winged then
        return "objects/KoseiHelper/Goomba/widewingedidle00"
    elseif winged then
        return "objects/KoseiHelper/Goomba/wingedidle00"
    elseif wide then
        return "objects/KoseiHelper/Goomba/wideidle00"
    else
        return "objects/KoseiHelper/Goomba/idle00"
    end
end

return Goomba
