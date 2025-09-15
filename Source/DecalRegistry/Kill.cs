using Celeste.Mod.Registry.DecalRegistryHandlers;
using Microsoft.Xna.Framework;
using Monocle;
using System.Xml;

namespace Celeste.Mod.KoseiHelper.DecalRegistry;

internal class KillDecalRegistryHandler : DecalRegistryHandler
{
    private float offsetX, offsetY;
    private float width, height;
    private string flag;
    public override string Name => "koseihelper.kill";

    public override void Parse(XmlAttributeCollection xml)
    {
        offsetX = Get(xml, "offsetX", 0f);
        offsetY = Get(xml, "offsetY", 0f);
        width = Get(xml, "width", 8f);
        height = Get(xml, "height", 8f);
        flag = Get(xml, "flag", "");
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new KillComponent(offsetX, offsetY, width, height, flag));
    }

    internal class KillComponent(float offsetX, float offsetY, float width, float height, string flag) : Component(active: true, visible: false)
    {
        public override void EntityAwake()
        {
            base.EntityAwake();
            Decal decal = (Decal)Entity;
            float x = offsetX, y = offsetY, w = width, h = height;
            decal.ScaleRectangle(ref x, ref y, ref w, ref h);
            Vector2 position = decal.Position + new Vector2(x, y);
            decal.Scene.Add(new KillDecal(position, w, h, flag));
        }

        private class KillDecal : Entity
        {
            private readonly string flag;
            public KillDecal(Vector2 position, float width, float height, string flag)
            {
                Position = position;
                Collider = new Hitbox(width, height);
                this.flag = flag;
                Add(new PlayerCollider(OnPlayer));
            }

            private void OnPlayer(Player player)
            {
                if (!player.Dead && (string.IsNullOrEmpty(flag) || player.SceneAs<Level>().Session.GetFlag(flag)))
                {
                    player.Die((player.Center - Center).SafeNormalize());
                    RemoveSelf();
                }
            }
        }
    }
}