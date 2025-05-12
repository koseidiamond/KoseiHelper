using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/SpecialFlutterbird")]
[Tracked]
public class PregnantFlutterbird : Actor
{

    private Sprite sprite;
    private Vector2 start, currentPosition;
    private Coroutine routine;
    public bool flyingAway;
    private SoundSource tweetingSfx;
    private SoundSource flyawaySfx;
    private int customDepth;
    public EntityID entityID;

    private Vector2 Speed;
    private float noGravityTimer;

    public int childrenCount;
    public float timeToGiveBirth;
    public bool chaser;
    public float hoppingDistance; // unused for now
    public float scaredDistance;
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
    public bool polyamorous = true;
    public int partnerID;
    public Orientation orientation;
    public bool shootLasers;
    public bool killOnContact;
    public bool bouncy;
    public bool flyAway;
    public string flyAwayFlag;
    public string sterilizationFlag;
    public bool squishable;
    public string hopSfx;
    public string birthSfx;
    public string spriteID;
    public bool emitLight;
    public bool coyote;
    public bool explodes;

    private bool baby;
    private float birthCooldown;
    private SoundSource laserSfx;
    public ParticleType deathParticle;
    public ParticleType explosionParticle;
    private Circle pushRadius;
    private Color birdColor;

    public PregnantFlutterbird(EntityData data, Vector2 offset, EntityID eid) : base(data.Position + offset)
    {
        entityID = eid;
        start = currentPosition = Position;
        this.baby = false;
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
        polyamorous = data.Bool("polyamorous", true);
        partnerID = data.Int("partnerID", 0);
        hoppingDistance = data.Float("hoppingDistance", 8f);
        scaredDistance = data.Float("scaredDistance", 48f);
        explodes = data.Bool("explodes", false);
        if (shootLasers)
            Add(new Coroutine(ShootLasers()));
        killOnContact = data.Bool("killOnContact", false);
        bouncy = data.Bool("bouncy", false);
        flyAway = data.Bool("flyAway", true);
        flyAwayFlag = data.Attr("flyAwayFlag", "");
        sterilizationFlag = data.Attr("sterilizationFlag", "");
        squishable = data.Bool("squishable", true);
        hopSfx = data.Attr("hopSfx", "event:/game/general/birdbaby_hop");
        birthSfx = data.Attr("birthSfx", "event:/game/09_core/frontdoor_heartfill");
        spriteID = data.Attr("spriteID", "flutterbird");
        Depth = customDepth = data.Int("depth", -9999);
        coyote = data.Bool("coyote", false);
        emitLight = data.Bool("emitLight", false);
        Add(sprite = GFX.SpriteBank.Create(spriteID));
        sprite.Color = birdColor = data.HexColor("color", Color.White);

        if (emitLight)
        {
            Add(new BloomPoint(0.75f, 12f));
            Add(new VertexLight(Color.White, 0.8f, 12, 28));
        }
        Add(laserSfx = new SoundSource());
        pushRadius = new Circle(6f);
    }

    // this constructor is used when a baby bird is born.
    // Babies don't have a partner and it doesn't matter if they are poly or not because THEY CAN'T REPRODUCE (they are minors).
    public PregnantFlutterbird(Vector2 position, int childrenCount, float timeToGiveBirth, bool chaser, Gender gender, Orientation orientation, bool shootLasers,
        bool killOnContact, bool bouncy, bool flyAway, string flyAwayFlag, string sterilizationFlag, bool squishable, string hopSfx, string birthSfx, string spriteID,
        int depth, bool emitLight, bool coyote, float hoppingDistance, float scaredDistance, Color color, bool explodes) : base(position)
    {
        this.start = this.currentPosition = this.Position;
        this.Position.Y += 1f;
        this.baby = true;
        Add(routine = new Coroutine(IdleRoutine()));
        Add(this.flyawaySfx = new SoundSource());
        Add(tweetingSfx = new SoundSource());
        tweetingSfx.Play("event:/game/general/birdbaby_tweet_loop");

        base.Collider = new Hitbox(4f, 4f, -2f, -4f);
        Add(new PlayerCollider(OnPlayer));

        this.childrenCount = childrenCount;
        this.timeToGiveBirth = this.birthCooldown = timeToGiveBirth;
        this.chaser = chaser;
        this.gender = gender;
        this.orientation = orientation;
        this.hoppingDistance = hoppingDistance;
        this.scaredDistance = scaredDistance;
        this.shootLasers = shootLasers;
        if (this.shootLasers)
            Add(new Coroutine(ShootLasers()));
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
        this.explodes = explodes;

        Add(sprite = GFX.SpriteBank.Create(this.spriteID));
        this.sprite.Color = color;
        if (this.emitLight)
        {
            Add(new BloomPoint(0.75f, 12f));
            Add(new VertexLight(Color.White, 0.8f, 12, 28));
        }
        Add(laserSfx = new SoundSource());
        pushRadius = new Circle(6f);
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
        explosionParticle = new ParticleType(Seeker.P_Regen)
        {
            Color = birdColor,
            Color2 = birdColor
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
            if (explodes)
                Explode();
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
            if (!chaser)
                Speed.Y = Calc.Approach(Speed.Y, 200f, num * Engine.DeltaTime);
        }
        if (!flyingAway)
            MoveV(Speed.Y * Engine.DeltaTime);

        SquishCallback = (CollisionData d) =>
        {
            if (!TrySquishWiggle(d, 2, 2) && squishable)
            {
                Die();
            }
        };

        if (!baby && childrenCount > 0 && (string.IsNullOrEmpty(sterilizationFlag) || !level.Session.GetFlag(sterilizationFlag))) // If the bird is fertile and an adult, it will try to reproduce.
        {
            if (this.orientation == Orientation.Self && Scene.OnInterval(timeToGiveBirth)) // Self reproduction is separated from other orientations
                GiveBirth();

            foreach (PregnantFlutterbird otherbird in Scene.Tracker.GetEntities<PregnantFlutterbird>())
            {
                // If both birds are fertile then...
                if (CollideCheck(otherbird, this.Center) && otherbird != this && // The partner can't be sterile either
                    (string.IsNullOrEmpty(otherbird.sterilizationFlag) || !level.Session.GetFlag(otherbird.sterilizationFlag)))
                {
                    if (entityID.ID < otherbird.entityID.ID &&
                        ((polyamorous && otherbird.polyamorous) ||
                        (!polyamorous && !otherbird.polyamorous && otherbird.entityID.ID == partnerID && otherbird.partnerID == entityID.ID)))
                        // the bird with the lowest entity id has the dominant alleles. poly x poly or mono x mono
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
        }

        // Handle cooldown
        if (birthCooldown > 0)
            birthCooldown -= Engine.DeltaTime;
        else
            birthCooldown = 0f;
        if (chaser && player != null)
        {
            sprite.Play("fly");
            if (level.IsInCamera(Center, 40f))
            {
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
            Laser laser = Engine.Pooler.Create<Laser>().Init(this, player, 1.4f, 0.9f);
            laser.owner = this;
            SceneAs<Level>().Add(laser);
        }
        yield return 1.4f;
        laserSfx.Stop();
        Audio.Play("event:/char/badeline/boss_laser_fire", Position);
    }

    private void Explode()
    {
        Collider collider = base.Collider;
        base.Collider = pushRadius;
        Audio.Play("event:/new_content/game/10_farewell/puffer_splode", Position);
        Player player = CollideFirst<Player>();
        if (player != null && !base.Scene.CollideCheck<Solid>(Position, player.Center))
        {
            player.ExplodeLaunch(Position, snapUp: false, sidesOnly: true);
        }
        TheoCrystal theoCrystal = CollideFirst<TheoCrystal>();
        if (theoCrystal != null && !base.Scene.CollideCheck<Solid>(Position, theoCrystal.Center))
        {
            theoCrystal.ExplodeLaunch(Position);
        }
        foreach (TempleCrackedBlock entity in base.Scene.Tracker.GetEntities<TempleCrackedBlock>())
        {
            if (CollideCheck(entity))
            {
                entity.Break(Position);
            }
        }
        foreach (TouchSwitch entity2 in base.Scene.Tracker.GetEntities<TouchSwitch>())
        {
            if (CollideCheck(entity2))
            {
                entity2.TurnOn();
            }
        }
        foreach (FloatingDebris entity3 in base.Scene.Tracker.GetEntities<FloatingDebris>())
        {
            if (CollideCheck(entity3))
            {
                entity3.OnExplode(Position);
            }
        }
        base.Collider = collider;
        Level level = SceneAs<Level>();
        level.Shake();
        level.Displacement.AddBurst(Position, 0.4f, 12f, 36f, 0.5f);
        level.Displacement.AddBurst(Position, 0.4f, 24f, 48f, 0.5f);
        level.Displacement.AddBurst(Position, 0.4f, 36f, 60f, 0.5f);
        for (float num = 0f; num < MathF.PI * 2f; num += 0.17453292f)
        {
            Vector2 position = base.Center + Calc.AngleToVector(num + Calc.Random.Range(-MathF.PI / 90f, MathF.PI / 90f), Calc.Random.Range(12, 18));
            level.Particles.Emit(explosionParticle, position, num);
        }
        Die();
    }

    public void GiveBirth()
    {
        if (birthCooldown <= 0f)
        {
            this.birthCooldown = this.timeToGiveBirth;
            childrenCount--;
            Audio.Play(birthSfx, Center);
            SceneAs<Level>().Particles.Emit(ParticleTypes.SparkyDust, Position, Color.Pink);
            Scene.Add(new PregnantFlutterbird(Position, childrenCount, timeToGiveBirth, chaser, gender, orientation, shootLasers, killOnContact, bouncy, flyAway, flyAwayFlag,
                sterilizationFlag, squishable, hopSfx, birthSfx, spriteID, Depth, emitLight, coyote, hoppingDistance, scaredDistance, sprite.Color, explodes));
        }
    }

    private IEnumerator IdleRoutine()
    {
        while (true)
        {
            Player player = Scene.Tracker.GetEntity<Player>();
            float delay = 0.25f + Calc.Random.NextFloat(1f);
            for (float p2 = 0f; p2 < delay; p2 += Engine.DeltaTime) // TODO customize the scared distance
            {
                if (player != null && this.flyAway && Math.Abs(player.X - X) < scaredDistance && player.Y > Y - 40f && player.Y < Y + 8f)
                {
                    FlyAway(Math.Sign(X - player.X), Calc.Random.NextFloat(0.2f));
                }
                yield return null;
            }
            if (OnGround(new Vector2(CenterX, CenterY + 2f)))
                {
                if (!chaser)
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
                        //Position = bezier.GetPoint(p2);
                        MoveTowardsX(bezier.GetPoint(p2).X, 8f);
                        MoveTowardsY(bezier.GetPoint(p2).Y, 8f);
                        yield return null;
                    }
                    sprite.Scale.X = (float)Math.Sign(sprite.Scale.X) * 1.4f;
                    sprite.Scale.Y = 0.6f;
                    //Position = target;
                    MoveTowardsX(target.X, 1f);
                    MoveTowardsY(target.Y, 1f);
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
        level.ParticlesFG.Emit(Calc.Random.Choose(ParticleTypes.Dust), Position, -MathF.PI / 2f);
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
        Die();
    }

    public void Die()
    {
        Level level = SceneAs<Level>();
        if (!flyingAway) // doesn't make sense to actually "die" if they are just oob
        {
            Audio.Play("event:/game/05_mirror_temple/eyebro_eyemove", Position);
            level.Particles.Emit(deathParticle, Center, Color.Red, -1f);
        }
        foreach (Laser laser in level.Tracker.GetEntities<Laser>())
        {
            if (laser.owner == this)
                laser.Destroy();
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
        if (chaser && SceneAs<Level>().IsInCamera(Center, 40f))
            Draw.Line(this.Center, SceneAs<Level>().Tracker.GetEntity<Player>().Center, Color.Red);
        base.DebugRender(camera);
    }
}