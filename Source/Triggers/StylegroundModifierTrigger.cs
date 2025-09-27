using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/StylegroundModifierTrigger")]
public class StylegroundModifierTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public string linkedSlider, linkedCounter, linkedFlag;
    public float multiplier;
    public enum StylegroundDepth
    {
        Background,
        Foreground,
        Both
    };
    public StylegroundDepth stylegroundDepth;

    public Backdrop backdrop;

    public enum IdentificationMode
    {
        Index,
        Tag,
        Texture
    };
    public IdentificationMode identificationMode;
    public int index;
    public new string tag;
    public string texture;

    public enum ValueType
    {
        DirectValue,
        FadeValue,
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
        WindMultiplier,
        FadeIn,
        FlipX,
        FlipY,
        InstantIn,
        InstantOut,
        LoopX,
        LoopY,
        Custom
    };
    public FieldToModify fieldToModify;
    public string flag, notFlag;
    public Color color;
    public float positionX, positionY;
    public float scrollX, scrollY;
    public float speedX, speedY;
    public float alpha;
    public float windMultiplier;
    public bool fadeIn;
    public bool flipX, flipY;
    public bool instantIn, instantOut;
    public bool loopX, loopY;

    public bool absoluteValue;

    public float minValue, maxValue, valueWhileTrue, valueWhileFalse;
    public Color minColor, maxColor, colorWhileTrue, colorWhileFalse;

    private PositionModes positionMode;
    private string direction;
    public string customFieldName, customFieldValue;

    public StylegroundModifierTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        // General trigger settings
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        stylegroundDepth = data.Enum("stylegroundDepth", StylegroundDepth.Background);
        direction = data.Attr("positionMode");
        if (!string.IsNullOrEmpty(direction) && Enum.TryParse<PositionModes>(direction.ToString(), ignoreCase: true, out PositionModes result))
            positionMode = result;

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
        color = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("color", out object c1) ? c1.ToString() : null, Color.White);
        positionX = data.Float("positionX", 0f);
        positionY = data.Float("positionY", 0f);
        scrollX = data.Float("scrollX", 0f);
        scrollY = data.Float("scrollY", 0f);
        speedX = data.Float("speedX", 0f);
        speedY = data.Float("speedY", 0f);
        alpha = data.Float("alpha", 1f);
        windMultiplier = data.Float("windMultiplier", 1f);
        fadeIn = data.Bool("fadeIn", false);
        flipX = data.Bool("flipX", false);
        flipY = data.Bool("flipY", false);
        instantIn = data.Bool("instantIn", false);
        instantOut = data.Bool("instantOut", false);
        loopX = data.Bool("loopX", false);
        loopY = data.Bool("loopY", false);
        absoluteValue = data.Bool("absoluteValue", false);
        customFieldName = data.Attr("customFieldName", "");
        customFieldValue = data.Attr("customFieldValue", "1.0");

        // Parsing settings while linking to flag/counter/slider
        minValue = data.Float("minValue", 0f);
        maxValue = data.Float("maxValue", 1f);
        minColor = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("minColor", out object c2) ? c2.ToString() : null, Color.White);
        maxColor = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("maxColor", out object c3) ? c3.ToString() : null, Color.Black);
        colorWhileTrue = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("colorWhileTrue", out object c4) ? c4.ToString() : null, Color.White);
        colorWhileFalse = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("colorWhileFalse", out object c5) ? c5.ToString() : null, Color.Black);
        valueWhileTrue = data.Float("valueWhileTrue", 1f);
        valueWhileFalse = data.Float("valueWhileFalse", 0f);
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerMode == TriggerMode.OnEnter)
            PreFindBackdrop();
        Remove();
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (triggerMode == TriggerMode.OnLeave)
            PreFindBackdrop();
        Remove();
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (triggerMode == TriggerMode.OnStay)
            PreFindBackdrop();
        Remove();
    }

    private void PreFindBackdrop()
    {
        if (stylegroundDepth != StylegroundDepth.Background)
            FindBackdrop(level => level.Foreground.Backdrops);
        if (stylegroundDepth != StylegroundDepth.Foreground)
            FindBackdrop(level => level.Background.Backdrops);
    }

    private void FindBackdrop(Func<Level, IEnumerable<Backdrop>> getBackdrops)
    {
        Level level = SceneAs<Level>();
        IEnumerable<Backdrop> backdrops = getBackdrops(level);
        if (index < 0 || index >= backdrops.Count())
        {
            Logger.Log(LogLevel.Error, "KoseiHelper", "A Styleground Modifier is trying to find a styleground with a non-valid index!");
            return;
        }
        bool stylegroundTextureFound = false;
        foreach (Backdrop bd in backdrops)
        {
            bool match = false;
            switch (identificationMode)
            {
                case IdentificationMode.Texture:
                    if (!string.IsNullOrEmpty(bd.Name))
                        match = bd.Name.Contains(texture);
                    break;
                case IdentificationMode.Tag:
                    match = bd.Tags?.Contains(tag) == true;
                    break;
                default:
                    if (stylegroundDepth == StylegroundDepth.Background)
                        match = bd == level.Background.Backdrops.ElementAtOrDefault(index);
                    else if (stylegroundDepth == StylegroundDepth.Foreground)
                        match = bd == level.Foreground.Backdrops.ElementAtOrDefault(index);
                    break;
            }
            if (match)
            {
                backdrop = bd;
                ModifyBackdrop(backdrop);
                stylegroundTextureFound = true;
                break;
            }
        }
        if (!stylegroundTextureFound)
        {
            string identifierValue = identificationMode switch
            {
                IdentificationMode.Texture => texture,
                IdentificationMode.Tag => tag,
                _ => index.ToString()
            };
            Logger.Log(LogLevel.Warn, "KoseiHelper", $"StylegroundModifierTrigger: No matching backdrop found using {identificationMode} " +
                $"\"{(identificationMode == IdentificationMode.Texture ? texture : identificationMode == IdentificationMode.Tag ? tag : index.ToString())}\"!");
        }
    }

    private void ModifyBackdrop(Backdrop backdrop)
    {
        Session session = SceneAs<Level>().Session;
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.Position.X = MathHelper.Lerp(minValue, maxValue, fadeLerp);
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.Position.Y = MathHelper.Lerp(minValue, maxValue, fadeLerp);
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.Scroll.X = MathHelper.Lerp(minValue, maxValue, fadeLerp);
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.Scroll.Y = MathHelper.Lerp(minValue, maxValue, fadeLerp);
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.Speed.X = MathHelper.Lerp(minValue, maxValue, fadeLerp);
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.Speed.Y = MathHelper.Lerp(minValue, maxValue, fadeLerp);
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.FadeAlphaMultiplier = MathHelper.Lerp(minValue, maxValue, fadeLerp);
                        break;
                    default: // DirectValue
                        backdrop.FadeAlphaMultiplier = alpha;
                        break;
                }
                break;
            case FieldToModify.WindMultiplier:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if (absoluteValue)
                            backdrop.WindMultiplier = Math.Abs(session.GetCounter(linkedCounter)) * multiplier;
                        else
                            backdrop.WindMultiplier = session.GetCounter(linkedCounter) * multiplier;
                        break;
                    case ValueType.Slider:
                        if (absoluteValue)
                            backdrop.WindMultiplier = Math.Abs(session.GetSlider(linkedSlider)) * multiplier;
                        else
                            backdrop.WindMultiplier = session.GetSlider(linkedSlider) * multiplier;
                        break;
                    case ValueType.Flag:
                        if (session.GetFlag(linkedFlag))
                            backdrop.WindMultiplier = valueWhileTrue;
                        else
                            backdrop.WindMultiplier = valueWhileFalse;
                        break;
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.WindMultiplier = MathHelper.Lerp(minValue, maxValue, fadeLerp);
                        break;
                    default: // DirectValue
                        backdrop.WindMultiplier = windMultiplier;
                        break;
                }
                break;
            case FieldToModify.FadeIn:
                switch (valueType)
                {
                    case ValueType.Counter:
                        if ((!absoluteValue && session.GetCounter(linkedCounter) <= minValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) <= minValue))
                            if (backdrop is Parallax parallax1)
                                parallax1.DoFadeIn = false;
                        if ((!absoluteValue && session.GetCounter(linkedCounter) >= maxValue) || (absoluteValue && Math.Abs(session.GetCounter(linkedCounter)) >= maxValue))
                            if (backdrop is Parallax parallax2)
                                parallax2.DoFadeIn = true;
                        break;
                    case ValueType.Slider:
                        if ((!absoluteValue && session.GetSlider(linkedSlider) <= minValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) <= minValue))
                            if (backdrop is Parallax parallax3)
                                parallax3.DoFadeIn = false;
                        if ((!absoluteValue && session.GetSlider(linkedSlider) >= maxValue) || (absoluteValue && Math.Abs(session.GetSlider(linkedSlider)) >= maxValue))
                            if (backdrop is Parallax parallax4)
                                parallax4.DoFadeIn = true;
                        break;
                    case ValueType.Flag:
                        if (backdrop is Parallax parallax5)
                            parallax5.DoFadeIn = session.GetFlag(linkedFlag);
                        break;
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        if (backdrop is Parallax parallax6)
                            parallax6.DoFadeIn = fadeLerp >= 0.5f;
                        break;
                    default: // DirectValue
                        if (backdrop is Parallax parallax7)
                            parallax7.DoFadeIn = fadeIn;
                        break;
                }
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.FlipX = fadeLerp >= 0.5f;
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.FlipY = fadeLerp >= 0.5f;
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.InstantIn = fadeLerp >= 0.5f;
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.InstantOut = fadeLerp >= 0.5f;
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.LoopX = fadeLerp >= 0.5f;
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.LoopY = fadeLerp >= 0.5f;
                        break;
                    default: // DirectValue
                        backdrop.LoopY = loopY;
                        break;
                }
                break;
            case FieldToModify.Color: // Color
                switch (valueType)
                {
                    case ValueType.Counter: // Gradually changes the color from minColor to maxColor which correspond to minValue and maxValue each
                        float normalizedCounter;
                        if (absoluteValue)
                            normalizedCounter = MathHelper.Clamp((Math.Abs(session.GetCounter(linkedCounter)) - minValue) / (maxValue - minValue), 0f, 1f);
                        else
                            normalizedCounter = MathHelper.Clamp((session.GetCounter(linkedCounter) - minValue) / (maxValue - minValue), 0f, 1f);
                        backdrop.Color = Color.Lerp(minColor, maxColor, normalizedCounter);
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
                    case ValueType.FadeValue:
                        if (player == null) return;
                        float fadeLerp = GetPositionLerp(player, positionMode);
                        backdrop.Color = Color.Lerp(minColor, maxColor, fadeLerp);
                        break;
                    default: // DirectValue
                        backdrop.Color = color;
                        break;
                }
                break;
            case FieldToModify.Custom:
                if (string.IsNullOrEmpty(customFieldName))
                    break;
                object value = null;
                Type targetType = backdrop.GetType().
                    GetField(customFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase)?.FieldType ??
                    backdrop.GetType().GetProperty(customFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.PropertyType;
                if (targetType == null) break;
                float fadeAmount;
                switch (valueType)
                {
                    case ValueType.Counter:
                        float counterVal = session.GetCounter(linkedCounter);
                        if (absoluteValue) counterVal = Math.Abs(counterVal);
                        fadeAmount = MathHelper.Clamp((counterVal - minValue) / (maxValue - minValue), 0f, 1f);
                        value = targetType == typeof(Color)
                            ? (object)Color.Lerp(minColor, maxColor, fadeAmount)
                            : (object)MathHelper.Lerp(minValue, maxValue, fadeAmount);
                        break;

                    case ValueType.Slider:
                        float sliderVal = session.GetSlider(linkedSlider);
                        if (absoluteValue) sliderVal = Math.Abs(sliderVal);
                        fadeAmount = MathHelper.Clamp((sliderVal - minValue) / (maxValue - minValue), 0f, 1f);
                        value = targetType == typeof(Color)
                            ? (object)Color.Lerp(minColor, maxColor, fadeAmount)
                            : (object)MathHelper.Lerp(minValue, maxValue, fadeAmount);
                        break;

                    case ValueType.FadeValue:
                        if (player == null)
                            break;
                        fadeAmount = GetPositionLerp(player, positionMode);
                        value = targetType == typeof(Color)
                            ? (object)Color.Lerp(minColor, maxColor, fadeAmount)
                            : (object)MathHelper.Lerp(minValue, maxValue, fadeAmount);
                        break;

                    case ValueType.Flag:
                        value = session.GetFlag(linkedFlag) ? valueWhileTrue : valueWhileFalse;
                        break;

                    default: // DirectValue
                        value = TryConvertValue(customFieldValue, targetType) ?? alpha;
                        break;
                }

                SetCustomFieldValue(backdrop, customFieldName, value);
                break;
        }
    }

    private void SetCustomFieldValue(Backdrop backdrop, string fieldName, object rawValue)
    {
        Type type = backdrop.GetType();

        FieldInfo field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (field != null)
        {
            object converted = TryConvertValue(rawValue, field.FieldType);
            if (converted != null)
            {
                field.SetValue(backdrop, converted);
                return;
            }
        }
        PropertyInfo prop = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
        {
            object converted = TryConvertValue(rawValue, prop.PropertyType);
            if (converted != null)
            {
                prop.SetValue(backdrop, converted);
                return;
            }
        }
        Logger.Log(LogLevel.Warn, "KoseiHelper", $"Could not set field or property {fieldName} on {type.Name}. Value type: {rawValue?.GetType()?.Name ?? "null"}");
    }

    private object TryConvertValue(object rawValue, Type targetType)
    {
        try
        {
            if (targetType == typeof(float))
                return Convert.ToSingle(rawValue);
            if (targetType == typeof(int))
                return Convert.ToInt32(rawValue);
            if (targetType == typeof(bool))
            {
                if (rawValue is bool b) return b;
                if (rawValue is float f) return f >= 0.5f;
                if (rawValue is int i) return i != 0;
                if (rawValue is string s) return bool.TryParse(s, out bool parsed) && parsed;
            }

            if (targetType == typeof(string))
                return rawValue.ToString();
            if (targetType == typeof(Color))
            {
                if (rawValue is Color c)
                    return c;
                if (rawValue is string s)
                    return KoseiHelperUtils.ParseHexColor(s, Color.White);
            }
            if (targetType.IsAssignableFrom(rawValue.GetType()))
                return rawValue;
            return Convert.ChangeType(rawValue, targetType);
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Warn, "KoseiHelper", $"Could not convert value \"{rawValue}\" ({rawValue?.GetType()?.Name ?? "null"}) to {targetType.Name}: {e.Message}");
            return null;
        }
    }


    private void Remove()
    {
        if (onlyOnce)
            RemoveSelf();
    }
}