local FlagFakeBerry = {}

FlagFakeBerry.name = "KoseiHelper/FlagFakeBerry"
FlagFakeBerry.depth = -100
FlagFakeBerry.nodeLineRenderType = "fan"
FlagFakeBerry.nodeLimits = {0, -1}

function FlagFakeBerry.texture(room, entity)
    local moon = entity.moon
    local winged = entity.winged
    local hasNodes = entity.nodes and #entity.nodes > 0

    if moon then
        if winged or hasNodes then
            return "collectables/moonBerry/ghost00"

        else
            return "collectables/moonBerry/normal00"
        end

    else
        if winged then
            if hasNodes then
                return "collectables/ghostberry/wings01"

            else
                return "collectables/strawberry/wings01"
            end

        else
            if hasNodes then
                return "collectables/ghostberry/idle00"

            else
                return "collectables/strawberry/normal00"
            end
        end
    end
end

function FlagFakeBerry.nodeTexture(room, entity)
    local hasNodes = entity.nodes and #entity.nodes > 0

    if hasNodes then
        return "collectables/strawberry/seed00"
    end
end

FlagFakeBerry.placements = {
    {
        name = "FlagFakeBerry",
		alternativeName = "FlagTrollBerry",
        data = {
            winged = false,
            moon = false,
			flag = "KoseiHelper_FlagFakeBerry",
			reappearMode = "Never",
			collectSfx = "event:/game/general/seed_poof",
			reappearSfx = "event:/game/general/seed_reappear",
			depthBeforeFollowing = -100,
			depthWhileFollowing = -1000000,
			collectTime = 0.15,
			resetsFlag = true
        }
    }
}

FlagFakeBerry.fieldOrder = {
	"x",
	"y",
	"flag",
	"reappearMode",
	"collectTime",
	"collectSfx",
	"reappearSfx",
	"depthBeforeFollowing",
	"depthWhileFollowing",
	"winged",
	"moon",
	"resetsFlag"
}

FlagFakeBerry.fieldInformation = {
	reappearMode = {
		options = {
			"Never",
			"Instantly",
			"OnRoomReload"
		},
		editable = false
	},
	depthBeforeFollowing = { fieldType = "integer" },
	depthWhileFollowing = { fieldType = "integer" }
}

return FlagFakeBerry