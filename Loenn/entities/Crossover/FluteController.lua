local FluteController = {}

FluteController.name = "KoseiHelper/FluteController"
FluteController.texture = "objects/KoseiHelper/Crossover/FluteNotes/NoteController"
FluteController.depth= -10500
FluteController.placements = {
    name = "FluteController",
    data = {
	swapMusicLayers = true,
	sound = "event:/KoseiHelper/Crossover/YumeFlute",
	flagName = "KoseiHelper_isDreaming",
	notePath = "objects/KoseiHelper/Crossover/FluteNotes/note_"
    }
}

return FluteController