namespace Celeste.Mod.KoseiHelper;

using Microsoft.Xna.Framework.Input;

public class KoseiHelperModuleSettings : EverestModuleSettings {
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
    [SettingName("SpawnButton")]
    public ButtonBinding SpawnButton { get; set; }
}