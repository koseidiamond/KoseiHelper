using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/RPGController")]
public class TopDownViewController : Entity
{
    private Level level;

    public TopDownViewController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        KoseiHelperModule.ExtendedVariantImports.TriggerFloatVariant("UnderwaterSpeedX", (float)KoseiHelperModule.ExtendedVariantImports.GetCurrentVariantValue("UnderwaterSpeedY"), true);
        Water water = new Water(new Vector2(level.Bounds.Left, level.Bounds.Top - 20), false, false, level.Bounds.Width, level.Bounds.Height + 20);
        water.Visible = false;
        Scene.Add(water);
        // TODO block the displacement, make the controller load globally, block the underwater snapshot, and prevent tech
    }

    public override void Update()
    {
    }
}