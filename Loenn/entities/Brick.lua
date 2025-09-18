local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local Brick = {}

Brick.name = "KoseiHelper/Brick"
Brick.depth = -1000

function Brick.depth(room,entity)
	return entity.depth
end

Brick.placements = {
	{
		name = "Brick",
		data = {
			type = "Normal",
			sprite = "koseiHelper_Brick",
			depth = -1000,
			bumpSound = "event:/KoseiHelper/brickBump",
			breakSound = "event:/KoseiHelper/brickBreak",
			brokenByKevins = false
		}
	}
}

Brick.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {
		{"Bricks", -1000}
		}),
        editable = true
    },
	type = {
		options = {
			"Normal",
			"Ice",
			"Fortress"
		},
		editable = false
	}
}

function Brick.selection(room, entity)
    local width, height = 16, 16
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height)
end

function Brick.texture(room, entity)
    local type = entity.type
    if type == "Ice" then
        return "objects/KoseiHelper/Brick/Ice00"
	elseif type == "Fortress" then
        return "objects/KoseiHelper/Brick/Fortress00"
    else
        return "objects/KoseiHelper/Brick/Normal00"
    end
end

return Brick
