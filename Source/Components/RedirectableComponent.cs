using Monocle;

namespace Celeste.Mod.KoseiHelper.Components;

[Tracked(true)]
public class Redirectable : Component
{
    public MoveBlock Block;

    public MoveBlock.Directions Direction
    {
        get => Block.direction;
        set => Block.direction = value;
    }

    public float Angle
    {
        get => Block.angle;
        set => Block.angle = value;
    }

    public float Speed
    {
        get => Block.speed;
        set => Block.speed = value;
    }

    public float TargetSpeed
    {
        get => Block.targetSpeed;
        set => Block.targetSpeed = value;
    }

    public float TargetAngle
    {
        get => Block.targetAngle;
        set => Block.targetAngle = value;
    }

    public Redirectable(MoveBlock block)
        : base(true, false)
    {
        Block = block;
    }
}