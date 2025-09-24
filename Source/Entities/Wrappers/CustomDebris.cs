using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[Tracked]
[Pooled]
public class CustomDebris : Debris
{
    private Color tint = Color.White;
    private string customTexture = null;

    public CustomDebris SetTint(Color tint)
    {
        this.tint = tint;
        return this;
    }
    public CustomDebris SetTexture(string texturePath)
    {
        customTexture = texturePath;
        return this;
    }
    public new CustomDebris Init(Vector2 position, char tileType, bool playSound = true)
    {
        base.Init(position, tileType, playSound);
        if (!string.IsNullOrEmpty(customTexture))
        {
            if (GFX.Game.Has(customTexture))
            {
                Image img = Get<Image>();
                img.Texture = GFX.Game[customTexture];
                img.CenterOrigin();
            }
            else
            {
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Texture not found: {customTexture}");
            }
        }

        return this;
    }

    public new CustomDebris BlastFrom(Vector2 from)
    {
        base.BlastFrom(from);
        return this;
    }


    public override void Update()
    {
        base.Update();
        Image img = Get<Image>();
        img?.Color = new Color((img.Color.R * tint.R) / 255,(img.Color.G * tint.G) / 255,(img.Color.B * tint.B) / 255,(img.Color.A * tint.A) / 255);
    }
}