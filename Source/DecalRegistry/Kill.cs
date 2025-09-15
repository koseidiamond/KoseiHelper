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

    internal class KillComponent(float offsetX, float offsetY, float width, float height, string flag)
    : Component(active: true, visible: false)
    {
        private readonly Hitbox hitbox = new(width, height, offsetX, offsetY);
        private readonly string flag;

        public override void Update()
        {
            base.Update();

            if (Entity is not Decal decal || decal.Scene is not Level level)
                return;

            if (!string.IsNullOrEmpty(flag) && !level.Session.GetFlag(flag))
                return;

            Player player = level.Tracker.GetEntity<Player>();
            if (player == null || player.Dead)
                return;
            Rectangle killRect = new((int)(decal.X + hitbox.Left), (int)(decal.Y + hitbox.Top), (int)hitbox.Width, (int)hitbox.Height);
            if (player.CollideRect(killRect))
                player.Die((player.Center - decal.Center).SafeNormalize());
        }
    }
}