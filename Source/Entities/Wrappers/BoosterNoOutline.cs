using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[TrackedAs(typeof(Booster))]
public class BoosterNoOutline : Booster
{

    public BoosterNoOutline(Vector2 position, bool red)
         : base(position, red)
    {
        base.Depth = -8500;
        base.Collider = new Circle(10f, 0f, 2f);
        this.red = red;
        Add(new PlayerCollider(OnPlayer));
        Add(light = new VertexLight(Color.White, 1f, 16, 32));
        Add(bloom = new BloomPoint(0.1f, 16f));
        Add(dashRoutine = new Coroutine(removeOnComplete: false));
        Add(dashListener = new DashListener());
        Add(new MirrorReflection());
        Add(loopingSfx = new SoundSource());
        dashListener.OnDash = OnPlayerDashed;
        particleType = (red ? P_BurstRed : P_Burst);
    }

    public BoosterNoOutline(EntityData data, Vector2 offset)
        : this(data.Position + offset, data.Bool("red"))
    {
        Ch9HubBooster = data.Bool("ch9_hub_booster");
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Image image = new Image(GFX.Game["objects/booster/outline"]);
        image.CenterOrigin();
        image.Color = Color.White * 0f;
        outline = new Entity(Position);
        outline.Depth = 8999;
        outline.Visible = false;
        outline.Add(image);
        outline.Add(new MirrorReflection());
        scene.Add(outline);
    }
}
