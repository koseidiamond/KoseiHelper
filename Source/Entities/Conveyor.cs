using Celeste.Mod.Entities;
using Celeste.Mod.KoseiHelper.Components;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/Conveyor")]
[Tracked]
// Blatantly stolen from Factory Helper
public class Conveyor : Solid
{
    public float moveSpeed = 40.0f;
    public bool isBrokenDown;

    private string spriteRoot = "objects/KoseiHelper/Conveyor/blue";
    private float animationFrequency = 0.025f;

    private ParticleType particleRight = new()
    {
        Size = 1f,
        Color = Calc.HexToColor("6b675d"),
        Color2 = Calc.HexToColor("db8d2e"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 12f,
        SpeedMax = 15f,
        Acceleration = Vector2.UnitY * 2f,
        DirectionRange = (float)Math.PI / 2f,
        Direction = 0,
        LifeMin = 0.75f,
        LifeMax = 1.5f
    };
    private ParticleType particleLeft = new()
    {
        Size = 1f,
        Color = Calc.HexToColor("6b675d"),
        Color2 = Calc.HexToColor("db8d2e"),
        ColorMode = ParticleType.ColorModes.Blink,
        FadeMode = ParticleType.FadeModes.Late,
        SpeedMin = 12f,
        SpeedMax = 15f,
        Acceleration = Vector2.UnitY * 2f,
        DirectionRange = (float)Math.PI / 2f,
        Direction = (float)Math.PI,
        LifeMin = 0.75f,
        LifeMax = 1.5f
    };

    private readonly Sprite[] edgeSprites = new Sprite[2];
    private SoundSource sfx;
    private Sprite[] midSprites;

    public string reverseFlag;
    public bool movingLeft;
    public string conveyorSfx;

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

    private bool GetDesiredDirection()
    {
        bool direction = true;

        if (GetFlagState())
            direction = !direction;

        return direction;
    }


    private void SetupSprites()
    {
        bool hasEdge = GFX.Game.Has(spriteRoot + "_edge");

        for (int i = 0; i < 2; i++)
        {
            Add(edgeSprites[i] = new Sprite(GFX.Game, spriteRoot));

            string anim = hasEdge ? "_edge" : "_middle";
            edgeSprites[i].Add("left", anim, animationFrequency, "left");

            edgeSprites[i].Position = new Vector2((Width - 8) * i, 0);
        }

        if (hasEdge) // flip second edge sprite
        {
            edgeSprites[1].Rotation = (float)Math.PI;
            edgeSprites[1].Position += new Vector2(8, 8);
        }

        int middleCount = Math.Max(0, (int)(Width / 8) - 2);
        midSprites = new Sprite[middleCount];

        for (int i = 0; i < midSprites.Length; i++)
        {
            Add(midSprites[i] = new Sprite(GFX.Game, spriteRoot));
            midSprites[i].Add("left", "_middle", animationFrequency, "left");
            midSprites[i].Position = new Vector2(8 + (8 * i), 0);
        }
    }

    public override void Update()
    {
        base.Update();

        bool desiredState = GetDesiredDirection();

        if (desiredState != movingLeft)
        {
            movingLeft = desiredState;
            Add(new Coroutine(Sparks()));
            ReverseConveyorDirection();
        }

        if (!string.IsNullOrEmpty(conveyorSfx) && !sfx.Playing)
        {
            sfx.Play(conveyorSfx);
        }
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        movingLeft = GetDesiredDirection();
        StartAnimation();
        if (!movingLeft)
            ReverseConveyorDirection();
    }

    private bool GetFlagState()
    {
        return string.IsNullOrEmpty(reverseFlag) ? false : SceneAs<Level>().Session.GetFlag(reverseFlag);
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
        PlayAnimationInArray(edgeSprites);
        //edgeSprites[1].SetAnimationFrame(4);
    }

    private void ReverseConveyorDirection()
    {
        ReverseAnimationInArray(midSprites);
        ReverseAnimationInArray(edgeSprites);
    }

    private void ReverseAnimationInArray(Sprite[] array)
    {
        foreach (Sprite sprite in array)
        {
            sprite.Rate = -sprite.Rate;
        }
    }

    private void PlayAnimationInArray(Sprite[] array)
    {
        foreach (Sprite sprite in array)
        {
            sprite.Play("left");
            sprite.Rate = Math.Sign(sprite.Rate) * moveSpeed / 240f;
        }
    }

    private static void MovePlayer(On.Celeste.Player.orig_ctor orig, Player self, Vector2 position, PlayerSpriteMode spriteMode)
    {
        orig(self, position, spriteMode);
        ConveyorMover conveyorMover = new()
        {
            OnMove = (amount) => {
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