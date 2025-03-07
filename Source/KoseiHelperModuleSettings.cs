using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModuleSettings : EverestModuleSettings {
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
    [SettingName("SpawnButton")]
    public ButtonBinding SpawnButton { get; set; }

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.K)]
    [SettingName("SwapCharacter")]
    public ButtonBinding SwapCharacter { get; set; }

    [DefaultButtonBinding(Buttons.RightStick, Keys.F)]
    [SettingName("Nemesis Shot")]
    public ButtonBinding NemesisShot { get; set; }
}