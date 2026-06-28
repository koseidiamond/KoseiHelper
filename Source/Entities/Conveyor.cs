using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Celeste.Mod.KoseiHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/Conveyor")]
[Tracked]
// Blatantly stolen from Factory Helper
public class Conveyor : Solid
{
    public float moveSpeed = 40.0f;
    public bool isBrokenDown; // TODO

    private string spriteRoot = "objects/KoseiHelper/Conveyor/blue";
    private float animationFrequency = 0.025f;

    private readonly List<Type> affectedTypes = new();

    private static ParticleType CreateParticle(float direction) => new()
    {
        Size = 1f,
        Color = Calc.HexToColor("6b675d"),
        Color2 = Calc.HexToColor("db8d2e"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 12f,
        SpeedMax = 15f,
        Acceleration = Vector2.UnitY * 2f,
        DirectionRange = MathF.PI / 2f,
        Direction = direction,
        LifeMin = 0.75f,
        LifeMax = 1.5f
    };

    private readonly ParticleType particleRight = CreateParticle(0f);
    private readonly ParticleType particleLeft = CreateParticle(MathF.PI);

    private readonly Sprite[] edgeSprites = new Sprite[2];
    private SoundSource sfx;
    private Sprite[] midSprites;

    public string reverseFlag;
    public bool movingLeft;
    public string conveyorSfx, reverseSfx;

    public Conveyor(Vector2 position, float width, string flag, string spriteRoot, string conveyorSfx)
        : base(position, width, 8, false)
    {
        this.reverseFlag = flag;
        this.spriteRoot = spriteRoot;
        this.conveyorSfx = conveyorSfx;
        CreateConveyor();
    }

    public Conveyor(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, 8, false)
    {
        moveSpeed = data.Float("moveSpeed", 40f);
        reverseFlag = data.Attr("reverseFlag", "KoseiHelper_conveyor_right");
        spriteRoot = data.Attr("spriteRoot", "objects/KoseiHelper/Conveyor/blue");
        conveyorSfx = data.Attr("conveyorSfx", "event:/env/local/09_core/conveyor_idle");
        reverseSfx = data.Attr("reverseSfx", "event:/none");
        foreach (string path in data.Attr("affectedEntities", "Celeste.TheoCrystal,Celeste.Glider")
             .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            Type type = FakeAssembly.GetFakeEntryAssembly().GetType(path);
            if (type != null)
                affectedTypes.Add(type);
            else
                Logger.Log(LogLevel.Warn, "KoseiHelper", $"Couldn't find type '{path}'.");
        }
        CreateConveyor();
    }

    private void CreateConveyor()
    {
        SetupSprites();
        Add(new LightOcclude(0.2f));
        Add(sfx = new SoundSource() { Position = new Vector2(Width / 2, Height / 2) });

        string root = spriteRoot.ToLowerInvariant();

        (Color c1, Color c2) = root switch
        {
            var s when s.Contains("red") => (Calc.HexToColor("8b2d2d"), Calc.HexToColor("ff5555")),
            var s when s.Contains("orange") => (Calc.HexToColor("8b4d2d"), Calc.HexToColor("ff9933")),
            var s when s.Contains("yellow") => (Calc.HexToColor("8b7a2d"), Calc.HexToColor("ffdd55")),
            var s when s.Contains("green") => (Calc.HexToColor("2d8b4d"), Calc.HexToColor("55ff88")),
            var s when s.Contains("cyan") => (Calc.HexToColor("2d8b8b"), Calc.HexToColor("55ffff")),
            var s when s.Contains("blue") => (Calc.HexToColor("2d4d8b"), Calc.HexToColor("55aaff")),
            var s when s.Contains("purple") => (Calc.HexToColor("5a2d8b"), Calc.HexToColor("bb55ff")),
            var s when s.Contains("pink") => (Calc.HexToColor("8b2d6b"), Calc.HexToColor("ff66cc")),
            var s when s.Contains("white") => (Calc.HexToColor("b0b0b0"), Calc.HexToColor("ffffff")),
            var s when s.Contains("black") => (Calc.HexToColor("202020"), Calc.HexToColor("606060")),
            var s when s.Contains("gold") => (Calc.HexToColor("8b6f1a"), Calc.HexToColor("ffd700")),
            var s when s.Contains("silver") => (Calc.HexToColor("6a6a6a"), Calc.HexToColor("d8d8d8")),
            _ => (Calc.HexToColor("6b675d"), Calc.HexToColor("db8d2e"))
        };

        particleLeft.Color = particleRight.Color = c1;
        particleLeft.Color2 = particleRight.Color2 = c2;
    }

    public bool IsMovingLeft => movingLeft;

    private bool GetDirection()
    {
        bool direction = true;
        if (!string.IsNullOrEmpty(reverseFlag) && SceneAs<Level>().Session.GetFlag(reverseFlag))
            direction = !direction;
        return direction;
    }

    private void SetupSprites()
    {
        int middleCount = Math.Max(1, (int)(Width / 8));
        midSprites = new Sprite[middleCount];

        for (int i = 0; i < middleCount; i++)
        {
            Add(midSprites[i] = new Sprite(GFX.Game, spriteRoot));
            midSprites[i].Add("edge", "_middle", animationFrequency, "edge");
            midSprites[i].Position = new Vector2(i * 8, 0);
        }

        bool hasEdgeSprites = GFX.Game.Has(spriteRoot + "_edge00");
        if (hasEdgeSprites)
        {
            for (int i = 0; i < 2; i++)
            {
                Add(edgeSprites[i] = new Sprite(GFX.Game, spriteRoot));
                edgeSprites[i].Add("edge", "_edge", animationFrequency, "edge");
                edgeSprites[i].Position = new Vector2((Width - 8) * i, 0);
            }
            edgeSprites[1].FlipX = true;
        }
    }

    public override void Update()
    {
        base.Update();
        bool desiredState = GetDirection();

        if (desiredState != movingLeft)
        {
            movingLeft = desiredState;
            Add(new Coroutine(Sparks()));
            ReverseConveyor(midSprites);
            ReverseConveyor(edgeSprites);
        }

        if (!string.IsNullOrEmpty(conveyorSfx) && !sfx.Playing)
            sfx.Play(conveyorSfx);

        foreach (Entity entity in Scene.Entities)
        {
            if (!affectedTypes.Any(t => t.IsInstanceOfType(entity)))
                continue;

            if (entity.Get<ConveyorMover>() == null)
            {
                entity.Add(new ConveyorMover
                {
                    OnMove = amount =>
                    {
                        if (entity is Actor actor)
                            actor.MoveH(amount * Engine.DeltaTime);
                    }
                });
            }
        }
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        movingLeft = GetDirection();
        StartAnimation();

        if (!movingLeft)
        {
            ReverseConveyor(midSprites);
            ReverseConveyor(edgeSprites);
        }
    }

    private IEnumerator Sparks()
    {
        for (int i = 0; i < 10; i++)
        {
            SceneAs<Level>().ParticlesFG.Emit(particleLeft, 1, Position + new Vector2(4, 4), new Vector2(4, 4));
            SceneAs<Level>().ParticlesFG.Emit(particleRight, 1, Position + new Vector2(Width - 4, 4), new Vector2(4, 4));
            yield return 0.05f;
        }
    }

    private void StartAnimation()
    {
        PlayAnimationInArray(midSprites);
        if (GFX.Game.Has(spriteRoot + "_edge00"))
            PlayAnimationInArray(edgeSprites);
    }

    private void ReverseConveyor(Sprite[] array)
    {
        foreach (Sprite sprite in array)
        {
            if (sprite != null)
            {
                sprite.Rate = -sprite.Rate;
            }
        }
        if (!string.IsNullOrEmpty(reverseSfx))
            sfx.Play(reverseSfx);
    }

    private void PlayAnimationInArray(Sprite[] array)
    {
        foreach (Sprite sprite in array)
        {
            if (sprite == null)
                continue;
            sprite.Play("edge");
            sprite.Rate = Math.Sign(sprite.Rate) * moveSpeed / 240f;
        }
    }

    private static void MovePlayer(On.Celeste.Player.orig_ctor orig, Player self, Vector2 position, PlayerSpriteMode spriteMode)
    {
        orig(self, position, spriteMode);
        ConveyorMover conveyorMover = new()
        {
            OnMove = (amount) =>
            {
                if (self.StateMachine.State != Player.StClimb)
                {
                    self.MoveH(amount * Engine.DeltaTime);
                }
            }
        };
        self.Add(conveyorMover);
    }

    public static void Load()
    {
        On.Celeste.Player.ctor += MovePlayer;
    }

    public static void Unload()
    {
        On.Celeste.Player.ctor -= MovePlayer;
    }
}