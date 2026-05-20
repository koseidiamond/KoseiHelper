local MoveBlockCrystal = {
    name = "KoseiHelper/MoveBlockCrystal",
    depth = -100,
    --texture = "objects/CC/MoveBlockCrystal/idle00",
    placements = {
        {
            name = "MoveBlockCrystal",
            data = {
			singleUse = false,
			sprite = "objects/KoseiHelper/Crossover/MoveBlockCrystal/right/",
			behavior = "Move",
			direction = "Right",
			refillDash = true,
			moveBlockType = "CommunalConnected"
            }
        }
    }
}

MoveBlockCrystal.fieldInformation = {
	behavior = {
		options = {
		"Move",
		"MoveAndChangeDirection",
		"ChangeDirection",
		"Break"
		},
		editable = false
	},
	moveBlockType = {
		options = {
		"Vanilla",
		"CommunalConnected",
		"Both",
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
	},
	sprite = {
		options = {
		"objects/KoseiHelper/Crossover/MoveBlockCrystal/leftArrow/",
		"objects/KoseiHelper/Crossover/MoveBlockCrystal/rightArrow/",
		"objects/KoseiHelper/Crossover/MoveBlockCrystal/down/",
		"objects/KoseiHelper/Crossover/MoveBlockCrystal/left/",
		"objects/KoseiHelper/Crossover/MoveBlockCrystal/right/",
		"objects/KoseiHelper/Crossover/MoveBlockCrystal/up/",
		"objects/KoseiHelper/Crossover/MoveBlockCrystal/break/"
		},
		editable = true
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
	if entity.behavior == "MoveAndChangeDirection" or entity.behavior == "ChangeDirection" then
		doNotIgnore("direction")
	end
	return ignored
end

function MoveBlockCrystal.texture(room, entity)
	return entity.sprite .. "idle00"
end

return MoveBlockCrystal