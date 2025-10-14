using Celeste.Mod.Registry.DecalRegistryHandlers;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Celeste.Mod.KoseiHelper.DecalRegistry;

internal class MovingDecalRegistryHandler : DecalRegistryHandler
{
    private float xSpeed, ySpeed;
    private bool collideWithSolids;
    private float offsetX, offsetY;
    private float width, height;
    private bool moveWithWind;
    private string flag, flipFlag;
    private bool playerDirection;

    public override string Name => "koseihelper.moving";

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
        flag = Get(xml, "flag", "");
        flipFlag = Get(xml, "flipFlag", "");
        playerDirection = Get(xml, "moveTowardsPlayer", false);
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new MovingDecalComponent(xSpeed, ySpeed, collideWithSolids, moveWithWind, flag, flipFlag, playerDirection));
        decal.Collider = new Hitbox(width, height, offsetX, offsetY);
    }

    private class MovingDecalComponent : Component
    {
        private readonly float xSpeed, ySpeed;
        private readonly bool collideWithSolids, moveWithWind, playerDirection;
        private readonly string flag, flipFlag;

        public MovingDecalComponent(float xSpeed, float ySpeed, bool collideWithSolids, bool moveWithWind, string flag, string flipFlag, bool playerDirection) :
            base(active: true, visible: false)
        {
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
            this.collideWithSolids = collideWithSolids;
            this.moveWithWind = moveWithWind;
            this.flag = flag;
            this.flipFlag = flipFlag;
            this.playerDirection = playerDirection;
        }

        public override void Update()
        {
            base.Update();
            if (Entity is not Decal decal || decal.Collider is not Hitbox hitbox)
                return;

            if (decal.Scene is not Level level)
                return;

            if (!string.IsNullOrEmpty(flag) && !level.Session.GetFlag(flag))
                return;

            Vector2 move = new(xSpeed, ySpeed);
            if (playerDirection)
            {
                Vector2 playerCenter = level.Tracker.GetNearestEntity<Player>(decal.Center)?.Center ?? Vector2.Zero;
                if (xSpeed != 0)
                {
                    if (Math.Abs(decal.Position.X - playerCenter.X) > 1f)
                    {
                        if (playerCenter.X < decal.Position.X)
                        {
                            move.X = -Math.Abs(xSpeed);
                        }
                        else if (playerCenter.X > decal.Position.X)
                        {
                            move.X = Math.Abs(xSpeed);
                        }
                    }
                    else
                        move.X = 0;
                }
                if (ySpeed != 0)
                {
                    if (Math.Abs(decal.Position.Y - playerCenter.Y) > 1f)
                    {
                        if (playerCenter.Y < decal.Position.Y)
                        {
                            move.Y = -Math.Abs(ySpeed);
                        }
                        else if (playerCenter.Y > decal.Position.Y)
                        {
                            move.Y = Math.Abs(ySpeed);
                        }
                    }
                    else
                        move.Y = 0;
                }
                Rectangle playerHitbox = level.Tracker.GetNearestEntity<Player>(decal.Center)?.Collider.Bounds ?? Rectangle.Empty;
                if (playerHitbox.Intersects(new Rectangle((int)(decal.Position.X + move.X + hitbox.Left),
                    (int)(decal.Position.Y + move.Y + hitbox.Top), (int)hitbox.Width, (int)hitbox.Height)))
                {
                    if (move.X < 0)
                    {
                        move.X = Math.Max(move.X, playerHitbox.Right - decal.Position.X);
                    }
                    else if (move.X > 0)
                    {
                        move.X = Math.Min(move.X, playerHitbox.Left - (decal.Position.X + hitbox.Width));
                    }
                    if (move.Y < 0)
                    {
                        move.Y = Math.Max(move.Y, playerHitbox.Bottom - decal.Position.Y);
                    }
                    else if (move.Y > 0)
                    {
                        move.Y = Math.Min(move.Y, playerHitbox.Top - (decal.Position.Y + hitbox.Height));
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(flipFlag) && level.Session.GetFlag(flipFlag))
                    move = new(-xSpeed, -ySpeed);
                else
                    move = new(xSpeed, ySpeed);
            }
            if (moveWithWind)
            {
                move.X = level.Wind.X * 0.00168f * move.X;
                move.Y = level.Wind.Y * 0.00168f * move.Y;
            }
            if (collideWithSolids)
            {
                if (move.X != 0f)
                {
                    Rectangle checkRect = new Rectangle((int)(decal.Position.X + move.X + hitbox.Left), (int)(decal.Position.Y + hitbox.Top),
                        (int)hitbox.Width, (int)hitbox.Height);
                    if (!decal.Scene.CollideCheck<Solid>(checkRect))
                        decal.Position.X += move.X;
                }
                if (move.Y != 0f)
                {
                    Rectangle checkRect = new Rectangle((int)(decal.Position.X + hitbox.Left), (int)(decal.Position.Y + move.Y + hitbox.Top),
                        (int)hitbox.Width, (int)hitbox.Height);
                    if (!decal.Scene.CollideCheck<Solid>(checkRect))
                        decal.Position.Y += move.Y;
                }
            }
            else
                decal.Position += move;
            // move the solid(s) "spawned" by the decal
            DynamicData data = DynamicData.For(decal);
            var solids = data.Get<List<Solid>>("solids");
            if (solids != null)
            {
                foreach (Solid solid in solids)
                {
                    solid.Position = decal.Position;
                }
            }
        }
    }
}