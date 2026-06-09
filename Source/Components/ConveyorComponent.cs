using Celeste.Mod.KoseiHelper.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace Celeste.Mod.KoseiHelper.Components;

[Tracked(true)]
public class ConveyorMover : Component
{
    public Action<float> OnMove;
    public bool IsOnConveyor = false;

    public ConveyorMover()
        : base(true, true)
    {
    }

    public void Move(float amount)
    {
        OnMove?.Invoke(amount);
    }

    public override void Update()
    {
        base.Update();
        bool foundConveyor = false;
        foreach (Conveyor conveyor in Scene.Tracker.GetEntities<Conveyor>())
        {
            if (!conveyor.isBrokenDown && Collide.Check(conveyor, Entity, conveyor.Position - Vector2.UnitY)) // test, maybe it's +vector2.unity
            {
                foundConveyor = true;
                Move(conveyor.IsMovingLeft ? -conveyor.moveSpeed : conveyor.moveSpeed);
            }
        }

        IsOnConveyor = foundConveyor;
    }
}