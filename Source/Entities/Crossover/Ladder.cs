using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/Ladder")]
[Tracked]
public class Ladder : Entity
{
    public const float FallThruJumpThruTime = 0.3f;
    private Player player;
    /// <summary>
    ///     Prevents the player from entering the ladder state after doing an action like jumping.
    ///     Once the player has left the ladder's hitbox, this variable is deactivated.
    /// </summary>
    public float Speed;
    public bool invisible = false;
    public bool drainsStamina = false;
    public Color color;
    public string sound;
    public bool canClimbHorizontally = true;
    public bool leaveLadders = true;
    public float horizontalSpeedLimit = 150;
    public bool coyoteTime = false;
    public bool requiresGrabButton = false;
    public float regrabCooldown = 1;
    public float staminaDrainage = 1;
    public bool dummyMode = false;
    public int verticalOffset = 6;

    private bool disableUntilLeave = false;
    private float fallThruJumpThruTimer;
    private bool inLadderState = false;
    private float timeSinceLadderTech;
    private StaticMover staticMover;
    public static float decelerationValues = 1f;
    public bool singleUse;
    private bool hasBeenUsed;

    private MTexture textureSide;
    private MTexture textureMiddle;
    private MTexture textureThin;

    public bool InLadderState
    {
        get
        {
            return inLadderState;
        }
        set
        {
            inLadderState = value;
            if (value)
            {
                if (!dummyMode)
                    player.StateMachine.State = KoseiHelper_StLadder;
                else
                    player.StateMachine.State = 11; // StDummy == 11
                player.StateMachine.Locked = true;
                player.ForceCameraUpdate = true;
                player.DummyGravity = false;
                player.DummyAutoAnimate = false;
                player.Sprite.Y = 2;

                if (!canClimbHorizontally)
                { // Player will be in the middle of the ladder if they can't move horizontally
                    player.Speed.X = 0;
                    player.Position.X = X + Width / 2;
                }
                if (player.OnGround())
                    player.MoveV(player.CenterY > CenterY ? -1f : 1f);
            }
            else
                outOfLadders();
        }
    }

    public void outOfLadders()
    {
        player.DummyGravity = true;
        player.StateMachine.Locked = false;
        player.StateMachine.State = Player.StNormal;
        player.ForceCameraUpdate = false;
        player.IgnoreJumpThrus = false;
        player.Sprite.Y = 0;
        hasBeenUsed = true;
    }

    private readonly Sprite playerSprite, playerNoBackpackSprite, playerBadeline, playerPlayback, badeline;

    public Ladder(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Speed = data.Float("climbSpeed", 60f);
        sound = data.Attr("sound", "event:/KoseiHelper/Crossover/Ladder");
        Collider = new Hitbox(data.Width, data.Height);
        Depth = data.Int("depth");
        drainsStamina = data.Bool("drainsStamina", false);
        leaveLadders = data.Bool("leaveLaddersToRegrab", false);
        horizontalSpeedLimit = data.Float("horizontalSpeedLimit", 330f);
        regrabCooldown = data.Float("regrabCooldown", 1);
        coyoteTime = data.Bool("coyoteTime", false);
        staminaDrainage = data.Float("staminaDrainage", 1);
        requiresGrabButton = data.Bool("requiresGrabButton", false);
        canClimbHorizontally = data.Bool("canClimbHorizontally", true);
        decelerationValues = data.Float("decelerationValues", 1f);
        dummyMode = data.Bool("dummyMode", false);
        singleUse = data.Bool("singleUse", false);
        if (!data.Bool("isInvisible"))
        { // Gets the textures if the ladders are not invisible
            textureSide = GFX.Game[data.Attr("texture") + "_side"];
            textureMiddle = GFX.Game[data.Attr("texture") + "_middle"];
            textureThin = GFX.Game[data.Attr("texture") + "_thin"];
            verticalOffset = data.Int("verticalOffset");
        }
        else invisible = true;
        // TODO read color with my Utils thingie
        // If the ladders are not tinted, the bg ones will be slightly darker
        if (Depth < 0)
        {
            if (data.HexColor("color") == Color.White) color = Color.White;
            else color = Calc.HexToColor(data.Attr("color"));
        }
        else
        {
            if (data.HexColor("color") == Color.White) color = Color.LightGray;
            else color = Calc.HexToColor(data.Attr("color"));
        }
        Add(new PlayerCollider(OnPlayer));
        if (data.Bool("isAttached"))
        {
            // Creates a staticMover that appears behind the attaching entity
            staticMover = new StaticMover();
            staticMover.SolidChecker = (s) => CollideCheck(s, Position + Vector2.UnitY);
            staticMover.JumpThruChecker = (jt) => CollideCheck(jt, Position + Vector2.UnitY);
            Add(staticMover);
        }

        playerSprite = GFX.SpriteBank.Create("koseiHelper_player");
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        player = scene.Tracker.GetEntity<Player>();
    }

    private void OnPlayer(Player player)
    {
        if (player.Scene != null)
        {
            bool verticalMoveCheck = Input.MoveY == 1 && !player.CollideCheck<Solid>(player.Position + Vector2.UnitY);
            if (verticalMoveCheck && player.CollideCheckOutside<JumpThru>(player.Position + Vector2.UnitY))
            {
                fallThruJumpThruTimer -= Engine.DeltaTime;
                verticalMoveCheck = false;
                if (fallThruJumpThruTimer < 0f)
                {
                    player.IgnoreJumpThrus = true;
                    verticalMoveCheck = true;
                }
            }
            else
            {
                fallThruJumpThruTimer = FallThruJumpThruTime;
                player.IgnoreJumpThrus = false;
            }
            if (player.CollideCheck<Ladder>() && player.StateMachine.State.Equals(0))
            { //Conditions for ladder state: collide with player, player is in StNormal...
                if (!requiresGrabButton && Input.MoveY.Value == -1 || verticalMoveCheck && !player.wasOnGround || requiresGrabButton && (!player.wasOnGround || player.onGround && Input.MoveY.Value == -1 && Input.Grab) && player.Speed.X < horizontalSpeedLimit)
                { // ...press up/down (or grab in grab mode)
                    if (!disableUntilLeave && Math.Abs(player.Speed.X) < horizontalSpeedLimit && timeSinceLadderTech <= 0)
                    { // ...player is not moving too fast horizontally, and the LadderJump cooldown is finished
                        if (drainsStamina && player.Stamina > 20 || !drainsStamina)
                        {// Requires 20 stamina to grab on stamina mode
                            player.Get<LadderStateComponent>().CurrentLadder = this;
                            InLadderState = true;
                            if (singleUse && hasBeenUsed)
                                Break(player.Center, player.BottomCenter);
                        }
                    }
                }
            }
            if (requiresGrabButton && (Input.Grab.Released || !Input.Grab))
                InLadderState = false;
            if (player.StateMachine.State == Player.StDash && leaveLadders && timeSinceLadderTech <= 0)
                disableUntilLeave = true;
        }
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();
        if (level != null)
        {
            player = level.Tracker.GetEntity<Player>();
            if (timeSinceLadderTech >= 0)
                timeSinceLadderTech -= 2.5f * Engine.DeltaTime;
            if (player != null && (!player.CollideCheck(this) || player.OnGround()))
                disableUntilLeave = false;
            if (InLadderState && player != null)
            {
                if (drainsStamina)
                {
                    player.Stamina -= 12 * Engine.DeltaTime * staminaDrainage;//Progressively drains stamina
                    if (Input.MoveY.Value < 0f) player.Stamina -= 12 * Engine.DeltaTime * staminaDrainage;  //And drains it more quickly while climbing up
                    if (player.Stamina <= 0)
                    { // Falls if you run out of stamina
                        inLadderState = false;
                        player.Stamina = 0;
                        outOfLadders();
                    }
                }
                if (!player.CollideCheck<Ladder>() || player.OnGround() && Input.MoveY != -1) // stops being in ladder state if the player is not colliding anymore (or if they're on ground and crouch)
                    InLadderState = false;
                if (player.CanDash)
                {
                    if (Input.Dash.Pressed || Input.CrouchDash.Pressed)
                    {
                        InLadderState = false;
                        timeSinceLadderTech = 0.85f * regrabCooldown; // resets a timer so you have to wait a bit until Dashing again
                        if (coyoteTime)
                            player.StartJumpGraceTime();
                        if (leaveLadders)
                            disableUntilLeave = true;
                        else
                            player.OnGround(0);
                        if (player.Dashes > 0)
                            player.StateMachine.State = player.StartDash();
                    }
                }
                if (Input.Jump.Pressed && InLadderState)
                {
                    if (drainsStamina)
                    {
                        player.Stamina -= 25; // A ladder climbjump will consume 25 stamina, making sure it's never negative
                        if (player.Stamina < 0)
                            player.Stamina = 0;
                    }
                    InLadderState = false;
                    if (leaveLadders)
                        disableUntilLeave = true;
                    timeSinceLadderTech = 0.75f * regrabCooldown; // Resets a timer so you have to wait a bit until Jumping again
                    player.Jump();
                }
                if (player.CollideCheckOutside<JumpThru>(player.Position + Vector2.UnitY))
                {
                    fallThruJumpThruTimer -= Engine.DeltaTime;
                    if (fallThruJumpThruTimer < 0f)
                        player.IgnoreJumpThrus = true;
                }
                else
                {
                    fallThruJumpThruTimer = FallThruJumpThruTime;
                    player.IgnoreJumpThrus = false;
                }
                if (dummyMode)
                    player.Speed.Y = 0f; // if not dummy Mode, the speedY will use deceleration in the StLadderUpdate method
                System.Collections.Generic.List<Entity> ladders = player.CollideAll<Ladder>(); // Checks all the Ladders that are currently being collided so you can move between them
                float lowestY = float.MaxValue;
                foreach (Entity Ladder in ladders)
                {
                    if (Ladder.Position.Y < lowestY)
                        lowestY = Ladder.Position.Y;
                }
                if (player.Position.Y - 8 >= lowestY) // Makes sure the player is not on the very top of the stairs
                    player.MoveV(Input.MoveY.Value * Speed * Engine.DeltaTime);
                else if (Input.MoveY.Value > 0f)
                    // And if they are, they can only climb down
                    player.MoveV(Input.MoveY.Value * Speed * Engine.DeltaTime);
                if (canClimbHorizontally && !player.Ducking && level.IsInBounds(player)) // Only move while inside the level
                    player.MoveH(Input.MoveX.Value * Speed * Engine.DeltaTime);


                // Dummy Mode animations
                updateSprite(player);
            }
            base.Update();
        }
    }

    private void updateSprite(Player player)
    {
        if (player == null) return;
        if (Input.MoveY.Value != 0f || canClimbHorizontally && Input.MoveX.Value != 0f && !player.Ducking)
        {
            if (Scene.OnInterval(0.35f)) Audio.Play(sound); //Plays sounds when climbing

            player.Sprite.Visible = player.Hair.Visible = false;
            if (Depth < 0)
            { //Plays animation depending on bg/fg and number of dashes
                switch (player.Dashes)
                {
                    case 0:
                        if (player.Inventory.Dashes == 0)
                            playerSprite.Play("ladder_climb", false);
                        else
                            playerSprite.Play("ladder_climbBlue", false);
                        break;
                    case 1:
                        playerSprite.Play("ladder_climb", false);
                        break;
                    case 2:
                        playerSprite.Play("ladder_climbPink", false);
                        break;
                    default:
                        playerSprite.Play("ladder_climb", false);
                        break;
                }
            }
            else
            {
                switch (player.Dashes)
                {
                    case 0:
                        if (player.Inventory.Dashes == 0)
                            playerSprite.Play("ladder_bg_climb", false);
                        else
                            playerSprite.Play("ladder_bg_climbBlue", false);
                        break;
                    case 1:
                        playerSprite.Play("ladder_bg_climb", false);
                        break;
                    case 2:
                        playerSprite.Play("ladder_bg_climbPink", false);
                        break;
                    default:
                        playerSprite.Play("ladder_bg_climb", false);
                        break;
                }
            }
        }
        else
        {
            if (Depth < 0)
            {
                switch (player.Dashes)
                {
                    case 0:
                        if (player.Inventory.Dashes == 0)
                            playerSprite.Play("ladder_cling", false);
                        else
                            playerSprite.Play("ladder_clingBlue", false);
                        break;
                    case 1:
                        playerSprite.Play("ladder_cling", false);
                        break;
                    case 2:
                        playerSprite.Play("ladder_clingPink", false);
                        break;
                    default:
                        playerSprite.Play("ladder_cling", false);
                        break;
                }
            }
            else
            {
                switch (player.Dashes)
                {
                    case 0:
                        if (player.Inventory.Dashes == 0)
                            playerSprite.Play("ladder_bg_cling", false);
                        else
                            playerSprite.Play("ladder_bg_clingBlue", false);
                        break;
                    case 1:
                        playerSprite.Play("ladder_bg_cling", false);
                        break;
                    case 2:
                        playerSprite.Play("ladder_bg_clingPink", false);
                        break;
                    default:
                        playerSprite.Play("ladder_bg_cling", false);
                        break;
                }
            }
        }
        Add(playerSprite);
    }

    public override void Render()
    {
        if (!invisible)
        {
            if (Width <= 15)
                for (float i = Position.Y; i <= Position.Y + Collider.Height; i += 16)
                    textureThin.GetSubtexture(0, 0, (int)Collider.Width, (int)Collider.Height + 8)
                        .Draw(new Vector2(Position.X, i) - Vector2.UnitY * verticalOffset, Vector2.Zero, color);
            else
            {
                for (float i = Position.Y; i <= Position.Y + Collider.Height - 8; i += 16)
                { //Texture is 16 pixels tall so the counter increases by its height
                    textureSide.GetSubtexture(0, 0, (int)Collider.Width, (int)Collider.Height + 8)
                        .Draw(new Vector2(Position.X, i) - Vector2.UnitY * verticalOffset, Vector2.Zero, color); //Left side
                    textureSide.GetSubtexture(0, 0, (int)Collider.Width, (int)Collider.Height + 8)
                        .Draw(new Vector2(Position.X, i) + Vector2.UnitX * Collider.Width - Vector2.UnitY * verticalOffset, Vector2.Zero, color, new Vector2(-1, 1)); //Right side
                    for (float j = 0; j <= Collider.Width - 24; j += 8) //Texture is 8 pixels wide so the counter increases by its width
                        textureMiddle.GetSubtexture(0, 0, (int)Collider.Width - 8, (int)Collider.Height + 8)
                            .Draw(new Vector2(Position.X + 8 + j, i) - Vector2.UnitY * verticalOffset, Vector2.Zero, color);
                }
            }
            base.Render();
        }
    }


    internal static bool HookJumpThruBoostCheck(On.Celeste.Player.orig_JumpThruBoostBlockedCheck orig, Player self)
    {
        if (self.Scene.Tracker.GetEntity<Ladder>()?.InLadderState ?? false) return true;
        return orig(self);
    }

    private class LadderStateComponent : Component
    {
        public LadderStateComponent() : base(false, false) { }
        public Ladder CurrentLadder;
    }

    public static int KoseiHelper_StLadder;

    public static void RegisterLadderState(Player player)
    {
        KoseiHelper_StLadder = player.AddState("KoseiHelper_StLadder", StLadderUpdate, null, StLadderBegin, null);
        player.Add(new LadderStateComponent());
    }

    private static void StLadderBegin(Player player)
    {
        player.DummyAutoAnimate = false;
    }

    private static int StLadderUpdate(Player player)
    {
        float maxDeceleration = 800.0f;
        float decelerationThreshold = 50.0f;
        float zeroThreshold = 1.5f;
        ApplyDeceleration(ref player.Speed.X, maxDeceleration, decelerationThreshold, zeroThreshold);
        ApplyDeceleration(ref player.Speed.Y, maxDeceleration, decelerationThreshold, zeroThreshold);
        Ladder ladder = player.Get<LadderStateComponent>().CurrentLadder;

        ladder.updateSprite(player);

        return KoseiHelper_StLadder;
    }

    private static void ApplyDeceleration(ref float speedComponent, float maxDeceleration, float decelerationThreshold, float zeroThreshold)
    {
        float absSpeed = Math.Abs(speedComponent);

        if (absSpeed > zeroThreshold)
        {
            float decelerationFactor = 1.0f;
            if (absSpeed > decelerationThreshold)
            {
                decelerationFactor = MathF.Pow(absSpeed / decelerationThreshold, 3f * decelerationValues);
                decelerationFactor = MathF.Min(decelerationFactor, decelerationValues);
            }
            else
            {
                decelerationFactor = 0.5f;

                if (absSpeed < 10f)
                    decelerationFactor = 0.1f;
            }

            if (speedComponent > 0)
                speedComponent -= decelerationFactor * maxDeceleration * Engine.DeltaTime;
            else
                speedComponent += decelerationFactor * maxDeceleration * Engine.DeltaTime;
        }
        // If speed is below zeroThreshold, set it to zero
        if (Math.Abs(speedComponent) < zeroThreshold)
        {
            speedComponent = 0;
        }
    }

    public void Break(Vector2 from, Vector2 direction, bool playSound = true, bool playDebrisSound = true)
    {
        Audio.Play("event:/game/general/wall_break_wood", Position);
        for (int i = 0; i < Width / 8f; i++)
        {
            for (int j = 0; j < Height / 8f; j++)
            {
                Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(4 + i * 8, 4 + j * 8), '9', playDebrisSound).BlastFrom(from));
            }
        }
        Collidable = false;
        outOfLadders();
        RemoveSelf();
    }

    public override void DebugRender(Camera camera)
    {
        base.DebugRender(camera);
        Draw.Point(Position, Color.Aqua);
    }

    public static void OnPlayerSpriteUpdate(On.Celeste.Player.orig_UpdateSprite orig, Player p)
    {
        if (p.StateMachine.GetCurrentStateName() == "KoseiHelper_StLadder")
            p.Sprite.Scale = Vector2.One;
        else
            orig(p);
    }
}