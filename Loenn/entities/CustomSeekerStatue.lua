local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local CustomSeekerStatue = {}

CustomSeekerStatue.name = "KoseiHelper/CustomSeekerStatue"

function CustomSeekerStatue.depth(room,entity)
	return entity.depth
end

CustomSeekerStatue.nodeLineRenderType = "line"
CustomSeekerStatue.nodeLimits = {1, -1}
CustomSeekerStatue.texture = "decals/5-temple/statue_e"
CustomSeekerStatue.nodeTexture = "characters/monsters/predator73"
CustomSeekerStatue.placements = {
    {
        name = "CustomSeekerStatue",
        data = {
            hatchMode = "Distance",
			sound = "event:/game/05_mirror_temple/seeker_statue_break",
			flag = "",
			statueSprite = "statue",
			hatchSprite = "hatch",
			depth = 8999,
			distance = 220, -- Default distance for left/right is 32
			particles = true
        }
    }
}

CustomSeekerStatue.fieldInformation = {
    hatchMode = {
		options = {
			"Distance",
			"PlayerLeftOfX",
			"PlayerRightOfX",
			"Flag"
		},
        editable = false
    },
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	distance =
	{
		fieldType = "integer"
	}

}

return CustomSeekerStatue