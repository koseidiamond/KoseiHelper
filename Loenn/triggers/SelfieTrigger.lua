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
		oneUse = true,
		interactable = false,
		talkBubbleX = 0,
		talkBubbleY = 0
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
	},
	talkBubbleX = { fieldType = "integer" },
	talkBubbleY = { fieldType = "integer" }
}
end

function SelfieTrigger.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	"talkBubbleX",
	"talkBubbleY"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end

	if entity.interactable == true then
		doNotIgnore("talkBubbleX")
		doNotIgnore("talkBubbleY")
	end
	return ignored
end

return SelfieTrigger
