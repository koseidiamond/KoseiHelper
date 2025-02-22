local SelfieTrigger = {}

SelfieTrigger.name = "KoseiHelper/SelfieTrigger"
SelfieTrigger.depth = 100

SelfieTrigger.placements = {
	{
		name = "SelfieTrigger",
		data = {
		image = "selfieCampfire",
		buttonTexture = "textboxbutton",
		photoInSound = "event:/game/02_old_site/theoselfie_photo_in",
		photoOutSound = "event:/game/02_old_site/theoselfie_photo_out",
		inputSound = "event:/ui/main/button_lowkey",
		flash = true,
		timeToOpen = 0.5,
		flag = "",
		triggerMode = "OnEnter",
		oneUse = true
		}
	}
}

SelfieTrigger.fieldInformation = function (entity) return {
	timeToOpen = { minimumValue = 0.00001 },
		triggerMode = {
		options = {
		"OnEnter",
		"OnLeave"
		},
		editable = false
	}
}
end

return SelfieTrigger
