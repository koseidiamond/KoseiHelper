using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomCobweb")]
[Tracked]
public class CustomCobweb : Entity
{
    public Color color;
    public Color edgeColor;
    public float waveMultiplier;
    public float offshootMultiplier;
    public float edgeColorAlpha;
    public bool removeIfNotColliding;
    public float thickness;

    private Vector2 anchorA;
    private Vector2 anchorB;
    private List<Vector2> offshoots;
    private List<float> offshootEndings;
    private float waveTimer;

    public CustomCobweb(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = data.Int("depth", -1);
        color = data.HexColor("color", Calc.HexToColor("696a6a"));
        edgeColor = data.HexColor("edgeColor", Calc.HexToColor("0f0e17"));
        edgeColorAlpha = data.Float("edgeColorAlpha", 0.2f);
        waveMultiplier = data.Float("waveMultiplier", 1f);
        offshootMultiplier = data.Float("offshoots", 0.4f);
        removeIfNotColliding = data.Bool("removeIfNotColliding", true);
        thickness = data.Float("thickness", 1f);

        anchorA = (Position = data.Position + offset);
        anchorB = data.Nodes[0] + offset;
        Vector2[] nodes = data.Nodes;
        foreach (Vector2 vector in nodes)
        {
            if (offshoots == null)
            {
                offshoots = new List<Vector2>();
                offshootEndings = new List<float>();
            }
            else
            {
                offshoots.Add(vector + offset);
                offshootEndings.Add(0.3f + Calc.Random.NextFloat(offshootMultiplier));
            }
        }
        waveTimer = Calc.Random.NextFloat();
    }
    public override void Added(Scene scene)
    {
        base.Added(scene);
        edgeColor = Color.Lerp(color, edgeColor, edgeColorAlpha);
        if (removeIfNotColliding &&
            (!scene.CollideCheck<Solid>(new Rectangle((int)anchorA.X - 2, (int)anchorA.Y - 2, 4, 4)) || !scene.CollideCheck<Solid>(new Rectangle((int)anchorB.X - 2, (int)anchorB.Y - 2, 4, 4))))
        {
            RemoveSelf();
        }
        for (int i = 0; i < offshoots.Count; i++)
        {
            Vector2 vector = offshoots[i];
            if (!scene.CollideCheck<Solid>(new Rectangle((int)vector.X - 2, (int)vector.Y - 2, 4, 4)) && removeIfNotColliding)
            {
                offshoots.RemoveAt(i);
                offshootEndings.RemoveAt(i);
                i--;
            }
        }
    }

    public override void Update()
    {
        waveTimer += Engine.DeltaTime * waveMultiplier;
        base.Update();
    }

    public override void Render()
    {
        DrawCobweb(anchorA, anchorB, 12, drawOffshoots: true);
    }
    public void DrawCobweb(Vector2 a, Vector2 b, int steps, bool drawOffshoots)
    {
        SimpleCurve curve = new SimpleCurve(a, b, (a + b) / 2f + Vector2.UnitY * (8f + (float)Math.Sin(waveTimer) * 4f));
        if (drawOffshoots && offshoots != null)
        {
            for (int i = 0; i < offshoots.Count; i++)
            {
                DrawCobweb(offshoots[i], curve.GetPoint(offshootEndings[i]), 4, drawOffshoots: false);
            }
        }
        Vector2 vector = curve.Begin;
        if (CullHelper.IsCurveVisible(curve))
        {
            for (int j = 1; j <= steps; j++)
            {
                float percent = (float)j / (float)steps;
                Vector2 point = curve.GetPoint(percent);
                Draw.Line(vector, point, (j <= 2 || j >= steps - 1) ? edgeColor : color, thickness);
                vector = point + (vector - point).SafeNormalize();
            }
        }
    }
}