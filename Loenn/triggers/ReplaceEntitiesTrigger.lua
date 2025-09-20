local ReplaceEntitiesTrigger = {}

ReplaceEntitiesTrigger.name = "KoseiHelper/ReplaceEntitiesTrigger"
ReplaceEntitiesTrigger.placements = {
    name = "ReplaceEntitiesTrigger",
    data = {
		onlyOnce = false,
		fromEntity = "Celeste.CrystalStaticSpinner",
		toEntity = "Celeste.Booster",
		attributes = "red",
		attributeValues = "true"
    }
}

return ReplaceEntitiesTrigger