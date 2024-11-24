local FlagsFromFileTrigger = {}

FlagsFromFileTrigger.name = "KoseiHelper/FlagsFromFileTrigger"
FlagsFromFileTrigger.depth = 100

FlagsFromFileTrigger.placements =
{
	{
		name = "Flags From File Trigger",
		data =
		{
			filePath = "Assets/KoseiHelper/flags",
			writeToFile = false,
			flagsToWrite = "a,b,c"
		}
	}
}


return FlagsFromFileTrigger