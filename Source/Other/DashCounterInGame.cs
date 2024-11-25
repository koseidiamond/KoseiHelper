using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DashCounterInGame")]
[Tracked]
public class DashCounterInGame : Entity // TODO
{
    /*public const float YOffset = 96f;
    private MTexture bg;
    public float drawLerp;
    private float dashUpdateTimer;
    private float dashWaitTimer;
    private int dashCounter;
    private List<EntityID> dashdata;

    public DashCounterInGame()
    {
        base.Y = 96f + YOffset;
        base.Depth = -101;
        base.Tag = (int)Tags.HUD | (int)Tags.Global | (int)Tags.PauseUpdate | (int)Tags.TransitionUpdate;
        bg = GFX.Gui["strawberryCountBG"];
    }
    public override void Added(Scene scene)
    {
        base.Added(scene);
        Add(dashCounter = new DashCounter(centeredX: false, dashdata.Count)); //TODO replace these
    }

    public override void Update()
    {
        // Logger.Log(LogLevel.Info, "KoseiHelper", dashCounter.Position.ToString());
        base.Update();
        Level level = base.Scene as Level;
        if (dashdata.Count > dashCounter.Amount && dashUpdateTimer <= 0f)
        {
            dashUpdateTimer = 0.4f;
        }
        if (dashdata.Count > dashCounter.Amount || dashUpdateTimer > 0f || dashWaitTimer > 0f || (level.Paused && level.PauseMainMenuOpen))
        {
            drawLerp = Calc.Approach(drawLerp, 1f, 1.2f * Engine.RawDeltaTime);
        }
        else
        {
            drawLerp = Calc.Approach(drawLerp, 0f, 2f * Engine.RawDeltaTime);
        }
        if (dashWaitTimer > 0f)
        {
            dashWaitTimer -= Engine.RawDeltaTime;
        }
        if (dashUpdateTimer > 0f && drawLerp == 1f)
        {
            dashUpdateTimer -= Engine.RawDeltaTime;
            if (dashUpdateTimer <= 0f)
            {
                if (dashCounter.Amount < dashdata.Count)
                {
                    dashCounter.Amount++;
                }
                dashWaitTimer = 2f;
                if (dashCounter.Amount < dashdata.Count)
                {
                    dashUpdateTimer = 0.3f;
                }
            }
        }
        if (Visible)
        {
            float num = 96f + YOffset;
            if (!level.TimerHidden)
            {
                if (Settings.Instance.SpeedrunClock == SpeedrunType.Chapter)
                {
                    num += 58f;
                }
                else if (Settings.Instance.SpeedrunClock == SpeedrunType.File)
                {
                    num += 78f;
                }
            }
            base.Y = Calc.Approach(base.Y, num, Engine.DeltaTime * 800f);
        }
        Visible = drawLerp > 0f;
    }

    public override void Render()
    {
        Vector2 vec = Vector2.Lerp(new Vector2(-bg.Width, base.Y), new Vector2(32f, base.Y), Ease.CubeOut(drawLerp)).Round();
        bg.DrawJustified(vec + new Vector2(-96f, 12f), new Vector2(0f, 0.5f));
        dashCounter.Position = vec + new Vector2(0f, 0f - base.Y);
        dashCounter.Render();
    }*/
}