using Celeste.Mod.KoseiHelper.Apps;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper;

public static class MagicUtils
{
    public static ColliderList BuildCollider(List<PaintStroke> lines)
    {
        ColliderList list = new();
        foreach (PaintStroke line in lines)
        {
            float length = Vector2.Distance(line.From, line.To);
            int steps = Math.Max(1, (int)Math.Ceiling(length));
            for (int i = 0; i <= steps; i++)
            {
                Vector2 p = Vector2.Lerp(line.From, line.To, i / (float)steps);
                list.Add(new Hitbox(line.Thickness, line.Thickness, p.X - line.Thickness / 2f, p.Y - line.Thickness / 2f));
            }
        }
        return list;
    }
}