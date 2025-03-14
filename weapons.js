let weapons = [
    ["Boomerang",
	false,
	"objects/KoseiHelper/Guns/Bullets/TerrariaBoomerang",
	true,
	"FFFFFF",
	"FFFFFF",
	64,
	"particles/KoseiHelper/star",
	"None",
	"Horizontal",
	0,
	"objects/KoseiHelper/Guns/Bullets/Invisible",
	"event:/KoseiHelper/Guns/shotBoomerang",
	-0.05,
	2000,
	0,
	"None",
	16,
	120,
	0.4,
	0],
	["Crossbow",
	false,
	"objects/KoseiHelper/Guns/TowerFallCrossbow",
	true,
	"e0c9e7",
	"a79aad",
	30,
	"particles/KoseiHelper/star",
	"None",
	"FourDirections",
	0,
	"objects/KoseiHelper/Guns/Bullets/TowerFallArrow",
	"event:/KoseiHelper/Guns/shotArrow",
	-0.01,
	900,
	0.1,
	"VentDust",
	16,
	0,
	0.5,
	0.008],
	["Nemesis",
	true,
	"objects/KoseiHelper/Guns/Bullets/Nemesis",
	true,
	"4682b4",
	"ffff00",
	12,
	"particles/KoseiHelper/star",
	"ReplacesDash",
	"Horizontal",
	0,
	"objects/KoseiHelper/Guns/NemesisGun",
	"event:/KoseiHelper/Guns/shotDefault",
	0,
	350,
	0.25,
	"Normal",
	0,
	0,
	0.7,
	0],
	["Meowgun",
	false,
	"objects/KoseiHelper/Guns/Bullets/TerrariaMeow",
	true,
	"e716a0",
	"4312e6",
	9,
	"particles/KoseiHelper/star",
	"None",
	"EightDirections",
	0,
	"objects/KoseiHelper/Guns/Meowgun",
	"event:/KoseiHelper/Guns/shotMeow",
	0,
	650,
	1,
	"Ice",
	0,
	0,
	0.9,
	0],
	["Firework",
	true,
	"objects/KoseiHelper/Guns/Bullets/Invisible",
	true,
	"eb6e14",
	"e7b915",
	12,
	"particles/KoseiHelper/star",
	"None",
	"FourDirections",
	3,
	"objects/KoseiHelper/Guns/Firework",
	"event:/KoseiHelper/Guns/shotFirework",
	0.05,
	1800,
	1,
	"Fire",
	20,
	200,
	0.1,
	-0.015],
	["Löve",
	true,
	"objects/KoseiHelper/Guns/Bullets/Heart",
	true,
	"e95cec",
	"cd3784",
	8,
	"particles/KoseiHelper/star",
	"ConsumesDash",
	"EightDirections",
	2,
	"objects/KoseiHelper/Guns/Bullets/Heart",
	"event:/KoseiHelper/bulletB",
	-0.01,
	3600,
	0.35,
	"Custom",
	12,
	80,
	0.22,
	0.007],
];

function getEntity(name, bulletExplosion, bulletTexture, canShootInFeather, color1, color2, cooldown, customParticleTexture, dashBehavior, directions, freezeFrames,
gunTexture, gunshotSound, horizontalAcceleration, lifetime, particleAlpha, particleType, recoilCooldown, recoilStrength, speedMultiplier, verticalAcceleration) {
    return `{
    {
        _fromLayer = "triggers",
        _name = "KoseiHelper/NemesisGunSettings",
        bulletExplosion = ${bulletExplosion},
        bulletTexture = "${bulletTexture}",
        canShootInFeather = ${canShootInFeather},
        color1 = "${color1}",
        color2 = "${color2}",
        cooldown = "${cooldown}",
        customParticleTexture = "${customParticleTexture}",
        dashBehavior = "${dashBehavior}",
        directions = "${directions}",
        enabled = true,
        freezeFrames = "${freezeFrames}",
        gunTexture = "${gunTexture}",
        gunshotSound = "${gunshotSound}",
        height = 64,
        horizontalAcceleration = "${horizontalAcceleration}",
        lifetime = "${lifetime}",
        loseGunOnRespawn = false,
        particleAlpha = "${particleAlpha}",
        particleType = "${particleType}",
        recoilCooldown = "${recoilCooldown}",
        recoilStrength = "${recoilStrength}",
        speedMultiplier = "${speedMultiplier}",
        triggerMode = "OnStay",
        verticalAcceleration = "${verticalAcceleration}",
        width = 64,
        x = 0,
        y = 0
    }
}`;
}



function buttonClick(button) {
    let index = button.id.substring(7); // 7 is the length of "button_"
    let info = weapons[index]; // Get weapon data based on the index

    // Pass the whole weapon data array to the getEntity function
    navigator.clipboard.writeText(getEntity(...info)); // Spread the weapon array to pass all attributes

    // Change button text and style to indicate success
    button.innerHTML = "Copied!";
    button.style.backgroundColor = "#e1bde0";
    setDefaultColor(button);
}

async function setDefaultColor(button) {
    await sleep(1000);
    button.innerHTML = "Copy";
    button.style.backgroundColor = null;
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

for (let i = 0; i < weapons.length; i++) {
    let info = weapons[i];

    // Create a container div for the weapon's info
    let e = document.createElement("DIV");
    
    // Create and append a line break
    let br = document.createElement("BR");

    // Create and append the weapon's title (name)
    let title = document.createElement("H2");
    title.innerHTML = info[0];

    // Create and append the weapon's image
    let image = document.createElement("IMG");
    image.setAttribute("src", "./Images/weapons/" + info[0] + ".png");

    // Create and append the "Copy" button
    let button = document.createElement("BUTTON");
    button.innerHTML = "Copy";
    button.id = "button_" + i;
    button.setAttribute("onclick", "buttonClick(this)");

    // Append all elements to the container div
    e.append(title);
    e.append(image);
    e.append(br);
    e.append(button);

    // Append the container div to the document body
    document.body.append(e);
}
