using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/ReskinnableParallaxDebris")]
public class ReskinnableParallaxDebris : Entity
{
    private Vector2 start;
    private float parallax;
    public string texture;
    public new int Depth;
    public float sineMult, customParallax;
    public Color tint;
    public float rotationSpeed;
    public float fadeSpeed, alphaMin, alphaMax, scale;

    public enum Direction
    {
        Horizontal,
        Vertical,
        DiagonalUL_DR,
        DiagonalUR_DL,
        None
    };
    public Direction direction;
    public float secondSineHeight, secondSineWidth, bounceSpeed, sineSpeed;

    public ReskinnableParallaxDebris(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        start = Position;
        base.Depth = data.Int("depth", -999900);
        texture = data.Attr("texture", "scenery/fgdebris/KoseiHelper/rockA");
        sineMult = data.Float("sine", 2f);
        scale = data.Float("scale", 1f);
        tint = data.HexColor("tint", Color.White);
        direction = data.Enum("direction", Direction.Vertical);
        customParallax = data.Float("parallax", 0.05f);
        rotationSpeed = data.Float("rotationSpeed", 0f);
        fadeSpeed = data.Float("fadeSpeed", 1f);
        alphaMin = data.Float("alphaMin", 0.2f);
        alphaMax = data.Float("alphaMax", 1f);
        sineSpeed = data.Float("sineSpeed", 1f);
        secondSineHeight = data.Float("bounceHeight", 0f);
        secondSineWidth = data.Float("bounceWidth", 0f);
        bounceSpeed = data.Float("bounceSpeed", 1f);
        List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(texture);
        atlasSubtextures.Reverse();
        foreach (MTexture item in atlasSubtextures)
        {
            Image img = new Image(item);
            img.CenterOrigin();
            Add(img);
            img.Color = tint;
            img.Scale = new Vector2(scale, scale);
            SineWave sine = new SineWave(sineSpeed / 2.5f, 0f);
            sine.Randomize();
            sine.OnUpdate = (float f) =>
            {
                img.Y += (float)Math.Sin(f * Math.PI * secondSineHeight * bounceSpeed);
                img.X += (float)Math.Sin(f * Math.PI * secondSineWidth * bounceSpeed);
                switch (direction)
                {
                    case Direction.Horizontal:
                        img.X = sine.Value * sineMult;
                        break;
                    case Direction.DiagonalUL_DR:
                        img.X = sine.Value * sineMult;
                        img.Y = sine.Value * sineMult;
                        break;
                    case Direction.DiagonalUR_DL:
                        img.X = -sine.Value * sineMult;
                        img.Y = sine.Value * sineMult;
                        break;
                    case Direction.None:
                        break;
                    default:
                        img.Y = sine.Value * sineMult;
                        break;
                }
                img.Rotation += (rotationSpeed / 10) * f;
                float alpha = alphaMin + (alphaMax - alphaMin) * (float)Math.Sin(f * fadeSpeed);
                img.Color.A = (byte)(MathHelper.Clamp(alpha, 0f, 1f) * 255);
            };
            Add(sine);
        }
        parallax = customParallax + Calc.Random.NextFloat(0.08f);
    }

    public override void Render()
    {
        Vector2 vector = SceneAs<Level>().Camera.Position + new Vector2(320f, 180f) / 2f - start;
        Vector2 position = Position;
        Position -= vector * parallax;
        base.Render();
        Position = position;
    }
}