using System.Xml;
using Celeste.Mod.Registry.DecalRegistryHandlers;
using Microsoft.Xna.Framework;
using Monocle;

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

    internal class KillComponent : Component
    {
        public Decal decal => (Decal)base.Entity;

        public KillComponent(float offsetX, float offsetY, float width, float height) : base(active: true, visible: true)
        {
            //collider = new Hitbox(width, height, offsetX, offsetY);
            SceneAs<Level>().Add(new KillDecal(offsetX, offsetY, width, height));
        }

        public override void EntityAwake()
        {
            
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            base.EntityAwake();
        }

        public override void DebugRender(Camera camera)
        {
            Draw.Point(decal.Position, Color.Cyan);
            base.DebugRender(camera);
        }
    }

    internal class KillDecal : Entity
    {
        public KillDecal(float offsetX, float offsetY, float width, float height)
        {
        }

        public override void Update()
        {
            base.Update();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            if (player != null)
                KillPlayer(player);
        }

        private void KillPlayer(Player player)
        {
            {
                DeathEffect component = new DeathEffect(Player.NormalHairColor, base.Center - base.Position + new Vector2(-16, -32))
                {
                    OnEnd = delegate
                    {
                        this.RemoveSelf();
                    }
                };
                player.Add(component);
                player.Die(Vector2.Zero);
                RemoveSelf();
            }
        }
    }
}