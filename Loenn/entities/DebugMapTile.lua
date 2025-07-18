local utils = require("utils")

local DebugMapTile = {}
DebugMapTile.name = "KoseiHelper/DebugMapTile"
DebugMapTile.depth = 1
DebugMapTile.placements = {
    name = "DebugMapTile",
    data = {
        width = 8,
        height = 8,
        color = "6969ee"
    }
}
DebugMapTile.fieldInformation = {
    color = {
        fieldType = "color"
    }
}

function DebugMapTile.color(room, entity)
    local color = {0.411, 0.411, 0.933}
    if entity.color then
        local success, r, g, b = utils.parseHexColor(entity.color)
        if success then
            color = {r, g, b}
        end
    end
    return color
end

return DebugMapTile