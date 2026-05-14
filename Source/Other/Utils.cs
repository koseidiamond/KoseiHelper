using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Xml;
using static Celeste.Mod.DoonvHelper.Entities.CustomNPC;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperUtils
{
    public static Color ParseHexColor(string hex, Color defaultColor) // For entities
    {
        if (string.IsNullOrEmpty(hex))
            return defaultColor;
        if (hex.StartsWith("#"))
            hex = hex.Substring(1); //ignores that character
        try
        {
            if (hex.Length == 6)
            {
                byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                return Color.FromNonPremultiplied(r, g, b, 255);
            }
            else if (hex.Length == 8)
            {
                byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                byte a = Convert.ToByte(hex.Substring(6, 2), 16);
                return Color.FromNonPremultiplied(r, g, b, a);
            }
        }
        catch
        { // Default
        }
        return defaultColor;
    }
    public static Color ParseHexColorWithAlpha(XmlAttributeCollection xml, string attrName, Color defaultColor) // For xmls
    {
        string hex = xml[attrName]?.Value;
        return ParseHexColor(hex, defaultColor);
    }

    private const StringComparison Case = StringComparison.OrdinalIgnoreCase;
    public static readonly Ease.Easer HexIn = (float t) => t * t * t * t * t * t;
    public static readonly Ease.Easer HexOut = Ease.Invert(HexIn);
    public static readonly Ease.Easer HexInOut = Ease.Follow(HexIn, HexOut);
    public static readonly Ease.Easer SeptIn = (float t) => t * t * t * t * t * t * t;
    public static readonly Ease.Easer SeptOut = Ease.Invert(SeptIn);
    public static readonly Ease.Easer SeptInOut = Ease.Follow(SeptIn, SeptOut);
    public static readonly Ease.Easer OctIn = (float t) => t * t * t * t * t * t * t * t;
    public static readonly Ease.Easer OctOut = Ease.Invert(OctIn);
    public static readonly Ease.Easer OctInOut = Ease.Follow(OctIn, OctOut);
    public static readonly Ease.Easer NonIn = (float t) => t * t * t * t * t * t * t * t * t;
    public static readonly Ease.Easer NonOut = Ease.Invert(NonIn);
    public static readonly Ease.Easer NonInOut = Ease.Follow(NonIn, NonOut);
    public static readonly Ease.Easer DecIn = (float t) => t * t * t * t * t * t * t * t * t * t;
    public static readonly Ease.Easer DecOut = Ease.Invert(DecIn);
    public static readonly Ease.Easer DecInOut = Ease.Follow(DecIn, DecOut);
    public static readonly Ease.Easer Round = (float t) => (float)Math.Round(t);

    public static Ease.Easer Easer(string easer, Ease.Easer defaultValue = null)
    {
        if (string.IsNullOrWhiteSpace(easer)) return defaultValue;
        switch (easer.ToLower())
        {
            case "linear": return Ease.Linear;
            case "sinein": return Ease.SineIn;
            case "sineout": return Ease.SineOut;
            case "sineinout": return Ease.SineInOut;
            case "quadin": return Ease.QuadIn;
            case "quadout": return Ease.QuadOut;
            case "quadinout": return Ease.QuadInOut;
            case "cubein": return Ease.CubeIn;
            case "cubeout": return Ease.CubeOut;
            case "cubeinout": return Ease.CubeInOut;
            case "quintin": return Ease.QuintIn;
            case "quintout": return Ease.QuintOut;
            case "quintinout": return Ease.QuintInOut;
            case "expoin": return Ease.ExpoIn;
            case "expoout": return Ease.ExpoOut;
            case "expoinout": return Ease.ExpoInOut;
            case "backin": return Ease.BackIn;
            case "backout": return Ease.BackOut;
            case "backinout": return Ease.BackInOut;
            case "bigbackin": return Ease.BigBackIn;
            case "bigbackout": return Ease.BigBackOut;
            case "bigbackinout": return Ease.BigBackInOut;
            case "elasticin": return Ease.ElasticIn;
            case "elasticout": return Ease.ElasticOut;
            case "elasticinout": return Ease.ElasticInOut;
            case "bouncein": return Ease.BounceIn;
            case "bounceout": return Ease.BounceOut;
            case "bounceinout": return Ease.BounceInOut;
            case "hexin": return HexIn;
            case "hexout": return HexOut;
            case "hexinout": return HexInOut;
            case "septin": return SeptIn;
            case "septout": return SeptOut;
            case "septinout": return SeptInOut;
            case "octin": return OctIn;
            case "octout": return OctOut;
            case "octinout": return OctInOut;
            case "nonin": return NonIn;
            case "nonout": return NonOut;
            case "noninout": return NonInOut;
            case "decin": return DecIn;
            case "decout": return DecOut;
            case "decinout": return DecInOut;
            case "round": return Round;
        }
        Logger.Log(LogLevel.Warn, "KoseiHelper", $"Ease.Easer {easer} cannot be interpreted, using {defaultValue} instead");
        return defaultValue;
    }

    public static void PointBounce(Vector2 from, Player player, bool refillDash = true, bool refillStamina = true, bool releaseBooster = true,
        bool coyote = false)
    {
        if (player.StateMachine.State == 2)
        {
            player.StateMachine.State = 0;
        }

        if (player.StateMachine.State == 4 && player.CurrentBooster != null && releaseBooster)
        {
            player.CurrentBooster.PlayerReleased();
        }
        if (refillDash)
            player.RefillDash();
        if (refillStamina)
            player.RefillStamina();
        Vector2 vector = (player.Center - from).SafeNormalize();
        if (vector.Y > -0.2f && vector.Y <= 0.4f)
            vector.Y = -0.2f;

        player.Speed = vector * 220f;
        player.Speed.X *= 1.5f;
        if (Math.Abs(player.Speed.X) < 100f)
        {
            if (player.Speed.X == 0f)
                player.Speed.X = (float)(0 - player.Facing) * 100f;
            else
                player.Speed.X = (float)Math.Sign(player.Speed.X) * 100f;
        }
        if (coyote) // for funsies
            player.jumpGraceTimer = 0.15f;
    }
}