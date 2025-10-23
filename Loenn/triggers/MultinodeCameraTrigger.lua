local MultinodeCameraTrigger = {}

MultinodeCameraTrigger.name = "KoseiHelper/MultinodeCameraTrigger"
MultinodeCameraTrigger.nodeLimits = {1, 9999}
MultinodeCameraTrigger.category = "camera"
MultinodeCameraTrigger.placements = {
    name = "MultinodeCameraTrigger",
    data = {
		onlyOnce = false,
		flag = "",
		lerpStrength = 1,
		xOnly = false,
		yOnly = false,
		smoothingStrength = 0.5,
		maxSmoothingDistance = 200
    }
}

MultinodeCameraTrigger.fieldInformation = {
	smoothingStrength = {
		minimumValue = 0.01
	},
	maxSmoothingDistance = {
		minimumValue = 8
	},
	lerpStrength = {
		minimumValue = 0
	}
}

return MultinodeCameraTrigger