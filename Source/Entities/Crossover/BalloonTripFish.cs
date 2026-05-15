using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/BalloonTripFish")]
[Tracked]
public class BalloonTripFish : Entity
{
    public Sprite sprite;
    public float distance = 32f;
    public float chaseSpeed = 1f;
    public string idleFlag;
    private bool wasNear, wasFar;
    public bool hideAfterAttack;

    public BalloonTripFish(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = data.Int("depth", -10500);
        Add(sprite = GFX.SpriteBank.Create(data.Attr("spriteID", "koseiHelper_BalloonTripFish")));
        distance = data.Float("distance", 32f);
        chaseSpeed = data.Float("chaseSpeed", 1f);
        idleFlag = data.Attr("idleFlag", "");
        sprite.CenterOrigin();
        base.Collider = new Hitbox(16, 20, -8, -4);
        Add(new PlayerCollider(OnPlayer));
        sprite.Play("waitBelow");
        sprite.FlipX = data.Bool("flip", false);
        hideAfterAttack = data.Bool("hideAfterAttack", false);
    }

    public override void Update()
    {
        base.Update();
        Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
        if (player == null)
            return;

        if (!SceneAs<Level>().Session.GetFlag(idleFlag) && !player.JustRespawned)
            Position.X = Calc.Approach(Position.X, player.Position.X, chaseSpeed);
        Collidable = sprite.CurrentAnimationID != "waitBelow";

        bool near = Position.Y < player.Position.Y + distance;
        bool far = Position.Y > player.Position.Y + distance;

        if (near && !wasNear)
        {
            sprite.Play("attack");
            wasNear = true;
            wasFar = false;
        }
        if (far && !wasFar)
        {
            sprite.Play("dive");
            wasFar = true;
            wasNear = false;
        }
    }

    public void OnPlayer(Player player)
    {
        if (hideAfterAttack)
            sprite.Play("dive");
        player.Die(TopCenter);
    }
}