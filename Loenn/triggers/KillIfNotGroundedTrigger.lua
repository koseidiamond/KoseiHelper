local KillIfNotGroundedTrigger = {}

KillIfNotGroundedTrigger.name = "KoseiHelper/KillIfNotGroundedTrigger"
KillIfNotGroundedTrigger.depth = 100

KillIfNotGroundedTrigger.placements = {
	{
		name = "KillIfNotGroundedTrigger",
		data = {
			spareIfClimbing = false,
			killIfGrounded = false
		}
	}
}

return KillIfNotGroundedTrigger