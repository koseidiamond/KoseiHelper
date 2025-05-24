using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

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
                if (session.GetFlag(flagName))
                {
                    if (!reverseValue)
                        session.SetCounter(counterName, (int)(valueWhileTrue * multiplierFactor));
                    else
                        session.SetCounter(counterName, (int)(-valueWhileTrue * multiplierFactor));

                }
                if (!session.GetFlag(flagName))
                {
                    if (!reverseValue)
                        session.SetCounter(counterName, (int)(valueWhileFalse * multiplierFactor));
                    else
                        session.SetCounter(counterName, (int)(-valueWhileFalse * multiplierFactor));
                }
                break;
            case FCS.FlagToSlider:
                if (session.GetFlag(flagName))
                {
                    if (!reverseValue)
                        session.SetSlider(sliderName, valueWhileTrue * multiplierFactor);
                    else
                        session.SetSlider(sliderName, -valueWhileTrue * multiplierFactor);
                }
                if (!session.GetFlag(flagName))
                {
                    if (!reverseValue)
                        session.SetSlider(sliderName, valueWhileFalse * multiplierFactor);
                    else
                        session.SetSlider(sliderName, -valueWhileFalse * multiplierFactor);
                }
                break;
            case FCS.CounterToFlag:
                if (absoluteValue)
                {
                    if (Math.Abs(session.GetCounter(counterName)) <= minValueForFalse)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, false);
                        else
                            session.SetFlag(flagName, true);
                    }
                    if (Math.Abs(session.GetCounter(counterName)) >= maxValueForTrue)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, true);
                        else
                            session.SetFlag(flagName, false);
                    }
                }
                else
                {
                    if (session.GetCounter(counterName) <= minValueForFalse)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, false);
                        else
                            session.SetFlag(flagName, true);
                    }
                    if (session.GetCounter(counterName) >= maxValueForTrue)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, true);
                        else
                            session.SetFlag(flagName, false);
                    }
                }
                break;
            case FCS.SliderToFlag:
                if (absoluteValue)
                {
                    if (Math.Abs(session.GetSlider(sliderName)) <= minValueForFalse)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, false);
                        else
                            session.SetFlag(flagName, true);
                    }
                    if (Math.Abs(session.GetSlider(sliderName)) >= maxValueForTrue)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, true);
                        else
                            session.SetFlag(flagName, false);
                    }
                }
                else
                {
                    if (session.GetSlider(sliderName) <= minValueForFalse)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, false);
                        else
                            session.SetFlag(flagName, true);
                    }
                    if (session.GetSlider(sliderName) >= maxValueForTrue)
                    {
                        if (!reverseValue)
                            session.SetFlag(flagName, true);
                        else
                            session.SetFlag(flagName, false);
                    }
                }
                break;
            case FCS.SliderToCounter:
                if (absoluteValue)
                {
                    if (!reverseValue)
                        session.SetCounter(counterName, (int)(Math.Abs(session.GetSlider(sliderName)) * multiplierFactor));
                    else
                        session.SetCounter(counterName, (int)(-Math.Abs(session.GetSlider(sliderName)) * multiplierFactor));
                }
                else
                {
                    if (!reverseValue)
                        session.SetCounter(counterName, (int)(session.GetSlider(sliderName) * multiplierFactor));
                    else
                        session.SetCounter(counterName, (int)(-session.GetSlider(sliderName) * multiplierFactor));
                }
                break;
            default: // CounterToSlider
                if (absoluteValue)
                {
                    if (!reverseValue)
                        session.SetSlider(sliderName, Math.Abs(session.GetCounter(counterName)) * multiplierFactor);
                    else
                        session.SetSlider(sliderName, -Math.Abs(session.GetCounter(counterName)) * multiplierFactor);
                }
                else
                {
                    if (!reverseValue)
                        session.SetSlider(sliderName, session.GetCounter(counterName) * multiplierFactor);
                    else
                        session.SetSlider(sliderName, -session.GetCounter(counterName) * multiplierFactor);
                }
                break;
        }
    }
}