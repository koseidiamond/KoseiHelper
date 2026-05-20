using Celeste.Mod.CommunalHelper;
using Celeste.Mod.CommunalHelper.Components;
using Celeste.Mod.CommunalHelper.Entities;
using Celeste.Mod.Entities;
using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Reflection;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/MoveBlockCrystal")]
[Tracked]
class MoveBlockCrystal : Entity
{
    private readonly Sprite sprite;
    private readonly Sprite flash;
    private readonly Image outline;
    private readonly Wiggler wiggler;
    private readonly BloomPoint bloom;
    private readonly VertexLight light;
    private readonly SineWave sine;
    private readonly ParticleType p_shatter;
    private readonly ParticleType p_glow;
    private readonly ParticleType p_regen;
    private readonly string soundEffect = "event:/game/general/diamond_touch";
    private float respawnTimer;
    private bool singleUse;
    private bool refillDash;
    private readonly float angle;
    private enum Behavior
    {
        Move,
        MoveAndChangeDirection,
        ChangeDirection,
        Break
    };
    private Behavior behavior;
    private MoveBlock.Directions direction;
    private enum MoveBlockType
    {
        Vanilla,
        CommunalConnected,
        Both
    };

    private MoveBlockType moveBlockType;

    public MoveBlockCrystal(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Collider = new Hitbox(16f, 16f, -8f, -8f);
        singleUse = data.Bool("singleUse", false);
        string spritePrefix = data.Attr("sprite", "objects/CC/MoveBlockCrystal/");
        behavior = data.Enum("behavior", Behavior.Move);
        direction = data.Enum("direction", MoveBlock.Directions.Right);
        if (float.TryParse(data.Attr("direction"), out float fAngle))
            angle = fAngle;
        else
            angle = direction.Angle();
        refillDash = data.Bool("refillDash", true);
        moveBlockType = data.Enum("moveBlockType", MoveBlockType.CommunalConnected);
        base.Add(new PlayerCollider(new Action<Player>(OnPlayer), null, null));
        base.Add(outline = new Image(GFX.Game[spritePrefix + "outline"]));
        outline.CenterOrigin();
        outline.Visible = false;
        base.Add(sprite = new Sprite(GFX.Game, spritePrefix + "idle"));
        sprite.AddLoop("idle", "", 0.1f);
        sprite.Play("idle", false, false);
        sprite.CenterOrigin();
        base.Add(flash = new Sprite(GFX.Game, spritePrefix + "flash"));
        flash.Add("flash", "", 0.05f);
        flash.OnFinish = delegate (string anim) {
            flash.Visible = false;
        };
        flash.CenterOrigin();

        Color color1 = Color.LightGreen;
        Color color2 = Calc.HexToColor("30b335");
        if (spritePrefix.IndexOf("break", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            color1 = Calc.HexToColor("cc2541");
            color2 = Calc.HexToColor("474070");
        }

        p_shatter = new ParticleType(Refill.P_Shatter)
        {
            Source = GFX.Game["particles/triangle"],
            Color = color2,
            Color2 = color1
        };
        p_glow = new ParticleType(Refill.P_Glow)
        {
            Color = color1,
            Color2 = color2
        };
        p_regen = new ParticleType(Refill.P_Regen)
        {
            Color = color1,
            Color2 = color2
        };
        base.Add(wiggler = Wiggler.Create(1f, 4f, delegate (float v) {
            sprite.Scale = (flash.Scale = Vector2.One * (1f + v * 0.2f));
        }, false, false));

        base.Add(new MirrorReflection());
        base.Add(bloom = new BloomPoint(0.7f, 20f));
        base.Add(light = new VertexLight(Color.White, 0.5f, 16, 48));
        base.Add(sine = new SineWave(0.6f, 0f));
        sine.Randomize();
        UpdateY();
        base.Depth = -100;

        base.Add(new DashListener
        {
            OnDash = (Vector2 dir) => {
            }
        });
        if (moveBlockType != MoveBlockType.CommunalConnected)
        {
            Tracker.AddTypeToTracker(typeof(MoveBlock));
            Tracker.Refresh();
        }
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (respawnTimer > 0f)
        {
            respawnTimer -= Engine.DeltaTime;
            if (respawnTimer <= 0f)
            {
                Respawn(level);
            }
        }
        else if (base.Scene.OnInterval(0.1f))
        {
            level.ParticlesFG.Emit(p_glow, 1, Position, Vector2.One * 5f);
        }
        UpdateY();
        light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 0.45f : 0f, 4f * Engine.DeltaTime);
        bloom.Alpha = light.Alpha * 0.6f;
        if (base.Scene.OnInterval(2f) && sprite.Visible)
        {
            flash.Play("flash", true, false);
            flash.Visible = true;
        }
    }

    public override void Render()
    {
        if (sprite.Visible)
        {
            sprite.DrawOutline(1);
        }
        base.Render();
    }

    private IEnumerator TheFreezinator(Level level)
    {
        Celeste.Freeze(0.05f);
        yield return null;
        level.Shake(0.3f);
        yield break;
    }

    private void OnPlayer(Player player)
    {
        Level level = SceneAs<Level>();
        base.Add(new Coroutine(TheFreezinator(level), true));
        Audio.Play(soundEffect, Position);
        if (refillDash)
            player.UseRefill(false);

        switch (moveBlockType)
        {
            case MoveBlockType.Both:
                CommunalMoveBlocks(level);
                VanillaMoveBlocks(level);
                break;
            case MoveBlockType.Vanilla:
                VanillaMoveBlocks(level);
                break;
            default: // Communal
                CommunalMoveBlocks(level);
                break;
        }

        float num = Calc.Angle(player.Position, Position);

        // respawn stuff
        sprite.Visible = (flash.Visible = false);
        Collidable = false;
        respawnTimer = 2.5f;
        outline.Visible = true;
        Depth = 8999;
        level.ParticlesFG.Emit(p_shatter, 10, Position, Vector2.One * 4f, num);
        SlashFx.Burst(Position, (float)Math.PI);
        if (singleUse)
            RemoveSelf();
    }

    private void VanillaMoveBlocks(Level level)
    {
        foreach (Entity entity in level)
        { // iterate all entities because

            if (entity is MoveBlock mb)
            {
                switch (behavior)
                {
                    case Behavior.ChangeDirection:
                        mb.direction = direction;
                        break;
                    case Behavior.MoveAndChangeDirection:
                        mb.triggered = true;
                        mb.state = MoveBlock.MovementState.Moving;
                        mb.direction = direction;
                        mb.angle = angle;
                        break;
                    case Behavior.Break:
                        if (mb.state != MoveBlock.MovementState.Breaking)
                            mb.Add(new Coroutine(BreakRoutine(level, mb), true));
                        break;
                    default: // Move
                        mb.triggered = true;
                        mb.state = MoveBlock.MovementState.Moving;
                        break;
                }
            }
        }
    }

    private void CommunalMoveBlocks(Level level)
    {
        foreach (Entity entity in level)
        {
            if (entity is ConnectedMoveBlock cmb)
            {
                DynamicData blockData = DynamicData.For(cmb);

                switch (behavior)
                {
                    case Behavior.ChangeDirection:
                        cmb.Direction = direction;
                        blockData.Set("direction", cmb.Direction);
                        break;
                    case Behavior.MoveAndChangeDirection:
                        blockData.Set("triggered", true);
                        blockData.Set("State", GroupableMoveBlock.MovementState.Moving);
                        blockData.Set("angle", angle);
                        blockData.Set("targetAngle", angle);
                        cmb.Direction = direction;
                        blockData.Set("direction", cmb.Direction);
                        break;
                    case Behavior.Break:
                        // whatever (needs to Invoke and stuff, see CornerBoostFallingBlockSequence from FallingDashCrystal for an example)
                        //if(cmb.state == MoveBlock.MovementState.Idling)
                        //cmb.Add(new Coroutine(BreakRoutine(level, cmb), true));
                        break;
                    default: // Move
                        blockData.Set("triggered", true);
                        blockData.Set("State", GroupableMoveBlock.MovementState.Moving);
                        break;
                }
            }
        }
    }

    private IEnumerator BreakRoutine(Level level, MoveBlock mb)
    {
        Coroutine controller = null;

        foreach (Component c in mb.Components)
        {
            if (c is Coroutine co)
            {
                controller = co;
                break;
            }
        }
        controller?.RemoveSelf();

        Audio.Play("event:/game/04_cliffside/arrowblock_break", mb.Position);
        mb.moveSfx?.Stop();
        mb.state = MoveBlock.MovementState.Breaking;
        mb.speed = (mb.targetSpeed = 0f);
        mb.angle = (mb.targetAngle = mb.homeAngle);
        mb.StartShaking(0.2f);
        mb.StopPlayerRunIntoAnimation = true;
        yield return 0.2f;
        mb.BreakParticles();
        MoveBlock moveBlock = mb;
        Vector2 amount = mb.startPosition - mb.Position;
        mb.DisableStaticMovers();
        mb.MoveStaticMovers(amount);
        mb.Position = mb.startPosition;
        MoveBlock moveBlock2 = mb;
        MoveBlock moveBlock3 = mb;
        bool visible = false;
        moveBlock3.Collidable = false;
        moveBlock2.Visible = visible;
        yield return 2.2f;

        while (mb.CollideCheck<Actor>() || mb.CollideCheck<Solid>())
        {
            yield return null;
        }

        mb.Collidable = true;
        EventInstance instance = Audio.Play("event:/game/04_cliffside/arrowblock_reform_begin", mb.Position);
        MoveBlock moveBlock4 = mb;

        Audio.Play("event:/game/04_cliffside/arrowblock_reappear", mb.Position);
        mb.Visible = true;
        mb.Collidable = true;
        mb.triggered = false;
        //mb.Components.Get<Coroutine>().Cancel()
        mb.Active = true;
        mb.state = MoveBlock.MovementState.Idling;
        mb.EnableStaticMovers();
        mb.speed = (mb.targetSpeed = 0f);
        mb.angle = (mb.targetAngle = mb.homeAngle);
        mb.noSquish = null;
        mb.fillColor = MoveBlock.idleBgFill;
        mb.UpdateColors();
        mb.flash = 1f;

        DynamicData data = DynamicData.For(mb);
        IEnumerator controllerEnum = (IEnumerator)typeof(MoveBlock).GetMethod("Controller", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(mb, null);
        mb.Add(new Coroutine(controllerEnum, true));
    }

    private void Respawn(Level level)
    {
        if (!Collidable)
        {
            Collidable = true;
            sprite.Visible = true;
            outline.Visible = false;
            base.Depth = -100;
            wiggler.Start();
            Audio.Play("event:/game/general/diamond_return", Position);
            level.ParticlesFG.Emit(p_regen, 16, Position, Vector2.One * 2f);
        }
    }
    private void UpdateY()
    {
        flash.Y = (sprite.Y = (bloom.Y = sine.Value * 2f));
    }
}