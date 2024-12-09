local SleepyController = {}

SleepyController.name = "KoseiHelper/SleepyController"
SleepyController.depth = -9500
SleepyController.placements = {
	{
		name = "SleepyController",
		data = {
		minTime = 1.2,
		maxTime = 1.8,
		napTime = 0.15,
		adjustHairColor = true,
		cassetteSleep = false
		}
	}
}

SleepyController.fieldInformation = {
	minTime = { minimumValue = 0.0000001 },
	maxTime = { minimumValue = 0.0000001 },
	napTime = { minimumValue = 0.0000001 }
}

SleepyController.fieldOrder = {
	"minTime",
	"maxTime",
	"napTime",
	"adjustHairColor",
	"cassetteSleep"
}

function SleepyController.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id"
	}
	return ignored
end

function SleepyController.texture(room, entity)
    return "objects/KoseiHelper/Controllers/SleepyController"
end

return SleepyController