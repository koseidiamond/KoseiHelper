local easers = {}

function easers.getEasers()    
    local list = {
        "Linear", "SineIn", "SineOut", "SineInOut", "QuadIn", "QuadOut", "QuadInOut",
        "CubeIn", "CubeOut", "CubeInOut", "QuintIn", "QuintOut", "QuintInOut",
        "ExpoIn", "ExpoOut", "ExpoInOut", "BackIn", "BackOut", "BackInOut",
        "BigBackIn", "BigBackOut", "BigBackInOut", "ElasticIn", "ElasticOut",
        "ElasticInOut", "BounceIn", "BounceOut", "BounceInOut", "HexIn", "HexOut",
        "HexInOut", "SeptIn", "SeptOut", "SeptInOut", "OctIn", "OctOut", "OctInOut",
        "NonIn", "NonOut", "NonInOut", "DecIn", "DecOut", "DecInOut", "Round"
    }
    return list
end

function easers.addEasers(list, toAdd)
    for i,p in ipairs(toAdd) do
        easers.addEaser(list, p[1])
    end
    return list
end

function easers.addEaser(list, name)
    local newList = {}
    for i, p in ipairs(list) do
        if name == p[1] then
            list[i][1] = name
            return list
        end

        if name < p[1] then
            table.insert(list, i, {name})
            return list
        end
    end
    table.insert(list, {name})
    return list
end

return easers