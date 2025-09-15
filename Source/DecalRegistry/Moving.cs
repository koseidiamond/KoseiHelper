using Celeste.Mod.Registry.DecalRegistryHandlers;
using Microsoft.Xna.Framework;
using Monocle;
using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace Celeste.Mod.KoseiHelper.DecalRegistry;

internal class MovingDecal : DecalRegistryHandler
{
    private float xSpeed, ySpeed;
    private bool collideWithSolids;
    private float offsetX, offsetY;
    private float width, height;
    private bool moveWithWind;

    public override string Name => "koseihelper.movingDecal";

    public override void Parse(XmlAttributeCollection xml)
    {
        xSpeed = Get(xml, "xSpeed", 0f);
        ySpeed = Get(xml, "ySpeed", 0f);
        collideWithSolids = Get(xml, "collideWithSolids", false);
        offsetX = Get(xml, "offsetX", 0f);
        offsetY = Get(xml, "offsetY", 0f);
        width = Get(xml, "width", 8f);
        height = Get(xml, "height", 8f);
        moveWithWind = Get(xml, "moveWithWind", false);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new MovingDecalComponent(xSpeed, ySpeed, collideWithSolids, moveWithWind));
        decal.Collider = new Hitbox(width, height, offsetX, offsetY);
    }

    private class MovingDecalComponent : Component
    {
        private readonly float xSpeed, ySpeed;
        private readonly bool collideWithSolids, moveWithWind;

        public MovingDecalComponent(float xSpeed, float ySpeed, bool collideWithSolids, bool moveWithWind) : base(active: true, visible: false)
        {
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
            this.collideWithSolids = collideWithSolids;
            this.moveWithWind = moveWithWind;
        }

        public override void Update()
        {
            base.Update();
            if (Entity is not Decal decal || decal.Collider is not Hitbox hitbox)
                return;

            Vector2 move = new(xSpeed, ySpeed);
            if (moveWithWind && decal.Scene is Level level)
            {
                move.X = level.Wind.X * 0.00168f * xSpeed;
                move.Y = level.Wind.Y * 0.00168f * ySpeed;
            }
            if (collideWithSolids)
            {
                if (move.X != 0f)
                {
                    Rectangle checkRect = new Rectangle((int)(decal.Position.X + move.X + hitbox.Left), (int)(decal.Position.Y + hitbox.Top), (int)hitbox.Width, (int)hitbox.Height);
                    if (!decal.Scene.CollideCheck<Solid>(checkRect))
                        decal.Position.X += move.X;
                }

                if (move.Y != 0f)
                {
                    Rectangle checkRect = new Rectangle((int)(decal.Position.X + hitbox.Left), (int)(decal.Position.Y + move.Y + hitbox.Top), (int)hitbox.Width, (int)hitbox.Height);
                    if (!decal.Scene.CollideCheck<Solid>(checkRect))
                        decal.Position.Y += move.Y;
                }
            }
            else
                decal.Position += move;
        }
    }
}