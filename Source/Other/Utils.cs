using Microsoft.Xna.Framework;
using System;
using System.Xml;
using System.Xml.Linq;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperUtils
{
    public static Color ParseHexColor(string hex, Color defaultColor)
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
    public static Color ParseHexColorWithAlpha(XmlAttributeCollection xml, string attrName, Color defaultColor)
    {
        string hex = xml[attrName]?.Value;
        return ParseHexColor(hex, defaultColor);
    }
}