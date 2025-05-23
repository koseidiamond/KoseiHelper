using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FlagCounterSliderTranslator")]
[Tracked]
public class FlagCounterSliderTranslator : Entity
{
    public FlagCounterSliderTranslator(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Tag = Tags.Global;
    }

    public override void Update()
    {
        Session session = SceneAs<Level>().Session;
        base.Update();
    }
}