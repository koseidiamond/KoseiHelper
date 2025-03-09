using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DashLimiterController")]
[Tracked]
public class DashLimiterController : Entity
{
    public int count = 3;
    private float countCooldown = 0f;
    public bool demoDashOnly = false;
    public int indicatorsPerRow = 5;
    public string sound = "event:/none";
    public Color indicatorColor = Color.LightSlateGray;
    public bool persistent = false;
    private EntityID eid;
    private enum IndicatorShape
        {
        Circle,
        FilledCircle,
        Pixel,
        FilledSquare,
        HollowSquare,
        Number,
        None
        };
    private IndicatorShape indicatorShape;

    public DashLimiterController(EntityData data, Vector2 offset, EntityID eid) : base(data.Position + offset)
    {
        this.eid = eid;
        count = data.Int("count", 3);
        demoDashOnly = data.Bool("demoDashOnly", false);
        indicatorsPerRow = data.Int("indicatorsPerRow", 5);
        sound = data.Attr("sound", "event:/none");
        indicatorColor = data.HexColor("indicatorColor", Color.LightSlateGray);
        indicatorShape = data.Enum("indicatorShape", IndicatorShape.FilledCircle);
        persistent = data.Bool("persistent", false);
        Depth = -9999999;
        if (persistent)
            base.Tag = Tags.Global;
    }
    public override void Added(Scene scene)
    {
        Level level = SceneAs<Level>();
        if (level.Tracker.GetEntity<DashLimiterController>().count > 1)
            this.RemoveSelf();
        base.Added(scene);
        level.Session.SetCounter("KoseiHelper_RemainingDashes", count);
        if (persistent)
            level.Session.DoNotLoad.Add(eid);
    }

    public override void Update()
    {
        base.Update();
        if (Scene.Tracker.GetEntity<Player>() is not { } player)
        {
            SceneAs<Level>().Session.DoNotLoad.Remove(eid);
            return;
        }
        if ((!demoDashOnly && player.dashCooldownTimer > 0) || (demoDashOnly && player.demoDashed))
            Add(new Coroutine(spentADash()));
        if (countCooldown > 0f)
        {
            countCooldown -= Engine.DeltaTime;
            if (countCooldown < 0)
                countCooldown = 0;
        }
        if (count == 0)
            Add(new Coroutine(killPlayerRoutine()));
    }

    private IEnumerator killPlayerRoutine()
    {
        Logger.Debug(nameof(KoseiHelperModule), $"The player has spent all dashes. Goodbye!");
        yield return 0.01f;
        if (Scene.Tracker.GetEntity<Player>() is not { } player)
            yield break;
        player.Die(Vector2.Zero, true, true);
    }

    private IEnumerator spentADash()
    {
        if (Scene.Tracker.GetEntity<Player>() is not { } player)
            yield break;
        if (countCooldown == 0f)
        {
            count -= 1;
            SceneAs<Level>().Session.SetCounter("KoseiHelper_RemainingDashes", count);
            Audio.Play(sound, player.Center);
            countCooldown = 0.2f;
        }
        yield break;
    }

    public override void Render()
    {
        base.Render();
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (player != null)
        {
            indicatorsPerRow = 5;
            if (indicatorShape == IndicatorShape.Number)
                Draw.Text(Draw.DefaultFont, count.ToString(), SceneAs<Level>().Camera.Position + new Vector2(4, 14), indicatorColor);
            for (int i = 0; i < count; i++)
            {
                int column = i % indicatorsPerRow;
                int row = i / indicatorsPerRow;
                int circlesInRow = Math.Min(indicatorsPerRow, count - row * indicatorsPerRow);
                int totalWidth = circlesInRow * 4;
                float startX = player.TopCenter.X - totalWidth / 2;
                float xPos = startX + 4 * column;
                int yOffset = row * 4; // Move 4 pixels higher for each new row
                float yPos = player.TopCenter.Y - 9 - yOffset;
                switch (indicatorShape)
                {
                    case IndicatorShape.Circle:
                        Draw.Circle(xPos + 2, yPos, 2, indicatorColor, 2);
                        break;
                    case IndicatorShape.Pixel:
                        Draw.Point(new Vector2(xPos + 2, yPos), indicatorColor);
                        break;
                    case IndicatorShape.FilledSquare:
                        Draw.Rect(xPos + 2, yPos, 2, 2, indicatorColor);
                        break;
                    case IndicatorShape.HollowSquare:
                        Draw.HollowRect(xPos + 1, yPos, 3, 3, indicatorColor);
                        break;
                    case IndicatorShape.FilledCircle:
                        Draw.Circle(xPos + 2, yPos, 2, indicatorColor, 1);
                        break;
                    default: // None or Number
                        break;
                }
            }
        }
    }
}