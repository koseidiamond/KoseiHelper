using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/SpringBall")]
public class SpringBall : Spring
{
    private const float ResetTime = 0.8f;
    private new Sprite sprite;
    private Sprite arrow;
    private float resetTimer;
    private Level level;
    private SineWave sine;
    private float atY;
    private float atX;
    private SoundSource spawnSfx;
    private Vector2 spawnPosition;
    public float speed = 200f;
    public float sineLength;
    public float sineSpeed;
    public bool vertical;
    public Color color;
    public bool visible;
    public string spawnSound;
    public bool trackTheo;
    public float spawnOffset;
    public string flag;

    public SpringBall(EntityData data, Vector2 offset)
        : base(data.Position + offset, data.Enum("orientation", Orientations.Floor), true)
    {
        base.Depth = -12500;
        base.Collider = new Hitbox(12f, 12f, -6f, -6f);
        speed = data.Float("speed", 200f);
        sineLength = data.Float("sineLength", 4f);
        sineSpeed = data.Float("sineSpeed", 0.5f);
        vertical = data.Bool("vertical", false);
        visible = data.Bool("visible", true);
        color = data.HexColor("color", Color.White);
        spawnSound = data.Attr("spawnSound", "event:/none");
        trackTheo = data.Bool("trackTheo", false);
        spawnOffset = data.Float("offset", 0f);
        flag = data.Attr("flag", "");
        Add(sine = new SineWave(sineSpeed, 0f));
        Add(sprite = GFX.SpriteBank.Create("koseiHelper_springBall"));
        Add(arrow = GFX.SpriteBank.Create("koseiHelper_springBall"));
        arrow.Color = sprite.Color = color;
        if (Orientation == Orientations.WallLeft)
            arrow.Rotation = MathHelper.PiOver2;
        if (Orientation == Orientations.WallRight)
            arrow.Rotation = -MathHelper.PiOver2;
        if (visible)
        {
            sprite.Play("spin");
            arrow.Play("arrow");
        }
        Add(spawnSfx = new SoundSource());
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        Collidable = Visible = false;
        if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag) && !string.IsNullOrEmpty(flag))
            ResetPosition();
    }

    private void ResetPosition()
    {
        Player player = level.Tracker.GetEntity<Player>();
        TheoCrystal theo = level.Tracker.GetEntity<TheoCrystal>();
        if (player != null && !trackTheo)
        { // Makes sure that the player is not close to the bounds of the screen
            if ((!vertical && speed >= 0 && player.Right < (level.Bounds.Right - 40)) ||
                (!vertical && speed < 0 && player.Left > level.Bounds.Left + 40) ||
                (vertical && speed >= 0 && player.Bottom < level.Bounds.Bottom - 32) ||
                (vertical && speed < 0 && player.Top > level.Bounds.Top + 32))
            {
                spawnSfx.Play(spawnSound);
                Collidable = Visible = true;
                resetTimer = 0f;
                if (!vertical)// Set position to offscreen right/left and same vertical position as player
                {
                    if (speed >= 0)
                        spawnPosition = new Vector2(level.Camera.Right + 10f, player.CenterY + spawnOffset);
                    else
                        spawnPosition = new Vector2(level.Camera.Left - 10f, player.CenterY + spawnOffset);
                    atY = spawnPosition.Y;
                }
                else// Set position to offscreen bottom/top and same horizontal position as player
                {
                    if (speed >= 0)
                        spawnPosition = new Vector2(player.CenterX + spawnOffset, level.Camera.Bottom + 16f);
                    else
                        spawnPosition = new Vector2(player.CenterX + spawnOffset, level.Camera.Top - 16f);
                    atX = spawnPosition.X;
                }
                sine.Reset();
                base.Position = spawnPosition;
            }
        }
        else if (theo != null && trackTheo)
        {
            { // Makes sure that Theo is not close to the bounds of the screen
                if ((!vertical && speed >= 0 && theo.Right < (level.Bounds.Right - 40)) ||
                    (!vertical && speed < 0 && theo.Left > level.Bounds.Left + 40) ||
                    (vertical && speed >= 0 && theo.Bottom < level.Bounds.Bottom - 32) ||
                    (vertical && speed < 0 && theo.Top < level.Bounds.Top + 32))
                {
                    spawnSfx.Play(spawnSound);
                    Collidable = Visible = true;
                    resetTimer = 0f;
                    if (!vertical)// Set position to offscreen right/left and same vertical position as Theo
                    {
                        if (speed >= 0)
                            spawnPosition = new Vector2(level.Camera.Right + 10f, theo.CenterY);
                        else
                            spawnPosition = new Vector2(level.Camera.Left - 10f, theo.CenterY);
                        atY = spawnPosition.Y;
                    }
                    else// Set position to offscreen bottom/top and same horizontal position as Theo
                    {
                        if (speed >= 0)
                            spawnPosition = new Vector2(theo.CenterX, level.Camera.Bottom + 16f);
                        else
                            spawnPosition = new Vector2(theo.CenterX, level.Camera.Top - 16f);
                        atX = spawnPosition.X;
                    }
                    sine.Reset();
                    base.Position = spawnPosition;
                }
            }
        }
        else
        {
            resetTimer = 0.05f;
        }
    }

    public override void Update()
    {
        base.Update();
        if (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag) && !string.IsNullOrEmpty(flag))
        {
            if (vertical)
            {
                base.Y -= speed * Engine.DeltaTime;
                base.X = atX + sineLength * sine.Value;

                // Check if it's off-screen (top/bottom)
                if (base.Y < level.Camera.Top - 60f || (speed < 0 && base.Y > level.Camera.Bottom + 60f))
                {
                    resetTimer += Engine.DeltaTime;
                    if (resetTimer >= ResetTime)
                    {
                        ResetPosition();
                    }
                }
            }
            else
            {
                base.X -= speed * Engine.DeltaTime;
                base.Y = atY + sineLength * sine.Value;

                // Check if it's off-screen (left/right)
                if (base.X < level.Camera.Left - 60f || (speed < 0 && base.X > level.Camera.Right + 60f))
                {
                    resetTimer += Engine.DeltaTime;
                    if (resetTimer >= ResetTime)
                    {
                        ResetPosition();
                    }
                }
            }
        }
        else
            Collidable = Visible = false;
    }

    public override void Render()
    {
            sprite.DrawOutline();
        base.Render();
    }
}
