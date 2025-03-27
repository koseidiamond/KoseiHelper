using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using Celeste.Mod.Helpers;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomWire")]
[Tracked]
public class CustomWire : Entity
{
    public float sineX, sineY;
    public SimpleCurve curve;
    public Color color;
    public Vector2 node;
    public bool affectedByWind;
    public int thickness;
    public CustomWire(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = data.Int("depth");
        color = data.HexColor("color", Color.Olive);
        affectedByWind = data.Bool("affectedByWind", true);
        thickness = data.Int("thickness", 1);
        Vector2[] array = data.NodesOffset(offset);
        if (array.Length != 0)
        {
            node = array[0];
        }
        curve.Begin = Position;
        curve.End = node;
    }

    public override void Render()
    {
        Level level = SceneAs<Level>();
        Vector2 vector = Vector2.Zero;
        if (affectedByWind)
        {
            if (level.Wind != Vector2.Zero)
                vector = new Vector2((float)Math.Sin(sineX + level.WindSineTimer * 2f), (float)Math.Sin(sineY + level.WindSineTimer * 2.8f)) * 8f * level.VisualWind / 100f;
            else
                vector = new Vector2((float)Math.Sin(sineX + level.WindSineTimer * 2f), (float)Math.Sin(sineY + level.WindSineTimer * 2.8f)) * 8f * (float)(Math.Sin(Engine.DeltaTime) + 1.0) / 2f;
        }
        else
            vector = new Vector2((float)Math.Sin(sineX + level.WindSineTimer * 2f), (float)Math.Sin(sineY + level.WindSineTimer * 2.8f)) * 8f * (float)(Math.Sin(Engine.DeltaTime) + 1.0) / 2f;
        
        curve.Control = (curve.Begin + curve.End) / 2f + new Vector2(0f, 24f) + vector;
        if (IsVisible())
        {
            Vector2 start = curve.Begin;
            for (int i = 1; i <= 16; i++)
            {
                float percent = (float)i / 16f;
                Vector2 point = curve.GetPoint(percent);
                Draw.Line(start, point, color, thickness);
                start = point;
            }
        }
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        Draw.Line(curve.Begin, curve.End, Color.Red);
    }

    private bool IsVisible()
    {
        return CullHelper.IsCurveVisible(curve, 2f);
    }
}