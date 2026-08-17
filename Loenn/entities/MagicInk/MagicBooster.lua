local MagicBooster = {}

MagicBooster.name = "KoseiHelper/MagicBooster"
MagicBooster.depth = -8500
MagicBooster.placements = {
    {
        name = "MagicBooster",
        data = {
            --red = true,
            --ch9_hub_booster = false
        }
    }
}

function MagicBooster.texture(room, entity)
    local red = entity.red or true

    if red then
        return "objects/KoseiHelper/MagicBooster/grayBoosterRed/loennBooster"
    else
        return "objects/KoseiHelper/MagicBooster/grayBooster/loennBooster"
    end
end

return MagicBooster