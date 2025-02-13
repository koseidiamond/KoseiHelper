using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModuleSession : EverestModuleSession {
    public bool HasPufferDash { get; set; } = false;
    public bool PufferDashActive { get; set; } = false;

    public bool HasFlagDash { get; set; } = false;
    public bool FlagDashActive { get; set; } = false;
    public bool HasCounterDash { get; set; } = false;
    public bool CounterDashActive { get; set; } = false;

    public bool DebugMapModified { get; set; } = false;
    // Flag Refill Controller options
    public Color FlagDashColor = Color.FromNonPremultiplied(230, 0, 30, 255);
    public Color CounterDashColor = Color.FromNonPremultiplied(30, 0, 230, 255);
    public string flagRefillFlag = "KoseiHelper_FlagRefill";
    // Counter Refill Controller options
    public bool counterRefillDecrease = false;
    public bool counterRefillWhenUsed = false;
    public bool oobClimbFix { get; set; } = false;
}