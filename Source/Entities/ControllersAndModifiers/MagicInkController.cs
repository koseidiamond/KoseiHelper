using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Apps;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MagicInkController")]
[Tracked]
public class MagicInkController : Entity
{
    public float timeToLive;
    private readonly List<PaintStroke> currentStroke = new();
    private Vector2? lastMouse;
    public int thickness;
    private Color CurrentColor;
    private bool clearNextFrame;
    public float currentInk, spentInk, maxInk, regenerationRate;
    private int surfaceSoundIndex;
    public string flag;
    public int inkDepth;
    private static EventInstance drawingSound;
    public const float cooldown = 1f;
    public float regenerationCooldown = cooldown;
    public bool recoverInkUponShattering;

    public MagicInkController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        timeToLive = data.Float("timeToLive", 3f);
        maxInk = data.Float("maxInk", 300f);
        regenerationRate = data.Float("regenerationRate", 60f);
        thickness = data.Int("thickness", 8);
        surfaceSoundIndex = data.Int("surfaceSoundIndex", 32);
        flag = data.Attr("flag", "");
        inkDepth = data.Int("depth", 1);
        recoverInkUponShattering = data.Bool("recoverInkUponShattering", true);
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
        if (!KoseiHelperUtils.CheckFlag(level, flag))
            return;
        if (level.Tracker.GetEntity<InkDisplay>() == null)
            level.Add(new InkDisplay());

        if (!MInput.Mouse.CheckLeftButton && regenerationCooldown == 0f)
        {
            float maxAvailable = Math.Max(0f, maxInk - spentInk);
            currentInk = Math.Min(maxAvailable, currentInk + regenerationRate * Engine.DeltaTime);
        }
        if (regenerationCooldown > 0f)
            regenerationCooldown -= Engine.DeltaTime;
        if (regenerationCooldown < 0f)
            regenerationCooldown = 0f;

        CurrentColor = Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f);
        Vector2 mouse = MInput.Mouse.Position;
        // zoomout compatibility
        float scaleX = (float)(level.Camera.Viewport.Width) / Engine.Graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
        float scaleY = (float)(level.Camera.Viewport.Height) / Engine.Graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
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
                        if (!Audio.IsPlaying(drawingSound))
                            drawingSound = Audio.Play("event:/KoseiHelper/magicDrawing", end);
                        PaintStroke stroke = new(lastMouse.Value, end, CurrentColor, thickness);
                        currentStroke.Add(stroke);
                        SpawnInk(level, stroke);
                        currentInk -= usableDistance;
                        spentInk += usableDistance;
                    }
                    lastMouse = end;
                }
            }
            else
            {
                lastMouse = screenMouse;
            }
        }

        if (clearNextFrame)
        {
            currentStroke.Clear();
            clearNextFrame = false;
        }


        if (MInput.Mouse.ReleasedLeftButton)
        {
            Audio.Stop(drawingSound);
            currentStroke.Clear();
            lastMouse = null;
            clearNextFrame = true;
        }
        if (Audio.IsPlaying(drawingSound))
        {
            Player player = level.Tracker.GetEntity<Player>();
            if (!MInput.Mouse.CheckLeftButton || player == null || player.Dead)
                Audio.Stop(drawingSound);
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

    private void SpawnInk(Level level, PaintStroke stroke)
    {
        List<PaintStroke> lines = new() { stroke };
        ColliderList collider = BuildCollider(lines);
        PaintDecal decal = new PaintDecal(Vector2.Zero, lines, inkDepth, timeToLive);
        PaintBarrier barrier = new PaintBarrier(Vector2.Zero, collider, decal, timeToLive, surfaceSoundIndex, Vector2.Distance(stroke.From, stroke.To));
        level.Add(decal);
        level.Add(barrier);
        regenerationCooldown = cooldown;
    }

    public void AddInk(float amount, bool canOverfill = false)
    {
        if (canOverfill)
        {
            float recovered = Math.Min(amount, spentInk);
            spentInk -= recovered;
            amount -= recovered;

            currentInk += recovered + amount;
        }
        else
        {
            float recovered = Math.Min(amount, spentInk);
            spentInk -= recovered;

            float maxAvailable = maxInk - spentInk;
            currentInk = Math.Min(currentInk + recovered + amount, maxAvailable);
        }
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
        Level level = SceneAs<Level>();
        MagicInkController controller = level.Tracker.GetEntity<MagicInkController>();
        if (controller == null)
            return;


        const int position = 20;
        int width = (int)(controller.maxInk / 2);
        const int height = 12;

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