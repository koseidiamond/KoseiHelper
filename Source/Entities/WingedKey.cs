using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/WingedKey")]
[TrackedAs(typeof(Key))]
public class WingedKey : Key
{

    private Wiggler rotateWiggler;
    private bool flyingAway, isCollected = false;
    public float flapSpeed;
    public Sprite wingSprite;
    public new EntityID ID;
    private Vector2 start;

    public WingedKey(EntityData data, Vector2 offset, EntityID eid) : base(data.Position + offset, eid, data.NodesOffset(offset))
    {
        ID = eid;
        Position = (start = data.Position + offset);
        flapSpeed = -data.Float("flapSpeed", 200f);
        Add(wingSprite = GFX.SpriteBank.Create("koseiHelper_key"));
        //sprite.CenterOrigin();
        wingSprite.Play("idle");
        sprite.Visible = false;
        Add(new DashListener { OnDash = OnDashFlyAway });
        base.Collider = new Hitbox(12f, 12f, -6f, -6f);
        Get<PlayerCollider>().OnCollide = OnPlayer;
        Add(follower = new Follower(eid));
        Add(new TransitionListener
        {
            OnOut = delegate
            {
                StartedUsing = false;
                if (!IsUsed)
                {
                    if (tween != null)
                    {
                        tween.RemoveSelf();
                        tween = null;
                    }

                    if (alarm != null)
                    {
                        alarm.RemoveSelf();
                        alarm = null;
                    }

                    Turning = false;
                    Visible = true;
                    sprite.Visible = false;
                    sprite.Rate = 1f;
                    sprite.Scale = Vector2.One;
                    sprite.Play("idle");
                    sprite.Rotation = 0f;
                    wiggler.Stop();
                    follower.MoveTowardsLeader = true;
                }
            }
        });
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        sprite.Visible = false;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        Add(rotateWiggler = Wiggler.Create(0.5f, 3f, (float v) =>
        {
            wingSprite.Rotation = v * 15f * (MathF.PI / 180f);
        }));
    }

    public new void OnPlayer(Player player)
    {
        Logger.Debug(nameof(KoseiHelperModule), $"A winged key has been collected.");
        SceneAs<Level>().Particles.Emit(P_Collect, 10, Position, Vector2.One * 3f);
        Audio.Play("event:/game/general/key_get", Position);
        Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
        player.Leader.GainFollower(follower);
        Collidable = false;
        Session session = SceneAs<Level>().Session;
        session.DoNotLoad.Add(ID);
        session.Keys.Add(ID);
        session.UpdateLevelStartDashes();
        wiggler.Start();
        base.Depth = -1000000;
        if (nodes != null && nodes.Length >= 2)
        {
            Add(new Coroutine(NodeRoutine(player)));
        }
        isCollected = true;
        Remove(Get<DashListener>());
    }

    public override void Update()
    {
        wingSprite.Play("idle");
        base.Update();
        {
            base.Y += flapSpeed * Engine.DeltaTime;
            if (flyingAway)
            {
                if (base.Y < (float)(SceneAs<Level>().Bounds.Top - 16))
                    RemoveSelf();
            }
            else
            {
                flapSpeed = Calc.Approach(flapSpeed, 20f, 170f * Engine.DeltaTime);
                if (base.Y < start.Y - 5f)
                    base.Y = start.Y - 5f;
                else if (base.Y > start.Y + 5f)
                    base.Y = start.Y + 5f;
            }
        }

    }

    public void OnDashFlyAway(Vector2 dir)
    {
        if (!flyingAway && !isCollected)
        {
            Add(new Coroutine(FlyAwayCoroutine()));
            flyingAway = true;
        }
    }

    private IEnumerator FlyAwayCoroutine()
    {
        if (!isCollected)
        {
            rotateWiggler.Start();
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, start: true);
            tween.OnUpdate = (Tween t) => { flapSpeed = MathHelper.Lerp(-200f, 0f, t.Eased); };
            Add(tween);
            yield return 0.1f;
            Audio.Play("event:/game/general/strawberry_laugh", base.Position);
            yield return 0.2f;
            Audio.Play("event:/game/general/strawberry_flyaway", Position);
            tween = Tween.Create(Tween.TweenMode.Oneshot, null, 0.5f, start: true);
            tween.OnUpdate = (Tween t) => { flapSpeed = MathHelper.Lerp(0f, -200f, t.Eased); };
            Add(tween);
        }
    }
}