local KillIfNotGroundedTrigger = {}

KillIfNotGroundedTrigger.name = "KoseiHelper/KillIfNotGroundedTrigger"
KillIfNotGroundedTrigger.depth = 100

KillIfNotGroundedTrigger.placements = {
	{
		name = "KillIfNotGroundedTrigger",
		data = {
			spareIfClimbing = false,
			killIfGrounded = false,
			delay = 0.01
		}
	}
}

KillIfNotGroundedTrigger.fieldInformation = {
	delay = {
		minimumValue = 0
	}
}

KillIfNotGroundedTrigger.triggerText = "Kill If (Not) Grounded"

return KillIfNotGroundedTrigger