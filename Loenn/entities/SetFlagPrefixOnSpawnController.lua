local controller = {}

controller.name = "KoseiHelper/SetFlagPrefixOnSpawnController"
controller.depth = -11000
controller.texture = "objects/KoseiHelper/Controllers/SetFlagPrefixOnSpawnController"
controller.placements = {
    name = "SetFlagPrefixOnSpawnController",
    data = {
        flag = "flag_prefix",
        enable = false,
        onlyOnRespawn = false,
        ifFlag = ""
    }
}

return controller