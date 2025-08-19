local KillIfNotGroundedTrigger = {}

KillIfNotGroundedTrigger.name = "KoseiHelper/KillIfNotGroundedTrigger"
KillIfNotGroundedTrigger.depth = 100

KillIfNotGroundedTrigger.placements = {
	{
		name = "KillIfNotGroundedTrigger",
		data = {
			spareIfClimbing = false,
			killIfGrounded = false,
			delay = 0.01,
			group = 0
		}
	}
}

KillIfNotGroundedTrigger.fieldInformation = {
	delay = {
		minimumValue = 0
	},
	group = {
		fieldType = "integer"
	}
}

KillIfNotGroundedTrigger.triggerText = "Kill If (Not) Grounded"

return KillIfNotGroundedTrigger