using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/Brick")]
[Tracked]
public class Brick : Solid
{
    public enum BrickType
    {
        Normal,
        Ice,
        Fortress
    };
    private Sprite sprite;
    public BrickType brickType;
    private string spriteID;
    private bool tryingToBreak, breaking;
    private bool fireMode;
    public string bumpSound, breakSound;
    private bool brokenByKevins;
    public Brick(EntityData data, Vector2 offset) : base(data.Position + offset, 16, 16, true)
    {
        Depth = data.Int("depth", -1000);
        brickType = data.Enum("type", BrickType.Normal);
        spriteID = data.Attr("sprite", "koseiHelper_Brick");
        brokenByKevins = data.Bool("brokenByKevins", false);
        Add(sprite = GFX.SpriteBank.Create(spriteID));
        if (brickType == BrickType.Normal)
            sprite.Play("Normal", false, true);
        else if (brickType == BrickType.Ice)
            sprite.Play("Ice", false, true);
        else
            sprite.Play("Fortress", false, true);
        bumpSound = data.Attr("bumpSound", "event:/KoseiHelper/brickBump");
        breakSound = data.Attr("breakSound", "event:/KoseiHelper/brickBreak");
        Collider = new Hitbox(16, 16, -8, -8);
        Add(new CoreModeListener(OnChangeMode));
        Tracker.AddTypeToTracker(typeof(CrushBlock));
        Tracker.Refresh();
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
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (brokenByKevins)
        {
            foreach (CrushBlock crushBlock in level.Tracker.GetEntities<CrushBlock>())
            {
                DynamicData dd = DynamicData.For(crushBlock);
                Vector2 crushDir = dd.Get<Vector2>("crushDir");
                if (crushDir != Vector2.Zero && !breaking)
                {
                    Vector2 checkOffset = crushDir.SafeNormalize() * 1f;
                    if (CollideCheck(crushBlock, Position - checkOffset))
                    {
                        Add(new Coroutine(Break()));
                        break;
                    }
                }
            }
        }
        if (player != null)
        {
            switch (brickType)
            {
                case BrickType.Ice:
                    if ((player.StateMachine.state == 5 || player.StateMachine.state == 10 || player.StateMachine.state == 2 || player.StateMachine.state == 0) &&
                        !player.Ducking)
                        if (CollideCheck(player, BottomCenter + new Vector2(0, -6)) && !breaking && player.Speed.Y <= 0)
                            Add(new Coroutine(Break()));
                    if ((CollideCheck<Shot>() || fireMode) && !breaking)
                        Add(new Coroutine(Break()));
                    else if (CollideCheck(player, BottomCenter + new Vector2(0, -6)) && !tryingToBreak)
                        Add(new Coroutine(TryBreak()));
                    break;
                case BrickType.Fortress: // Can only be broken with a red booster or badeline launch
                    if (player.StateMachine.state != 5 && player.StateMachine.state != 10)
                    {
                        if (CollideCheck(player, BottomCenter + new Vector2(0, -6)) && !tryingToBreak)
                            Add(new Coroutine(TryBreak()));
                    }
                    else
                    {
                        if (CollideCheck(player, BottomCenter + new Vector2(0, -6)) && !breaking && player.Speed.Y <= 0 &&
                            player.StateMachine.state != 5 && player.StateMachine.state != 10 && !player.Ducking &&
                            level.Session.GetFlag("KoseiHelper_BreakFortressBrick"))
                            Add(new Coroutine(Break()));
                    }
                    break;
                default:
                    // If player is not in the states: RedDash, SummitLaunch, StarFly, Dash or Normal, or if they're crouching, the block can't be broken
                    if (((player.StateMachine.state != 5 && player.StateMachine.state != 10 && player.StateMachine.state != 2 && player.StateMachine.state != 0) ||
                        player.Ducking))
                    {
                        if (CollideCheck(player, BottomCenter + new Vector2(0, -6)) && !tryingToBreak)
                            Add(new Coroutine(TryBreak()));
                    }
                    else
                    {
                        if (CollideCheck(player, BottomCenter + new Vector2(0, -6)) && !breaking && player.Speed.Y <= 0)
                            Add(new Coroutine(Break()));
                    }
                    break;
            }
        }
    }

    private IEnumerator TryBreak()
    {
        tryingToBreak = true;
        Audio.Play(bumpSound, Center);
        sprite.Position.Y = Vector2.One.Y - 3f;
        BumpActor();
        yield return 0.1f;
        sprite.Position.Y = Vector2.One.Y - 1f;
        yield return 0.2f;
        tryingToBreak = false;
        yield return null;
    }

    private IEnumerator Break()
    {
        breaking = true;
        Audio.Play(breakSound, Center);
        BumpActor();
        if (brickType == BrickType.Ice)
        {
            for (int i = 0; (float)i < base.Width / 8f; i++)
                for (int j = 0; (float)j < base.Height / 8f; j++)
                    base.Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(i * 8, 4 + j * 8), '3', false).BlastFrom(Center));
        }
        else if (brickType == BrickType.Fortress)
        {
            for (int i = 0; (float)i < base.Width / 8f; i++)
                for (int j = 0; (float)j < base.Height / 8f; j++)
                    base.Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(i * 8, 4 + j * 8), '4', false).BlastFrom(Center));
        }
        else
        {
            for (int i = 0; (float)i < base.Width / 8f; i++)
                for (int j = 0; (float)j < base.Height / 8f; j++)
                    base.Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(i * 8, 4 + j * 8), '1', false).BlastFrom(Center));
        }

        yield return null;
        RemoveSelf();
    }
    public void OnChangeMode(Session.CoreModes coreMode)
    {
        fireMode = coreMode == Session.CoreModes.Hot;
    }

    private void BumpActor()
    {
        Goomba goomba = SceneAs<Level>().Tracker.GetEntity<Goomba>();
        TheoCrystal theo = SceneAs<Level>().Tracker.GetEntity<TheoCrystal>();
        if (SceneAs<Level>().Tracker.GetEntity<Player>() != null)
        {
            if (goomba != null)
            {
                if (CollideCheck(goomba, TopCenter + new Vector2(0, -1)))
                    goomba.Killed(SceneAs<Level>().Tracker.GetEntity<Player>(), SceneAs<Level>());
            }
            if (theo != null)
            {
                if (CollideCheck(theo, TopCenter + new Vector2(0, -1)))
                    theo.HitSpinner(this);
            }
        }
    }
}