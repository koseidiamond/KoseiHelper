using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/NemesisZipMover")]
[Tracked]
public class NemesisZipMover : Solid
{
    public static ParticleType P_Scrape = ZipMover.P_Scrape;
    public static ParticleType P_Sparks = ZipMover.P_Sparks;
    public MTexture[,] edges = new MTexture[3, 3];
    public Sprite streetlight;
    public BloomPoint bloom;
    public NemesisZipMoverPathRenderer pathRenderer;
    public List<MTexture> innerCogs;
    public MTexture temp = new MTexture();
    public bool drawBlackBorder;
    public Vector2 start, target;
    public float percent;
    public static Color ropeColor = Calc.HexToColor("663931");
    public static Color ropeLightColor = Calc.HexToColor("9b6157");
    public SoundSource sfx = new SoundSource();
    public bool triggerable = true;
    public string sfxPath, blockPath, cogPath, innerCogPath, lightPath;
    public float normalSpeedMultiplier, retractionSpeedMultiplier;
    public float timeBeforeRetraction, timeAfterRetraction, timeBeforeMoving;
    public bool triggeredByPlayer;

    public NemesisZipMover(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, false)
    {
        base.Depth = -9999;
        target = data.Nodes[0] + offset;
        start = Position;
        Add(new LightOcclude());
        sfxPath = data.Attr("sfxPath", "event:/game/01_forsaken_city/zip_mover");
        lightPath = data.Attr("lightPath", "objects/zipmover/light");
        blockPath = data.Attr("blockPath", "objects/zipmover/block");
        innerCogPath = data.Attr("innerCogPath", "objects/zipmover/innercog");
        cogPath = data.Attr("cogPath", "objects/zipmover/cog");
        drawBlackBorder = data.Bool("drawBlackBorder", true);
        normalSpeedMultiplier = data.Float("normalSpeedMultiplier", 1f);
        retractionSpeedMultiplier = data.Float("retractionSpeedMultiplier", 1f);

        timeBeforeRetraction = data.Float("timeBeforeRetraction", 0.5f);
        timeAfterRetraction = data.Float("timeAfterRetraction", 0.5f);
        timeBeforeMoving = data.Float("timeBeforeMoving", 0.1f);

        ropeColor = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("ropeColor", out object c1) ? c1.ToString() : null, Calc.HexToColor("663931"));
        ropeLightColor = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("ropeLightColor", out object c2) ? c2.ToString() : null, Calc.HexToColor("9b6157"));
        triggeredByPlayer = data.Bool("triggeredByPlayer", false);

        innerCogs = GFX.Game.GetAtlasSubtextures(innerCogPath);
        Add(streetlight = new Sprite(GFX.Game, lightPath));
        streetlight.Add("frames", "", 1f);
        streetlight.Play("frames");
        streetlight.Active = false;
        streetlight.SetAnimationFrame(1);
        streetlight.Position = new Vector2(base.Width / 2f - streetlight.Width / 2f, 0f);
        Add(bloom = new BloomPoint(data.Float("bloomAlpha", 1f), data.Float("bloomRadius", 6f)));
        bloom.Position = new Vector2(base.Width / 2f, 4f);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
                edges[i, j] = GFX.Game[blockPath].GetSubtexture(i * 8, j * 8, 8, 8);
        }

        SurfaceSoundIndex = 7;
        sfx.Position = new Vector2(base.Width, base.Height) / 2f;
        Add(sfx);
        triggerable = true;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        scene.Add(pathRenderer = new NemesisZipMoverPathRenderer(this));
    }

    public override void Removed(Scene scene)
    {
        scene.Remove(pathRenderer);
        pathRenderer = null;
        base.Removed(scene);
    }

    public override void Update()
    {
        base.Update();
        bloom.Y = streetlight.CurrentAnimationFrame * 3;
        if (triggeredByPlayer)
        {
            if (triggerable && HasPlayerRider())
                Add(new Coroutine(Sequence()));
        }
    }

    public void ScrapeParticlesCheck(Vector2 to)
    {
        if (!base.Scene.OnInterval(0.03f))
            return;

        bool flag = to.Y != base.ExactPosition.Y;
        bool flag2 = to.X != base.ExactPosition.X;
        if (flag && !flag2)
        {
            int num = Math.Sign(to.Y - base.ExactPosition.Y);
            Vector2 vector = ((num != 1) ? base.TopLeft : base.BottomLeft);
            int num2 = 4;
            if (num == 1)
                num2 = Math.Min((int)base.Height - 12, 20);
            int num3 = (int)base.Height;
            if (num == -1)
                num3 = Math.Max(16, (int)base.Height - 16);

            if (base.Scene.CollideCheck<Solid>(vector + new Vector2(-2f, num * -2)))
            {
                for (int i = num2; i < num3; i += 8)
                    SceneAs<Level>().ParticlesFG.Emit(P_Scrape, base.TopLeft + new Vector2(0f, (float)i + (float)num * 2f), (num == 1) ? (-MathF.PI / 4f) : (MathF.PI / 4f));
            }

            if (base.Scene.CollideCheck<Solid>(vector + new Vector2(base.Width + 2f, num * -2)))
            {
                for (int j = num2; j < num3; j += 8)
                    SceneAs<Level>().ParticlesFG.Emit(P_Scrape, base.TopRight + new Vector2(-1f, (float)j + (float)num * 2f), (num == 1) ? (MathF.PI * -3f / 4f) : (MathF.PI * 3f / 4f));
            }
        }
        else
        {
            if (!flag2 || flag)
                return;

            int num4 = Math.Sign(to.X - base.ExactPosition.X);
            Vector2 vector2 = ((num4 != 1) ? base.TopLeft : base.TopRight);
            int num5 = 4;
            if (num4 == 1)
                num5 = Math.Min((int)base.Width - 12, 20);
            int num6 = (int)base.Width;
            if (num4 == -1)
                num6 = Math.Max(16, (int)base.Width - 16);

            if (base.Scene.CollideCheck<Solid>(vector2 + new Vector2(num4 * -2, -2f)))
            {
                for (int k = num5; k < num6; k += 8)
                    SceneAs<Level>().ParticlesFG.Emit(P_Scrape, base.TopLeft + new Vector2((float)k + (float)num4 * 2f, -1f), (num4 == 1) ? (MathF.PI * 3f / 4f) : (MathF.PI / 4f));
            }

            if (base.Scene.CollideCheck<Solid>(vector2 + new Vector2(num4 * -2, base.Height + 2f)))
            {
                for (int l = num5; l < num6; l += 8)
                    SceneAs<Level>().ParticlesFG.Emit(P_Scrape, base.BottomLeft + new Vector2((float)l + (float)num4 * 2f, 0f), (num4 == 1) ? (MathF.PI * -3f / 4f) : (-MathF.PI / 4f));
            }
        }
    }

    public IEnumerator Sequence()
    {
        triggerable = false;
        Vector2 start = Position;
        sfx.Play(sfxPath); // made small corrections to try to improve the sound if different speeds/timings are set
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Short);
        StartShaking(0.1f);
        yield return timeBeforeMoving;
        if (sfx?.instance != null && sfx.instance.getTimelinePosition(out int sfxImpact) == FMOD.RESULT.OK && sfxImpact > 490)
            sfx.instance.setTimelinePosition(600); // jumps to the "impact" label (ensuring at least most of the "touch" label has been played first)
        streetlight.SetAnimationFrame(3); // green
        StopPlayerRunIntoAnimation = false;
        float at2 = 0f;
        while (at2 < 1f)
        {
            yield return null;
            at2 = Calc.Approach(at2, 1f, 2f * Engine.DeltaTime * normalSpeedMultiplier);
            percent = Ease.SineIn(at2);
            Vector2 vector = Vector2.Lerp(start, target, percent);
            ScrapeParticlesCheck(vector);
            if (Scene.OnInterval(0.1f))
                pathRenderer.CreateSparks();
            MoveTo(vector);
        }
        StartShaking(0.2f);
        Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
        SceneAs<Level>().Shake();
        StopPlayerRunIntoAnimation = true;
        yield return timeBeforeRetraction;
        if (sfx?.instance != null && sfx.instance.getTimelinePosition(out int sfxReturn) == FMOD.RESULT.OK && sfxReturn > 950)
            sfx?.instance?.setTimelinePosition(1100); // jumps to the "return" label
        StopPlayerRunIntoAnimation = false;
        streetlight.SetAnimationFrame(2); // yellow
        at2 = 0f;
        while (at2 < 1f)
        {
            yield return null;
            at2 = Calc.Approach(at2, 1f, 0.5f * Engine.DeltaTime * retractionSpeedMultiplier);
            percent = 1f - Ease.SineIn(at2);
            Vector2 position = Vector2.Lerp(target, start, Ease.SineIn(at2));
            MoveTo(position);
        }

        StopPlayerRunIntoAnimation = true;
        StartShaking(0.2f);
        streetlight.SetAnimationFrame(1); // red
        yield return timeAfterRetraction;
        // jumps to the resetting label, but for some reason, vanilla events have different timings: "reset" and "but actually" labels
        if (sfx?.instance != null && sfx.instance.getTimelinePosition(out int sfxReset) == FMOD.RESULT.OK && sfxReset > 1500)
            sfx?.instance?.setTimelinePosition(sfxPath == "event:/new_content/game/10_farewell/zip_mover" ? 3116 : 3000);
        Remove(Get<Coroutine>());
        triggerable = true;
        yield return null;
    }

    public override void Render()
    {
        Vector2 position = Position;
        Position += base.Shake;
        Draw.Rect(base.X + 1f, base.Y + 1f, base.Width - 2f, base.Height - 2f, Color.Black);
        int num = 1;
        float num2 = 0f;
        int count = innerCogs.Count;
        for (int i = 4; (float)i <= base.Height - 4f; i += 8)
        {
            int num3 = num;
            for (int j = 4; (float)j <= base.Width - 4f; j += 8)
            {
                int index = (int)(((num2 + num * percent * MathF.PI * 4f) % (MathF.PI / 2f) + MathF.PI / 2f) % (MathF.PI / 2f) / (MathF.PI / 2f) * count);
                MTexture mTexture = innerCogs[index];
                Rectangle rectangle = new Rectangle(0, 0, mTexture.Width, mTexture.Height);
                Vector2 zero = Vector2.Zero;
                if (j <= 4)
                {
                    zero.X = 2f;
                    rectangle.X = 2;
                    rectangle.Width -= 2;
                }
                else if ((float)j >= base.Width - 4f)
                {
                    zero.X = -2f;
                    rectangle.Width -= 2;
                }
                if (i <= 4)
                {
                    zero.Y = 2f;
                    rectangle.Y = 2;
                    rectangle.Height -= 2;
                }
                else if ((float)i >= base.Height - 4f)
                {
                    zero.Y = -2f;
                    rectangle.Height -= 2;
                }

                mTexture = mTexture.GetSubtexture(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, temp);
                mTexture.DrawCentered(Position + new Vector2(j, i) + zero, Color.White * ((num < 0) ? 0.5f : 1f));
                num = -num;
                num2 += MathF.PI / 3f;
            }

            if (num3 == num)
                num = -num;
        }

        for (int k = 0; (float)k < base.Width / 8f; k++)
        {
            for (int l = 0; (float)l < base.Height / 8f; l++)
            {
                int num4 = ((k != 0) ? (((float)k != base.Width / 8f - 1f) ? 1 : 2) : 0);
                int num5 = ((l != 0) ? (((float)l != base.Height / 8f - 1f) ? 1 : 2) : 0);
                if (num4 != 1 || num5 != 1)
                {
                    edges[num4, num5].Draw(new Vector2(base.X + (float)(k * 8), base.Y + (float)(l * 8)));
                }
            }
        }

        base.Render();
        Position = position;
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        if (!triggerable)
            Draw.HollowRect(Collider.Bounds, Color.DarkRed);
    }

    public class NemesisZipMoverPathRenderer : Entity
    {
        public NemesisZipMover ZipMover;
        public MTexture cog;
        public Vector2 from, to;
        public Vector2 sparkAdd;
        public float sparkDirFromA, sparkDirFromB, sparkDirToA, sparkDirToB;

        public NemesisZipMoverPathRenderer(NemesisZipMover zipMover)
        {
            base.Depth = 5000;
            ZipMover = zipMover;
            from = ZipMover.start + new Vector2(ZipMover.Width / 2f, ZipMover.Height / 2f);
            to = ZipMover.target + new Vector2(ZipMover.Width / 2f, ZipMover.Height / 2f);
            sparkAdd = (from - to).SafeNormalize(5f).Perpendicular();
            float num = (from - to).Angle();
            sparkDirFromA = num + MathF.PI / 8f;
            sparkDirFromB = num - MathF.PI / 8f;
            sparkDirToA = num + MathF.PI - MathF.PI / 8f;
            sparkDirToB = num + MathF.PI + MathF.PI / 8f;
            cog = GFX.Game[zipMover.cogPath];
        }

        public void CreateSparks()
        {
            SceneAs<Level>().ParticlesBG.Emit(P_Sparks, from + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromA);
            SceneAs<Level>().ParticlesBG.Emit(P_Sparks, from - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirFromB);
            SceneAs<Level>().ParticlesBG.Emit(P_Sparks, to + sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToA);
            SceneAs<Level>().ParticlesBG.Emit(P_Sparks, to - sparkAdd + Calc.Random.Range(-Vector2.One, Vector2.One), sparkDirToB);
        }

        public override void Render()
        {
            DrawCogs(Vector2.UnitY, Color.Black);
            DrawCogs(Vector2.Zero);
            if (ZipMover.drawBlackBorder)
                Draw.Rect(new Rectangle((int)(ZipMover.X + ZipMover.Shake.X - 1f), (int)(ZipMover.Y + ZipMover.Shake.Y - 1f), (int)ZipMover.Width + 2, (int)ZipMover.Height + 2), Color.Black);
        }

        public void DrawCogs(Vector2 offset, Color? colorOverride = null)
        {
            Vector2 vector = (to - from).SafeNormalize();
            Vector2 vector2 = vector.Perpendicular() * 3f;
            Vector2 vector3 = -vector.Perpendicular() * 4f;
            float rotation = ZipMover.percent * MathF.PI * 2f;
            Draw.Line(from + vector2 + offset, to + vector2 + offset, colorOverride.HasValue ? colorOverride.Value : ropeColor);
            Draw.Line(from + vector3 + offset, to + vector3 + offset, colorOverride.HasValue ? colorOverride.Value : ropeColor);
            for (float num = 4f - ZipMover.percent * MathF.PI * 8f % 4f; num < (to - from).Length(); num += 4f)
            {
                Vector2 vector4 = from + vector2 + vector.Perpendicular() + vector * num;
                Vector2 vector5 = to + vector3 - vector * num;
                Draw.Line(vector4 + offset, vector4 + vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ropeLightColor);
                Draw.Line(vector5 + offset, vector5 - vector * 2f + offset, colorOverride.HasValue ? colorOverride.Value : ropeLightColor);
            }
            cog.DrawCentered(from + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
            cog.DrawCentered(to + offset, colorOverride.HasValue ? colorOverride.Value : Color.White, 1f, rotation);
        }
    }
}