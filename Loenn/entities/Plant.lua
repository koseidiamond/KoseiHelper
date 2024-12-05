local Plant = {}

Plant.name = "KoseiHelper/Plant"
Plant.depth = -100

Plant.placements = {
	{
		name = "Plant",
		data = {
			plantType = "Black",
			movingSpeed = 1,
			canShoot = false,
			shootSpeed = 1,
			distance = 64,
			direction = "Up",
			cycleOffset = false
		}
	}
}

function Plant.offset(room, entity)
	local plantType = entity.plantType
	local direction = entity.direction
	if plantType == "Green" and (direction == "Right" or direction == "Down") then
		return {0,0}
	elseif plantType == "Green" or plantType == "Red" then
		return {0, 32}
	else
		return {0, 0}
	end
end

function Plant.selection(room, entity)
    local width, height = 16, 16
	local plantType = entity.plantType
	local direction = entity.direction
	if plantType == "Black" or plantType == "Jumping" then
		return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height)
	else
		if direction == "Down" and plantType ~= "Green" then
			return utils.rectangle (entity.x - width / 2, entity.y - height +32, width, height + 16)
		elseif direction == "Down" and plantType == "Green" then
			return utils.rectangle (entity.x - width / 2, entity.y - height, width, height + 16)
		elseif direction == "Left" then
			return utils.rectangle (entity.x - width / 2 - 40, entity.y - height + 8, width + 16, height)
		elseif direction == "Right" and plantType ~= "Red" then
			return utils.rectangle (entity.x - width / 2 - 8, entity.y - height + 8, width + 16, height)
		elseif direction == "Right" and plantType == "Red" then
			return utils.rectangle (entity.x - width / 2 +24, entity.y - height + 8, width + 16, height)
		else
			return utils.rectangle (entity.x - width / 2, entity.y - height - 32, width, height + 16)
		end
	end
end

function Plant.rotation(room, entity)
	local direction = entity.direction
	if direction == "Left" then
		return (math.pi * 1.5)
	elseif direction == "Right" then
		return (math.pi / 2)
	elseif direction == "Down" then
		return math.pi
	else
		return 0
	end
end

function Plant.scale(room, entity)
	local direction = entity.direction
	if direction == "Right" then
		return {-1, 1}
	else
		return {1, 1}
	end
end

Plant.fieldInformation = {
	plantType = {
		options = {
			"Black",
			"Jumping",
			"Green",
			"Red",
			"Melon"
		},
		editable = false
	},
	direction = {
		options = {
			"Up",
			"Left",
			"Right",
			"Down"
		},
		editable = false
	},
	distance = { fieldType = "integer"}
}

function Plant.texture(room, entity)
	local plantType = entity.plantType
	local canShoot = entity.canShoot
	if plantType == "Melon" then
		return "objects/KoseiHelper/Plant/Melon00"
	end
	if not canShoot then
		if plantType == "Jumping" then
			return "objects/KoseiHelper/Plant/Jumping00"
		elseif plantType == "Green" then
			return "objects/KoseiHelper/Plant/GreenIdle00"
		elseif plantType == "Red" then
			return "objects/KoseiHelper/Plant/RedIdle00"
		else
			return "objects/KoseiHelper/Plant/Black00"
		end
	else
		if plantType == "Jumping" then
			return "objects/KoseiHelper/Plant/Jumping00"
		elseif plantType == "Green" then
			return "objects/KoseiHelper/Plant/GreenShootDown00"
		elseif plantType == "Red" then
			return "objects/KoseiHelper/Plant/RedShootDown00"
		else
			return "objects/KoseiHelper/Plant/Black00"
		end
	end
end

return Plant
