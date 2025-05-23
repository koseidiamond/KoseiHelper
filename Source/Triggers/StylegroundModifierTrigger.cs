using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/StylegroundModifierTrigger")]
public class StylegroundModifierTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public string linkedSlider, linkedCounter, linkedFlag;
    public float multiplier;

    public Backdrop backdrop;

    public enum IdentificationMode
    {
        Index,
        Tag,
        Texture
    };
    public IdentificationMode identificationMode;
    public int index;
    public string tag;
    public string texture;

    public enum ValueType
    {
        DirectValue,
        Slider,
        Counter,
        Flag
    };
    public ValueType valueType;

    public enum FieldToModify
    {
        Flag,
        NotFlag,
        Color,
        PositionX,
        PositionY,
        ScrollX,
        ScrollY,
        SpeedX,
        SpeedY,
        Alpha,
        FadeIn,
        FlipX,
        FlipY,
        InstantIn,
        InstantOut,
        LoopX,
        LoopY
    };
    public FieldToModify fieldToModify;
    public string flag, notFlag;
    public Color color;
    public float positionX, positionY;
    public float scrollX, scrollY;
    public float speedX, speedY;
    public float alpha;
    public bool fadeIn;
    public bool flipX, flipY;
    public bool instantIn, instantOut;
    public bool loopX, loopY;

    public bool absoluteValue;

    public float minValue, maxValue, valueWhileTrue, valueWhileFalse;
    public Color minColor, maxColor, colorWhileTrue, colorWhileFalse;

    public StylegroundModifierTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        // General trigger settings
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);

        // Identification settings
        identificationMode = data.Enum("identificationMode", IdentificationMode.Index);
        index = data.Int("index", 0);
        tag = data.Attr("tag", "");
        texture = data.Attr("texture", "");

        // Value type settings (link to a flag/counter/slider or direct input)
        valueType = data.Enum("valueType", ValueType.DirectValue);
        linkedSlider = data.Attr("linkedSlider", "");
        linkedCounter = data.Attr("linkedCounter", "");
        linkedFlag = data.Attr("linkedFlag", "");
        multiplier = data.Float("multiplier", 1f);

        // Bg field settings
        fieldToModify = data.Enum("fieldToModify", FieldToModify.Color);
        flag = data.Attr("flag", "");
        notFlag = data.Attr("notFlag", "");
        color = data.HexColor("color", Color.White);
        positionX = data.Float("positionX", 0f);
        positionY = data.Float("positionY", 0f);
        scrollX = data.Float("scrollX", 0f);
        scrollY = data.Float("scrollY", 0f);
        speedX = data.Float("speedX", 0f);
        speedY = data.Float("speedY", 0f);
        alpha = data.Float("alpha", 1f);
        fadeIn = data.Bool("fadeIn", false);
        flipX = data.Bool("flipX", false);
        flipY = data.Bool("flipY", false);
        instantIn = data.Bool("instantIn", false);
        instantOut = data.Bool("instantOut", false);
        loopX = data.Bool("loopX", false);
        loopY = data.Bool("loopY", false);
        absoluteValue = data.Bool("absoluteValue", false);

        // Parsing settings while linking to flag/counter/slider
        minValue = data.Float("minValue", 0f);
        maxValue = data.Float("maxValue", 1f);
        minColor = data.HexColor("minColor", Color.White);
        maxColor = data.HexColor("maxColor", Color.Black);
        colorWhileTrue = data.HexColor("colorWhileTrue", Color.White);
        colorWhileFalse = data.HexColor("colorWhileFalse", Color.Black);
        valueWhileTrue = data.Float("valueWhileTrue", 1f);
        valueWhileFalse = data.Float("valueWhileFalse", 0f);
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerMode == TriggerMode.OnEnter)
            FindBackdrop();
        Remove();
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (triggerMode == TriggerMode.OnLeave)
            FindBackdrop();
        Remove();
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (triggerMode == TriggerMode.OnStay)
            FindBackdrop();
        Remove();
    }

    private void FindBackdrop()
    {
        Level level = SceneAs<Level>();
        switch (identificationMode)
        {
            case IdentificationMode.Texture:
                foreach (Backdrop bd in level.Background.Backdrops)
                {
                    if (bd.Name.Contains(texture))
                    {
                        backdrop = bd;
                        ModifyBackdrop(backdrop);
                    }
                }
                break;
            case IdentificationMode.Tag:
                foreach (Backdrop bd in level.Background.Backdrops)
                {
                    if (bd.Tags?.Contains(tag) == true)
                    {
                        backdrop = bd;
                        ModifyBackdrop(backdrop);
                    }
                }
                break;
                throw new Exception($"No backdrops found with tag: {tag}");
            default: // Index
                backdrop = level.Background.Backdrops[index];
                ModifyBackdrop(backdrop);
                break;
        }
    }

    private void ModifyBackdrop(Backdrop backdrop)
    {
        Session session = SceneAs<Level>().Session;
        switch (fieldToModify)
        {
            case FieldToModify.Flag:
                backdrop.OnlyIfFlag = flag;
                break;
            case FieldToModify.NotFlag:
                backdrop.OnlyIfNotFlag = notFlag;
                break;
            case FieldToModify.PositionX:
                switch (valueType)
                {
                    case ValueType.Counter:
                            if (absoluteValue)
                            backdrop.Position.X = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.Position.X = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.Position.X = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.Position.X = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.Position.X = valueWhileTrue;
                        else
                            backdrop.Position.X = valueWhileFalse;
                        break;
                    default: // DirectValue
                        backdrop.Position.X = positionX;
                        break;
                }
                break;
            case FieldToModify.PositionY:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if (absoluteValue)
                            backdrop.Position.Y = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.Position.Y = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.Position.Y = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.Position.Y = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.Position.Y = valueWhileTrue;
                        else
                            backdrop.Position.Y = valueWhileFalse;
                        break;
                    default: // DirectValue
                        backdrop.Position.Y = positionY;
                        break;
                }
                break;
            case FieldToModify.ScrollX:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if (absoluteValue)
                            backdrop.Scroll.X = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.Scroll.X = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.Scroll.X = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.Scroll.X = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.Scroll.X = valueWhileTrue;
                        else
                            backdrop.Scroll.X = valueWhileFalse;
                        break;
                    default: // DirectValue
                        backdrop.Scroll.X = scrollX;
                        break;
                }
                break;
            case FieldToModify.ScrollY:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if (absoluteValue)
                            backdrop.Scroll.Y = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.Scroll.Y = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.Scroll.Y = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.Scroll.Y = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.Scroll.Y = valueWhileTrue;
                        else
                            backdrop.Scroll.Y = valueWhileFalse;
                        break;
                    default: // DirectValue
                        backdrop.Scroll.Y = scrollY;
                        break;
                }
                break;
            case FieldToModify.SpeedX:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if (absoluteValue)
                            backdrop.Speed.X = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.Speed.X = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.Speed.X = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.Speed.X = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.Speed.X = valueWhileTrue;
                        else
                            backdrop.Speed.X = valueWhileFalse;
                        break;
                    default: // DirectValue
                        backdrop.Speed.X = speedX;
                        break;
                }
                break;
            case FieldToModify.SpeedY:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if (absoluteValue)
                            backdrop.Speed.Y = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.Speed.Y = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.Speed.Y = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.Speed.Y = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.Speed.Y = valueWhileTrue;
                        else
                            backdrop.Speed.Y = valueWhileFalse;
                        break;
                    default: // DirectValue
                        backdrop.Speed.Y = speedY;
                        break;
                }
                break;
            case FieldToModify.Alpha:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if (absoluteValue)
                            backdrop.FadeAlphaMultiplier = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.FadeAlphaMultiplier = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.FadeAlphaMultiplier = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.FadeAlphaMultiplier = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.FadeAlphaMultiplier = valueWhileTrue;
                        else
                            backdrop.FadeAlphaMultiplier = valueWhileFalse;
                        break;
                    default: // DirectValue
                        backdrop.FadeAlphaMultiplier = alpha;
                        break;
                }
                break;
            case FieldToModify.FadeIn:
                throw new System.Exception("The FadeIn feature is not implemented yet!");
                break;
            case FieldToModify.FlipX:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if ((!absoluteValue && session.GetCounter(linkedCounter) <= minValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) <= minValue))
                            backdrop.FlipX = false;
                        if ((!absoluteValue && session.GetCounter(linkedCounter) >= maxValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) >= maxValue))
                            backdrop.FlipX = true;
                        break;
                    case ValueType.Slider:
                        if ((!absoluteValue && session.GetSlider(linkedSlider) <= minValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) <= minValue))
                            backdrop.FlipX = false;
                        if ((!absoluteValue && session.GetSlider(linkedSlider) >= maxValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) >= maxValue))
                            backdrop.FlipX = true;
                        break;
                    case ValueType.Flag:
                            backdrop.FlipX = session.GetFlag(linkedFlag);
                        break;
                    default: // DirectValue
                        backdrop.FlipX = flipX;
                        break;
                }
                break;
            case FieldToModify.FlipY:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if ((!absoluteValue && session.GetCounter(linkedCounter) <= minValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) <= minValue))
                            backdrop.FlipY = false;
                        if ((!absoluteValue && session.GetCounter(linkedCounter) >= maxValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) >= maxValue))
                            backdrop.FlipY = true;
                        break;
                    case ValueType.Slider:
                        if ((!absoluteValue && session.GetSlider(linkedSlider) <= minValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) <= minValue))
                            backdrop.FlipY = false;
                        if ((!absoluteValue && session.GetSlider(linkedSlider) >= maxValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) >= maxValue))
                            backdrop.FlipY = true;
                        break;
                    case ValueType.Flag:
                        backdrop.FlipY = session.GetFlag(linkedFlag);
                        break;
                    default: // DirectValue
                        backdrop.FlipY = flipY;
                        break;
                }
                break;
            case FieldToModify.InstantIn:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if ((!absoluteValue && session.GetCounter(linkedCounter) <= minValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) <= minValue))
                            backdrop.InstantIn = false;
                        if ((!absoluteValue && session.GetCounter(linkedCounter) >= maxValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) >= maxValue))
                            backdrop.InstantIn = true;
                        break;
                    case ValueType.Slider:
                        if ((!absoluteValue && session.GetSlider(linkedSlider) <= minValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) <= minValue))
                            backdrop.InstantIn = false;
                        if ((!absoluteValue && session.GetSlider(linkedSlider) >= maxValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) >= maxValue))
                            backdrop.InstantIn = true;
                        break;
                    case ValueType.Flag:
                        backdrop.InstantIn = session.GetFlag(linkedFlag);
                        break;
                    default: // DirectValue
                        backdrop.InstantIn = instantIn;
                        break;
                }
                break;
            case FieldToModify.InstantOut:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if ((!absoluteValue && session.GetCounter(linkedCounter) <= minValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) <= minValue))
                            backdrop.InstantOut = false;
                        if ((!absoluteValue && session.GetCounter(linkedCounter) >= maxValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) >= maxValue))
                            backdrop.InstantOut = true;
                        break;
                    case ValueType.Slider:
                        if ((!absoluteValue && session.GetSlider(linkedSlider) <= minValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) <= minValue))
                            backdrop.InstantOut = false;
                        if ((!absoluteValue && session.GetSlider(linkedSlider) >= maxValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) >= maxValue))
                            backdrop.InstantOut = true;
                        break;
                    case ValueType.Flag:
                        backdrop.InstantOut = session.GetFlag(linkedFlag);
                        break;
                    default: // DirectValue
                        backdrop.InstantOut = instantOut;
                        break;
                }
                break;
            case FieldToModify.LoopX:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if ((!absoluteValue && session.GetCounter(linkedCounter) <= minValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) <= minValue))
                            backdrop.LoopX = false;
                        if ((!absoluteValue && session.GetCounter(linkedCounter) >= maxValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) >= maxValue))
                            backdrop.LoopX = true;
                        break;
                    case ValueType.Slider:
                        if ((!absoluteValue && session.GetSlider(linkedSlider) <= minValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) <= minValue))
                            backdrop.LoopX = false;
                        if ((!absoluteValue && session.GetSlider(linkedSlider) >= maxValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) >= maxValue))
                            backdrop.LoopX = true;
                        break;
                    case ValueType.Flag:
                        backdrop.LoopX = session.GetFlag(linkedFlag);
                        break;
                    default: // DirectValue
                        backdrop.LoopX = loopX;
                        break;
                }
                break;
            case FieldToModify.LoopY:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if ((!absoluteValue && session.GetCounter(linkedCounter) <= minValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) <= minValue))
                            backdrop.LoopY = false;
                        if ((!absoluteValue && session.GetCounter(linkedCounter) >= maxValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) >= maxValue))
                            backdrop.LoopY = true;
                        break;
                    case ValueType.Slider:
                        if ((!absoluteValue && session.GetSlider(linkedSlider) <= minValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) <= minValue))
                            backdrop.LoopY = false;
                        if ((!absoluteValue && session.GetSlider(linkedSlider) >= maxValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) >= maxValue))
                            backdrop.LoopY = true;
                        break;
                    case ValueType.Flag:
                        backdrop.LoopY = session.GetFlag(linkedFlag);
                        break;
                    default: // DirectValue
                        backdrop.LoopY = loopY;
                        break;
                }
                break;
            default: // Color
                switch (valueType)
                {
                    case ValueType.Counter: // Gradually changes the color from minColor to maxColor which correspond to minValue and maxValue each
                        float normalizedCounter;
                        if (absoluteValue)
                            normalizedCounter = MathHelper.Clamp((Math.Abs(session.GetCounter(linkedCounter)) - minValue) / (maxValue - minValue), 0f, 1f);
                        else
                            normalizedCounter = MathHelper.Clamp((session.GetCounter(linkedCounter) - minValue) / (maxValue - minValue), 0f, 1f);
                        backdrop.Color = Color.Lerp(minColor, maxColor, MathHelper.Lerp(minValue, maxValue, session.GetSlider(linkedCounter)));
                        break;
                    case ValueType.Slider: // Gradually changes the color from minColor to maxColor which correspond to minValue and maxValue each
                        float normalizedSlider;
                        if (absoluteValue)
                            normalizedSlider = MathHelper.Clamp((Math.Abs(session.GetSlider(linkedSlider)) - minValue) / (maxValue - minValue), 0f, 1f);
                        else
                            normalizedSlider = MathHelper.Clamp((session.GetSlider(linkedSlider) - minValue) / (maxValue - minValue), 0f, 1f);
                        backdrop.Color = Color.Lerp(minColor, maxColor, normalizedSlider);
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.Color = colorWhileTrue;
                        else
                            backdrop.Color = colorWhileFalse;
                            break;
                    default: // DirectValue
                        backdrop.Color = color;
                        break;
                }
                break;
        }
    }

    private void Remove()
    {
        if (onlyOnce)
            RemoveSelf();
    }
}