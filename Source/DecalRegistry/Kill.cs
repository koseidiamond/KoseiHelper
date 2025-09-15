using Celeste.Mod.Registry.DecalRegistryHandlers;
using Microsoft.Xna.Framework;
using Monocle;
using System.Xml;

namespace Celeste.Mod.KoseiHelper.DecalRegistry;

internal class KillDecalRegistryHandler : DecalRegistryHandler
{
    private float offsetX, offsetY;
    private float width, height;

    public override string Name => "koseihelper.kill";

    public override void Parse(XmlAttributeCollection xml)
    {
        offsetX = Get(xml, "offsetX", 0f);
        offsetY = Get(xml, "offsetY", 0f);
        width = Get(xml, "width", 8f);
        height = Get(xml, "height", 8f);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new KillComponent(offsetX, offsetY, width, height));
    }

    internal class KillComponent(float offsetX, float offsetY, float width, float height) : Component(active: true, visible: false)
    {
        public override void EntityAwake()
        {
            base.EntityAwake();
            Decal decal = (Decal)Entity;
            float x = offsetX, y = offsetY, w = width, h = height;
            decal.ScaleRectangle(ref x, ref y, ref w, ref h);
            Vector2 position = decal.Position + new Vector2(x, y);
            decal.Scene.Add(new KillDecal(position, w, h));
        }

        private class KillDecal : Entity
        {
            public KillDecal(Vector2 position, float width, float height)
            {
                Position = position;
                Collider = new Hitbox(width, height);
                Add(new PlayerCollider(OnPlayer));
            }

            private void OnPlayer(Player player)
            {
                if (!player.Dead)
                {
                    player.Die((player.Center - Center).SafeNormalize());
                    RemoveSelf();
                }
            }
        }
    }
}