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
    public CustomWire(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = data.Int("depth");
        color = data.HexColor("color", Color.Olive);
        Vector2[] array = data.NodesOffset(offset);
        if (array.Length != 0)
        {
            node = array[0];
        }
        curve.Begin = Position;
        curve.End = node;
        //Tag = TagsExt.SubHUD;
    }

    public override void Render()
    {
        Level level = SceneAs<Level>();


        // For hd fun
        /*Camera cam = SceneAs<Level>().Camera;
        Vector2 displayPos = 6f * (this.Position - cam.Position) + 6f * new Vector2(12,12);
        Position = displayPos;*/


        Vector2 vector = new Vector2((float)Math.Sin(sineX + level.WindSineTimer * 2f), (float)Math.Sin(sineY + level.WindSineTimer * 2.8f)) * 8f * level.VisualWind;
        curve.Control = (curve.Begin + curve.End) / 2f + new Vector2(0f, 24f) + vector;
        if (IsVisible())
        {
            Vector2 start = curve.Begin;
            for (int i = 1; i <= 16; i++)
            {
                float percent = (float)i / 16f;
                Vector2 point = curve.GetPoint(percent);
                Draw.Line(start, point, color);
                start = point;
            }
        }
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        Draw.Line(curve.Begin, curve.End, Color.Red);
        //Draw.Line(Position, node, Color.Red);
    }

    private bool IsVisible()
    {
        return CullHelper.IsCurveVisible(curve, 2f);
    }
}