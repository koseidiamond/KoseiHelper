using Celeste.Mod.KoseiHelper.Entities;
using Microsoft.Xna.Framework;
using Monocle;
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
        bool invertedGravity = SceneAs<Level>()?.Session.GetFlag("GravityHelper_PlayerInverted") ?? false;
        foreach (Conveyor conveyor in Scene.Tracker.GetEntities<Conveyor>())
        {
            if (!conveyor.isBrokenDown && Collide.Check(conveyor, Entity, conveyor.Position + (invertedGravity ? Vector2.UnitY : -Vector2.UnitY)))
            {
                foundConveyor = true;
                Move(conveyor.IsMovingLeft ? -conveyor.moveSpeed : conveyor.moveSpeed);
            }
        }

        IsOnConveyor = foundConveyor;
    }
}