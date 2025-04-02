local ReverseDepthsTrigger = {}

ReverseDepthsTrigger.name = "KoseiHelper/ReverseDepthsTrigger"
ReverseDepthsTrigger.depth = 100

ReverseDepthsTrigger.placements = {
	{
		name = "ReverseDepthsTrigger",
		data = {
		onlyOnce = false,
		onLeave = false,
		reverseStylegrounds = false,
		reverseTiles = true,
		reverseEntities = false,
		flag = ""
		}
	}
}

return ReverseDepthsTrigger