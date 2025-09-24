--local fakeTilesHelper = require("helpers.fake_tiles")

local ProceduralGeneratorTrigger = {}

ProceduralGeneratorTrigger.name = "KoseiHelper/ProceduralGeneratorTrigger"
ProceduralGeneratorTrigger.placements = {
    name = "ProceduralGeneratorTrigger",
    data = {
		tileChars = "13456789abcdefgGhijklmn",
		density = 0.5,
		noiseScale = 0.01,
		noiseThreshold = 0.3,
		octaves = 4,
		persistence = 0.5,
		lacunarity = 2.0,
		sizeX = 100,
		sizeY = 100
    }
}
return ProceduralGeneratorTrigger