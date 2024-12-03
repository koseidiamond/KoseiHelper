local Plant = {}

Plant.name = "KoseiHelper/Plant"
Plant.depth = -100

function Plant.offset(room, entity)
	local plantType = entity.plantType
	if plantType == "Green" or plantType == "Red" then
		return {0, 32}
	else
		return {0, 0}
	end
end

Plant.placements = {
	{
		name = "Plant",
		data = {
			plantType = "Black",
			movingSpeed = 1,
			canShoot = false,
			shootSpeed = 1.5,
			distance = 88
		}
	}
}

Plant.fieldInformation = {
	plantType = {
		options = {
			"Black",
			"Jumping",
			"Green",
			"Red"
		},
		editable = false
	}
}

function Plant.selection(room, entity)
    local width, height = 16, 16
	local plantType = entity.plantType
	if plantType == "Black" or plantType == "Jumping" then
		return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height)
	else
		return utils.rectangle (entity.x - width / 2, entity.y - height - 32, width, height + 16)
	end
	
end

function Plant.texture(room, entity)
	local plantType = entity.plantType
	local canShoot = entity.canShoot
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
