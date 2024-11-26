local CustomPlayerSeeker = {}

CustomPlayerSeeker.name = "KoseiHelper/CustomPlayerSeeker"
CustomPlayerSeeker.depth = -100

CustomPlayerSeeker.placements = {
	{
		name = "CustomPlayerSeeker",
		data = {
			nextRoom = "c-00",
			colorgrade = "templevoid",
			needsToBreakStatue = true,
			sprite = "seeker",
			seekerDashSound = "event:/game/05_mirror_temple/seeker_dash",
			vanillaEffects = false
		}
	}
}

CustomPlayerSeeker.texture = "decals/5-temple/statue_e"

return CustomPlayerSeeker
