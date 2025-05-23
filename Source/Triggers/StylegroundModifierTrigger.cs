using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/StylegroundModifierTrigger")]
public class StylegroundModifierTrigger : Trigger
{
    public bool onlyOnce;
    public TriggerMode triggerMode;
    public string slider;
    public float sliderFinal;
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

    public StylegroundModifierTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        onlyOnce = data.Bool("onlyOnce", false);
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        slider = data.Attr("slider", "");
        multiplier = data.Float("multiplier", 1f);

        identificationMode = data.Enum("identificationMode", IdentificationMode.Index);
        index = data.Int("index", 0);
        tag = data.Attr("tag", "");
        texture = data.Attr("texture", "");

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
                throw new System.Exception("The Texture feature is not implemented yet!");
                break;
            case IdentificationMode.Tag:
                throw new System.Exception("The Tag feature is not implemented yet!");
                break;
            default: // Index
                backdrop = level.Background.Backdrops[index];
                ModifyBackdrop(backdrop);
                break;
        }
    }

    private void ModifyBackdrop(Backdrop backdrop)
    {
        Session session = SceneAs<Level>().Session;
        if (!string.IsNullOrEmpty(slider))
            sliderFinal = session.GetSlider(slider) * multiplier;

        switch (fieldToModify)
        {
            case FieldToModify.Flag:
                backdrop.OnlyIfFlag = flag;
                break;
            case FieldToModify.NotFlag:
                backdrop.OnlyIfNotFlag = notFlag;
                break;
            case FieldToModify.PositionX:
                if (string.IsNullOrEmpty(slider))
                    backdrop.Position.X = positionX;
                else
                    backdrop.Position.X = sliderFinal;
                    break;
            case FieldToModify.PositionY:
                if (string.IsNullOrEmpty(slider))
                    backdrop.Position.Y = positionY;
                else
                    backdrop.Position.Y = sliderFinal;
                break;
            case FieldToModify.ScrollX:
                if (string.IsNullOrEmpty(slider))
                    backdrop.Scroll.X = scrollX;
                else
                    backdrop.Scroll.X = sliderFinal;
                    break;
            case FieldToModify.ScrollY:
                if (string.IsNullOrEmpty(slider))
                    backdrop.Scroll.Y = scrollY;
                else
                    backdrop.Scroll.Y = sliderFinal;
                    break;
            case FieldToModify.SpeedX:
                if (string.IsNullOrEmpty(slider))
                    backdrop.Speed.X = speedX;
                else
                    backdrop.Speed.X = sliderFinal;
                    break;
            case FieldToModify.Alpha:
                if (string.IsNullOrEmpty(slider))
                    backdrop.FadeAlphaMultiplier = alpha;
                else
                    backdrop.FadeAlphaMultiplier = sliderFinal;
                    break;
            case FieldToModify.FadeIn:
                throw new System.Exception("The FadeIn feature is not implemented yet!");
                break;
            case FieldToModify.FlipX:
                backdrop.FlipX = flipX;
                break;
            case FieldToModify.FlipY:
                backdrop.FlipY = flipY;
                break;
            case FieldToModify.InstantIn:
                backdrop.InstantIn = instantIn;
                break;
            case FieldToModify.InstantOut:
                backdrop.InstantOut = instantOut;
                break;
            case FieldToModify.LoopX:
                backdrop.LoopX = loopX;
                break;
            case FieldToModify.LoopY:
                backdrop.LoopY = loopY;
                break;
            default: // Color
                backdrop.Color = color;
                break;
        }
    }

    private void Remove()
    {
        if (onlyOnce)
            RemoveSelf();
    }
}