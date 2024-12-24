namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModuleSession : EverestModuleSession {
    public bool HasPufferDash { get; set; } = false;
    public bool PufferDashActive { get; set; } = false;

    public bool HasFlagDash { get; set; } = false;
    public bool FlagDashActive { get; set; } = false;
}