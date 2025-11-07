local StoreSpeedTrigger = {}

StoreSpeedTrigger.name = "KoseiHelper/StoreSpeedTrigger"
StoreSpeedTrigger.depth = 100

StoreSpeedTrigger.placements = {
	{
		name = "StoreSpeedTrigger",
		data = {
			storesDirection = true,
			onlyOnce = false,
			addSpeed = false,
			storeSfx = "event:/none",
			factor = 1,
			flag = ""
		}
	}
}

return StoreSpeedTrigger