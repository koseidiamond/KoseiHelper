using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

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
    public AutoscrollerController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        cameraDirection = data.Enum("cameraDirection", CameraDirection.Right);
        autoscrollerMode = data.Enum("pushMode", AutoscrollerMode.PushNCrush);
        speed = data.Float("speed", 60f);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            target = new Vector2(player.Position.X - 160, player.Position.Y - 90);
        }
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
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
                            if (player.Position.Y > player.CameraAnchor.Y + 204)
                                player.Die(player.BottomCenter);
                            break;
                        case AutoscrollerMode.SafePush:
                            if (player.Position.Y > player.CameraAnchor.Y + 204)
                            {
                                player.PointBounce(new Vector2(player.CenterX, player.Bottom));
                                Audio.Play("event:/game/general/assist_screenbottom");
                            }
                            break;
                        default:
                            if (!addedBarrier)
                            {
                                Scene.Add(lowerBound = new InvisibleBarrier(new Vector2(level.Bounds.Left, player.CameraAnchor.Y + 200f), level.Bounds.Right-level.Bounds.Left, 8));
                                addedBarrier = true;
                            }
                            if (lowerBound != null)
                                lowerBound.MoveTo(new Vector2(lowerBound.X, player.CameraAnchor.Y + 200f));
                            break;
                    }
                    if (player.Position.Y < player.CameraAnchor.Y + 8)
                        player.Position.Y = player.CameraAnchor.Y + 8;
                    break;
                case CameraDirection.Down:
                    if (player.CameraAnchor.Y < level.Bounds.Bottom)
                        target += new Vector2(0, speed / 60);
                    player.CameraAnchor.Y = target.Y;
                    player.CameraAnchorLerp.Y = 1;
                    switch (autoscrollerMode)
                    {
                        case AutoscrollerMode.ImmediateDeath:
                            if (player.Position.Y < player.CameraAnchor.Y - 16)
                                player.Die(player.TopCenter);
                            break;
                        case AutoscrollerMode.SafePush:
                            if (player.Position.Y < player.CameraAnchor.Y - 8)
                                player.Position.Y = player.CameraAnchor.Y - 8;
                            if (player.Position.Y > player.CameraAnchor.Y + 204)
                                player.Die(player.BottomCenter);
                            break;
                        default:
                            if (!addedBarrier)
                            {
                                Scene.Add(upperBound = new InvisibleBarrier(new Vector2(level.Bounds.Left, player.CameraAnchor.Y - 20f), level.Bounds.Right - level.Bounds.Left, 8));
                                addedBarrier = true;
                            }
                            if (upperBound != null)
                                upperBound.MoveTo(new Vector2(upperBound.X, player.CameraAnchor.Y - 20f));
                            if (player.Position.Y > player.CameraAnchor.Y + 204)
                                player.Die(player.BottomCenter);
                            break;
                    }
                    break;
                case CameraDirection.Left:
                    if (player.CameraAnchor.X > level.Bounds.Left)
                        target += new Vector2(-1 * speed / 60, 0);
                    player.CameraAnchor.X = target.X;
                    player.CameraAnchorLerp.X = 1;
                    switch (autoscrollerMode)
                    {
                        case AutoscrollerMode.ImmediateDeath:
                            if (player.Position.X > player.CameraAnchor.X + 340)
                                player.Die(player.CenterRight);
                            break;
                        case AutoscrollerMode.SafePush:
                            if (player.Position.X > player.CameraAnchor.X + 328)
                                player.Position.X = player.CameraAnchor.X + 328;
                            break;
                        default:
                            if (!addedBarrier)
                            {
                                Scene.Add(leftBound = new InvisibleBarrier(new Vector2(player.CameraAnchor.X + 332f, level.Camera.Y - 8f), 8, 196));
                                addedBarrier = true;
                            }
                            if (leftBound != null)
                                leftBound.MoveTo(new Vector2(player.CameraAnchor.X + 332f, level.Camera.Y - 8f));
                            break;
                    }
                    if (player.Position.X < player.CameraAnchor.X + 8)
                        player.Speed.X = 0;
                    break;
                default: //Right
                    if (player.CameraAnchor.X < level.Bounds.Right)
                        target += new Vector2(1 * speed/60, 0);
                    player.CameraAnchor.X = target.X;
                    player.CameraAnchorLerp.X = 1;
                    switch (autoscrollerMode)
                    {
                        case AutoscrollerMode.ImmediateDeath:
                            if (player.Position.X < player.CameraAnchor.X - 20)
                                player.Die(player.CenterLeft);
                                break;
                        case AutoscrollerMode.SafePush:
                            if (player.Position.X < player.CameraAnchor.X -8)
                                player.Position.X = player.CameraAnchor.X -8;
                            break;
                        default:
                            if (!addedBarrier)
                            {
                                Scene.Add(rightBound = new InvisibleBarrier(new Vector2(player.CameraAnchor.X -20f, level.Camera.Y -8f), 8, 196));
                                addedBarrier = true;
                            }
                            if (rightBound != null)
                                rightBound.MoveTo(new Vector2(player.CameraAnchor.X -20f, level.Camera.Y -8f));
                            break;
                    }
                    if (player.Position.X > player.CameraAnchor.X + 312)
                        player.Speed.X = 0;
                    break;
            }
        }
    }
}