using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModuleSettings : EverestModuleSettings {
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
    [SettingName("SpawnButton")]
    public ButtonBinding SpawnButton { get; set; }

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.K)]
    [SettingName("SwapCharacter")]
    public ButtonBinding SwapCharacter { get; set; }

    [SettingSubText("Enables the Nemesis always. Maps that use the Nemesis can override this setting if it's off.")]
    [SettingName("Nemesis Settings")]
    public bool GunEnabled { get; set; } = false;

    [DefaultButtonBinding(Buttons.RightStick, Keys.F)]
    [SettingName("Nemesis Shot")]
    public ButtonBinding NemesisShot { get; set; }
}