using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FlagRefillController")]
public class FlagRefillController : Entity
{
    private Color flagRefillHairColor, counterRefillHairColor;
    private string flagRefillCustomFlag;
    private string counterRefillCustomCounter;

    private bool counterRefillDecrease, counterRefillWhenUsed;
    private bool persistent;
    public FlagRefillController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        flagRefillHairColor = data.HexColor("flagHairColor", Color.FromNonPremultiplied(230, 0, 30, 255));
        counterRefillHairColor = data.HexColor("counterHairColor", Color.FromNonPremultiplied(30, 0, 230, 255));
        flagRefillCustomFlag = data.Attr("flagName", "KoseiHelper_FlagRefill");
        counterRefillCustomCounter = data.Attr("counterName", "KoseiHelper_CounterRefill");

        counterRefillDecrease = data.Bool("decrease", false);
        counterRefillWhenUsed = data.Bool("countWhenUsed", false);

        persistent = data.Bool("persistent", false);
        if (persistent)
            Tag = Tags.Persistent;
        KoseiHelperModule.Session.FlagDashColor = flagRefillHairColor;
        KoseiHelperModule.Session.flagRefillFlag = flagRefillCustomFlag;

        KoseiHelperModule.Session.counterRefillDecrease = counterRefillDecrease;
        KoseiHelperModule.Session.counterRefillWhenUsed = counterRefillWhenUsed;
        KoseiHelperModule.Session.CounterDashColor = counterRefillHairColor;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        KoseiHelperModule.Session.FlagDashColor = flagRefillHairColor;
        KoseiHelperModule.Session.flagRefillFlag = flagRefillCustomFlag;
        KoseiHelperModule.Session.CounterDashColor = counterRefillHairColor;
    }
}