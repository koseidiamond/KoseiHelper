using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/_Empty")]
[Tracked]
public class _Empty : Entity
{
    public _Empty(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
    }
}