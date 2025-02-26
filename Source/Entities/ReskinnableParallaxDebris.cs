using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/ReskinnableParallaxDebris")]
public class ReskinnableParallaxDebris : Entity
{

    private Vector2 start;
    private float parallax;
    public string texture;
    public int Depth;
    public float sineMult, customParallax;
    public Color tint;
    public enum Direction
    {
        Horizontal,
        Vertical,
        DiagonalUL_DR,
        DiagonalUR_DL,
        None
    };
    public Direction direction;

    public ReskinnableParallaxDebris(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        start = Position;
        base.Depth = data.Int("depth", -999900);
        texture = data.Attr("texture","scenery/fgdebris/KoseiHelper/rockA");
        sineMult = data.Float("sine", 2f);
        tint = data.HexColor("tint", Color.White);
        direction = data.Enum("direction", Direction.Vertical);
        customParallax = data.Float("parallax", 0.05f);
        List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(texture);
        atlasSubtextures.Reverse();
        foreach (MTexture item in atlasSubtextures)
        {
            Image img = new Image(item);
            img.CenterOrigin();
            Add(img);
            img.Color = tint;
            SineWave sine = new SineWave(0.4f, 0f);
            sine.Randomize();
            sine.OnUpdate = (float f) =>
            {
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
