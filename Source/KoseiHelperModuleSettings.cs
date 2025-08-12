using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModuleSettings : EverestModuleSettings
{
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
    [SettingName("SpawnButton")]
    public ButtonBinding SpawnButton { get; set; }

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.K)]
    [SettingName("SwapCharacter")]
    public ButtonBinding SwapCharacter { get; set; }

    [DefaultButtonBinding(Buttons.RightStick, Keys.F)]
    [SettingName("Nemesis Shot")]
    public ButtonBinding NemesisShot { get; set; }

    [SettingSubMenu]
    public class NemesisSettings
    {
        [SettingSubText("Enables the Nemesis Gun on maps that don't use it. Default off.")]
        public bool GunEnabled { get; set; } = false;

        [SettingSubText("Whether you can shoot while in the feather state. Default off.")]
        public bool CanShootInFeather { get; set; } = false;

        [SettingSubText("How much time until you can shoot again, in frames. Default 8.")]
        [SettingRange(min: 0, max: 999)]
        public int Cooldown { get; set; } = 8;

        [SettingSubText("How much time until the bullets expire. Default 600.")]
        [SettingRange(min: 0, max: 999)]
        public int Lifetime { get; set; } = 600;

        [SettingSubText("How much time until you can be pushed backwards again from shooting. Default 16.")]
        [SettingRange(min: 0, max: 999)]
        public int RecoilCooldown { get; set; } = 16;

        [SettingSubText("Whether the recoil should push you upwards instead of working like an horizontal backboost.")]
        public bool RecoilUpwards { get; set; } = false;

        [SettingSubText("Whether the recoil should happen only when a bullet interacts with an entity.")]
        public bool RecoilOnlyOnInteraction { get; set; } = false;

        [SettingSubText("How many freeze frames occur each time you shoot. Default 0.")]
        [SettingRange(min: 0, max: 60)]
        public int FreezeFrames { get; set; } = 0;

        [SettingSubText("How fast the bullets should travel at. High values are not recommended.")]
        [SettingNumberInput(allowNegatives: false, maxLength: 10)]
        [SettingInGame(false)]
        public float SpeedMultiplier { get; set; } = 1f;

        [SettingSubText("How much you should be pushed backwards when shooting. 80 is the same as a vanilla backboost.")]
        [SettingNumberInput(allowNegatives: true, maxLength: 999)]
        [SettingInGame(false)]
        public float Recoil { get; set; } = 80f;

        [SettingSubText("How much the bullets should accelerate horizontally on each frame.\n" +
            "Using both accelerations can result in curved trajectories! Recommended values are between -1 and 1.")]
        [SettingNumberInput(allowNegatives: true, maxLength: 99)]
        [SettingInGame(false)]
        public float HorizontalAcceleration { get; set; } = 0f;

        [SettingSubText("How much the bullets should accelerate vertically on each frame.\n" +
            "Using both accelerations can result in curved trajectories! Recommended values are between -1 and 1.")]
        [SettingNumberInput(allowNegatives: true, maxLength: 99)]
        [SettingInGame(false)]
        public float VerticalAcceleration { get; set; } = 0f;

        public enum DashBehavior
        {
            ReplacesDash,
            ConsumesDash,
            None
        }

        [SettingSubText("Whether the gun should replace the dash (taking priority over variants), spend a dash, or nothing. Default None.\n" +
            "If it doesn't replace the dash, the custom keybind will be used.")]
        [SettingName("KoseiHelper_NemesisSettings_DashBehavior")]
        public DashBehavior dashBehavior { get; set; } = DashBehavior.None;

        public enum GunDirections
        {
            Horizontal,
            FourDirections,
            EightDirections
        }

        [SettingSubText("Whether you can shoot cardinally, cardinally+diagonally, or only left and right. Default 4 directions.")]
        [SettingName("KoseiHelper_NemesisSettings_GunDirections")]
        public GunDirections gunDirections { get; set; } = GunDirections.EightDirections;
    }
    [SettingSubText("General Nemesis Gun settings. These can be overridden by specific map settings.\n" +
        "Some options are not available while a map is loaded. Go to the Overworld to configure them.")]
    public NemesisSettings GunSettings { get; set; } = new();

    [SettingSubMenu]
    public class NemesisInteractions
    {
        [SettingSubText("Whether bullets should be able to kill you upon contact if they bounce back from a bumper/spring,\nor if they shoot at their golden/silver berry.")]
        public bool CanKillPlayer { get; set; } = false;

        [SettingSubText("Whether bounce blocks (core blocks) should break when shot.")]
        public bool BreakBounceBlocks { get; set; } = true;

        [SettingSubText("Whether falling blocks/falling platforms should start falling when shot.")]
        public bool ActivateFallingBlocks { get; set; } = true;

        [SettingSubText("Whether bullets should collide with platforms like jump throughs, moving platforms or clouds.")]
        public bool CollideWithPlatforms { get; set; } = true;

        [SettingSubText("Whether swap blocks should move when shot.")]
        public bool MoveSwapBlocks { get; set; } = true;

        [SettingSubText("Whether bullets should be able to harm/kill enemies (Oshiro, goombas, seekers and plants).\nIf the enemy is a Badeline Boss, the player will be teleported to it in order to hit it.")]
        public bool HarmEnemies { get; set; } = true;

        [SettingSubText("Whether bullets should be able to break spinners.")]
        public bool BreakSpinners { get; set; } = true;

        [SettingSubText("Whether spinners can be shattered, regardless of the distance (they are uncollidable if they are too far).\nEnable this only if you need it, since it can impact performance.")]
        public bool SpinnerFix { get; set; } = false;

        [SettingSubText("Whether bullets can activate moving blocks.")]
        public bool MoveMovingBlocks { get; set; } = true;

        [SettingSubText("Whether bullets should be able to break blades/stars/dust bunnies that move/rotate.")]
        public bool BreakMovingBlades { get; set; } = true;

        [SettingSubText("Whether bullets should be able to use feathers (or permanently destroy their bubbles if shielded).")]
        public bool UseFeathers { get; set; } = true;

        [SettingSubText("Whether the player can collect entities (berries/seeds, keys, gems, hearts, cassettes) by shooting.")]
        public bool Collectables { get; set; } = true;

        [SettingSubText("Whether bullets can use refills for you.")]
        public bool UseRefills { get; set; } = true;

        [SettingSubText("Whether bullets can enable dash switches.")]
        public bool PressDashSwitches { get; set; } = true;

        [SettingSubText("Whether bullets can bounce back on certain entities,\n" +
            "like bumpers, springs, fake hearts or shielded feathers.")]
        public bool CanBounce { get; set; } = true;

        [SettingSubText("Whether shooting a bird will make it fling away.")]
        public bool ScareBirds { get; set; } = true;

        [SettingSubText("Whether bullets should be able to collect touch switches.")]
        public bool CollectTouchSwitches { get; set; } = true;

        [SettingSubText("Whether bullets should make Badeline orbs to absorb you upon contact.")]
        public bool CollectBadelineOrbs { get; set; } = true;

        [SettingSubText("Whether bullets should be able to swap Core Mode Toggles.")]
        public bool CoreModeToggles { get; set; } = true;

        [SettingSubText("The player will immediately teleport and use the booster when shooting it.")]
        public bool UseBoosters { get; set; } = false;

        [SettingSubText("The player will immediately teleport and use the booster when shooting it.")]
        public bool DashOnKevins { get; set; } = true;

        [SettingSubText("How much the water will slow down the bullets. Recommended values are 0.990 to 1.000. Default 0.995.")]
        [SettingNumberInput(allowNegatives: true, maxLength: 9)]
        [SettingInGame(false)]
        public float WaterFriction { get; set; } = 0.995f;

        [SettingSubText("Whether bullets can destroy dash blocks. Also affects (Custom) Temple Cracked Blocks.\n" +
            "BreakSome can only break those that would normally be broken with a dash.")]
        [SettingName("KoseiHelper_NemesisSettings_DashBlockBehavior")]
        public DashBlockBehavior dashBlockBehavior { get; set; } = DashBlockBehavior.BreakAll;
        public enum DashBlockBehavior
        {
            NoBreak,
            BreakSome,
            BreakAll
        }

        public enum TheoBehavior
        {
            None,
            Kill,
            HitSpinner,
            HitSpring
        }

        [SettingSubText("Whether Theo should die when shot, bounce like when he hits a spring,\n" +
            "or jump like when he is inside spinners. Default HitSpring.")]
        [SettingName("KoseiHelper_NemesisSettings_TheoBehavior")]
        public TheoBehavior theoBehavior { get; set; } = TheoBehavior.HitSpring;

        public enum JellyfishBehavior
        {
            None,
            Kill,
            HitSpring,
            Throw
        }

        [SettingSubText("Whether Jellyfishes should die when shot, bounce like when they hit a spring,\n" +
            "be pushed like when you throw them, or nothing. Default HitSpring.")]
        [SettingName("KoseiHelper_NemesisSettings_JellyfishBehavior")]
        public JellyfishBehavior jellyfishBehavior { get; set; } = JellyfishBehavior.HitSpring;

        public enum DreamBlockBehavior
        {
            GoThrough,
            Destroy,
            None
        }

        [SettingSubText("Whether bullets should be able to pass through dream blocks, destroy them\n" +
            "or collide with them (None). Default GoThrough.")]
        [SettingName("KoseiHelper_NemesisSettings_DreamBlockBehavior")]
        public DreamBlockBehavior dreamBlockBehavior { get; set; } = DreamBlockBehavior.GoThrough;

        public enum PufferBehavior
        {
            None,
            Explode,
            HitSpring
        }

        [SettingSubText("Whether pufferfishes should explode or bounced like when they hit a spring. Default Explode.")]
        [SettingName("KoseiHelper_NemesisSettings_PufferBehavior")]
        public PufferBehavior pufferBehavior { get; set; } = PufferBehavior.Explode;
    }
    [SettingSubText("Determines which entities react to the Nemesis Gun bullets.\n" +
        "These can be overridden by specific map settings.\n")]
    public NemesisInteractions GunInteractions { get; set; } = new();
}