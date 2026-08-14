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
    private bool clearNextFrame;
    public float currentInk, spentInk, maxInk, regenerationRate;
    private int surfaceSoundIndex;
    public string flag;
    public int inkDepth;
    private EventInstance drawingSound;
    public const float cooldown = 1f;
    public float regenerationCooldown = cooldown;
    public bool recoverInkUponShattering;
    private bool renderCursor = true;

    public MagicInkController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        timeToLive = data.Float("timeToLive", 3f);
        maxInk = data.Float("maxInk", 300f);
        regenerationRate = data.Float("regenerationRate", 20f);
        thickness = data.Int("thickness", 8);
        surfaceSoundIndex = data.Int("surfaceSoundIndex", 32);
        renderCursor = data.Bool("renderCursor", true);
        flag = data.Attr("flag", "");
        inkDepth = data.Int("depth", 1);
        recoverInkUponShattering = data.Bool("recoverInkUponShattering", true);
        currentInk = maxInk;
        base.AddTag(Tags.TransitionUpdate);
        base.AddTag(Tags.Persistent);
        if (data.Bool("global", false))
            base.AddTag(Tags.Global);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        if (KoseiHelperUtils.CheckFlag(scene as Level, flag))
            scene.Add(new InkDisplay(renderCursor));
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
            level.Add(new InkDisplay(renderCursor));

        if (!MInput.Mouse.CheckLeftButton && regenerationCooldown == 0f)
        {
            float maxAvailable = Math.Max(0f, maxInk - spentInk);
            if (currentInk < maxAvailable)
                currentInk = Math.Min(maxAvailable, currentInk + regenerationRate * Engine.DeltaTime);
        }
        if (regenerationCooldown > 0f)
            regenerationCooldown -= Engine.DeltaTime;
        if (regenerationCooldown < 0f)
            regenerationCooldown = 0f;
        Vector2 mouse = MInput.Mouse.Position;
        // zoomout compatibility
        float scaleX = (float)(level.Camera.Viewport.Width) / 1920f;
        float scaleY = (float)(level.Camera.Viewport.Height) / 1080f;
        Vector2 screenMouse = level.Camera.Position + new Vector2(mouse.X * scaleX, mouse.Y * scaleY);

        if (MInput.Mouse.PressedLeftButton)
        {
            currentStroke.Clear();
            lastMouse = screenMouse;
        }

        if (MInput.Mouse.CheckLeftButton)
        {
            if (currentInk <= 0f)
            {
                if (Audio.IsPlaying(drawingSound))
                    Audio.Stop(drawingSound);
            }
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
                        else
                            Audio.Position(drawingSound, end);
                        PaintStroke stroke = new(lastMouse.Value, end, Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f), thickness);
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

    private void SpawnInk(Level level, PaintStroke stroke, Entity paintOwner = null)
    {
        List<PaintStroke> lines = new() { stroke };
        ColliderList collider = MagicUtils.BuildCollider(lines);
        PaintDecal decal = new PaintDecal(Vector2.Zero, lines, inkDepth, timeToLive);
        PaintBarrier barrier = new PaintBarrier(Vector2.Zero, collider, decal, timeToLive, surfaceSoundIndex, Vector2.Distance(stroke.From, stroke.To), paintOwner);
        level.Add(decal);
        level.Add(barrier);
        regenerationCooldown = cooldown;
    }

    /// <summary>
    /// Use this method to recover some of your ink. Negative values should go in DrainInk(amount) instead so the spentInk is untouched.
    /// </summary>
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

    /// <summary>
    /// Use this method for entities that absorb your ink.
    /// </summary>
    public void DrainInk(float amount)
    {
        amount = Math.Max(0f, amount);
        currentInk -= Math.Min(currentInk, amount);
        regenerationCooldown = cooldown;
    }

    /// <summary>
    /// Used for entities that spawn paint, like the Magic Ink Box.
    /// It also keeps the rainbow in case the controller's flag becomes inactive.
    /// </summary>
    public bool TrySpawnPaint(Vector2 from, Vector2 to, Entity paintOwner = null)
    {
        if (IsInPreventionArea(from) || IsInPreventionArea(to))
            return false;
        float cost = Vector2.Distance(from, to);
        if (currentInk < cost)
            return false;
        currentInk -= cost;
        spentInk += cost;
        regenerationCooldown = cooldown;
        Color color = Calc.HsvToColor((SceneAs<Level>().TimeActive * 0.25f) % 1f, 1f, 1f);
        PaintStroke stroke = new(from, to, color, thickness);
        SpawnInk(SceneAs<Level>(), stroke, paintOwner);
        return true;
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
    private bool renderCursor;
    public InkDisplay(bool renderCursor)
    {
        this.renderCursor = renderCursor;
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

        if (renderCursor)
        {
            Image image = new Image(GFX.Gui["dot_outline"]);
            image.Color = Calc.HsvToColor((level.TimeActive * 0.25f) % 1f, 1f, 1f);
            image.Position = new Vector2(MInput.Mouse.Position.X * ((float)level.Camera.Viewport.Width / 1920f) * 6f * (320f / (float)level.Camera.Viewport.Width) - image.Width / 2f,
                MInput.Mouse.Position.Y * ((float)level.Camera.Viewport.Height / 1080f) * 6f * (180f / (float)level.Camera.Viewport.Height) - image.Height / 2f);
            image.Render();
        }
    }
}