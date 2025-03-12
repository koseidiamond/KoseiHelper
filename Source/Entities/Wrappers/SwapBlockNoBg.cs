using System;
using System.Runtime.CompilerServices;
using Celeste;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[Tracked(false)]
public class SwapBlockNoBg : Solid
{
    public enum Themes
    {
        Normal,
        Moon
    }

    private class PathRendererNoBg : Entity
    {
        private SwapBlockNoBg block;
        private MTexture pathTexture;
        private MTexture clipTexture = new MTexture();

        private float timer;

        public PathRendererNoBg(SwapBlockNoBg block)
            : base(block.Position)
        {
            this.block = block;
            base.Depth = 8999;
            pathTexture = GFX.Game["objects/KoseiHelper/Other/invisible"];
            timer = Calc.Random.NextFloat();
        }

        public override void Update()
        {
            base.Update();
            timer += Engine.DeltaTime * 4f;
        }

        public override void Render()
        {
            if (block.Theme != Themes.Moon)
            {
                for (int i = block.moveRect.Left; i < block.moveRect.Right; i += pathTexture.Width)
                {
                    for (int j = block.moveRect.Top; j < block.moveRect.Bottom; j += pathTexture.Height)
                    {
                        pathTexture.GetSubtexture(0, 0, Math.Min(pathTexture.Width, block.moveRect.Right - i), Math.Min(pathTexture.Height, block.moveRect.Bottom - j), clipTexture);
                        clipTexture.DrawCentered(new Vector2(i + clipTexture.Width / 2, j + clipTexture.Height / 2), Color.White);
                    }
                }
            }
            float num = 0.5f * (0.5f + ((float)Math.Sin(timer) + 1f) * 0.25f);
            block.DrawBlockStyle(new Vector2(block.moveRect.X, block.moveRect.Y), block.moveRect.Width, block.moveRect.Height, block.nineSliceTarget, null, Color.White * num);
        }
    }

    public static ParticleType P_Move;
    private const float ReturnTime = 0.8f;
    public Vector2 Direction;
    public bool Swapping;
    public Themes Theme;
    private Vector2 start;
    private Vector2 end;
    private float lerp;
    private int target;
    private Rectangle moveRect;
    private float speed;
    private float maxForwardSpeed;
    private float maxBackwardSpeed;
    private float returnTimer;
    private float redAlpha = 1f;
    private MTexture[,] nineSliceGreen;
    private MTexture[,] nineSliceRed;
    private MTexture[,] nineSliceTarget;
    private Sprite middleGreen;
    private Sprite middleRed;
    private PathRendererNoBg path;
    private EventInstance moveSfx;
    private EventInstance returnSfx;
    private DisplacementRenderer.Burst burst;
    private float particlesRemainder;

    public SwapBlockNoBg(Vector2 position, float width, float height, Vector2 node, Themes theme)
        : base(position, width, height, safe: false)
    {
        Theme = theme;
        start = Position;
        end = node;
        maxForwardSpeed = 360f / Vector2.Distance(start, end);
        maxBackwardSpeed = maxForwardSpeed * 0.4f;
        Direction.X = Math.Sign(end.X - start.X);
        Direction.Y = Math.Sign(end.Y - start.Y);
        Add(new DashListener
        {
            OnDash = OnDash
        });
        int num = (int)MathHelper.Min(base.X, node.X);
        int num2 = (int)MathHelper.Min(base.Y, node.Y);
        int num3 = (int)MathHelper.Max(base.X + base.Width, node.X + base.Width);
        int num4 = (int)MathHelper.Max(base.Y + base.Height, node.Y + base.Height);
        moveRect = new Rectangle(num, num2, num3 - num, num4 - num2);
        MTexture mTexture;
        MTexture mTexture2;
        MTexture mTexture3;
        if (Theme == Themes.Moon)
        {
            mTexture = GFX.Game["objects/swapblock/moon/block"];
            mTexture2 = GFX.Game["objects/swapblock/moon/blockRed"];
            mTexture3 = GFX.Game["objects/swapblock/moon/target"];
        }
        else
        {
            mTexture = GFX.Game["objects/swapblock/block"];
            mTexture2 = GFX.Game["objects/swapblock/blockRed"];
            mTexture3 = GFX.Game["objects/swapblock/target"];
        }
        nineSliceGreen = new MTexture[3, 3];
        nineSliceRed = new MTexture[3, 3];
        nineSliceTarget = new MTexture[3, 3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                nineSliceGreen[i, j] = mTexture.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                nineSliceRed[i, j] = mTexture2.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
                nineSliceTarget[i, j] = mTexture3.GetSubtexture(new Rectangle(i * 8, j * 8, 8, 8));
            }
        }
        if (Theme == Themes.Normal)
        {
            Add(middleGreen = GFX.SpriteBank.Create("swapBlockLight"));
            Add(middleRed = GFX.SpriteBank.Create("swapBlockLightRed"));
        }
        else if (Theme == Themes.Moon)
        {
            Add(middleGreen = GFX.SpriteBank.Create("swapBlockLightMoon"));
            Add(middleRed = GFX.SpriteBank.Create("swapBlockLightRedMoon"));
        }
        Add(new LightOcclude(0.2f));
        base.Depth = -9999;
    }

    public SwapBlockNoBg(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Width, data.Height, data.Nodes[0] + offset, data.Enum("theme", Themes.Normal))
    {
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        scene.Add(path = new PathRendererNoBg(this));
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        Audio.Stop(moveSfx);
        Audio.Stop(returnSfx);
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        Audio.Stop(moveSfx);
        Audio.Stop(returnSfx);
    }

    private void OnDash(Vector2 direction)
    {
        Swapping = lerp < 1f;
        target = 1;
        returnTimer = 0.8f;
        burst = (base.Scene as Level).Displacement.AddBurst(base.Center, 0.2f, 0f, 16f);
        if (lerp >= 0.2f)
        {
            speed = maxForwardSpeed;
        }
        else
        {
            speed = MathHelper.Lerp(maxForwardSpeed * 0.333f, maxForwardSpeed, lerp / 0.2f);
        }
        Audio.Stop(returnSfx);
        Audio.Stop(moveSfx);
        if (!Swapping)
        {
            Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", base.Center);
        }
        else
        {
            moveSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_move", base.Center);
        }
    }

    public override void Update()
    {
        base.Update();
        if (returnTimer > 0f)
        {
            returnTimer -= Engine.DeltaTime;
            if (returnTimer <= 0f)
            {
                target = 0;
                speed = 0f;
                returnSfx = Audio.Play("event:/game/05_mirror_temple/swapblock_return", base.Center);
            }
        }
        if (burst != null)
        {
            burst.Position = base.Center;
        }
        redAlpha = Calc.Approach(redAlpha, (target != 1) ? 1 : 0, Engine.DeltaTime * 32f);
        if (target == 0 && lerp == 0f)
        {
            middleRed.SetAnimationFrame(0);
            middleGreen.SetAnimationFrame(0);
        }
        if (target == 1)
        {
            speed = Calc.Approach(speed, maxForwardSpeed, maxForwardSpeed / 0.2f * Engine.DeltaTime);
        }
        else
        {
            speed = Calc.Approach(speed, maxBackwardSpeed, maxBackwardSpeed / 1.5f * Engine.DeltaTime);
        }
        float num = lerp;
        lerp = Calc.Approach(lerp, target, speed * Engine.DeltaTime);
        if (lerp != num)
        {
            Vector2 liftSpeed = (end - start) * speed;
            Vector2 position = Position;
            if (target == 1)
            {
                liftSpeed = (end - start) * maxForwardSpeed;
            }
            if (lerp < num)
            {
                liftSpeed *= -1f;
            }
            if (target == 1 && base.Scene.OnInterval(0.02f))
            {
                MoveParticles(end - start);
            }
            MoveTo(Vector2.Lerp(start, end, lerp), liftSpeed);
            if (position != Position)
            {
                Audio.Position(moveSfx, base.Center);
                Audio.Position(returnSfx, base.Center);
                if (Position == start && target == 0)
                {
                    Audio.SetParameter(returnSfx, "end", 1f);
                    Audio.Play("event:/game/05_mirror_temple/swapblock_return_end", base.Center);
                }
                else if (Position == end && target == 1)
                {
                    Audio.Play("event:/game/05_mirror_temple/swapblock_move_end", base.Center);
                }
            }
        }
        if (Swapping && lerp >= 1f)
        {
            Swapping = false;
        }
        StopPlayerRunIntoAnimation = lerp <= 0f || lerp >= 1f;
    }

    private void MoveParticles(Vector2 normal)
    {
        Vector2 position;
        Vector2 positionRange;
        float num;
        if (normal.X > 0f)
        {
            position = base.CenterLeft;
            positionRange = Vector2.UnitY * (base.Height - 6f);
            num = Math.Max(2f, base.Height / 14f);
        }
        else if (normal.X < 0f)
        {
            position = base.CenterRight;
            positionRange = Vector2.UnitY * (base.Height - 6f);
            num = Math.Max(2f, base.Height / 14f);
        }
        else if (normal.Y > 0f)
        {
            position = base.TopCenter;
            positionRange = Vector2.UnitX * (base.Width - 6f);
            num = Math.Max(2f, base.Width / 14f);
        }
        else
        {
            position = base.BottomCenter;
            positionRange = Vector2.UnitX * (base.Width - 6f);
            num = Math.Max(2f, base.Width / 14f);
        }
        particlesRemainder += num;
        int num2 = (int)particlesRemainder;
        particlesRemainder -= num2;
        positionRange *= 0.5f;
    }

    public override void Render()
    {
        Vector2 vector = Position + base.Shake;
        if (lerp != (float)target && speed > 0f)
        {
            Vector2 vector2 = (end - start).SafeNormalize();
            if (target == 1)
            {
                vector2 *= -1f;
            }
            float num = speed / maxForwardSpeed;
            float num2 = 16f * num;
            for (int i = 2; (float)i < num2; i += 2)
            {
                DrawBlockStyle(vector + vector2 * i, base.Width, base.Height, nineSliceGreen, middleGreen, Color.White * (1f - (float)i / num2));
            }
        }
        if (redAlpha < 1f)
        {
            DrawBlockStyle(vector, base.Width, base.Height, nineSliceGreen, middleGreen, Color.White);
        }
        if (redAlpha > 0f)
        {
            DrawBlockStyle(vector, base.Width, base.Height, nineSliceRed, middleRed, Color.White * redAlpha);
        }
    }

    private void DrawBlockStyle(Vector2 pos, float width, float height, MTexture[,] ninSlice, Sprite middle, Color color)
    {
        int num = (int)(width / 8f);
        int num2 = (int)(height / 8f);
        ninSlice[0, 0].Draw(pos + new Vector2(0f, 0f), Vector2.Zero, color);
        ninSlice[2, 0].Draw(pos + new Vector2(width - 8f, 0f), Vector2.Zero, color);
        ninSlice[0, 2].Draw(pos + new Vector2(0f, height - 8f), Vector2.Zero, color);
        ninSlice[2, 2].Draw(pos + new Vector2(width - 8f, height - 8f), Vector2.Zero, color);
        for (int i = 1; i < num - 1; i++)
        {
            ninSlice[1, 0].Draw(pos + new Vector2(i * 8, 0f), Vector2.Zero, color);
            ninSlice[1, 2].Draw(pos + new Vector2(i * 8, height - 8f), Vector2.Zero, color);
        }
        for (int j = 1; j < num2 - 1; j++)
        {
            ninSlice[0, 1].Draw(pos + new Vector2(0f, j * 8), Vector2.Zero, color);
            ninSlice[2, 1].Draw(pos + new Vector2(width - 8f, j * 8), Vector2.Zero, color);
        }
        for (int k = 1; k < num - 1; k++)
        {
            for (int l = 1; l < num2 - 1; l++)
            {
                ninSlice[1, 1].Draw(pos + new Vector2(k, l) * 8f, Vector2.Zero, color);
            }
        }
        if (middle != null)
        {
            middle.Color = color;
            middle.RenderPosition = pos + new Vector2(width / 2f, height / 2f);
            middle.Render();
        }
    }
}