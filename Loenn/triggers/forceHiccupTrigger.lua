local ForceHiccupTrigger = {}

ForceHiccupTrigger.name = "KoseiHelper/ForceHiccupTrigger"
ForceHiccupTrigger.depth = 100

ForceHiccupTrigger.placements = {
	{
		name = "Force Hiccup Trigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		interval = 0.5,
		flag = "cold"
		}
	}
}

function ForceHiccupTrigger.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	"interval",
	"flag"
	}
	local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.triggerMode == "OnStay" then
		doNotIgnore("interval")
	end
	if entity.triggerMode == "OnFlagEnabled" then
		doNotIgnore("flag")
	end
	return ignored
end

ForceHiccupTrigger.fieldInformation = function (entity) return {
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnStay",
		"OnFlagEnabled",
		"OnCassetteBeat"
		},
		editable = false
	}
}
end

return ForceHiccupTrigger
