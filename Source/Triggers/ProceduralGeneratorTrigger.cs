using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Triggers
{
    [CustomEntity("KoseiHelper/ProceduralGeneratorTrigger")]
    public class ProceduralGeneratorTrigger : Trigger
    {
        private readonly float density;
        private readonly List<char> tileChars;
        private readonly float baseNoiseScale;
        private readonly float noiseThreshold;
        private readonly int octaves;
        private readonly float persistence;
        private readonly float lacunarity;
        private readonly Random rng;
        private readonly int[] permutation;
        private readonly int sizeX, sizeY;
        private readonly Rectangle tileBounds;

        public ProceduralGeneratorTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            density = data.Float("density", 0.7f);
            baseNoiseScale = data.Float("noiseScale", 0.01f);
            noiseThreshold = data.Float("noiseThreshold", 0.4f);
            octaves = data.Int("octaves", 3);
            persistence = data.Float("persistence", 0.6f);
            lacunarity = data.Float("lacunarity", 1.5f);
            sizeX = data.Int("sizeX", 100);
            sizeY = data.Int("sizeY", 100);
            tileBounds = new Rectangle(0, 0, sizeX, sizeY);
            string tileCharsStr = data.Attr("tileChars", "3n");
            tileChars = new List<char>(tileCharsStr.ToCharArray());
            rng = new Random();
            permutation = new int[512];
            int[] p = new int[256];
            for (int i = 0; i < 256; i++)
                p[i] = i;
            for (int i = 255; i > 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                int temp = p[i];
                p[i] = p[swapIndex];
                p[swapIndex] = temp;
            }
            for (int i = 0; i < 512; i++)
                permutation[i] = p[i & 255];
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            Level level = SceneAs<Level>();
            if (level == null)
                return;
            for (int x = tileBounds.X; x < tileBounds.X + tileBounds.Width; x++)
            {
                for (int y = tileBounds.Y; y < tileBounds.Y + tileBounds.Height; y++)
                {
                    if (level.SolidsData[x, y] != '0')
                        continue;
                    Vector2 localTilePos = new Vector2(x - tileBounds.X, y - tileBounds.Y);
                    float noiseVal = FractalPerlinNoise(x, y, baseNoiseScale, octaves, persistence, lacunarity);
                    float adjustedThreshold = noiseThreshold;
                    if (noiseVal > adjustedThreshold && rng.NextDouble() < density)
                    {
                        int index = (int)(noiseVal * tileChars.Count);
                        index = (int)MathHelper.Clamp(index, 0, tileChars.Count - 1);
                        char chosenTile = tileChars[index];
                        level.SolidsData[x, y] = chosenTile;
                    }
                }
            }
            PrintTileData(level, tileBounds);
        }


        private float FractalPerlinNoise(float x, float y, float baseScale, int octaves, float persistence, float lacunarity)
        {
            float amplitude = 1;
            float frequency = baseScale;
            float noiseHeight = 0;
            float maxAmplitude = 0;

            for (int i = 0; i < octaves; i++)
            {
                float noiseValue = PerlinNoise(x * frequency, y * frequency);
                noiseHeight += noiseValue * amplitude;

                maxAmplitude += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            noiseHeight /= maxAmplitude;
            return (noiseHeight + 1) * 0.5f;
        }

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10); //don't ask
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        private static float Grad(int hash, float x, float y)
        {
            int h = hash & 7;
            float u = h < 4 ? x : y;
            float v = h < 4 ? y : x;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        private float PerlinNoise(float x, float y)
        {
            int X = (int)MathF.Floor(x) & 255;
            int Y = (int)MathF.Floor(y) & 255;
            float xf = x - MathF.Floor(x);
            float yf = y - MathF.Floor(y);
            float u = Fade(xf);
            float v = Fade(yf);
            int aa = permutation[permutation[X] + Y];
            int ab = permutation[permutation[X] + Y + 1];
            int ba = permutation[permutation[X + 1] + Y];
            int bb = permutation[permutation[X + 1] + Y + 1];
            float x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
            float x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);
            return Lerp(x1, x2, v);
        }
        private static void PrintTileData(Level level, Rectangle tileBounds)
        {
            List<string> tileRows = new List<string>();

            for (int y = 0; y < tileBounds.Height; y++)
            {
                string row = "";
                for (int x = 0; x < tileBounds.Width; x++)
                {
                    char tile = level.SolidsData[tileBounds.X + x, tileBounds.Y + y];
                    row += tile;
                }
                tileRows.Add(row);
            }
            string tileData = string.Join("\\n", tileRows);
            string output = $@"
{{
    {{
        _fromLayer = ""tilesFg"",
        height = {tileBounds.Height},
        tiles = ""{tileData}"",
        width = {tileBounds.Width},
        x = {tileBounds.X},
        y = {tileBounds.Y}
    }}
}}";
            Logger.Log("KoseiHelperModule", output);
        }
    }
}