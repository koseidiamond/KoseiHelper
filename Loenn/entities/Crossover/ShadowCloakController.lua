local ShadowCloakController = {}

ShadowCloakController.name = "KoseiHelper/ShadowCloakController"
ShadowCloakController.texture = "objects/KoseiHelper/Crossover/Controllers/ShadowCloakController"
ShadowCloakController.placements = {
    name = "ShadowCloakController",
    data = {
        flagRequired = "",
		cooldown = 1.5,
		recoverShadowDashSfx = "event:/none",
		useShadowDashSfx = "event:/KoseiHelper/Crossover/ShadowCloak"
    }
}

return ShadowCloakController