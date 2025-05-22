local MaryBlock = {}

MaryBlock.name = "KoseiHelper/MaryBlock"
MaryBlock.depth = -9500

MaryBlock.placements = {
	{
		name = "Mary Block",
		data = {
			outline = false,
			affectTheo = false,
			oneUse = true,
			maryType = "Idle",
			shutUp = false,
			spriteID = "koseiHelper_maryBlock"
		}
	}
}

MaryBlock.fieldInformation = {
	maryType = {
		options = {
			"Potted",
			"Idle",
			"Mary",
			"Bubble",
			"Dark",
			"Bald"
		},
		editable = false
	}
}

function MaryBlock.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"shutUp"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.maryType == "Bald" then
		doNotIgnore("shutUp")
	end
	return ignored
end

function MaryBlock.selection(room, entity)
    local width, height = 16, 16
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height + 8)
end

function MaryBlock.texture(room, entity)
    local maryType = entity.maryType
    if maryType == "Potted" then
        return "objects/KoseiHelper/MaryBlock/potted00"
    elseif maryType == "Idle" then
        return "objects/KoseiHelper/MaryBlock/idle00"
	elseif maryType == "Mary" then
		return "objects/KoseiHelper/MaryBlock/mary00"
	elseif maryType == "Bubble" then
		return "objects/KoseiHelper/MaryBlock/marybuble00"
	elseif maryType == "Dark" then
		return "objects/KoseiHelper/MaryBlock/dark00"
		elseif maryType == "Bald" then
		return "objects/KoseiHelper/MaryBlock/bald00"
    end
end

return MaryBlock
