local utils = require("utils")
local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local chest = {}

chest.name = "KoseiHelper/Chest"

function chest.depth(room,entity)
	return entity.depth
end

function chest.selection(room, entity)
    return utils.rectangle(entity.x - 8, entity.y - 8, 16, 16)
end

chest.justification = {0.5, 0.6}
chest.placements = {
    {
        name = "Chest",
        data = {
			depth = 8999,
            path = "objects/KoseiHelper/Crossover/Chest/normalchest_",
			flagSet = "normalchestopened",
			flagRequired = "",
			flagForExtra = "Midas",
			minCoins = 50,
			maxCoins = 60,
			currencyName = "coins",
			openedMessage = "This chest is empty.",
			lockedMessage = "This chest is locked!",
			openSound = "event:/CC/CC_KoseiDiamond_sounds/Chest",
			paymentSound = "event:/KoseiHelper/Crossover/Coin",
			counterName = "KoseiHelper_coins"
        }
    }
}

chest.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {}),
        editable = true
    },
	minCoins = {
		minimumValue = 0,
		fieldType = "integer"
	},
	maxCoins = {
		minimumValue = 0,
		fieldType = "integer"
	}
}

chest.fieldOrder = {
	"x",
	"y",
	"counterName",
	"path",
	"flagForExtra",
	"minCoins",
	"maxCoins",
	"flagRequired",
	"flagSet",
	"lockedMessage",
	"currencyName",
	"openedMessage",
	"openSound",
	"paymentSound"
}

function chest.texture(room, entity)
    return entity.path .. "closed"
end

return chest