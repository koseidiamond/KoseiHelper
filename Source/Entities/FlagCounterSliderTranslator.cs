using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FlagCounterSliderTranslator")]
[Tracked]
public class FlagCounterSliderTranslator : Entity
{
    public enum FCS
    {
        FlagToCounter,
        FlagToSlider,
        CounterToFlag,
        CounterToSlider,
        SliderToFlag,
        SliderToCounter
    };

    public FCS fcs;

    public string flagName, counterName, sliderName;

    public float valueWhileFalse, valueWhileTrue;
    public float minValueForFalse, maxValueForFalse;
    public bool absoluteValue;
    public FlagCounterSliderTranslator(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Tag = Tags.Global;
        fcs = data.Enum("mode", FCS.CounterToSlider);

        flagName = data.Attr("flagName", "");
        counterName = data.Attr("counterName", "");
        sliderName = data.Attr("sliderName", "");

        valueWhileFalse = data.Float("valueWhileFalse", 0f);
        valueWhileTrue = data.Float("valueWhileTrue", 1f);
        minValueForFalse = data.Float("minValueForFalse", 0f);
        maxValueForFalse = data.Float("maxValueForTrue", 1f);
        absoluteValue = data.Bool("absoluteValue", false);
    }

    public override void Update()
    {
        Session session = SceneAs<Level>().Session;
        base.Update();
        switch (fcs)
        {
            case FCS.FlagToCounter:
                if (session.GetFlag(flagName))
                    session.SetCounter(counterName, (int)valueWhileTrue);
                if (!session.GetFlag(flagName))
                    session.SetCounter(counterName, (int)valueWhileFalse);
                break;
                // rest of cases: todo
            case FCS.FlagToSlider:
                break;
            case FCS.CounterToFlag:
                break;
            case FCS.SliderToFlag:
                break;
            case FCS.SliderToCounter:
                break;
            default: // CounterToSlider
                break;
        }
    }
}