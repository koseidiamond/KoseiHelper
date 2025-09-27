local ReverseDepthsTrigger = {}

ReverseDepthsTrigger.name = "KoseiHelper/ReverseDepthsTrigger"
ReverseDepthsTrigger.depth = 100
ReverseDepthsTrigger.category = "visual"

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