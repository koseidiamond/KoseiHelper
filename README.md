# KoseiHelper
Celeste mod with several random mechanics.
### Customization
All entities are customizable through Sprites.xml. Feel free to change the animations as you need. You can copy the sprite tags and paste them into your own Sprites.xml, just make sure your path is unique.

# Entities
### Goomba
This entity follows the player horizontally, killing on contact, and can optionally be bounced on. It has several customizable options, including spawning mini versions each x seconds, flying away like strawberries, or be winged. They can also interact with springs.
### Mary Block
Single-use flowers that launch you with more or less strength.
### Tile Refill
Can enable or disable the Collidable and Visible properties of foreground tiles.
### Bound Refill
Outbound refills allow the player to escape the current screen. Inbound refills bound the player to the screen they are overlapping.
### IRL Controller
Sets flags depending on the time of the day, day and month. For example, if it's 17:00, November 18th, the flags `kosei_irlHour17`, `kosei_irlDay18` and `kosei_irlMonth11` will be set. It also sets the flags `kosei_irlMorning`, `kosei_irlAfternoon` and `kosei_irlNight`. It also has an option to adapt the darkness of the room to the current hour.
### Custom Pause Controller
Allows customization of the Pause Menu, disabling Retry, Save&Quit, disable pausing, killing player after pausing, or preventing the timer from running on rooms with the controller.
### Balls
**Puffer balls** and **Spring balls** are a variation of Snowballs. They are customizable, supporting all 4 directions, sine variations, and multiple of them per screen.
Additionally, spring balls have some visual options and can track Theo instead of Madeline.
Note: Unlike most controller entities, the position of this entity determines where they will first appear on the chosen axis.

# Triggers
### Force Throw Trigger
It forces the player to immediately throw the holdable, even before the grabbing animation finishes.
