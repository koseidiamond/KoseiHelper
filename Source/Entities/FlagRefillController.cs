using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FlagRefillController")]
public class FlagRefillController : Entity
{
    public Color flagRefillHairColor;
    public string flagRefillCustomFlag;
    public bool persistent;
    public FlagRefillController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        flagRefillHairColor = data.HexColor("hairColor", Color.FromNonPremultiplied(230, 0, 30, 255));
        flagRefillCustomFlag = data.Attr("flagName", "KoseiHelper_FlagRefill");
        persistent = data.Bool("persistent", false);
        if (persistent)
            Tag = Tags.Persistent;
        KoseiHelperModule.Session.FlagDashColor = flagRefillHairColor;
        KoseiHelperModule.Session.flagRefillFlag = flagRefillCustomFlag;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        KoseiHelperModule.Session.FlagDashColor = flagRefillHairColor;
        KoseiHelperModule.Session.flagRefillFlag = flagRefillCustomFlag;
    }
}