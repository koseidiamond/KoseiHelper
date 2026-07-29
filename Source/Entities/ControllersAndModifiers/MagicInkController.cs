using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Apps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MagicInkController")]
public class MagicInkController : Entity
{
    public float timeToLive;
    private readonly List<PaintStroke> currentStroke = new();

    private Vector2? lastMouse;
    private const int thickness = 8;
    private Color CurrentColor;
    private bool clearNextFrame;

    public MagicInkController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        timeToLive = data.Float("timeToLive", 3f);
        base.AddTag(Tags.TransitionUpdate);
    }

    public override void Update()
    {
        base.Update();

        Level level = SceneAs<Level>();
        if (level == null) return;

        CurrentColor = Calc.HsvToColor((level.TimeActive * 0.2f) % 1f,1f, 1f);

        Vector2 mouse = MInput.Mouse.Position;
        float scaleX = 320f / Engine.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
        float scaleY = 180f / Engine.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        Vector2 worldMouse = level.Camera.Position + new Vector2(mouse.X * scaleX, mouse.Y * scaleY);

        if (MInput.Mouse.PressedLeftButton)
        {
            currentStroke.Clear();
            lastMouse = worldMouse;
        }
        if (MInput.Mouse.CheckLeftButton)
        {
            if (lastMouse.HasValue)
            {
                currentStroke.Add(new PaintStroke(lastMouse.Value, worldMouse, CurrentColor, thickness));
            }
            lastMouse = worldMouse;
        }

        if (clearNextFrame)
        {
            currentStroke.Clear();
            clearNextFrame = false;
        }

        if (MInput.Mouse.ReleasedLeftButton)
        {
            if (currentStroke.Count > 0)
            {
                SpawnInk(level);
                clearNextFrame = true; // this is to prevent some stupid blinking inbetween frames
            }
            lastMouse = null;
        }
    }

    public override void Render()
    {
        base.Render();
        foreach (PaintStroke line in currentStroke)
        {
            Draw.Line(line.From, line.To, line.Color, line.Thickness);
        }
    }

    private void SpawnInk(Level level)
    {
        List<PaintStroke> lines = new(currentStroke);
        ColliderList collider = BuildCollider(lines);
        level.Add(new PaintDecal(Vector2.Zero, lines, 1, timeToLive));
        level.Add(new PaintBarrier(Vector2.Zero, collider, timeToLive));
    }

    private ColliderList BuildCollider(List<PaintStroke> lines)
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