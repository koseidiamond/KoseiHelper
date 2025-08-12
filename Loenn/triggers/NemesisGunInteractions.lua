local NemesisGunInteractions = {}

NemesisGunInteractions.name = "KoseiHelper/NemesisGunInteractions"
NemesisGunInteractions.depth = -10500
NemesisGunInteractions.placements = {
	{
		name = "NemesisGunInteractions",
		data = {
		triggerMode = "OnStay",
		canKillPlayer = false,
		dreamBlockBehavior = "GoThrough",
		breakBounceBlocks = true,
		activateFallingBlocks = true,
		harmEnemies = true,
		theoInteraction = "HitSpring",
		jellyfishInteraction = "HitSpring",
		breakSpinners = true,
		spinnerFix = false,
		breakMovingBlades = true,
		dashBlockBehavior = "BreakAll",
		collectables = true,
		useRefills = true,
		pufferInteraction = "Explode",
		pressDashSwitches = true,
		canBounce = true,
		useBoosters = false,
		waterFriction = 0.995,
		scareBirds = true,
		collectTouchSwitches = true,
		collectBadelineOrbs = true,
		useFeathers = true,
		coreModeToggles = true,
		moveSwapBlocks = true,
		collideWithPlatforms = true,
		moveMovingBlocks = true,
		dashOnKevins = true
		}
	}
}

NemesisGunInteractions.fieldInformation = 
{
	triggerMode =
	{
		options =
		{
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	theoInteraction =
	{
		options =
		{
		"Kill",
		"None",
		"HitSpinner",
		"HitSpring"
		},
		editable = false
	},
	jellyfishInteraction =
	{
		options =
		{
		"Kill",
		"None",
		"HitSpring",
		"Throw"
		},
		editable = false
	},
	pufferInteraction =
	{
		options =
		{
		"Explode",
		"HitSpring",
		"None"
		},
		editable = false
	},
	dreamBlockBehavior =
	{
		options =
		{
		"GoThrough",
		"Destroy",
		"None"
		},
		editable = false
	},
	dashBlockBehavior =
	{
		options =
		{
		"BreakAll",
		"BreakSome",
		"NoBreak"
		},
		editable = false
	}
}

return NemesisGunInteractions