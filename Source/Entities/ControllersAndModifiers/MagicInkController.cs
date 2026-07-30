using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Apps;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MagicInkController")]
[Tracked]
public class MagicInkController : Entity // TODO ADD DRAWING SOUND
{
    public float timeToLive;
    private readonly List<PaintStroke> currentStroke = new();
    private Vector2? lastMouse;
    public int thickness;
    private Color CurrentColor;
    private bool clearNextFrame;
    public float currentInk, maxInk, regenerationRate;
    private int surfaceSoundIndex;
    public string flag;
    public int inkDepth;


    public MagicInkController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        timeToLive = data.Float("timeToLive", 3f);
        maxInk = data.Float("maxInk", 300f);
        regenerationRate = data.Float("regenerationRate", 60f);
        thickness = data.Int("thickness", 8);
        surfaceSoundIndex = data.Int("surfaceSoundIndex", 32);
        flag = data.Attr("flag", "");
        inkDepth = data.Int("depth", 1);
        currentInk = maxInk;
        base.AddTag(Tags.TransitionUpdate);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        if (KoseiHelperUtils.CheckFlag(scene as Level, flag))
            scene.Add(new InkDisplay());
    }

    public override void Update()
    {
        base.Update();

        Level level = SceneAs<Level>();
        if (level == null) return;

        // add display if it wasn't added yet because of the flag
        if (!KoseiHelperUtils.CheckFlag(level, flag)) return;
        if (level.Tracker.GetEntity<InkDisplay>() == null)
            level.Add(new InkDisplay());

        if (!MInput.Mouse.CheckLeftButton && !(level.Tracker.CountEntities<PaintDecal>() > 0 || level.Tracker.CountEntities<PaintBarrier>() > 0) && currentInk < maxInk)
            currentInk = Math.Min(maxInk, currentInk + regenerationRate * Engine.DeltaTime);

        CurrentColor = Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f);
        Vector2 mouse = MInput.Mouse.Position;
        float scaleX = 320f / Engine.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
        float scaleY = 180f / Engine.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
        Vector2 screenMouse = level.Camera.Position + new Vector2(mouse.X * scaleX, mouse.Y * scaleY);

        if (MInput.Mouse.PressedLeftButton)
        {
            currentStroke.Clear();
            lastMouse = screenMouse;
        }

        if (MInput.Mouse.CheckLeftButton)
        {
            if (lastMouse.HasValue)
            {
                float distance = Vector2.Distance(lastMouse.Value, screenMouse);
                if (distance > 0f && currentInk > 0f)
                {
                    float usableDistance = Math.Min(distance, currentInk);
                    Vector2 end = Vector2.Lerp(lastMouse.Value, screenMouse, usableDistance / distance);

                    if (!IsInPreventionArea(lastMouse.Value) && !IsInPreventionArea(end))
                    {
                        currentStroke.Add(new PaintStroke(lastMouse.Value, end, CurrentColor, thickness));
                        currentInk -= usableDistance;
                    }

                    lastMouse = end;
                }

            }
            else
                lastMouse = screenMouse;
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
        PaintDecal decal = new PaintDecal(Vector2.Zero, lines, inkDepth, timeToLive);
        PaintBarrier barrier = new PaintBarrier(Vector2.Zero, collider, decal, timeToLive, surfaceSoundIndex);
        level.Add(decal);
        level.Add(barrier);
    }

    public void AddInk(float amount, bool canOverfill = false)
    {
        if (canOverfill)
            currentInk += amount;
        else
            currentInk = Math.Min(currentInk + amount, maxInk);
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

    private bool IsInPreventionArea(Vector2 position)
    {
        foreach (MagicPreventionArea area in Scene.Tracker.GetEntities<MagicPreventionArea>())
        {
            if (area.Collider != null && area.Collider.Collide(position))
                return true;
        }

        return false;
    }
}

[CustomEntity("KoseiHelper/InkDisplay")]
[Tracked]
public class InkDisplay : Entity
{
    // The reason for this class to exist is to render it in the SubHUD, unlike the ink which is not HUD
    public InkDisplay()
    {
        base.AddTag(Tags.TransitionUpdate);
        base.AddTag(TagsExt.SubHUD);
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        MagicInkController controller = level.Tracker.GetEntity<MagicInkController>();
        if (controller == null)
            return;

        if (!KoseiHelperUtils.CheckFlag(level, controller.flag))
            RemoveSelf();
    }

    public override void Render()
    {
        base.Render();
        const int position = 20;
        const int width = 160;
        const int height = 12;
        Level level = SceneAs<Level>();
        MagicInkController controller = level.Tracker.GetEntity<MagicInkController>();
        if (controller == null)
            return;

        float percent = controller.currentInk / controller.maxInk;
        float normalPercent = Math.Min(percent, 1f);
        float overfillPercent = Math.Max(percent - 1f, 0f);
        Draw.Rect(position, position, width, height, Color.Black);
        int filled = (int)(width * normalPercent);

        for (int i = 0; i < filled; i++)
        {
            Color c = Calc.HsvToColor(((i / (float)width) + SceneAs<Level>().TimeActive * 0.2f) % 1f, 1f, 1f);

            Draw.Rect(position + i, position, 1, height, c);
        }
        Draw.HollowRect(position, position, width, height, Color.White);

        // Overfill
        if (overfillPercent > 0f)
        {
            int overfillWidth = (int)(width * overfillPercent);

            for (int i = 0; i < overfillWidth; i++)
            {
                Color c = Calc.HsvToColor((((width + i) / (float)width) + SceneAs<Level>().TimeActive * 0.2f) % 1f, 1f, 1f);
                Draw.Rect(position + width + i, position, 1, height, c);
            }
            Draw.HollowRect(position + width, position, overfillWidth, height, Color.White);
        }
    }
}