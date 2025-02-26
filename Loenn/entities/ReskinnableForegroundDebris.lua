local ReskinnableParallaxDebris = {}

ReskinnableParallaxDebris.name = "KoseiHelper/ReskinnableParallaxDebris"
ReskinnableParallaxDebris.depth = -9500

ReskinnableParallaxDebris.placements = {
	{
		name = "ReskinnableParallaxDebris",
		data = {
			depth = -999900,
			texture = "scenery/fgdebris/KoseiHelper/rockA",
			sine = 2,
			tint = "42605d",
			customParallax = 0.05,
			direction = "Vertical"
		}
	}
}

ReskinnableParallaxDebris.fieldInformation = {
	depth = {
		fieldType = "integer"
	},
	sine = {
		minimumValue = 0
	},
	direction = {
		options = {
			"Vertical",
			"Horizontal",
			"DiagonalUL_DR",
			"DiagonalUR_DL",
			"None"
		},
		editable = false
	},
	tint = {
		fieldType = "color"
	}
}

function ReskinnableParallaxDebris.texture(room, entity)
        return "scenery/fgdebris/KoseiHelper/rock00"
end

return ReskinnableParallaxDebris
