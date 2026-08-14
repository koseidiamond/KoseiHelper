local ClimbJumpController = {}

ClimbJumpController.name = "KoseiHelper/ClimbJumpController"
ClimbJumpController.depth = -9500
ClimbJumpController.texture = "objects/KoseiHelper/Controllers/ClimbJumpController"
ClimbJumpController.placements = {
	{
		name = "ClimbJumpController",
		data = {
		staminaSpent = 27.5,
		wallBoostTimer = 0.2,
		--wallBoostStaminaRefund = 27.5,
		spendStaminaOnGround = false,
		checkWallBoostDirection = true,
		dustType = "Normal",
		climbJumpSound = "event:/char/madeline/jump_climb_",
		horizontalSpeedMultiplier = 1,
		sweat = true,
		speedMultiplierCase = "AllCases"
		}
	}
}

ClimbJumpController.fieldInformation = {
	dustType = {
		options = {
			"Normal",
			"Sparkly",
			"Chimney",
			"Steam",
			"VentDust",
			"None"
		},
		editable = false
	},
	speedMultiplierCase = {
		options = {
			"AllCases",
			"OnlyNeutrals",
			"OnlyNonNeutrals"
		},
		editable = false
	}
}

return ClimbJumpController