local mods = require("mods")
local depths = mods.requireFromPlugin("libraries.depths")

local MagicInkBox = {}

MagicInkBox.name = "KoseiHelper/MagicInkBox"
MagicInkBox.depth = -1000

function MagicInkBox.depth(room,entity)
	return entity.depth
end

--MagicInkBox.fillColor = { 26 / 255, 16 / 255, 56 / 255 }
--MagicInkBox.borderColor = { 17 / 255, 10 / 255, 37 / 255 }

local function hsvToRgb(h, s, v)
    local i = math.floor(h * 6)
    local f = h * 6 - i
    local p = v * (1 - s)
    local q = v * (1 - f * s)
    local t = v * (1 - (1 - f) * s)

    i = i % 6

    if i == 0 then
        return {v, t, p, 1}
    elseif i == 1 then
        return {q, v, p, 1}
    elseif i == 2 then
        return {p, v, t, 1}
    elseif i == 3 then
        return {p, q, v, 1}
    elseif i == 4 then
        return {t, p, v, 1}
    else
        return {v, p, q, 1}
    end
end

local function lerpColor(a, b, amount)
    return { a[1] + (b[1] - a[1]) * amount, a[2] + (b[2] - a[2]) * amount, a[3] + (b[3] - a[3]) * amount, 1 }
end

local function multiplyColor(color, amount)
    return { color[1] * amount, color[2] * amount, color[3] * amount, color[4] or 1 }
end

local function drawLine(x1, y1, x2, y2, color, width)
    love.graphics.setColor(color)
    love.graphics.setLineWidth(width)
    love.graphics.line(x1, y1, x2, y2)
end

function MagicInkBox.draw(room, entity, viewport) -- do not do this at home
    local x = entity.x
    local y = entity.y
    local width = entity.width or 16
    local height = entity.height or 16
    local hue = (love.timer.getTime() * 0.25) % 1
    local border = hsvToRgb(hue, 1, 1)
    local dark = {26 / 255, 16 / 255, 56 / 255, 1}
    local fill = lerpColor(dark, border, 0.2)

    love.graphics.setColor(fill)
    love.graphics.rectangle("fill", x, y, width, height)

    local canBreak = entity.canBreak
    local health = entity.health or 1

    if canBreak
        and health > 1
        and math.max(width, height) / math.min(width, height) <= 1.99
    then
        local radius = (width + height) / 6
        local points = {}

        for i = 0, health - 1 do
            local angle = math.pi * 2 * i / health - math.pi / 2
            points[i + 1] = { x + width / 2 + math.cos(angle) * radius, y + height / 2 + 1 + math.sin(angle) * radius }
        end
		
        local glyphColor = lerpColor(border, fill, 0.85)
        for i = 1, health do
            for j = i + 1, health do
                drawLine(points[i][1], points[i][2], points[j][1], points[j][2], glyphColor, 1)
            end
        end

        local activeColor = multiplyColor(border, 0.4)
        for i = 1, health do
            for j = i + 1, health do
                drawLine(points[i][1], points[i][2], points[j][1], points[j][2], activeColor, 1.5)
            end
        end
    end

    local px = x + 2
    local py = y + 2
    local w = width - 4
    local h = height - 4

    drawLine(px - 1, py - 1, px + w + 1, py - 1, border, 2)
    drawLine(px - 1, py + h + 1, px + w + 1, py + h + 1, border, 2)
    drawLine(px - 1, py - 1, px - 1, py + h + 1, border, 2)
    drawLine(px + 1 + w, py - 1, px + w + 1, py + h + 1, border, 2)

    if not canBreak then
        local xColor = lerpColor(border, dark, 0.4)
        drawLine(px - 1.5, py - 1.5, px + w + h + 1.5 - h, py + h + 1.5, xColor, 2)
        drawLine(px + w + 1.5, py - 1.5, px - 1.5, py + h + 1.5, xColor, 2)
    end

    love.graphics.setColor(0, 0, 0, 1)
    love.graphics.setLineWidth(1)
    love.graphics.rectangle("line", x, y, width, height)

    love.graphics.setColor(1, 1, 1, 1)
    love.graphics.setLineWidth(1)
end

MagicInkBox.placements = {
	{
		name = "MagicInkBox",
		data = {
		width = 16,
		height = 16,
			depth = -1000,
			pushSpeed = 200,
			canBreak = false,
			health = 1,
			bumpSfx = "event:/game/03_resort/forcefield_bump",
			breakSfx = "event:/KoseiHelper/magicShatter",
			noInkConsumption = false
		}
	}
}

MagicInkBox.fieldInformation = {
	depth = {
        fieldType = "integer",
        options = depths.addDepths(depths.getDepths(), {
		{"MagicInkBox", -1000}
		}),
        editable = true
    },
	health = {
		fieldType = "integer",
		minimumValue = 1
	}
}

function MagicInkBox.ignoredFields(entity)
	local ignored = {
	"_name",
    "_id",
	"health"
	}
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end
	if entity.canBreak == true then
		doNotIgnore("health")
	end
	return ignored
end

return MagicInkBox
