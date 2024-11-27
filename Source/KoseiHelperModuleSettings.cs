namespace Celeste.Mod.KoseiHelper;

using Microsoft.Xna.Framework.Input;

public class KoseiHelperModuleSettings : EverestModuleSettings {
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
    [SettingName("SpawnButton")]
    public ButtonBinding SpawnButton { get; set; }

    [DefaultButtonBinding(Buttons.RightShoulder, Keys.K)]
    [SettingName("SwapCharacter")]
    public ButtonBinding SwapCharacter { get; set; }
}