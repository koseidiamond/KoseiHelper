using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Linq;

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
    public float minValueForFalse, maxValueForTrue;
    public bool absoluteValue, reverseValue;
    public float multiplierFactor;
    public bool flagSuffix;
    public FlagCounterSliderTranslator(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Tag = Tags.Global;
        fcs = data.Enum("mode", FCS.CounterToSlider);

        flagName = data.Attr("flagName", "");
        counterName = data.Attr("counterName", "");
        sliderName = data.Attr("sliderName", "");

        flagSuffix = data.Bool("flagSuffix", false);

        valueWhileFalse = data.Float("valueWhileFalse", 0f);
        valueWhileTrue = data.Float("valueWhileTrue", 1f);
        minValueForFalse = data.Float("minValueForFalse", 0f);
        maxValueForTrue = data.Float("maxValueForTrue", 1f);

        absoluteValue = data.Bool("absoluteValue", false);
        reverseValue = data.Bool("reverseValue", false);
        multiplierFactor = data.Float("multiplierFactor", 1f);
    }

    public override void Update()
    {
        Session session = SceneAs<Level>().Session;
        base.Update();
        switch (fcs)
        {
            case FCS.FlagToCounter:
                if (flagSuffix)
                    session.SetCounter(counterName,(int)((reverseValue ? -1 : 1) * session.Flags.Count(f => f.StartsWith(flagName)) * multiplierFactor));
                else
                    session.SetCounter(counterName, (int)((reverseValue ? -1 : 1) * (session.GetFlag(flagName) ? valueWhileTrue : valueWhileFalse) * multiplierFactor));
                break;
            case FCS.FlagToSlider:
                if (flagSuffix)
                    session.SetSlider(sliderName, (reverseValue ? -1 : 1) * session.Flags.Count(f=> f.StartsWith(flagName)) * multiplierFactor);
                else
                    session.SetSlider(sliderName, (reverseValue ? -1 : 1) * (session.GetFlag(flagName) ? valueWhileTrue : valueWhileFalse) * multiplierFactor);
                break;
            case FCS.CounterToFlag:
                if (flagSuffix)
                    session.SetFlag(flagName + session.GetCounter(counterName).ToString(), true);
                else
                {
                    if (absoluteValue)
                    {
                        if (Math.Abs(session.GetCounter(counterName)) <= minValueForFalse)
                            session.SetFlag(flagName, reverseValue);
                        if (Math.Abs(session.GetCounter(counterName)) >= maxValueForTrue)
                            session.SetFlag(flagName, !reverseValue);
                    }
                    else
                    {
                        if (session.GetCounter(counterName) <= minValueForFalse)
                            session.SetFlag(flagName, reverseValue);
                        if (session.GetCounter(counterName) >= maxValueForTrue)
                            session.SetFlag(flagName, !reverseValue);
                    }
                }
                break;
            case FCS.SliderToFlag:
                if (flagSuffix)
                    session.SetFlag(flagName + session.GetSlider(sliderName).ToString(), true);
                else
                {
                    if (absoluteValue)
                    {
                        if (Math.Abs(session.GetSlider(sliderName)) <= minValueForFalse)
                            session.SetFlag(flagName, reverseValue);
                        if (Math.Abs(session.GetSlider(sliderName)) >= maxValueForTrue)
                            session.SetFlag(flagName, !reverseValue);
                    }
                    else
                    {
                        if (session.GetSlider(sliderName) <= minValueForFalse)
                            session.SetFlag(flagName, reverseValue);
                        if (session.GetSlider(sliderName) >= maxValueForTrue)
                            session.SetFlag(flagName, !reverseValue);
                    }
                }
                break;
            case FCS.SliderToCounter:
                if (absoluteValue)
                    session.SetCounter(counterName, (int)((reverseValue ? -1 : 1) * Math.Abs(session.GetSlider(sliderName)) * multiplierFactor));
                else
                    session.SetCounter(counterName, (int)((reverseValue ? -1 : 1) * session.GetSlider(sliderName) * multiplierFactor));
                break;
            default: // CounterToSlider
                if (absoluteValue)
                    session.SetSlider(sliderName, (reverseValue ? -1 : 1) * Math.Abs(session.GetCounter(counterName)) * multiplierFactor);
                else
                    session.SetSlider(sliderName, (reverseValue ? -1 : 1) * session.GetCounter(counterName) * multiplierFactor);
                break;
        }
    }
}