using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

public enum CameraDirection
{
    Up,
    Down,
    Left,
    Right
}

public enum AutoscrollerMode
{
    SafePush,
    PushNCrush,
    ImmediateDeath
}

[CustomEntity("KoseiHelper/AutoscrollerController")]
public class AutoscrollerController : Entity
{
    public string flag;
    public float speed = 60;
    public AutoscrollerMode autoscrollerMode = AutoscrollerMode.SafePush;
    private Vector2 target;
    public CameraDirection cameraDirection = CameraDirection.Right;
    private InvisibleBarrier lowerBound, upperBound, leftBound, rightBound;
    private bool addedBarrier;
    private float behindOffset, aheadOffset;
    public AutoscrollerController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        cameraDirection = data.Enum("cameraDirection", CameraDirection.Right);
        autoscrollerMode = data.Enum("pushMode", AutoscrollerMode.PushNCrush);
        speed = data.Float("speed", 60f);
        behindOffset = data.Float("behindOffset", 0f);
        aheadOffset = data.Float("aheadOffset", 0f);
        flag = data.Attr("flag", "");
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            target = new Vector2(player.Position.X, player.Position.Y - 90);
        }
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (!string.IsNullOrEmpty(flag) && !level.Session.GetFlag(flag) && player != null) // Resets stuff if flag becomes false
        {
            if (upperBound != null)
                upperBound.RemoveSelf();
            if (lowerBound != null)
                lowerBound.RemoveSelf();
            if (leftBound != null)
                leftBound.RemoveSelf();
            if (rightBound != null)
                rightBound.RemoveSelf();
            if (cameraDirection == CameraDirection.Right)
                target = new Vector2(player.CenterX - 16, player.Position.Y - 90);
            if (cameraDirection == CameraDirection.Left)
                target = new Vector2(player.CenterX + 16, player.Position.Y - 90);
            if (cameraDirection == CameraDirection.Up)
                target = new Vector2(player.Position.X, player.CenterY - 90);
            if (cameraDirection == CameraDirection.Down)
                target = new Vector2(player.Position.X, player.CenterY -90);
            player.CameraAnchorLerp = Vector2.Zero;
            addedBarrier = false;
        }
        if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag) && !string.IsNullOrEmpty(flag))
        {
            if (player != null && !player.JustRespawned && player.InControl)
            {
                switch (cameraDirection)
                {
                    case CameraDirection.Up:
                        if (player.CameraAnchor.Y > level.Bounds.Top)
                            target -= new Vector2(0, speed / 60);
                        player.CameraAnchor.Y = target.Y;
                        player.CameraAnchorLerp.Y = 1;
                        switch (autoscrollerMode)
                        {
                            case AutoscrollerMode.ImmediateDeath:
                                if (player.Position.Y > player.CameraAnchor.Y + 204 + behindOffset)
                                    player.Die(player.BottomCenter);
                                break;
                            case AutoscrollerMode.SafePush:
                                if (player.Position.Y > player.CameraAnchor.Y + 204 + behindOffset)
                                {
                                    player.Position.Y--;
                                    player.PointBounce(new Vector2(player.CenterX, player.Bottom));
                                    Audio.Play("event:/game/general/assist_screenbottom");
                                }
                                break;
                            default: // PushNCrush
                                if (!addedBarrier)
                                {
                                    Scene.Add(lowerBound = new InvisibleBarrier(new Vector2(level.Bounds.Left, player.CameraAnchor.Y + 196f + behindOffset), level.Bounds.Right - level.Bounds.Left, 8));
                                    addedBarrier = true;
                                }
                                if (lowerBound != null && lowerBound.Position.Y > level.Bounds.Top + 180)
                                    lowerBound.MoveTo(new Vector2(lowerBound.X, player.CameraAnchor.Y + 196f + behindOffset));
                                break;
                        }
                        if (player.Position.Y < player.CameraAnchor.Y + 8 + aheadOffset && player.CameraAnchor.Y > level.Bounds.Top)
                            player.Position.Y = player.CameraAnchor.Y + 8 + aheadOffset;
                        break;
                    case CameraDirection.Down:
                        if (player.CameraAnchor.Y < level.Bounds.Bottom - 180)
                            target += new Vector2(0, speed / 60);
                        player.CameraAnchor.Y = target.Y;
                        player.CameraAnchorLerp.Y = 1;
                        switch (autoscrollerMode)
                        {
                            case AutoscrollerMode.ImmediateDeath:
                                if (player.Position.Y < player.CameraAnchor.Y - 16 + behindOffset)
                                    player.Die(player.TopCenter);
                                break;
                            case AutoscrollerMode.SafePush:
                                if (player.Position.Y < player.CameraAnchor.Y - 8 + behindOffset)
                                    player.Position.Y = player.CameraAnchor.Y - 8 + behindOffset;
                                if (player.Position.Y > player.CameraAnchor.Y + 204 + aheadOffset && player.CameraAnchor.Y < level.Bounds.Bottom - 180)
                                    player.Die(player.BottomCenter);
                                break;
                            default: //PushNCrush
                                if (!addedBarrier)
                                {
                                    Scene.Add(upperBound = new InvisibleBarrier(new Vector2(level.Bounds.Left, player.CameraAnchor.Y - 20f + behindOffset), level.Bounds.Right - level.Bounds.Left, 8));
                                    addedBarrier = true;
                                }
                                if (upperBound != null && upperBound.Position.Y < level.Bounds.Bottom - 188)
                                    upperBound.MoveTo(new Vector2(upperBound.X, player.CameraAnchor.Y - 20f + behindOffset));
                                if (player.Position.Y > player.CameraAnchor.Y + 196)
                                    player.Die(player.BottomCenter);
                                break;
                        }
                        break;
                    case CameraDirection.Left:
                        if (player.CameraAnchor.X > level.Bounds.Left)
                            target += new Vector2(-1 * speed / 60, 0);
                        player.CameraAnchor.X = target.X - 304;
                        player.CameraAnchorLerp.X = 1;
                        switch (autoscrollerMode)
                        {
                            case AutoscrollerMode.ImmediateDeath:
                                if (player.Position.X > player.CameraAnchor.X + 340 + behindOffset && player.Position.X >= level.Bounds.Left + 324)
                                    player.Die(player.CenterRight);
                                break;
                            case AutoscrollerMode.SafePush:
                                if (player.Position.X > player.CameraAnchor.X + 328 + behindOffset && player.Position.X >= level.Bounds.Left + 324)
                                    player.Position.X = player.CameraAnchor.X + 328 + behindOffset;
                                break;
                            default: // PushNCrush
                                if (!addedBarrier && player.Position.X >= level.Bounds.Left + 320)
                                {
                                    Scene.Add(leftBound = new InvisibleBarrier(new Vector2(player.CameraAnchor.X + 332f + behindOffset, level.Camera.Y - 8f), 8, 196));
                                    addedBarrier = true;
                                }
                                if (!addedBarrier && player.Position.X <= level.Bounds.Left + 320)
                                {
                                    Scene.Add(leftBound = new InvisibleBarrier(new Vector2(level.Bounds.Left + 320, level.Camera.Y - 8f), 8, 196));
                                    addedBarrier = true;
                                }
                                if (leftBound != null && leftBound.Position.X > level.Bounds.Left + 320)
                                    leftBound.MoveTo(new Vector2(player.CameraAnchor.X + 332f + behindOffset, level.Camera.Y - 8f));
                                break;
                        }
                        if (player.Position.X < player.CameraAnchor.X + 4 + aheadOffset && player.CameraAnchor.X > level.Bounds.Left)
                            player.Speed.X = 0;
                        break;
                    default: //Right
                        if (player.CameraAnchor.X < level.Bounds.Right - 320)
                            target += new Vector2(1 * speed / 60, 0);
                        player.CameraAnchor.X = target.X;
                        player.CameraAnchorLerp.X = 1;
                        switch (autoscrollerMode)
                        {
                            case AutoscrollerMode.ImmediateDeath:
                                if (player.Position.X < player.CameraAnchor.X - 20 + behindOffset && player.Position.X <= level.Bounds.Right - 324)
                                    player.Die(player.CenterLeft);
                                break;
                            case AutoscrollerMode.SafePush:
                                if (player.Position.X < player.CameraAnchor.X - 8 + behindOffset && player.Position.X <= level.Bounds.Right - 324)
                                    player.Position.X = player.CameraAnchor.X - 8 + behindOffset;
                                break;
                            default: // PushNCrush
                                if (!addedBarrier && player.Position.X <= level.Bounds.Right - 320) // Normal barrier
                                {
                                    Scene.Add(rightBound = new InvisibleBarrier(new Vector2(player.CameraAnchor.X - 20f + behindOffset, level.Camera.Y - 8f), 8, 196));
                                    addedBarrier = true;
                                }
                                if (!addedBarrier && player.Position.X > level.Bounds.Right - 320) // Barrier to prevent backtracking if coming from next room
                                {
                                    Scene.Add(rightBound = new InvisibleBarrier(new Vector2(level.Bounds.Right - 320, level.Camera.Y - 8f), 8, 196));
                                    addedBarrier = true;
                                }
                                if (rightBound != null && rightBound.Position.X < level.Bounds.Right - 328)
                                    rightBound.MoveTo(new Vector2(player.CameraAnchor.X - 20f + behindOffset, level.Camera.Y - 8f));
                                break;
                        }
                        if (player.Position.X > player.CameraAnchor.X + 316 + aheadOffset && player.CameraAnchor.X < level.Bounds.Right - 320)
                            player.Speed.X = 0;
                        break;
                }
            }
        }
    }
}