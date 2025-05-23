local StylegroundModifierTrigger = {}

StylegroundModifierTrigger.name = "KoseiHelper/StylegroundModifierTrigger"
StylegroundModifierTrigger.depth = 100

StylegroundModifierTrigger.placements = {
	{
		name = "StylegroundModifierTrigger",
		data = {
		onlyOnce = false,
		triggerMode = "OnEnter",
		
		identificationMode = "Index",
		index = 0,
		tag = "",
		texture = "",
		
		valueType = "DirectValue",
		linkedSlider = "",
		linkedCounter = "",
		linkedFlag = "",
		multiplier = 1,
		
		fieldToModify = "Color",
		flag = "",
		notFlag = "",
		color = "FFFFFF",
		positionX = 0,
		positionY = 0,
		scrollX = 0,
		scrollY = 0,
		speedX = 0,
		speedY = 0,
		alpha = 1,
		fadeIn = false,
		flipX = false,
		flipY = false,
		instantIn = false,
		instantOut = false,
		loopX = false,
		loopY = false,
		
		minValue = 0,
		maxValue = 1,
		minColor = "FFFFFF",
		maxColor = "000000",
		colorWhileTrue = "FFFFFF",
		colorWhileFalse = "000000",
		valueWhileTrue = 1,
		valueWhileFalse = 0
		}
	}
}

StylegroundModifierTrigger.fieldInformation = function (entity) return {
	triggerMode = {
		options = {
		"OnEnter",
		"OnLeave",
		"OnStay"
		},
		editable = false
	},
	identificationMode = {
		options = {
		"Index",
		"Tag",
		"Texture"
		},
		editable = false
	},
	valueType = {
		options = {
		"DirectValue",
		"Slider",
		"Counter",
		"Flag"
		},
		editable = false
	},
	fieldToModify = {
		options = {
		"Flag",
		"NotFlag",
		"Color",
		"PositionX",
		"PositionY",
		"ScrollX",
		"ScrollY",
		"SpeedX",
		"SpeedY",
		"Alpha",
		"FadeIn",
		"FlipX",
		"FlipY",
		"InstantIn",
		"InstantOut",
		"LoopX",
		"LoopY"
		},
		editable = false
	},
	index = {
		fieldType = "integer",
		minimumValue = 0
	},
	alpha = {
		fieldType = "number",
		minimumValue = 0
	},
	color = {
		fieldType = "color"
	},
	minColor = {
		fieldType = "color"
	},
	maxColor = {
		fieldType = "color"
	},
	colorWhileTrue = {
		fieldType = "color"
	},
	colorWhileFalse = {
		fieldType = "color"
	}
}
end

StylegroundModifierTrigger.fieldOrder= {
	"x",
	"y",
	"width",
	"height",
	"identificationMode",
	"index",
	"tag",
	"texture",
	"fieldToModify",
	"triggerMode",
	"valueType",
	"linkedSlider",
	"linkedCounter",
	"linkedFlag",
	"multiplier"
}

function StylegroundModifierTrigger.ignoredFields(entity)
	local ignored = {
	"_name",
	"_id",
	
	"linkedSlider",
	"linkedCounter",
	"linkedFlag",
	"multiplier",
	
	"index",
	"tag",
	"texture",
	
	"flag",
	"notFlag",
	"color",
	"positionX",
	"positionY",
	"scrollX",
	"scrollY",
	"speedX",
	"speedY",
	"alpha",
	"fadeIn",
	"flipX",
	"flipY",
	"instantIn",
	"instantOut",
	"loopX",
	"loopY",
	
	"minValue",
	"maxValue",
	"minColor",
	"maxColor",
	"colorWhileTrue",
	"colorWhileFalse",
	"valueWhileTrue",
	"valueWhileFalse"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end

	if entity.identificationMode == "Index" then
		doNotIgnore("index")
	end
	if entity.identificationMode == "Tag" then
		doNotIgnore("tag")
	end
	if entity.identificationMode == "Texture" then
		doNotIgnore("texture")
	end
		
	if entity.valueType == "DirectValue" then
		if entity.fieldToModify == "Flag" then
			doNotIgnore("flag")
		end
		if entity.fieldToModify == "NotFlag" then
			doNotIgnore("notFlag")
		end
		if entity.fieldToModify == "Color" then
			doNotIgnore("color")
		end
		if entity.fieldToModify == "PositionX" then
			doNotIgnore("positionX")
		end
		if entity.fieldToModify == "PositionY" then
			doNotIgnore("positionY")
		end
		if entity.fieldToModify == "ScrollX" then
			doNotIgnore("scrollX")
		end
		if entity.fieldToModify == "ScrollY" then
			doNotIgnore("scrollY")
		end
		if entity.fieldToModify == "SpeedX" then
			doNotIgnore("speedX")
		end
		if entity.fieldToModify == "SpeedY" then
			doNotIgnore("speedY")
		end
		if entity.fieldToModify == "Alpha" then
			doNotIgnore("alpha")
		end
		if entity.fieldToModify == "FadeIn" then
			doNotIgnore("fadeIn")
		end
		if entity.fieldToModify == "FlipX" then
			doNotIgnore("flipX")
		end
		if entity.fieldToModify == "FlipY" then
			doNotIgnore("flipY")
		end
		if entity.fieldToModify == "InstantIn" then
			doNotIgnore("instantIn")
		end
		if entity.fieldToModify == "InstantOut" then
			doNotIgnore("instantOut")
		end
		if entity.fieldToModify == "LoopX" then
			doNotIgnore("loopX")
		end
		if entity.fieldToModify == "LoopY" then
			doNotIgnore("loopY")
		end
	end
	if entity.valueType == "Slider" then
		doNotIgnore("linkedSlider")
		-- Float from float
		if entity.fieldToModify == "PositionX" or entity.fieldToModify == "PositionY" or entity.fieldToModify == "ScrollX" or entity.fieldToModify == "ScrollY" or entity.fieldToModify == "SpeedX" or entity.fieldToModify == "SpeedY" or entity.fieldToModify == "Alpha" then
			doNotIgnore("multiplier")
		end
		-- Color from float
		if entity.fieldToModify == "Color" then
			doNotIgnore("minValue")
			doNotIgnore("maxValue")
			doNotIgnore("minColor")
			doNotIgnore("maxColor")
		end
		-- Bool from float
		if entity.fieldToModify == "FadeIn" or entity.fieldToModify == "FlipX" or entity.fieldToModify == "FlipY" or entity.fieldToModify == "InstantIn" or entity.fieldToModify == "InstantOut" or entity.fieldToModify == "LoopX" or entity.fieldToModify == "LoopY" then
			doNotIgnore("minValue") -- min value for the bool to change to false
			doNotIgnore("maxValue") -- max value for the bool to change to true
		end
	end
	if entity.valueType == "Counter" then
		doNotIgnore("linkedCounter")
		-- Float from int
		if entity.fieldToModify == "PositionX" or entity.fieldToModify == "PositionY" or entity.fieldToModify == "ScrollX" or entity.fieldToModify == "ScrollY" or entity.fieldToModify == "SpeedX" or entity.fieldToModify == "SpeedY" or entity.fieldToModify == "Alpha" then
			doNotIgnore("multiplier")
		end
		-- Color from int
		if entity.fieldToModify == "Color" then
			doNotIgnore("minValue")
			doNotIgnore("maxValue")
			doNotIgnore("minColor")
			doNotIgnore("maxColor")
		end
		-- Boolean from int
		if entity.fieldToModify == "FadeIn" or entity.fieldToModify == "FlipX" or entity.fieldToModify == "FlipY" or entity.fieldToModify == "InstantIn" or entity.fieldToModify == "InstantOut" or entity.fieldToModify == "LoopX" or entity.fieldToModify == "LoopY" then
			doNotIgnore("minValue") -- min value for the bool to change to false
			doNotIgnore("maxValue") -- max value for the bool to change to true
		end
	end
	if entity.valueType == "Flag" then
		doNotIgnore("linkedFlag")
		-- Float from bool
		if entity.fieldToModify == "PositionX" or entity.fieldToModify == "PositionY" or entity.fieldToModify == "ScrollX" or entity.fieldToModify == "ScrollY" or entity.fieldToModify == "SpeedX" or entity.fieldToModify == "SpeedY" or entity.fieldToModify == "Alpha" then
			doNotIgnore("valueWhileTrue")
			doNotIgnore("valueWhileFalse")
		end
		-- Color from bool
		if entity.fieldToModify == "Color" then
			doNotIgnore("colorWhileTrue")
			doNotIgnore("colorWhileFalse")
		end
	end
	return ignored
end

return StylegroundModifierTrigger