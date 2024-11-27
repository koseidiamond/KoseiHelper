local CustomPlayerSeeker = {}

CustomPlayerSeeker.name = "KoseiHelper/CustomPlayerSeeker"
CustomPlayerSeeker.depth = -100

CustomPlayerSeeker.placements = {
	{
		name = "Custom Player Seeker",
		data = {
			nextRoom = "c-00",
			colorgrade = "templevoid",
			sprite = "seeker",
			seekerDashSound = "event:/game/05_mirror_temple/seeker_dash",
			vanillaEffects = false,
			canSwitchCharacters = false,
			speedMultiplier = 1,
			isFriendly = false
		}
	}
}

CustomPlayerSeeker.texture = "decals/5-temple/statue_e"

return CustomPlayerSeeker