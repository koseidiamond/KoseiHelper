local enums = require("consts.celeste_enums")

local CustomBirdTutorial = {}

CustomBirdTutorial.name = "KoseiHelper/CustomBirdTutorial"
CustomBirdTutorial.depth = -20000
CustomBirdTutorial.nodeLineRenderType = "line"
CustomBirdTutorial.justification = {0.5, 1.0}
CustomBirdTutorial.texture = "characters/bird/crow00"
CustomBirdTutorial.nodeLimits = {0, -1}
CustomBirdTutorial.fieldInformation = {
    info = {
        options = enums.everest_bird_tutorial_tutorials
    }
}
CustomBirdTutorial.placements = {
    name = "CustomBirdTutorial",
    data = {
        faceLeft = true,
        --birdId = "",
        onlyOnce = false,
        caw = true,
        info = "TUTORIAL_DREAMJUMP",
        controls = "DownRight,+,Dash,tinyarrow,Jump",
		bgColor = "061526",
		lineColor = "ffffffff",
		titleColor = "ffffffff",
		textColor = "ffffffff",
		secondaryTextColor = "6179e2ff",
		directionColor = "ffffffff",
		buttonColor = "ffffffff",
		imageColor = "ffffffff",
		buttonPadding = 8,
		sizeMultiplier = 1,
		renderTriangleBelow = true,
		rectangleShape = true,
		noBird = false,
		flag = "",
		verticalOffset = -16
    }
}

function CustomBirdTutorial.scale(room, entity)
    return entity.faceLeft and -1 or 1, 1
end

CustomBirdTutorial.fieldOrder = {
	"x",
	"y",
	"info",
	"controls",
	"lineColor",
	"bgColor",
	"titleColor",
	"textColor",
	"secondaryTextColor",
	"directionColor",
	"buttonColor",
	"imageColor",
	"buttonPadding",
	"sizeMultiplier",
	--"birdId",
	"verticalOffset",
	"flag",
	"faceLeft",
	"caw",
	"rectangleShape",
	"renderTriangleBelow",
	"noBird",
	"onlyOnce"
}

CustomBirdTutorial.fieldInformation = {
	bgColor = {
        fieldType = "color",
		useAlpha = true
    },
	lineColor = {
        fieldType = "color",
		useAlpha = true
    },
	titleColor = {
        fieldType = "color",
		useAlpha = true
    },
	textColor = {
        fieldType = "color",
		useAlpha = true
    },
	secondaryTextColor = {
        fieldType = "color",
		useAlpha = true
    },
	directionColor = {
        fieldType = "color",
		useAlpha = true
    },
	buttonColor = {
        fieldType = "color",
		useAlpha = true
    },
	imageColor = {
        fieldType = "color",
		useAlpha = true
    },
	buttonPadding = {
		minimumValue = 1
	},
	sizeMultiplier = {
		minimumValue = 0.001
	}
}

return CustomBirdTutorial