using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.IO;
namespace Celeste.Mod.KoseiHelper.Other.Apps;

[Tracked]
public abstract class App : Entity
{
    public App(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Render()
    {
        base.Render();
    }
}