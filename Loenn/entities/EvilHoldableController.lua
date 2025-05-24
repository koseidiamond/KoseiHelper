local EvilHoldableController = {}

EvilHoldableController.name = "KoseiHelper/EvilHoldableController"
EvilHoldableController.depth = -9500
EvilHoldableController.texture = "objects/KoseiHelper/Controllers/EvilHoldableController"
EvilHoldableController.placements = {
	{
		name = "EvilHoldableController",
		data = {
		timeToKill = 1,
		drainsStamina = true,
		dropIfNoStamina = false,
		drainsDash = false,
		sound = "event:/game/05_mirror_temple/eyebro_eyemove",
		staminaDrainRate = 1
		}
	}
}

function EvilHoldableController.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"staminaDrainRate"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.drainsStamina == true then
		doNotIgnore("staminaDrainRate")
	end
	return ignored
end

return EvilHoldableController