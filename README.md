# KoseiHelper
Celeste mod with several random mechanics.

# Table of contents
* [Entities](#entities)
  * [Miscellaneous](#miscellaneous)
  * [Blocks](#blocks)
  * [Refills](#refills)
  * [Controllers](#controllers)
* [Triggers](#triggers)

# Entities
All entities are customizable through Sprites.xml. Feel free to change the animations as you need. You can copy the sprite tags and paste them into your own Sprites.xml. Make sure your path is unique.

## Miscellaneous

### Goomba
This entity follows the player horizontally, killing on contact, and can optionally be bounced on. It has several customizable options, including spawning mini versions each x seconds, flying away like strawberries, or be winged. They can also interact with springs.
### Plant
This entity can move in a cycle, move when the player gets near, or jump when the player is above, and works in all 4 directions. Optionally, it can also shoot. The bullets can melt Defrostable Blocks.
The small plants (Black and Jumping) respond like normal Actors (which means they can be pushed by blocks or fall down).
### Mary Block
Single-use flowers that launch you with more or less strength depending on the mode.
### Winged Key
They function as normal keys but they can fly away like winged strawberries at a customizable speed.
### Falling Platform/Donut
Jumpthroughs (platforms) that fall/move in the chosen direction at a custom speed, after a certain time standing on them. Two different visual variants of the same entity.
### Balls
**Puffer balls** and **Spring balls** are a combination of Snowballs and Puffers/Springs. They are customizable, supporting all 4 directions, sine variations, flags, offsets, and multiple of them per screen.
Additionally, spring balls have some visual options and can track Theo instead.
*Note: Unlike controller entities, the position of these entities (or, well, "semi-controllers") determines where they will first appear on the chosen axis.*

## Blocks

### Bricks
Blocks of fixed size that can be broken from below (while not crouched or in a feather).
They have an ice variant (with the Defrostable Block functionality combined) and a fortress variant (only breakable with red boosters/badeline launches.
Goombas on top will be killed if the block is hit (regardless of whether it is destroyed) and Theos on top will be bumped.
### Defrostable Block
Ice blocks that melt when they come in contact with a bullet or if the Core Mode is hot.
### Oshiro Doors
Oshiro/ghost doors that are slightly bouncy but have more options (mainly visual/sfx and the option to always refill a dash, being one use, or bounce slightly differently). They come in two flavors: sprite or tileset.
### Custom Temple Cracked Block
The red blocks in chapter 5 that can be broken by seekers, but allowing custom visuals (debris, texture and tint), sounds, and health (requiring x number of hits to be destroyed).

## Refills

### Tile Refill
Can enable or disable the Collidable and Visible properties of foreground tiles.
### Bound Refill
Outbound refills allow the player to escape the current screen. Inbound refills bound the player to the screen they are overlapping.
### Puffer Refill
A refill that grants a puffer explosion after the player dashes.

# Controllers

### IRL Controller
Sets flags depending on the time of the day, day and month. For example, if it's 17:00, November 18th, the flags `kosei_irlHour17`, `kosei_irlDay18` and `kosei_irlMonth11` will be set. It also sets the flags `kosei_irlMorning`, `kosei_irlAfternoon` and `kosei_irlNight`. It also has an option to adapt the darkness of the room to the current hour.
### Custom Pause Controller
Allows customization of the Pause Menu, disabling Retry, Save&Quit, disable pausing, killing player after pausing, or preventing the timer from running on rooms with the controller.
### Autoscroller Controller
This controller moves the camera in the specified direction as soon as the player starts moving. The player can be killed or pushed (safely or not) if they get behind the camera.
If the speed is changed, the parts of the screen where the player is forced to stay can have an offset due to the camera movement, so it's recommended to adjust the offsets or the Camera Catchup Speed.
Multiple autoscrollers on the same room, inverted gravity or zoom out are not supported.
### Sleepy Controller
A silly controller that makes the player to take short naps every certain intervals of time (random or fixed). While sleeping, the player will keep travelling in the same direction/speed they were moving before the nap.
### Entity Spawners
The **Entity Spawner Controllers** allow you to spawn different types of entities on a certain position when a condition is met. They are highly customizable.
- You can specify the position where the entity will spawn. This position can be relative to the player (always left/right) or adjust to the facing (in front of/behind). Noded entities (like swap blocks) also support these settings for the node.
- The following **entities** can be spawned: Puffers, Clouds, Badeline orbs, Fake hearts, Dream blocks, Boosters, Bumpers, Ice/Lava barriers, Dash blocks, Core blocks, Falling blocks, Feathers, Ice/Fireballs, Moving blocks, Refills, Seekers, Swap blocks, Zip movers, Crumble platforms, Glass blocks, Starjump blocks, Jump throughs, Floaty blocks, Kevins and Decals. You can also "spawn" (set to true) new flags that follow the naming `koseiFlagN` where `N` is a number that increases each time it's "spawned" (koseiFlag9, koseiFlag10, and so on), starting at `koseiFlag1`.
- You can specify a **flag** that needs a specific value (true/false) before the Spawner has effect.
- You can specify a max number of entities that can spawn per room (**Limit**), the time that needs to pass until a new one can appear (**Cooldown**) and if they should disappear after a certain time (TTL or **Time To Live**).
- Appear/disappear **sounds** are customizable. Set them to `event:/none` if you don't want a sound.
- The following **spawn conditions/modes** are supported: **OnFlagEnabled** (when a spawn flag toggles its value), **OnDash** (whenever the player dashes, or every certain amount of dashes), **OnCassetteBeat** (when the beat of the cassette changes), **OnCustomButtonPress** (this button can be configured in Mod Settings), **OnSpeedX** (whenever the player reaches a certain speed threshold) and OnInterval (every x seconds).
- Entities with **sizes** can be adjusted as needed (blocks, jumpthrus and crumble platforms).
- Optionally, spawning an entity can **spend** one **dash or** all of your **stamina**.
- Each entity has its own **settings**, similar to vanilla. Since they were adjusted manually, a few of them may have slight differences (Badeline orbs are single use and will always fly upwards. Dash blocks don't give the usual 3 freeze frames. Boosters and swap blocks don't render their usual outlines/bg respectively).
- You can combine multiple spawners with different settings each in the same room, or use a Flag Spawner as the main controller to spawn a different entity each time!
- For Custom Entities: Only the ones with EntityData are supported. You can specify key-value pairs but if you leave some empty, the default values will be used instead (for example, not specifying a `winged`, `false` attribute for a `Celeste.Strawberry` will fall to the default value, `false`). You can use ILSpy or Mapping Utils to see the code (more specifically the constructor) of an entity, but when in doubt, you can try to input the names that you see on the Lönn plugin.
### Custom Player Seeker
The Player Seeker, but more functional. Includes multiple customization options and a new game mode where you can swap between Madeline and the Seeker with a custom button press. It has multiple new interactions with vanilla entities, including:
- **Touch Switches** (they are enabled).
- Most **dangerous entities** kill the seeker on contact (spikes kill regardless of the direction).
- **Seekers** can be attacked by the Player Seeker.
- Being inside **water** disallows dashes.
- **Flag triggers** can be enabled.
- **Puffers**, **boosters** and **feathers** will disappear. On consumption, puffers explode, boosters light up the room a bit, and feathers give a slight speed bonus.
- **Bumpers** behave as usual, but normal ones will push Madeline too!
- **Core Mode** can be toggled with switches as normal.
- Entities that react to player dashes, like **Swap blocks**, will react as normal.
- **Dream blocks** are enabled/disabled when being dashed.
- **Dash blocks** can be broken as normal. Same for **Temple Cracked Blocks** and **Bounce Blocks**.
- **Kevins** can be enabled with a dash.
- If the **Switch Characters** mode is enabled, the Player Seeker **can be bounced** when controlling Madeline.
- If the Player Seeker gets stuck inside a **Seeker Barrier**, it will be **crushed**.
- They are also vulnerable to Left/Right **Wind**.
- The flag `kosei_PlayerSeeker` will be true while controlling the Player Seeker.

# Triggers

### Force Throw Trigger
It forces the player to immediately throw the holdable, even before the grabbing animation finishes.
### Force Hiccup Trigger
It forces the player to have a **hiccup** (like a minijump) when they enter/exit the trigger, stay inside (on a certain interval) or when the cassette beat changes.
### OOB Trigger
The trigger variant of Bounded Refills. Allows the player to explore the physical space of others without transitioning, or be bounded (transition) to a certain screen.
