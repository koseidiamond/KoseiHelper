using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/PregnantFlutterbird")]
[Tracked]
public class PregnantFlutterbird : Actor
{
    private static Color[] colors = new Color[4]
    {
        Calc.HexToColor("89fbff"),
        Calc.HexToColor("f0fc6c"),
        Calc.HexToColor("f493ff"),
        Calc.HexToColor("93baff")
    };

    private Sprite sprite;
    private Vector2 start, currentPosition;
    private Coroutine routine;
    private bool flyingAway;
    private SoundSource tweetingSfx;
    private SoundSource flyawaySfx;
    private int customDepth;

    private Vector2 Speed;
    private float noGravityTimer;

    public int childrenCount;
    public float timeToGiveBirth;
    public bool chaser;
    public enum Gender
    {
        Nonbinary,
        Male,
        Female
    };
    public Gender gender;
    public enum Orientation
    {
        Gay,
        Straight,
        Asexual,
        Bisexual,
        Self
    };
    public Orientation orientation; // done (hopefully)
    public bool shootLasers;
    public bool killOnContact; // done
    public bool bouncy;
    public bool flyAway; // done
    public string flyAwayFlag; // done
    public string sterilizationFlag; // done
    public bool squishable;
    public string hopSfx; // done
    public string birthSfx; // done
    public string spriteID; // done
    public bool emitLight; // done
    public bool coyote; // done

    private float birthCooldown;
    private SoundSource laserSfx;
    public static ParticleType deathParticle;

    public PregnantFlutterbird(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        start = currentPosition = Position;
        
        Add(routine = new Coroutine(IdleRoutine()));
        Add(flyawaySfx = new SoundSource());
        Add(tweetingSfx = new SoundSource());
        tweetingSfx.Play("event:/game/general/birdbaby_tweet_loop");
        base.Collider = new Hitbox(4f, 4f, -2f, -4f);
        Add(new PlayerCollider(OnPlayer));

        //Read the custom properties from data
        childrenCount = data.Int("childrenCount", 1);
        timeToGiveBirth = data.Float("timeToGiveBirth", 5);
        chaser = data.Bool("chaser", false);
        gender = data.Enum("gender", Gender.Nonbinary);
        orientation = data.Enum("orientation", Orientation.Gay);
        shootLasers = data.Bool("shootLasers", false);
        killOnContact = data.Bool("killOnContact", false);
        bouncy = data.Bool("bouncy", false);
        flyAway = data.Bool("flyAway", true);
        flyAwayFlag = data.Attr("flyAwayFlag", "");
        sterilizationFlag = data.Attr("sterilizationFlag", "");
        squishable = data.Bool("squishable", false);
        hopSfx = data.Attr("hopSfx", "event:/game/general/birdbaby_hop");
        birthSfx = data.Attr("birthSfx", "event:/game/09_core/frontdoor_heartfill");
        spriteID = data.Attr("spriteID", "flutterbird");
        Depth = customDepth = data.Int("depth", -9999);
        coyote = data.Bool("coyote", false);

        Add(sprite = GFX.SpriteBank.Create(spriteID));
        sprite.Color = Calc.Random.Choose(colors);

        if (emitLight)
        {
            Add(new BloomPoint(0.45f, 8f));
            Add(new VertexLight(Color.White, 0.45f, 12, 28));
        }
    }

    // this constructor is used when a baby bird is born
    public PregnantFlutterbird(Vector2 position, int childrenCount, float timeToGiveBirth, bool chaser, Gender gender, Orientation orientation, bool shootLasers,
        bool killOnContact, bool bouncy, bool flyAway, string flyAwayFlag, string sterilizationFlag, bool squishable, string hopSfx, string birthSfx, string spriteID,
        int depth, bool emitLight, bool coyote) : base(position)
    {
        this.start = this.currentPosition = this.Position;

        Add(routine = new Coroutine(IdleRoutine()));
        Add(this.flyawaySfx = new SoundSource());
        Add(tweetingSfx = new SoundSource());
        tweetingSfx.Play("event:/game/general/birdbaby_tweet_loop");

        base.Collider = new Hitbox(4f, 4f, -2f, -2f);
        Add(new PlayerCollider(OnPlayer));

        this.childrenCount = childrenCount;
        this.timeToGiveBirth = this.birthCooldown = timeToGiveBirth;
        this.chaser = chaser;
        this.gender = gender;
        this.orientation = orientation;
        this.shootLasers = shootLasers;
        this.killOnContact = killOnContact;
        this.bouncy = bouncy;
        this.flyAwayFlag = flyAwayFlag;
        this.sterilizationFlag = sterilizationFlag;
        this.squishable = squishable;
        this.hopSfx = hopSfx;
        this.birthSfx = birthSfx;
        this.spriteID = spriteID;
        this.Depth = depth;
        this.emitLight = emitLight;
        this.coyote = coyote;

        Add(sprite = GFX.SpriteBank.Create(this.spriteID));
        sprite.Color = Calc.Random.Choose(colors);

        if (this.emitLight)
        {
            Add(new BloomPoint(0.45f, 8f));
            Add(new VertexLight(Color.White, 0.45f, 12, 28));
        }
    }


    public override void Added(Scene scene)
    {
        deathParticle = new ParticleType(ParticleTypes.Dust)
        {
            LifeMin = 1.5f,
            LifeMax = 2.25f,
            Friction = 2f,
            SpeedMax = -2f,
            SpeedMin = 0.25f
        };
        base.Added(scene);
    }

    private void OnPlayer(Player player)
    {
        if (player.Scene != null)
        {
            if (killOnContact)
                player.Die(player.Center);
            if (bouncy)
            {
                //player.PointBounce(Center);
                player.Bounce(CenterY);
            }
            if (coyote)
                player.jumpGraceTimer = 0.15f;
        }
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        sprite.Scale.X = Calc.Approach(sprite.Scale.X, Math.Sign(sprite.Scale.X), 4f * Engine.DeltaTime);
        sprite.Scale.Y = Calc.Approach(sprite.Scale.Y, 1f, 4f * Engine.DeltaTime);

        if (noGravityTimer > 0f)
            noGravityTimer -= Engine.DeltaTime;
        else
            noGravityTimer = 0f;

            float num = 800f;
        if (Math.Abs(Speed.Y) <= 30f)
        {
            num *= 0.5f;
        }
        float num2 = 350f;
        if (Speed.Y < 0f)
        {
            num2 *= 0.5f;
        }
        Speed.X = Calc.Approach(Speed.X, 0f, num2 * Engine.DeltaTime);
        if (noGravityTimer > 0f)
        {
            noGravityTimer -= Engine.DeltaTime;
        }
        else
        {
            Speed.Y = Calc.Approach(Speed.Y, 200f, num * Engine.DeltaTime);
        }
        if (!flyingAway)
            MoveV(Speed.Y * Engine.DeltaTime);

        SquishCallback = (CollisionData d) =>
        {
            if (!TrySquishWiggle(d, 2, 2) && squishable)
            {
                Audio.Play("event:/game/05_mirror_temple/eyebro_eyemove", Position);
                SceneAs<Level>().Particles.Emit(deathParticle, Center, Color.Red, - 1f);
                RemoveSelf();
            }
        };

        if (string.IsNullOrEmpty(sterilizationFlag) || !level.Session.GetFlag(sterilizationFlag)) // If the bird is fertile, it will try to reproduce
        {
            if (this.orientation == Orientation.Self && Scene.OnInterval(timeToGiveBirth)) // Self reproduction is separated from other orientations
                GiveBirth();

            foreach (PregnantFlutterbird otherbird in Scene.Tracker.GetEntities<PregnantFlutterbird>())
            {
                // If both birds are fertile then...
                if (CollideCheck(otherbird, this.Center) && string.IsNullOrEmpty(otherbird.sterilizationFlag) || !level.Session.GetFlag(otherbird.sterilizationFlag))
                {
                    switch (this.gender)
                        // We make sure they are attracted to each other based on gender/orientation of both of them
                    {
                        case Gender.Male:
                            switch (this.orientation)
                            {
                                case Orientation.Gay: // Gay x Gay/Bi males
                                    if (otherbird.gender == Gender.Male && (otherbird.orientation == Orientation.Gay || otherbird.orientation == Orientation.Bisexual))
                                        GiveBirth();
                                    break;
                                case Orientation.Bisexual: // Bi x anyone attracted to males
                                    if (otherbird.gender == Gender.Male && (otherbird.orientation == Orientation.Gay || otherbird.orientation == Orientation.Bisexual) ||
                                        otherbird.gender == Gender.Female && (otherbird.orientation == Orientation.Straight || otherbird.orientation == Orientation.Bisexual) ||
                                        otherbird.gender == Gender.Nonbinary && otherbird.orientation == Orientation.Straight)
                                        GiveBirth();
                                    break;
                                case Orientation.Straight: // Straight x straight/bi females
                                    if (otherbird.gender == Gender.Female && (otherbird.orientation == Orientation.Straight || otherbird.orientation == Orientation.Bisexual))
                                        GiveBirth();
                                    break;
                                case Orientation.Asexual:
                                    // Asexual males do not reproduce
                                    break;
                                case Orientation.Self:
                                    // Logic handled separatedly
                                    break;
                            }
                            break;
                        case Gender.Female:
                            switch (this.orientation)
                            {
                                case Orientation.Gay: // Lesbian x Lesbian/Bi females
                                    if (otherbird.gender == Gender.Female && (otherbird.orientation == Orientation.Gay || otherbird.orientation == Orientation.Bisexual))
                                        GiveBirth();
                                    break;
                                case Orientation.Bisexual: // Bi x anyone attracted to females
                                    if (otherbird.gender == Gender.Female && (otherbird.orientation == Orientation.Gay || otherbird.orientation == Orientation.Bisexual) ||
                                        otherbird.gender == Gender.Male && (otherbird.orientation == Orientation.Straight || otherbird.orientation == Orientation.Bisexual) ||
                                        otherbird.gender == Gender.Nonbinary && otherbird.orientation == Orientation.Straight)
                                        GiveBirth();
                                    break;
                                case Orientation.Straight: // Straight x straight/bi females
                                    if (otherbird.gender == Gender.Male && (otherbird.orientation == Orientation.Straight || otherbird.orientation == Orientation.Bisexual))
                                        GiveBirth();
                                    break;
                                case Orientation.Asexual:
                                    // Asexual females do not reproduce
                                    break;
                                case Orientation.Self:
                                    // Logic handled separatedly
                                    break;
                            }
                            break;
                        case Gender.Nonbinary:
                            switch (this.orientation)
                            {
                                case Orientation.Gay: // enby x Gay/Bi enby
                                    if (otherbird.gender == Gender.Nonbinary && (otherbird.orientation == Orientation.Gay || otherbird.orientation == Orientation.Bisexual))
                                        GiveBirth();
                                    break;
                                case Orientation.Bisexual: // enby x anyone attracted to enbies
                                    if (otherbird.gender == Gender.Female && otherbird.orientation == Orientation.Bisexual ||
                                        otherbird.gender == Gender.Male && otherbird.orientation == Orientation.Bisexual ||
                                        otherbird.gender == Gender.Nonbinary && (otherbird.orientation == Orientation.Gay || otherbird.orientation == Orientation.Bisexual))
                                        GiveBirth();
                                    break;
                                case Orientation.Straight: // enby x bi males/females
                                    if (otherbird.orientation == Orientation.Bisexual && (otherbird.gender == Gender.Male || otherbird.gender == Gender.Female))
                                        GiveBirth();
                                    break;
                                case Orientation.Asexual:
                                    // Asexual nonbinary birds do not reproduce
                                    break;
                                case Orientation.Self:
                                    // Logic handled separatedly
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        // Handle cooldown
        if (birthCooldown > 0)
            birthCooldown -= Engine.DeltaTime;
        else
            birthCooldown = 0f;
        if (chaser)
        {
            sprite.Play("fly");
            if (Depth < Depths.FakeWalls)
                Position = Calc.Approach(Position, player.Position, 1f);
            else
            {
                MoveTowardsX(player.Position.X, 1f);
                MoveTowardsY(player.Position.Y, 0.8f);
            }
            if (player.Position.X < Position.X)
                sprite.FlipX = true;
            else
                sprite.FlipX = false;
        }
        base.Update();
    }

    public IEnumerator ShootLasers()
    {
        while (true)
        {
            yield return Beam();
            yield return 0.7f;
        }
    }

    private IEnumerator Beam()
    {
        Level level = SceneAs<Level>();
        laserSfx.Play("event:/char/badeline/boss_laser_charge");
        yield return 0.1f;
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            //level.Add(Engine.Pooler.Create<FinalBossBeam>().Init(this, player));
            // TODO properly create a beam
        }
        yield return 1.4f;
        laserSfx.Stop();
        Audio.Play("event:/char/badeline/boss_laser_fire", Position);
    }

    public void GiveBirth()
    {
        Logger.Debug(nameof(KoseiHelperModule), $"A bird has become pregnant! GG! Birth cooldown: {birthCooldown}");
        if (birthCooldown <= 0f)
        {
            this.birthCooldown = this.timeToGiveBirth;
            Audio.Play(birthSfx, Center);
            SceneAs<Level>().Particles.Emit(ParticleTypes.SparkyDust, Position, Color.Pink);
            Scene.Add(new PregnantFlutterbird(Position, childrenCount, timeToGiveBirth, chaser, gender, orientation, shootLasers, killOnContact, bouncy, flyAway, flyAwayFlag,
                sterilizationFlag, squishable, hopSfx, birthSfx, spriteID, Depth, emitLight, coyote));
            
        }
    }

    private IEnumerator IdleRoutine()
    {
        while (true)
        {
            Player player = Scene.Tracker.GetEntity<Player>();

            float delay = 0.25f + Calc.Random.NextFloat(1f);
            for (float p2 = 0f; p2 < delay; p2 += Engine.DeltaTime)
            {
                if (player != null && this.flyAway && Math.Abs(player.X - X) < 48f && player.Y > Y - 40f && player.Y < Y + 8f)
                {
                    FlyAway(Math.Sign(X - player.X), Calc.Random.NextFloat(0.2f));
                }
                yield return null;
            }
            if (OnGround(new Vector2(CenterX, CenterY + 3f)))
                {
                Audio.Play(hopSfx, Position);
                noGravityTimer = 0.1f;
                currentPosition = Position;
                Vector2 target = currentPosition + new Vector2(-4f + Calc.Random.NextFloat(8f), 0f);
                sprite.Scale.X = Math.Sign(target.X - Position.X);
                if (!chaser)
                {
                    SimpleCurve bezier = new SimpleCurve(Position, target, (Position + target) / 2f - Vector2.UnitY * 14f);
                    for (float p2 = 0f; p2 < 1f; p2 += Engine.DeltaTime * 4f)
                    {
                        Position = bezier.GetPoint(p2);
                        yield return null;
                    }
                    sprite.Scale.X = (float)Math.Sign(sprite.Scale.X) * 1.4f;
                    sprite.Scale.Y = 0.6f;
                    Position = target;
                }
            }
        }
    }

    private IEnumerator FlyAwayRoutine(int direction, float delay)
    {
        Level level = Scene as Level;
        yield return delay;
        sprite.Play("fly");
        if (!string.IsNullOrEmpty(flyAwayFlag))
            SceneAs<Level>().Session.SetFlag(flyAwayFlag, true);
        sprite.Scale.X = (float)(-direction) * 1.25f;
        sprite.Scale.Y = 1.25f;
        level.ParticlesFG.Emit(Calc.Random.Choose<ParticleType>(ParticleTypes.Dust), Position, -MathF.PI / 2f);
        Vector2 from = Position;
        Vector2 to = Position + new Vector2(direction * 4, -8f);
        for (float p = 0f; p < 1f; p += Engine.DeltaTime * 3f)
        {
            Position = from + (to - from) * Ease.CubeOut(p);
            yield return null;
        }
        Depth = customDepth - 2;
        sprite.Scale.X = 0f - sprite.Scale.X;
        to = new Vector2(direction, -4f) * 8f;
        while (Y + 8f > (float)level.Bounds.Top)
        {
            to += new Vector2(direction * 64, -128f) * Engine.DeltaTime;
            Position += to * Engine.DeltaTime;
            if (Scene.OnInterval(0.1f) && Y > level.Camera.Top + 32f)
            {
                foreach (Entity entity in Scene.Tracker.GetEntities<FlutterBird>())
                {
                    if (Math.Abs(X - entity.X) < 48f && Math.Abs(Y - entity.Y) < 48f && !(entity as FlutterBird).flyingAway)
                    {
                        (entity as FlutterBird).FlyAway(direction, Calc.Random.NextFloat(0.25f));
                    }
                }
            }
            yield return null;
        }
        Scene.Remove(this);
        RemoveSelf(); // just in case idk
    }

    public void FlyAway(int direction, float delay)
    {
        if (!flyingAway)
        {
            tweetingSfx.Stop();
            flyingAway = true;
            flyawaySfx.Play("event:/game/general/birdbaby_flyaway");
            Remove(routine);
            Add(routine = new Coroutine(FlyAwayRoutine(direction, delay)));
        }
    }

    public override void DebugRender(Camera camera)
    {
        if (chaser)
            Draw.Line(this.Center, SceneAs<Level>().Tracker.GetEntity<Player>().Center, Color.Red);
        base.DebugRender(camera);
    }
}