using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomGoldenBlock")]
[Tracked]
public class CustomGoldenBlock : Solid
{
    private MTexture[,] nineSlice;
    private Image berry;
    private float startY;
    private float yLerp;
    private float sinkTimer;
    private float renderLerp;
    private bool flag2;

    private float sinkOffset, startSinkingTimer;
    private string iconTexture, blockTexture;
    private int surfaceSoundIndex = 32;
    private new int depth = -10000;
    private bool occludesLight, drawOutline = true;
    private bool allBerryTypes;

    public CustomGoldenBlock(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, data.Bool("isSafe",false))
    {
        sinkOffset = data.Float("sinkOffset", 12f);
        startSinkingTimer = data.Float("startSinkingTimer", 0.1f);
        iconTexture = data.Attr("iconTexture", "collectables/goldberry/idle00");
        blockTexture = data.Attr("blockTexture", "objects/goldblock");
        surfaceSoundIndex = data.Int("surfaceSoundIndex", 32);
        depth = data.Int("depth", -10000);
        occludesLight = data.Bool("occludesLight", true);
        allBerryTypes = data.Bool("allBerryTypes", false);
        drawOutline = data.Bool("drawOutline", true);

        startY = Y;
        berry = new Image(GFX.Game[iconTexture]);
        berry.CenterOrigin();
        berry.Position = new Vector2(Width / 2f, Height / 2f);
        MTexture mTexture = GFX.Game[blockTexture];
        nineSlice = new MTexture[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                nineSlice[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
            }
        }
        Depth = depth;
        if (occludesLight)
            Add(new LightOcclude());
        Add(new MirrorSurface());
        SurfaceSoundIndex = surfaceSoundIndex;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        DisableStaticMovers();
        Visible = false;
        Collidable = false;
        renderLerp = 1f;
        bool flag = false;
        foreach (Strawberry item in scene.Entities.FindAll<Strawberry>())
        {
            if ((item.Golden && item.Follower.Leader != null) || (allBerryTypes && item.Follower.Leader != null))
            {
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            DestroyStaticMovers();
            RemoveSelf();
        }

    }

    public override void Update()
    {
        base.Update();
        if (!Visible)
        {
            Player entity = base.Scene.Tracker.GetEntity<Player>();
            if (entity != null && entity.X > base.X - 80f)
            {
                Visible = true;
                Collidable = true;
                renderLerp = 1f;
            }
        }
        if (Visible)
        {
            renderLerp = Calc.Approach(renderLerp, 0f, Engine.DeltaTime * 3f);
        }
        if (HasPlayerRider())
        {
            sinkTimer = startSinkingTimer;
        }
        else if (sinkTimer > 0f)
        {
            sinkTimer -= Engine.DeltaTime;
        }
        if (sinkTimer > 0f)
        {
            yLerp = Calc.Approach(yLerp, 1f, 1f * Engine.DeltaTime); // TODO test
        }
        else
        {
            yLerp = Calc.Approach(yLerp, 0f, 1f * Engine.DeltaTime);
        }
        float y = MathHelper.Lerp(startY, startY + sinkOffset, Ease.SineInOut(yLerp));
        MoveToY(y);
        if (renderLerp == 0f)
            EnableStaticMovers();
        else
            DisableStaticMovers();
    }

    private void DrawBlock(Vector2 offset, Color color)
    {
        float num = base.Collider.Width / 8f - 1f;
        float num2 = base.Collider.Height / 8f - 1f;
        for (int i = 0; (float)i <= num; i++)
        {
            for (int j = 0; (float)j <= num2; j++)
            {
                int num3 = (((float)i < num) ? Math.Min(i, 1) : 2);
                int num4 = (((float)j < num2) ? Math.Min(j, 1) : 2);
                nineSlice[num3, num4].Draw(Position + offset + base.Shake + new Vector2(i * 8, j * 8), Vector2.Zero, color);
            }
        }
    }

    public override void Render()
    {
        Level level = base.Scene as Level;
        Vector2 vector = new Vector2(0f, ((float)level.Bounds.Bottom - startY + 32f) * Ease.CubeIn(renderLerp));
        Vector2 position = Position;
        Position += vector;
        if (drawOutline)
        {
            DrawBlock(new Vector2(-1f, 0f), Color.Black);
            DrawBlock(new Vector2(1f, 0f), Color.Black);
            DrawBlock(new Vector2(0f, -1f), Color.Black);
            DrawBlock(new Vector2(0f, 1f), Color.Black);
        }
        DrawBlock(Vector2.Zero, Color.White);
        berry.Color = Color.White;
        berry.RenderPosition = base.Center;
        berry.Render();
        Position = position;
    }
}
