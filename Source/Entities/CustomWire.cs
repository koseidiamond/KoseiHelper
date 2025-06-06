using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomWire")]
[Tracked]
public class CustomWire : Entity
{
    public float sineX, sineY, wobbliness;
    public SimpleCurve curve;
    public Color color;
    public Vector2 node;
    public bool affectedByWind, attachToSolid;
    public int thickness;
    public float alpha;
    public string flag;

    public CustomWire(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = data.Int("depth");
        color = data.HexColor("color", Color.Olive);
        alpha = data.Float("alpha", 1f);
        flag = data.Attr("flag", "");
        affectedByWind = data.Bool("affectedByWind", true);
        thickness = data.Int("thickness", 1);
        attachToSolid = data.Bool("attachToSolids", true);
        wobbliness = data.Float("wobbliness", 1f);

        Vector2[] array = data.NodesOffset(offset);
        node = array[0];
        if (attachToSolid)
        {
            Collider = new Hitbox(4, 4, -2, -2);
            Add(new StaticMover
            {
                SolidChecker = IsRiding,
                OnDestroy = base.RemoveSelf,
                OnMove = Move
            });
        }
        curve.Begin = Position;
        curve.End = node;
    }

    public override void Update()
    {
        base.Update();
        if (SceneAs<Level>().Session.GetFlag(flag) || string.IsNullOrEmpty(flag))
            Visible = true;
        else
            Visible = false;
        if (attachToSolid)
        {
            curve.Begin = Position;
        }
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
        vector.Y *= wobbliness;
        curve.Control = (curve.Begin + curve.End) / 2f + new Vector2(0f, 24f) + vector;
        if (CullHelper.IsCurveVisible(curve, 2f))
        {
            Vector2 start = curve.Begin;
            for (int i = 1; i <= 16; i++)
            {
                float percent = (float)i / 16f;
                Vector2 point = curve.GetPoint(percent);
                Draw.Line(start, point, color * alpha, thickness); // color can be multiplied by alpha
                start = point;
            }
        }
    }

    private bool IsRiding(Solid solid)
    {
        return CollideCheck(solid, Position);
    }

    private void Move(Vector2 amount)
    {
        Position += amount;
    }
}