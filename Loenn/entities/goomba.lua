local Goomba = {}

Goomba.name = "KoseiHelper/Goomba"
Goomba.depth = -100

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
			canEnableTouchSwitches = false
		}
	}
}

Goomba.fieldInformation = {
	timeToSpawnMinis = {
		minimumValue = 0.001
	},
	gravityMultiplier = {
		minimumValue = 1.0
	},
	behavior = {
		options = {
			"Chaser",
			"Dumb",
			"Smart"
		},
		editable = false
	}
}

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
