using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MagicPreventionArea")]
[Tracked]
public class MagicPreventionArea : Entity
{
    public Color color;
    public ParticleType MagicParticles = new ParticleType(Player.P_Split)
    {
        Color = Color.Purple * 0.15f,
        Color2 = Calc.HexToColor("683aad") * 0.2f,
        SpeedMin = 5f,
        SpeedMax = 20f,
        SpeedMultiplier = 0.5f,
        Acceleration = new Vector2(0f, -10f),
        Friction = 20f,
        Direction = -MathF.PI / 2f,
        DirectionRange = MathF.PI / 6f,
        LifeMin = 0.5f,
        LifeMax = 1f,
        Size = 1.5f,
        SizeRange = 0.5f,
    };

    public MagicPreventionArea(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Collider = new Hitbox(data.Width, data.Height);
        color = Calc.HexToColor("deb887");
        Depth = 1;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        MagicInkController inkController = (scene as Level).Tracker.GetEntity<MagicInkController>();
        if (inkController != null)
            Depth = inkController.inkDepth;
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (Scene.OnInterval(0.12f))
        {
            level.ParticlesFG.Emit(MagicParticles, 1, new Vector2(X + Calc.Random.NextFloat(Width), Y + Calc.Random.NextFloat(Height)), Vector2.One);
        }
        if (Scene.OnInterval(0.4f))
        {
            Vector2 burstPosition = new Vector2(X + 8f + Calc.Random.NextFloat(Math.Max(0, Width - 16f)), Y + 8f + Calc.Random.NextFloat(Math.Max(0, Height - 16f)));
            SceneAs<Level>().Displacement.AddBurst(burstPosition, 0.5f, 8f, 16f, 0.35f);
        }
    }

    public override void Render()
    {
        base.Render();
        Color rainbow = Calc.HsvToColor((SceneAs<Level>().TimeActive * 0.25f) % 1f, 1f, 1f);
        Draw.Rect(X, Y, Width, Height, color * 0.2f);
        Draw.Rect(X - 1f, Y - 1f, Width + 2f, Height + 2f, color * 0.09f);
        Draw.HollowRect(X, Y, Width, Height, rainbow * 0.1f);
    }
}