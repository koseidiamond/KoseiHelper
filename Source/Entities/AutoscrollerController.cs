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
    private InvisibleBarrier upperBound, leftBound, rightBound;
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
            switch (cameraDirection)
            {
                case CameraDirection.Up:
                    break;
                case CameraDirection.Down:
                    break;
                case CameraDirection.Left:
                    target = new Vector2(player.Position.X - 160, player.Position.Y - 90);
                    break;
                default:
                    target = new Vector2(player.Position.X - 160, player.Position.Y - 90);
                    break;
            }
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
                    break;
                case CameraDirection.Down:
                    break;
                case CameraDirection.Left:
                    if (player.CameraAnchor.X > level.Bounds.Left)
                        target += new Vector2(-1 * speed / 60, 0);
                    player.CameraAnchor = target;
                    player.CameraAnchorLerp = Vector2.One;
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
                                Scene.Add(rightBound = new InvisibleBarrier(new Vector2(player.CameraAnchor.X + 332f, player.CameraAnchor.Y - 8f), 8, 196));
                                addedBarrier = true;
                            }
                            if (rightBound != null)
                            {
                                float xPos = player.CameraAnchor.X + 340f;
                                rightBound.MoveTo(new Vector2(player.CameraAnchor.X + 332f, player.CameraAnchor.Y - 8f));
                            }
                            break;
                    }
                    break;
                default:
                    if (player.CameraAnchor.X < level.Bounds.Right)
                        target += new Vector2(1 * speed/60, 0);
                    player.CameraAnchor = target;
                    player.CameraAnchorLerp = Vector2.One;
                    switch (autoscrollerMode)
                    {
                        case AutoscrollerMode.ImmediateDeath:
                            if (player.Position.X < player.CameraAnchor.X - 20)
                                player.Die(player.CenterLeft);
                                break;
                        case AutoscrollerMode.SafePush:
                            if (player.Position.X < player.CameraAnchor.X)
                                player.Position.X = player.CameraAnchor.X;
                            break;
                        default:
                            if (!addedBarrier)
                            {
                                Scene.Add(leftBound = new InvisibleBarrier(new Vector2(player.CameraAnchor.X -20f, player.CameraAnchor.Y -8f), 8, 196));
                                addedBarrier = true;
                            }
                            if (leftBound != null)
                            {
                                float xPos = player.CameraAnchor.X - 20f;
                                leftBound.MoveTo(new Vector2(player.CameraAnchor.X -20f, player.CameraAnchor.Y -8f));
                            }
                                break;
                    }
                    break;
            }
            if (player.Top > player.CameraAnchor.Y + 184)
                player.Die(player.BottomCenter);
        }
    }
}