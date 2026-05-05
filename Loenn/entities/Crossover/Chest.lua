local utils = require("utils")

local chest = {}

chest.name = "KoseiHelper/Chest"
chest.depth = 8999
chest.justification = {0.5, 0.5}
chest.placements = {
    {
        name = "Chest",
        data = {
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
	depth = { fieldType = "integer" },
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