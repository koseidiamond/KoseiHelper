using Celeste.Mod.Registry.DecalRegistryHandlers;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using System.Xml;

namespace Celeste.Mod.KoseiHelper.DecalRegistry;

internal class TrailDecalRegistryHandler : DecalRegistryHandler
{
    private float trailDuration;
    private float trailSpawnInterval;
    private Color trailColor;

    public override string Name => "koseihelper.trail";

    public override void Parse(XmlAttributeCollection xml)
    {
        trailDuration = Get(xml, "duration", 0.5f);
        trailSpawnInterval = Get(xml, "spawnInterval", 0.1f);
        trailColor = KoseiHelperUtils.ParseHexColorWithAlpha(xml, "color", Color.White * 0.4f);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new TrailDecalComponent(trailDuration, trailSpawnInterval, trailColor));
    }

    private class TrailDecalComponent(float trailDuration, float trailSpawnInterval, Color trailColor)
    : Component(active: true, visible: true)
    {
        private readonly float trailDuration = trailDuration;
        private readonly float trailSpawnInterval = trailSpawnInterval;
        private readonly Color trailColor = trailColor;
        private float trailTimer = 0f;
        private readonly List<DecalTrail> decalTrails = [];
        private Vector2 previousPosition;

        public override void Added(Entity entity)
        {
            base.Added(entity);
            if (entity is Decal decal)
                previousPosition = decal.Position;
        }

        public override void Update()
        {
            base.Update();
            if (Entity is not Decal decal)
                return;
            if (decal.Position != previousPosition)
            {
                trailTimer += Engine.DeltaTime;
                if (trailTimer >= trailSpawnInterval)
                {
                    trailTimer -= trailSpawnInterval;
                    MTexture currentTexture = decal.textures[(int)decal.frame];
                    decalTrails.Add(new DecalTrail(decal.Position - new Vector2(decal.Width, decal.Height), currentTexture, trailColor, trailDuration));
                }
                previousPosition = decal.Position;
            }
            for (int i = decalTrails.Count - 1; i >= 0; i--)
            {
                if (!decalTrails[i].Update())
                    decalTrails.RemoveAt(i);
            }
        }

        public override void Render()
        {
            base.Render();
            if (Entity is not Decal)
                return;
            foreach (DecalTrail decalTrail in decalTrails)
                decalTrail.Render();
        }

        private class DecalTrail(Vector2 position, MTexture texture, Color baseColor, float duration)
        {
            private float lifetime = 0f;
            public bool Update()
            {
                lifetime += Engine.DeltaTime;
                if (lifetime >= duration)
                    return false;

                float alpha = 1f - (lifetime / duration);
                alpha = Ease.CubeOut(alpha);
                byte alphaByte = (byte)(baseColor.A * alpha);
                baseColor.A = alphaByte;
                return true;
            }

            public void Render()
            {
                texture.Draw(position, Vector2.Zero, baseColor);
            }
        }
    }
}