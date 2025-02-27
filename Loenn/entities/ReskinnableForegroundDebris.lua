local ReskinnableParallaxDebris = {}

ReskinnableParallaxDebris.name = "KoseiHelper/ReskinnableParallaxDebris"

function ReskinnableParallaxDebris.depth(room, entity)
        return entity.depth
end

ReskinnableParallaxDebris.placements = {
	{
		name = "ReskinnableParallaxDebris",
		data = {
			depth = -999900,
			texture = "scenery/fgdebris/KoseiHelper/rockA00",
			sine = 2,
			tint = "42605d",
			parallax = 0.05,
			direction = "Vertical",
			rotationSpeed = 0,
			scale = 1,
			fadeSpeed = 1,
			alphaMin = 1,
			alphaMax = 1,
			bounceHeight = 0,
			bounceWidth = 0,
			bounceSpeed = 1
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
        return entity.texture
end

return ReskinnableParallaxDebris