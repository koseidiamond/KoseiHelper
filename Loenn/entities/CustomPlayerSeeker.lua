local CustomPlayerSeeker = {}

CustomPlayerSeeker.name = "KoseiHelper/CustomPlayerSeeker"
CustomPlayerSeeker.depth = -100

CustomPlayerSeeker.placements = {
	{
		name = "CustomPlayerSeeker",
		data = {
			nextRoom = "c-00",
			colorgrade = "templevoid",
			sprite = "seeker",
			seekerDashSound = "event:/game/05_mirror_temple/seeker_dash",
			vanillaEffects = false,
			onlyBreakAnimation = false,
			canSwitchCharacters = false,
			speedMultiplier = 1,
			isFriendly = false,
			flagToSwap = "",
			flagToHatch = "",
			canShatterSpinners = false
		}
	}
}

function CustomPlayerSeeker.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"flagToSwap",
	"onlyBreakAnimation"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.canSwitchCharacters then
		doNotIgnore("flagToSwap")
	end
	if entity.vanillaEffects then
		doNotIgnore("onlyBreakAnimation")
	end
	return ignored
end

CustomPlayerSeeker.texture = "decals/5-temple/statue_e"

return CustomPlayerSeeker