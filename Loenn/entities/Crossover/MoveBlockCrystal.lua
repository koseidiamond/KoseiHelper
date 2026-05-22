local MoveBlockCrystal = {
    name = "KoseiHelper/MoveBlockCrystal",
    depth = -100,
    --texture = "objects/CC/MoveBlockCrystal/idle00",
    placements = {
        {
            name = "MoveBlockCrystal",
            data = {
			singleUse = false,
			sprite = "objects/KoseiHelper/Crossover/MoveBlockCrystal/",
			behavior = "Move",
			direction = "Right",
			refillDash = true
            }
        }
    }
}

MoveBlockCrystal.fieldInformation = {
	behavior = {
		options = {
		"Move",
		"ChangeDirection",
		"Break"
		},
		editable = false
	},
	direction = {
		options = {
		"Down",
		"Left",
		"Right",
		"Up"
		},
		editable = false
	}
}

function MoveBlockCrystal.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"direction"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.behavior == "ChangeDirection" then
		doNotIgnore("direction")
	end
	return ignored
end

function MoveBlockCrystal.texture(room, entity)
	if entity.behavior == "ChangeDirection" then
		if entity.direction == "Left" then
			return entity.sprite .. "left/idle00"
		elseif entity.direction == "Right" then
			return entity.sprite .. "right/idle00"
		elseif entity.direction == "Down" then
			return entity.sprite .. "down/idle00"
		else
			return entity.sprite .. "up/idle00"
		end
	elseif entity.behavior == "Move" then
		return entity.sprite .. "move/idle00"
	else
		return entity.sprite .. "break/idle00"
	end
end

return MoveBlockCrystal