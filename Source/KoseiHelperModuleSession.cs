using Microsoft.Xna.Framework;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModuleSession : EverestModuleSession
{

    public HashSet<EntityID> SoftDoNotLoad { get; set; } = new(); // Used to remove staticMovers from entities that should not load
    public bool HasPufferDash { get; set; } = false;
    public bool PufferDashActive { get; set; } = false;

    public bool HasFlagDash { get; set; } = false;
    public bool FlagDashActive { get; set; } = false;

    public bool HasCounterDash { get; set; } = false;
    public bool CounterDashActive { get; set; } = false;

    public bool HasShadowDash { get; set; } = false;
    public bool ShadowDashActive { get; set; } = false;

    public bool DebugMapModified { get; set; } = false;

    public bool ClimbJumpHook { get; set; } = false;

    // Flag Refill Controller options
    public Color FlagDashColor = Color.FromNonPremultiplied(230, 0, 30, 255);
    public string flagRefillFlag = "KoseiHelper_FlagRefill";

    // Counter Refill Controller options
    public Color CounterDashColor = Color.FromNonPremultiplied(30, 0, 230, 255);
    public string counterRefillCounter = "KoseiHelper_CounterRefill";
    public bool counterRefillDecrease = false;
    public bool counterRefillWhenUsed = false;

    public bool oobClimbFix { get; set; } = false;
}