using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MouseController")]
public class MouseController : Entity
{
    float mouseX, mouseY;
    public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, 12, 12);

    public MouseController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Collider = new Hitbox(12, 12, 0, 0);
        base.Tag = Tags.HUD;
    }

    public override void Update()
    {
        base.Update();
        Scene scene = this.Scene;
        Level level = SceneAs<Level>();
        mouseX = MInput.Mouse.Position.X * (320f / 1920f);
        mouseY = MInput.Mouse.Position.Y * (180f / 1080f);
        Position = new Vector2(level.Camera.Position.X + mouseX - 6f, level.Camera.Position.Y + mouseY - 6f);
        if (MInput.Mouse.PressedLeftButton)
            CollisionCheck(scene);
        if (MInput.Mouse.PressedRightButton)
            CollisionCheckAlt(scene);
    }

    private void CollisionCheck(Scene scene)
    {
        if (scene.CollideFirst<TheoCrystal>(Hitbox) is TheoCrystal theoCrystal)
            theoCrystal.HitSpinner(this);
        if (scene.CollideFirst<DashBlock>(Hitbox) is DashBlock dashBlock)
            dashBlock.Break(this.Position, this.Position, true);
        if (scene.CollideFirst<CrystalStaticSpinner>(Hitbox) is CrystalStaticSpinner crystalStaticSpinner)
            crystalStaticSpinner.Destroy();
        if (scene.CollideFirst<FlingBird>(Hitbox) is FlingBird flingBird)
            flingBird.Skip();
        if (scene.CollideFirst<TouchSwitch>(Hitbox) is TouchSwitch touchSwitch)
            touchSwitch.TurnOn();
        if (scene.CollideFirst<Door>(Hitbox) is Door door)
            door.Open(this.X);
        if (scene.CollideFirst<Player>(Hitbox) is Player player)
            player.HiccupJump();
        foreach (Entity entity in (scene as Level).Entities)
        {
            if (entity is Puffer puffer && puffer.Collider.Bounds.Intersects(Hitbox) && puffer.Collidable)
            {
                NemesisGun.NemesisGun.pufferExplode.Invoke(puffer, null);
                NemesisGun.NemesisGun.pufferGotoGone.Invoke(puffer, null);
                return;
            }
        }
    }

    private void CollisionCheckAlt(Scene scene)
    {
        if (scene.CollideFirst<TheoCrystal>(Hitbox) is TheoCrystal theoCrystal)
        {
            if (theoCrystal.Left - 5f > Left)
            {
                theoCrystal.MoveTowardsY(CenterY + 5f, 4f);
                theoCrystal.Speed.X = 220f;
                theoCrystal.Speed.Y = -80f;
                theoCrystal.noGravityTimer = 0.1f;
            }
            else if (theoCrystal.Right + 5f < Right)
            {
                theoCrystal.MoveTowardsY(CenterY + 5f, 4f);
                theoCrystal.Speed.X = -220f;
                theoCrystal.Speed.Y = -80f;
                theoCrystal.noGravityTimer = 0.1f;
            }
            else
            {
                theoCrystal.Speed.X *= 0.5f;
                theoCrystal.Speed.Y = -160f;
                theoCrystal.noGravityTimer = 0.15f;
            }
        }
        foreach (Entity entity in (scene as Level).Entities)
        {
            if (entity is Puffer puffer && puffer.Collider.Bounds.Intersects(Hitbox) && puffer.Collidable)
            {
                if (puffer.Left - 5f > Left)
                {
                    puffer.facing.X = 1f;
                    puffer.GotoHitSpeed(280f * Vector2.UnitX);
                    Audio.Play("event:/new_content/game/10_farewell/puffer_boop", puffer.Position);
                    puffer.MoveTowardsY(CenterY, 4f);
                    puffer.bounceWiggler.Start();
                    puffer.Alert(restart: true, playSfx: false);
                }
                else if (puffer.Right + 5f < Right)
                {
                    puffer.facing.X = -1f;
                    puffer.GotoHitSpeed(280f * -Vector2.UnitX);
                    Audio.Play("event:/new_content/game/10_farewell/puffer_boop", puffer.Position);
                    puffer.MoveTowardsY(CenterY, 4f);
                    puffer.bounceWiggler.Start();
                    puffer.Alert(restart: true, playSfx: false);
                }
                else
                {
                    if (puffer.Top + 5f <= Bottom)
                    {
                        puffer.GotoHitSpeed(224f * -Vector2.UnitY);
                        Audio.Play("event:/new_content/game/10_farewell/puffer_boop", puffer.Position);
                        puffer.MoveTowardsX(CenterX, 4f);
                        puffer.bounceWiggler.Start();
                        puffer.Alert(restart: true, playSfx: false);
                    }
                    else
                    {
                        puffer.GotoHit(Center);
                        puffer.MoveToX(puffer.anchorPosition.X);
                        puffer.idleSine.Reset();
                        puffer.anchorPosition = (puffer.lastSinePosition = puffer.Position);
                        puffer.eyeSpin = 1f;
                        puffer.cannotHitTimer = 0.1f;
                    }
                }
            }
        }
    }

    public override void Render()
    {
        Level level = SceneAs<Level>();
        Image image = new Image(GFX.Gui["lookout/cursor"]);
        image.Position = new Vector2(mouseX * 6f - image.Width / 2f, mouseY * 6f - image.Height / 2f);
        image.Render();
    }
}