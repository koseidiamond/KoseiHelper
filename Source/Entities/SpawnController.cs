using Celeste.Mod.Entities;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static Celeste.Session;

namespace Celeste.Mod.KoseiHelper.Entities;

public enum EntityType
{
    BadelineBoost,
    Blade,
    Booster,
    BounceBlock,
    Bumper,
    Cloud,
    Counter,
    CrumblePlatform,
    DashBlock,
    DashSwitch,
    Decal,
    DreamBlock,
    DustBunny,
    ExitBlock,
    FallingBlock,
    Feather,
    Flag,
    FloatySpaceBlock,
    GlassBlock,
    Heart,
    Iceball,
    IceBlock,
    Jellyfish,
    JumpthruPlatform,
    Kevin,
    MoveBlock,
    Oshiro,
    Player,
    Puffer,
    Refill,
    Seeker,
    SpawnPoint,
    Spinner,
    Spring,
    StarJumpBlock,
    Strawberry,
    SwapBlock,
    TempleGate,
    TheoCrystal,
    Water,
    ZipMover,
    CustomEntity
}

public enum SpawnCondition
{
    OnFlagEnabled = 1,
    OnDash = 2,
    OnCassetteBeat = 3,
    OnCustomButtonPress = 4,
    OnSpeedX = 5,
    OnInterval = 6,
    OnClimb = 7,
    OnJump = 8,
    LeftClick = 9,
    RightClick = 10
}

public enum CassetteColor
{
    Any,
    Blue,
    Rose,
    BrightSun,
    Malachite
}

public enum DespawnMethod
{
    None,
    TTL,
    Flag,
    TTLFlag,
    OverriddenByNext
}

[CustomEntity("KoseiHelper/SpawnController")]
public class SpawnController : Entity
{
    //general customizable variables
    public int offsetX, offsetY;
    public bool removeDash, removeStamina;
    public EntityType entityToSpawn;
    public float spawnCooldown, spawnTime;
    public bool relativeToPlayerFacing, nodeRelativeToPlayerFacing;
    public SpawnCondition spawnCondition;
    public CassetteColor cassetteColor;
    public DespawnMethod despawnMethod;
    public string spawnFlag;
    public bool spawnFlagValue;
    public float spawnSpeed;
    public int spawnLimit;
    public bool persistency;
    public float timeToLive;
    public string flagToLive;
    public string counterPositive;
    public string appearSound;
    public string disappearSound;
    public int dashCount, everyXDashes;
#pragma warning disable CS0414
    private bool isBlock = false;
#pragma warning restore CS0414
    private bool hasSpawnedFromSpeed = false;
    private bool previousHasSpawnedFromFlag, currentHasSpawnedFromFlag;
    private bool canSpawnFromCassette = false;
    public int flagCount = 1;
    public int flagCycleAt = 9999;
    public bool flagStop;
    public bool poofWhenDisappearing;
    public bool absoluteCoords = false;
    public bool ignoreJustRespawned;
    public bool noNode;
    private bool randomLocation;
    public float clusteredChance;
    public bool clustered;
    public Color spawnAreaColor;
    private Random deterministicRandom;
    //other important variables
    private int entityID = 7388544; // Very high value so it doesn't conflict with other ids (hopefully)
    private List<EntityWithTTL> spawnedEntitiesWithTTL = new List<EntityWithTTL>();
    private List<Solid> spawnedSolids = new List<Solid>(); // So lava/ice solids can be removed
    public static ParticleType poofParticle = TouchSwitch.P_FireWhite;
    public Entity spawnedEntity = null;
    public static bool playerIsJumping;
    public bool globalEntity;
    public bool gridAligned;
    public int gridSize;
    private bool onlyOnBgTiles;
    private Entity bgTilesCollider;
    private bool doNotRepeatSpots;
    private HashSet<Vector2> usedSpawnPositions = new HashSet<Vector2>();
    private bool canSpawnThisFrame = true;

    //entity specific data

    public int nodeX, nodeY;
    public int blockWidth, blockHeight = 16;
    public char blockTileType;

    public bool dummyFix;

    public bool boosterRed, boosterSingleUse;

    public bool cloudFragile;

    public int minCounterCap = 0, maxCounterCap = 9999;
    public bool decreaseCounter;

    public bool dashBlockCanDash;

    public bool dashSwitchPersistent, dashSwitchAllGates;
    public string dashSwitchSprite;
    public string dashSwitchSide;

    public string decalTexture;
    public int decalDepth;
    public bool dontFlip;

    public bool fallingBlockBadeline, fallingBlockClimbFall;

    public bool featherShielded;
    public bool featherSingleUse;

    public string flagToEnableSpawner;
    public bool flagValue;

    public float iceballSpeed;
    public bool iceballAlwaysIce;

    public bool jellyfishBubble;

    public string jumpthruTexture;
    public int soundIndex;

    public bool moveBlockCanSteer, moveBlockFast;
    public MoveBlock.Directions moveBlockDirection;

    public SwapBlockNoBg.Themes swapBlockTheme;

    public bool refillTwoDashes;
    public bool refillOneUse;

    public bool blockSinks;

    public CrushBlock.Axes crushBlockAxis;
    public bool crushBlockChillout;
    private PlayerSpriteMode playerSpriteMode;


    public bool attachToSolid;
    public CrystalColor crystalColor;
    public bool circularMovement, bladeStar, bladeClockwise, bladeStartCenter;
    public TrackSpinner.Speeds bladeSpeed;

    public string orientation;
    public bool playerCanUse;

    private CoreModes coreMode;
    private CassetteBlockManager cassetteBlockManager;

    public bool onlyOnSafeGround;

    private bool isWinged;
    private bool isMoon;

    private TempleGate.Types templeGateType;
    private string templeGateSprite;

    public bool hasTop, hasBottom;

    public ZipMover.Themes zipMoverTheme;

    //Custom Entity Fields
    private string entityPath;
    private List<string> dictionaryKeys;
    private List<string> dictionaryValues;

    private bool isDragging = false;
    private Entity draggedEntity = null;
    private Vector2 dragOffset = Vector2.Zero;

    // Mouse mode

    public bool canDragAround;
    public bool oppositeDragButton;
    public bool mouseWheelMode;
    public string wheelOptions;
    public float wheelIndicatorX, wheelIndicatorY;
    public bool wheelIndicatorImage;
    private static EntityType[] WheelCycle;
    private static int wheelIndex;
    public static EntityType currentWheelValue => WheelCycle[wheelIndex];
    private EntityType[] BuildWheelCycle(string wheelList)
    {
        if (!string.IsNullOrWhiteSpace(wheelList))
        {
            try
            {
                return wheelList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(s => Enum.Parse<EntityType>(s, true)).ToArray();
            }
            catch
            {
                Logger.Log(LogLevel.Error, "KoseiHelper", "An invalid entity type was introduced in the Wheel Options list!\nUsing the default list instead.");
            }
        }

        return Enum.GetValues<EntityType>().Except(new[] { EntityType.Flag, EntityType.Counter, EntityType.SpawnPoint }).ToArray();
    }
    private static int lastWheel;

    public SpawnController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Add(new PostUpdateHook(() => { }));
        if (!data.Bool("noCollider", false))
            Collider = new Hitbox(8, 8, -4, -4);
        // General attributes
        offsetX = data.Int("offsetX", 0);
        offsetY = data.Int("offsetY", 8);
        absoluteCoords = data.Bool("absoluteCoords", false);
        entityToSpawn = data.Enum("entityToSpawn", EntityType.Puffer);
        spawnCooldown = spawnTime = data.Float("spawnCooldown", 0f);
        removeDash = data.Bool("removeDash", false);
        removeStamina = data.Bool("removeStamina", false);
        relativeToPlayerFacing = data.Bool("relativeToPlayerFacing", true);
        nodeRelativeToPlayerFacing = data.Bool("nodeRelativeToPlayerFacing", true);
        timeToLive = data.Float("timeToLive", 0f);
        flagToLive = data.Attr("flagToLive", "KoseiHelper_despawnEntity");
        counterPositive = data.Attr("counterPositive", "");
        appearSound = data.Attr("appearSound", "event:/KoseiHelper/spawn");
        disappearSound = data.Attr("disappearSound", "event:/game/general/assist_dash_aim");
        flagToEnableSpawner = data.Attr("flag", "");
        flagValue = data.Bool("flagValue", true);
        spawnCondition = data.Enum("spawnCondition", SpawnCondition.OnCustomButtonPress);
        cassetteColor = data.Enum("cassetteColor", CassetteColor.Any);
        despawnMethod = data.Enum("despawnMethod", DespawnMethod.TTL);
        spawnFlag = data.Attr("spawnFlag", "koseiHelper_spawn");
        spawnFlagValue = data.Bool("spawnFlagValue", true);
        spawnSpeed = data.Float("spawnSpeed", 300f);
        spawnLimit = data.Int("spawnLimit", 3);
        persistency = data.Bool("persistent", false);
        everyXDashes = data.Int("everyXDashes", 1);
        poofWhenDisappearing = data.Bool("poofWhenDisappearing", true);
        noNode = data.Bool("noNode", false);
        randomLocation = data.Bool("randomLocation", false);
        clustered = data.Bool("clustered", false);
        clusteredChance = Calc.Clamp(data.Float("clusteredChance", 80f) / 100f, 0f, 1f);
        spawnAreaColor = data.HexColor("spawnAreaColor", Color.BurlyWood);
        ignoreJustRespawned = data.Bool("ignoreJustRespawned", false);
        canDragAround = data.Bool("canDragAround", false);
        oppositeDragButton = data.Bool("oppositeDragButton", true);
        dashCount = 0;
        if (persistency)
            base.Tag = Tags.Persistent;
        if (data.Bool("transitionUpdate", false) || entityToSpawn == EntityType.Player)
            base.Tag = Tags.TransitionUpdate;
        globalEntity = data.Bool("globalEntity", false);
        onlyOnBgTiles = data.Bool("onlyOnBgTiles", false);
        gridAligned = data.Bool("gridAligned", false);
        gridSize = data.Int("gridSize", 8);
        doNotRepeatSpots = data.Bool("doNotRepeatSpots", true);

        mouseWheelMode = data.Bool("mouseWheelMode", false);
        wheelIndicatorX = data.Float("wheelIndicatorX", 20f);
        wheelIndicatorY = data.Float("wheelIndicatorY", 140f);
        wheelIndicatorImage = data.Bool("wheelIndicatorImage", false);
        if (mouseWheelMode)
        {
            this.Depth = -300000;
            if (!wheelIndicatorImage)
                base.Tag = Tags.HUD;
            wheelOptions = data.Attr("wheelOptions", "");
            WheelCycle = BuildWheelCycle(wheelOptions);
            entityToSpawn = currentWheelValue;
        }

        //Entity specific attributes

        nodeX = data.Int("nodeX", 0);
        nodeY = data.Int("nodeY", 0);
        blockWidth = data.Int("blockWidth", 16);
        blockHeight = data.Int("blockHeight", 16);
        blockTileType = data.Char("blockTileType", '3');

        dummyFix = data.Bool("dummyFix", true);

        boosterRed = data.Bool("boosterRed", false);
        boosterSingleUse = data.Bool("boosterSingleUse", false);

        cloudFragile = data.Bool("cloudFragile", true);

        minCounterCap = data.Int("minCounterCap", 0);
        maxCounterCap = data.Int("maxCounterCap", 9999);
        decreaseCounter = data.Bool("decreaseCounter", false);

        dashBlockCanDash = data.Bool("dashBlockCanDash", true);

        dashSwitchPersistent = data.Bool("dashSwitchPersistent", false);
        dashSwitchAllGates = data.Bool("dashSwitchAllGates", false);
        dashSwitchSprite = data.Attr("dashSwitchSprite", "Default");
        dashSwitchSide = data.Attr("dashSwitchSide", "Up");

        decalTexture = data.Attr("decalTexture", "10-farewell/creature_f00");
        decalDepth = data.Int("decalDepth", 9000);
        dontFlip = data.Bool("dontFlip", false);

        fallingBlockBadeline = data.Bool("fallingBlockBadeline", false);
        fallingBlockClimbFall = data.Bool("fallingBlockClimbFall", false);

        featherShielded = data.Bool("featherShielded", false);
        featherSingleUse = data.Bool("featherSingleUse", true);

        flagCycleAt = data.Int("flagCycleAt", 9999);
        flagStop = data.Bool("flagStop", false);

        iceballSpeed = data.Float("iceballSpeed", 1f);
        iceballAlwaysIce = data.Bool("iceballAlwaysIce", false);

        jellyfishBubble = data.Bool("jellyfishBubble", false);

        refillTwoDashes = data.Bool("refillTwoDashes", false);
        refillOneUse = data.Bool("refillOneUse", false);

        moveBlockCanSteer = data.Bool("moveBlockCanSteer", false);
        moveBlockFast = data.Bool("moveBlockFast", true);
        moveBlockDirection = data.Enum("moveBlockDirection", MoveBlock.Directions.Down);

        swapBlockTheme = data.Enum("swapBlockTheme", SwapBlockNoBg.Themes.Normal);

        jumpthruTexture = data.Attr("jumpthruTexture", "wood");
        soundIndex = data.Int("soundIndex", -1);
        blockSinks = data.Bool("blockSinks", false);
        crushBlockAxis = data.Enum("crushBlockAxe", CrushBlock.Axes.Both);
        crushBlockChillout = data.Bool("crushBlockChillout", false);

        hasTop = data.Bool("hasTop", true);
        hasBottom = data.Bool("hasBottom", false);

        attachToSolid = data.Bool("attachToSolid", false);
        crystalColor = data.Enum("crystalColor", CrystalColor.Blue);

        bladeStar = data.Bool("bladeStar", false);
        circularMovement = data.Bool("circularMovement", false);
        bladeClockwise = data.Bool("clockwise", false);
        bladeStartCenter = data.Bool("bladeStartCenter", false);
        bladeSpeed = data.Enum("bladeSpeed", BladeTrackSpinner.Speeds.Normal);

        orientation = data.Attr("orientation", "Floor");
        playerCanUse = data.Bool("playerCanUse", true);

        onlyOnSafeGround = data.Bool("onlyOnSafeGround", true);

        isWinged = data.Bool("winged", false);
        isMoon = data.Bool("moon", false);

        templeGateType = data.Enum("templeGateType", TempleGate.Types.NearestSwitch);
        templeGateSprite = data.Attr("templeGateSprite", "Default");

        playerSpriteMode = data.Enum("playerSpriteMode", PlayerSpriteMode.Madeline);

        zipMoverTheme = data.Enum("zipMoverTheme", ZipMover.Themes.Normal);

        //Custom Entities
        entityPath = data.Attr("entityPath");
        dictionaryKeys = data.Attr("dictKeys").Replace(" ", string.Empty).Split(',').ToList();
        dictionaryValues = System.Text.RegularExpressions.Regex.Matches(data.Attr("dictValues"), @"\[.*?\]|[^,]+")
            .Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value.Trim()).ToList(); // parses [a,b] for Vector2
        Add(new CoreModeListener(OnChangeMode));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        cassetteBlockManager = scene.Tracker.GetEntity<CassetteBlockManager>();
        Level level = SceneAs<Level>();
        if (level.Tracker.GetEntity<Player>() != null && level.Tracker.GetEntity<Player>().JustRespawned)
            spawnCooldown = 0f;
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        int seed = level.Session.Deaths * 37 + this.entityID * 13 + (SaveData.Instance?.Name?.GetHashCode() ?? 0);
        deterministicRandom = new Random(seed);
        if (level.Session.GetFlag("CelesteTAS_TAS_Was_Run"))
            Logger.Info(nameof(KoseiHelperModule), $"EntitySpawnArea RNG seed: Deaths: {level.Session.Deaths}, File: {SaveData.Instance.Name}, Entity ID: {this.entityID}");
        foreach (String slider in level.Session.Sliders.Keys.Where(k => k.StartsWith("KoseiHelper_EntitySpawnerTTL") || k.StartsWith("KoseiHelper_EntitySpawnerCooldown_")))
            level.Session.SetSlider(slider, 0f);
        if (onlyOnBgTiles)
        {
            Rectangle rectangle = new Rectangle(level.Bounds.Left / 8, level.Bounds.Y / 8, level.Bounds.Width / 8, level.Bounds.Height / 8);
            Rectangle tileBounds = level.Session.MapData.TileBounds;
            bool[,] array = new bool[rectangle.Width, rectangle.Height];
            for (int i = 0; i < rectangle.Width; i++)
            {
                for (int j = 0; j < rectangle.Height; j++)
                {
                    array[i, j] = level.BgData[i + rectangle.Left - tileBounds.Left, j + rectangle.Top - tileBounds.Top] != '0';
                }
            }
            bgTilesCollider = new Solid(new Vector2(level.Bounds.Left, level.Bounds.Top), 1f, 1f, safe: true);
            bgTilesCollider.Collider = new Grid(8f, 8f, array);
            bgTilesCollider.Collidable = false;
            base.Scene.Add(bgTilesCollider);
        }
    }

    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        Level level = SceneAs<Level>();
        canSpawnThisFrame = true;
        if (spawnCooldown > 0)
        {
            spawnCooldown -= Engine.RawDeltaTime;
            level.Session.SetSlider("KoseiHelper_EntitySpawnerCooldown_" + entityID, spawnCooldown);
        }
        else
            spawnCooldown = 0f;
        //This HashSet contains not only blocks, but all entities with width. Used to fix spawn positions on relative mode.
        HashSet<EntityType> blockEntities = new HashSet<EntityType>
        {
        EntityType.BounceBlock,
        EntityType.CrumblePlatform,
        EntityType.DashBlock,
        EntityType.ExitBlock,
        EntityType.FallingBlock,
        EntityType.FloatySpaceBlock,
        EntityType.GlassBlock,
        EntityType.JumpthruPlatform,
        EntityType.Kevin,
        EntityType.MoveBlock,
        EntityType.StarJumpBlock,
        EntityType.SwapBlock,
        EntityType.TempleGate,
        EntityType.Water,
        EntityType.ZipMover
        };
        bool isBlock = blockEntities.Contains(entityToSpawn);
        // If the flag flagToEnableSpawner is true, or if no flag is required, check if the spawn conditions are met
        // (Not to be confused with spawnFlag, this one is just a common requirement, the other is for the Flag Mode)
        if (((flagValue && level.Session.GetFlag(flagToEnableSpawner)) || string.IsNullOrEmpty(flagToEnableSpawner) ||
            (!flagValue && !level.Session.GetFlag(flagToEnableSpawner))) && player != null && (!player.JustRespawned || ignoreJustRespawned) &&
            (string.IsNullOrEmpty(counterPositive) || level.Session.GetCounter(counterPositive) > 0))
        {
            if (cassetteBlockManager != null)
            {
                if (cassetteColor == CassetteColor.Any ||
                    (cassetteColor == CassetteColor.Blue && cassetteBlockManager.currentIndex == 0) ||
                    (cassetteColor == CassetteColor.Rose && cassetteBlockManager.currentIndex == 1) ||
                    (cassetteColor == CassetteColor.BrightSun && cassetteBlockManager.currentIndex == 2) ||
                    (cassetteColor == CassetteColor.Malachite && cassetteBlockManager.currentIndex == 3))
                {
                    // Make sure the player is not too close to the bounds of the level to prevent transition jank
                    if (player.Bottom < (float)level.Bounds.Bottom && player.Top > (float)level.Bounds.Top + 4 &&
                    player.Left > (float)level.Bounds.Left + 8 && player.Right < (float)level.Bounds.Right - 8)
                    {
                        canSpawnFromCassette = true;
                    }
                }
                else
                    canSpawnFromCassette = false;
            }
            currentHasSpawnedFromFlag = spawnFlagValue ? level.Session.GetFlag(spawnFlag) : !level.Session.GetFlag(spawnFlag);
            bool conditionMet = spawnCondition switch
            {   //If the spawn conditions are met, spawn the entity:
                SpawnCondition.OnFlagEnabled => currentHasSpawnedFromFlag && !previousHasSpawnedFromFlag,
                SpawnCondition.OnSpeedX => Math.Abs(player.Speed.X) >= spawnSpeed && !hasSpawnedFromSpeed,
                SpawnCondition.OnDash => player.StartedDashing && player.StateMachine.state == 2 && dashCount % everyXDashes == 0,
                SpawnCondition.OnCassetteBeat => canSpawnFromCassette,
                SpawnCondition.OnInterval => spawnCooldown == 0,
                SpawnCondition.OnCustomButtonPress => KoseiHelperModule.Settings.SpawnButton.Pressed && spawnCooldown == 0,
                SpawnCondition.OnClimb => player.StateMachine.state == 1 && player.climbNoMoveTimer == 0.1f,
                SpawnCondition.OnJump => playerIsJumping,
                SpawnCondition.LeftClick => MInput.Mouse.PressedLeftButton,
                SpawnCondition.RightClick => MInput.Mouse.PressedRightButton,
                _ => false
            };
            if ((despawnMethod == DespawnMethod.Flag || despawnMethod == DespawnMethod.TTLFlag) && level.Session.GetFlag(flagToLive))
                conditionMet = false; // Can't spawn if it's supposed to be despawning
            if (onlyOnBgTiles && player != null)
            {
                bgTilesCollider.Collidable = true;
                bool collidingWithBgTiles = player.CollideCheck(bgTilesCollider, player.CenterLeft) && player.CollideCheck(bgTilesCollider, player.CenterRight);
                bgTilesCollider.Collidable = false;
                if (!collidingWithBgTiles && !player.onGround)
                    conditionMet = false;
            }
            playerIsJumping = false;
            if (conditionMet && spawnLimit != 0 && spawnCooldown == 0 && player != null)
            {
                if (entityToSpawn != EntityType.CustomEntity)
                    Logger.Debug(nameof(KoseiHelperModule), $"An entity is going to spawn: {entityToSpawn}");
                else //Logs the parameters used in Lönn + the original constructor
                {
                    if (!string.IsNullOrEmpty(entityPath))
                        Logger.Debug(nameof(KoseiHelperModule), $"An entity ({entityPath}) is going to spawn, with the attributes: " +
                        $"{string.Join(", ", dictionaryKeys.Zip(dictionaryValues, (key, value) => $"{key}={value}"))}, with parameters: " +
                        $"{string.Join(", ", FakeAssembly.GetFakeEntryAssembly().GetType(entityPath).GetConstructors().Select(constructor =>
                        $"{constructor.Name}({string.Join(", ", constructor.GetParameters().Select(param => $"{param.ParameterType.Name} {param.Name}"))})"))}.");
                }

                if (removeDash && Scene.Tracker.GetEntity<Player>().Dashes > 0)
                    player.Dashes -= 1;
                if (removeStamina)
                    player.Stamina = 0;
                if (spawnLimit > 0)
                    spawnLimit -= 1;
                //Calculate spawn position
                Vector2 spawnPosition = new Vector2(X + offsetX, Y + offsetY);
                if (!absoluteCoords)
                {
                    spawnPosition = new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY);
                    if (relativeToPlayerFacing && player.Facing == Facings.Left)
                        spawnPosition = new Vector2(player.Position.X - offsetX, player.Position.Y + offsetY);
                    if (relativeToPlayerFacing && player.Facing == Facings.Right && isBlock)
                        spawnPosition = new Vector2(player.Position.X + offsetX - blockWidth, player.Position.Y + offsetY);
                }

                if (randomLocation)
                {
                    List<EntitySpawnArea> spawnAreas = Scene.Tracker.GetEntities<EntitySpawnArea>().OfType<EntitySpawnArea>().Where(area => area.color == spawnAreaColor).ToList();

                    if (spawnAreas.Count > 0)
                    {
                        int originalAttempts = 3000; // tries tons of different spots until it finds a valid one idk how to better one (sounds expensive but it's not i think)
                        int attempts = originalAttempts;
                        foreach (EntitySpawnArea area in spawnAreas)
                        {
                            int areaSize = area.Collider.Bounds.Width * area.Collider.Bounds.Height;
                        }
                        bool foundValidSpot = false;
                        Vector2 finalTestPosition = Vector2.Zero;
                        List<Vector2> usedPositionsList = usedSpawnPositions.ToList(); // they are putting chemicals into our code and turning HashSets into Lists
                        // so we have better performance by doing this before the loop so we can then select a valid clustered spot with basePos

                        while (attempts-- > 0)
                        {
                            Vector2 testPos;
                            bool tryCluster = clustered && deterministicRandom.NextDouble() < clusteredChance && usedSpawnPositions.Count > 0;
                            if (tryCluster)
                            { // try to spawn adjacent to a used position
                                Vector2 basePos = usedPositionsList[deterministicRandom.Next(usedPositionsList.Count)];
                                // this means that if the cluster chance is very high, it has to be completely adjacent
                                // and if it's only mildly high, it can be 1 "tile" (gridSize) away, like close but not so much
                                // e.g. 100% chance will always be adjacent, 80% will be mostly adjacent but sometimes 1 tile away
                                int range = deterministicRandom.NextDouble() < clusteredChance ? 1 : 2;
                                int dx = deterministicRandom.Next(-range, range + 1);
                                int dy = deterministicRandom.Next(-range, range + 1);
                                testPos = new Vector2(basePos.X + dx * gridSize, basePos.Y + dy * gridSize);
                            }
                            else
                            {
                                // Calculate combined bounding rectangle of all spawn areas
                                int combinedLeft = spawnAreas.Min(area => area.Collider.Bounds.Left);
                                int combinedTop = spawnAreas.Min(area => area.Collider.Bounds.Top);
                                int combinedRight = spawnAreas.Max(area => area.Collider.Bounds.Right);
                                int combinedBottom = spawnAreas.Max(area => area.Collider.Bounds.Bottom);
                                Rectangle combinedBounds = new(combinedLeft, combinedTop, combinedRight - combinedLeft, combinedBottom - combinedTop);
                                float randX = deterministicRandom.Next(combinedBounds.Left, combinedBounds.Right);
                                float randY = deterministicRandom.Next(combinedBounds.Top, combinedBounds.Bottom);
                                testPos = new Vector2(randX, randY);

                                if (gridAligned)
                                    testPos = new Vector2((float)Math.Floor(testPos.X / gridSize) * gridSize, (float)Math.Floor(testPos.Y / gridSize) * gridSize);
                                // Same adjacency rejection logic as before
                                bool isAdjacent = false;
                                foreach (Vector2 pos in usedPositionsList)
                                {
                                    float dx = Math.Abs(pos.X - testPos.X);
                                    float dy = Math.Abs(pos.Y - testPos.Y);
                                    if (dx <= gridSize && dy <= gridSize && (dx > 0 || dy > 0))
                                    {
                                        isAdjacent = true;
                                        break;
                                    }
                                }

                                if (isAdjacent)
                                {
                                    if (clusteredChance > 0f)
                                    {
                                        if (deterministicRandom.NextDouble() >= clusteredChance)
                                            continue;
                                    }
                                    else
                                    {
                                        bool foundNonAdjacent = false;
                                        for (int i = 0; i < 10; i++)
                                        {
                                            float x = deterministicRandom.Next(combinedBounds.Left, combinedBounds.Right);
                                            float y = deterministicRandom.Next(combinedBounds.Top, combinedBounds.Bottom);
                                            Vector2 altPos = new Vector2(x, y);
                                            if (gridAligned)
                                                altPos = new Vector2((float)Math.Floor(altPos.X / gridSize) * gridSize, (float)Math.Floor(altPos.Y / gridSize) * gridSize);

                                            bool isAltAdjacent = false;
                                            foreach (Vector2 used in usedPositionsList)
                                            {
                                                float dx = Math.Abs(used.X - altPos.X);
                                                float dy = Math.Abs(used.Y - altPos.Y);
                                                if (dx <= gridSize && dy <= gridSize && (dx > 0 || dy > 0))
                                                {
                                                    isAltAdjacent = true;
                                                    break;
                                                }
                                            }
                                            if (!isAltAdjacent && (!doNotRepeatSpots || !usedSpawnPositions.Contains(altPos)))
                                            {
                                                foundNonAdjacent = true;
                                                break;
                                            }
                                        }
                                        if (foundNonAdjacent)
                                            continue; // There are still non-adjacent candidates: skip this adjacent one
                                    }
                                }
                            }
                            Vector2 normalized = gridAligned
                                ? new Vector2((float)Math.Floor(testPos.X / gridSize) * gridSize, (float)Math.Floor(testPos.Y / gridSize) * gridSize)
                                : testPos.Floor();

                            if (doNotRepeatSpots && usedSpawnPositions.Contains(normalized))
                                continue;
                            Entity dummyEntity = new Entity();
                            dummyEntity.Position = testPos;
                            Collider testCollider;
                            try
                            {
                                if (spawnedEntity?.Collider != null)
                                {
                                    testCollider = spawnedEntity.Collider.Clone();
                                    testCollider.Position = Vector2.Zero;
                                    dummyEntity.Collider = testCollider;
                                }
                                else
                                {
                                    testCollider = new Hitbox(8f, 8f, -4f, -4f);
                                    testCollider.Position = Vector2.Zero;
                                    dummyEntity.Collider = testCollider;
                                }
                            }
                            catch (Exception)
                            {
                                testCollider = new Hitbox(8f, 8f, -4f, -4f);
                                testCollider.Position = Vector2.Zero;
                                dummyEntity.Collider = testCollider;
                            }
                            Rectangle testBounds = testCollider.Bounds;

                            // This corner stuff is useful to determine if all corners of a block/rectangle collider fit inside any of the areas
                            bool allCornersInside = true;
                            Vector2[] corners = new Vector2[] {
                                new Vector2(testBounds.Left, testBounds.Top), new Vector2(testBounds.Right - 1, testBounds.Top),
                                new Vector2(testBounds.Left, testBounds.Bottom - 1), new Vector2(testBounds.Right - 1, testBounds.Bottom - 1)};
                            foreach (Vector2 corner in corners)
                            {
                                bool cornerInsideAnyArea = false;
                                foreach (EntitySpawnArea area in spawnAreas)
                                {
                                    if (area.Collider.Bounds.Contains((int)corner.X, (int)corner.Y))
                                    {
                                        cornerInsideAnyArea = true;
                                        break;
                                    }
                                }
                                if (!cornerInsideAnyArea)
                                {
                                    allCornersInside = false;
                                    break;
                                }
                            }
                            if (allCornersInside)
                            {
                                finalTestPosition = testPos;
                                foundValidSpot = true;
                                if (doNotRepeatSpots)
                                    usedSpawnPositions.Add(normalized);
                                break;
                            }
                        }

                        if (foundValidSpot)
                            spawnPosition = finalTestPosition + new Vector2(offsetX, offsetY);
                        else
                        {
                            Logger.Debug(nameof(KoseiHelperModule), $"Failed to find a valid spawn position after {originalAttempts} attempts.\nMake you have enough space to spawn entities (bigger spawn area or less requirements).");
                            canSpawnThisFrame = false;
                        }
                    }
                }

                if (spawnCondition == SpawnCondition.LeftClick || spawnCondition == SpawnCondition.RightClick)
                {
                    float mouseX = MInput.Mouse.Position.X * (320f / 1920f);
                    float mouseY = MInput.Mouse.Position.Y * (180f / 1080f);
                    spawnPosition = new Vector2(level.Camera.Position.X + mouseX + offsetX, level.Camera.Position.Y + mouseY + offsetY);
                    if (isBlock)
                        spawnPosition -= new Vector2(blockWidth / 2, blockHeight / 2);
                }

                if (gridAligned)
                    spawnPosition = new Vector2((float)Math.Floor(spawnPosition.X / gridSize) * gridSize, (float)Math.Floor(spawnPosition.Y / gridSize) * gridSize);

                //Calculate node position
                Vector2 nodePosition = new Vector2(X + nodeX, Y + nodeY);
                if (!absoluteCoords)
                {
                    nodePosition = new Vector2(player.Position.X + nodeX, player.Position.Y + nodeY);
                    if (nodeRelativeToPlayerFacing && player.Facing == Facings.Left)
                        nodePosition = new Vector2(player.Position.X - nodeX, player.Position.Y + nodeY);
                    if (relativeToPlayerFacing && player.Facing == Facings.Right && isBlock)
                        nodePosition = new Vector2(player.Position.X + nodeX - blockWidth, player.Position.Y + nodeY);
                }
                float entityTTL = timeToLive;
                string entityFTL = flagToLive;
                switch (entityToSpawn)
                { // If relativeToPlayerFacing is true, a positive X value will spawn in front of the player, and a negative X value will spawn behind the player
                    case EntityType.BadelineBoost:
                        spawnedEntity = new BadelineBoost(new Vector2[] { spawnPosition, new Vector2(player.Position.X + offsetX, level.Bounds.Top - 200) }, false, false, false, false, false);
                        break;
                    case EntityType.Blade:
                        EntityData bladeData = new()
                        {
                            Position = spawnPosition,
                            Level = level.Session.LevelData,
                            Nodes = new Vector2[] { nodePosition },
                            Values = new()
                        };
                        for (int i = 0; i < dictionaryKeys.Count; i++) // ??? why did i do this
                        {
                            bladeData.Values[dictionaryKeys[i]] = dictionaryValues.ElementAtOrDefault(i);
                        }
                        bladeData.Values["startCenter"] = bladeData.Bool("startCenter", bladeStartCenter).ToString();
                        bladeData.Values["clockwise"] = bladeData.Bool("clockwise", bladeClockwise).ToString();
                        bladeData.Values["speed"] = bladeData.Enum("speed", bladeSpeed).ToString();
                        if (circularMovement)
                        {
                            if (bladeStar)
                                spawnedEntity = new StarRotateSpinner(bladeData, Vector2.Zero);
                            else
                                spawnedEntity = new BladeRotateSpinner(bladeData, Vector2.Zero);
                        }
                        else
                        {
                            if (bladeStar)
                                spawnedEntity = new StarTrackSpinner(bladeData, Vector2.Zero);
                            else
                                spawnedEntity = new BladeTrackSpinner(bladeData, Vector2.Zero);
                        }
                        break;
                    case EntityType.Booster:
                        spawnedEntity = new BoosterNoOutline(spawnPosition, boosterRed, boosterSingleUse); // I had to make a new class so their outline doesn't stay after they poof
                        break;
                    case EntityType.BounceBlock:
                        spawnedEntity = new BounceBlock(spawnPosition, blockWidth, blockHeight);
                        break;
                    case EntityType.Bumper:
                        spawnedEntity = new Bumper(spawnPosition, null);
                        break;
                    case EntityType.Cloud:
                        spawnedEntity = new Cloud(spawnPosition, cloudFragile);
                        break;
                    case EntityType.Counter:
                        if (decreaseCounter && level.Session.GetCounter("koseiCounter") > minCounterCap)
                            level.Session.SetCounter("koseiCounter", level.Session.GetCounter("koseiCounter") - 1);
                        if (!decreaseCounter && level.Session.GetCounter("koseiCounter") < maxCounterCap)
                            level.Session.IncrementCounter("koseiCounter");
                        Audio.Play(appearSound, player.Position); //Audio is here because it doesn't actually spawn anything
                        if (spawnCondition == SpawnCondition.OnInterval)
                            spawnCooldown = spawnTime;
                        break;
                    case EntityType.CrumblePlatform:
                        spawnedEntity = new CrumblePlatform(spawnPosition, blockWidth);
                        break;
                    case EntityType.DashBlock:
                        spawnedEntity = new NoFreezeDashBlock(spawnPosition, blockTileType, blockWidth, blockHeight, false, true, dashBlockCanDash, new EntityID("koseiHelper_spawnedDashBlock", entityID));
                        entityID++;
                        break;
                    case EntityType.DashSwitch:
                        DashSwitch.Sides side = DashSwitch.Sides.Up;
                        switch (dashSwitchSide)
                        {
                            case "Left":
                                side = DashSwitch.Sides.Left;
                                break;
                            case "Right":
                                side = DashSwitch.Sides.Right;
                                break;
                            case "Down":
                                side = DashSwitch.Sides.Down;
                                break;
                            case "MovingDirection":
                                if (player.Speed != Vector2.Zero)
                                {
                                    side = Math.Abs(player.Speed.X) > Math.Abs(player.Speed.Y)
                                        ? (player.Speed.X > 0f ? DashSwitch.Sides.Left : DashSwitch.Sides.Right)
                                        : (player.Speed.Y > 0f ? DashSwitch.Sides.Up : DashSwitch.Sides.Down);
                                }
                                else
                                {
                                    side = player.onGround
                                        ? DashSwitch.Sides.Up
                                        : (player.Facing == Facings.Left ? DashSwitch.Sides.Left : DashSwitch.Sides.Right);
                                }
                                break;
                        }
                        spawnedEntity = new DashSwitch(spawnPosition + new Vector2(0f, -8f), side, dashSwitchPersistent, dashSwitchAllGates,
                            new EntityID("koseiHelper_spawnedDashSwitch", entityID), dashSwitchSprite);
                        break;
                    case EntityType.Decal:
                        if (player.Facing == Facings.Left && !dontFlip)
                            spawnedEntity = new Decal(decalTexture, spawnPosition, new Vector2(-1, 1), decalDepth);
                        else
                            spawnedEntity = new Decal(decalTexture, spawnPosition, new Vector2(1, 1), decalDepth);
                        break;
                    case EntityType.DreamBlock:
                        spawnedEntity = new DreamBlock(spawnPosition, blockWidth, blockHeight, null, false, true);
                        break;
                    case EntityType.DustBunny:
                        if (noNode)
                            spawnedEntity = new DustStaticSpinner(spawnPosition, attachToSolid, false);
                        else
                        {
                            EntityData dustSpinnerData = new()
                            {
                                Position = spawnPosition,
                                Level = level.Session.LevelData,
                                Nodes = new Vector2[] { nodePosition },
                                Values = new()
                            };
                            for (int i = 0; i < dictionaryKeys.Count; i++) // ??? why did i do this
                            {
                                dustSpinnerData.Values[dictionaryKeys[i]] = dictionaryValues.ElementAtOrDefault(i);
                            }
                            dustSpinnerData.Values["startCenter"] = dustSpinnerData.Bool("startCenter", bladeStartCenter).ToString();
                            dustSpinnerData.Values["clockwise"] = dustSpinnerData.Bool("clockwise", bladeClockwise).ToString();
                            dustSpinnerData.Values["speed"] = dustSpinnerData.Enum("speed", bladeSpeed).ToString();
                            if (circularMovement)
                                spawnedEntity = new DustRotateSpinner(dustSpinnerData, Vector2.Zero);
                            else
                                spawnedEntity = new DustTrackSpinner(dustSpinnerData, Vector2.Zero);
                        }
                        break;
                    case EntityType.ExitBlock:
                        spawnedEntity = new ExitBlock(spawnPosition, blockWidth, blockHeight, blockTileType);
                        break;
                    case EntityType.FallingBlock:
                        spawnedEntity = new FallingBlock(spawnPosition, blockTileType, blockWidth, blockHeight, fallingBlockBadeline, false, fallingBlockClimbFall);
                        break;
                    case EntityType.Feather:
                        spawnedEntity = new FlyFeather(spawnPosition, featherShielded, featherSingleUse);
                        break;
                    case EntityType.Flag:
                        if (flagCount > flagCycleAt) // If the counter has exceeded the max (e.g. 4>3)
                        {
                            level.Session.SetFlag("koseiFlag" + flagCycleAt, false);
                            if (!flagStop)
                                flagCount = 1; // The counter is reset
                            else
                                RemoveSelf();
                        }
                        level.Session.SetFlag("koseiFlag" + flagCount, true);
                        level.Session.SetFlag("koseiFlag" + (flagCount - 1), false);
                        Audio.Play(appearSound, player.Position); //Audio is here because it doesn't actually spawn anything
                        if (spawnCondition == SpawnCondition.OnInterval)
                            spawnCooldown = spawnTime;
                        flagCount++;
                        break;
                    case EntityType.FloatySpaceBlock:
                        spawnedEntity = new FloatySpaceBlock(spawnPosition, blockWidth, blockHeight, blockTileType, true);
                        break;
                    case EntityType.GlassBlock:
                        spawnedEntity = new GlassBlock(spawnPosition, blockWidth, blockHeight, blockSinks);
                        break;
                    case EntityType.Heart:
                        spawnedEntity = new FakeHeart(spawnPosition);
                        break;
                    case EntityType.Iceball:
                        spawnedEntity = new FireBall(new Vector2[] { spawnPosition, nodePosition }, 1, 1, 0, iceballSpeed, iceballAlwaysIce);
                        break;
                    case EntityType.IceBlock: //It also works for lava actually
                        if (coreMode == CoreModes.Hot)
                            spawnedEntity = new FireBarrier(spawnPosition, blockWidth, blockHeight);
                        else if (coreMode == CoreModes.Cold)
                            spawnedEntity = new IceBlock(spawnPosition, blockWidth, blockHeight);
                        if (spawnedEntity is IceBlock iceBlock)
                        {
                            Solid solid = iceBlock.solid;
                            if (solid != null)
                                spawnedSolids.Add(solid);
                        }
                        else if (spawnedEntity is FireBarrier fireBarrier)
                        {
                            Solid solid = fireBarrier.solid;
                            if (solid != null)
                                spawnedSolids.Add(solid);
                        }
                        break;
                    case EntityType.Jellyfish:
                        spawnedEntity = new Glider(spawnPosition, jellyfishBubble, false);
                        break;
                    case EntityType.JumpthruPlatform:
                        spawnedEntity = new JumpthruPlatform(spawnPosition, blockWidth, jumpthruTexture, soundIndex);
                        break;
                    case EntityType.Kevin:
                        spawnedEntity = new CrushBlock(spawnPosition, blockWidth, blockHeight, crushBlockAxis, crushBlockChillout);
                        break;
                    case EntityType.MoveBlock:
                        spawnedEntity = new MoveBlock(spawnPosition, blockWidth, blockHeight, moveBlockDirection, moveBlockCanSteer, moveBlockFast);
                        break;
                    case EntityType.Oshiro:
                        spawnedEntity = new AngryOshiro(spawnPosition, false);
                        break;
                    case EntityType.Player:
                        spawnedEntity = new Player(spawnPosition, playerSpriteMode);
                        break;
                    case EntityType.Refill:
                        spawnedEntity = new Refill(spawnPosition, refillTwoDashes, refillOneUse);
                        break;
                    case EntityType.StarJumpBlock:
                        spawnedEntity = new StarJumpBlock(spawnPosition, blockWidth, blockHeight, blockSinks);
                        break;

                    case EntityType.Puffer:
                        spawnedEntity = new Puffer(spawnPosition, player.Facing == Facings.Right);
                        break;
                    case EntityType.Seeker:
                        spawnedEntity = new Seeker(spawnPosition, new Vector2[] { spawnPosition });
                        break;
                    case EntityType.SpawnPoint:
                        if (!onlyOnSafeGround || (onlyOnSafeGround && player.OnSafeGround && !player.SwimUnderwaterCheck() && !player._IsOverWater()))
                        {
                            AreaData.Areas[level.Session.Area.ID].Mode[(int)level.Session.Area.Mode].MapData.Get(level.Session.Level).Spawns.Add(spawnPosition);
                            level.Session.HitCheckpoint = true;
                            level.Session.RespawnPoint = spawnPosition;
                            //copypaste because we didn't actually spawn an entity
                            spawnCooldown = spawnTime;
                            hasSpawnedFromSpeed = true;
                            Audio.Play(appearSound, spawnPosition);
                            if (poofWhenDisappearing)
                            {
                                if (!(entityToSpawn == EntityType.Booster && boosterSingleUse))
                                    level.ParticlesFG.Emit(poofParticle, 5, spawnPosition, Vector2.One * 4f, 0 - (float)Math.PI / 2f);
                            }
                        }
                        break;
                    case EntityType.Spinner:
                        spawnedEntity = new CrystalStaticSpinner(spawnPosition, attachToSolid, crystalColor);
                        break;
                    case EntityType.Spring:
                        switch (orientation)
                        {
                            case "WallLeft":
                                spawnedEntity = new Spring(spawnPosition, Spring.Orientations.WallLeft, playerCanUse);
                                break;
                            case "WallRight":
                                spawnedEntity = new Spring(spawnPosition, Spring.Orientations.WallRight, playerCanUse);
                                break;
                            case "PlayerFacing":
                                if (player.Facing == Facings.Left)
                                    spawnedEntity = new Spring(spawnPosition, Spring.Orientations.WallRight, playerCanUse);
                                else
                                    spawnedEntity = new Spring(spawnPosition, Spring.Orientations.WallLeft, playerCanUse);
                                break;
                            case "PlayerFacingOpposite":
                                if (player.Facing == Facings.Left)
                                    spawnedEntity = new Spring(spawnPosition, Spring.Orientations.WallLeft, playerCanUse);
                                else
                                    spawnedEntity = new Spring(spawnPosition, Spring.Orientations.WallRight, playerCanUse);
                                break;
                            default: // "Floor"
                                spawnedEntity = new Spring(spawnPosition, Spring.Orientations.Floor, playerCanUse);
                                break;
                        }
                        break;
                    case EntityType.Strawberry:
                        EntityData strawberryData = new()
                        {
                            Position = spawnPosition,
                            Level = level.Session.LevelData,
                            Values = new()
                        };
                        for (int i = 0; i < dictionaryKeys.Count; i++)
                        {
                            strawberryData.Values[dictionaryKeys[i]] = dictionaryValues.ElementAtOrDefault(i);
                        }
                        strawberryData.Values["winged"] = strawberryData.Bool("Winged", isWinged).ToString();
                        strawberryData.Values["moon"] = strawberryData.Bool("Moon", isMoon).ToString();
                        EntityID strawberryID = new EntityID(level.Session.LevelData.Name, entityID++);
                        spawnedEntity = new Strawberry(strawberryData, Vector2.Zero, strawberryID);
                        break;
                    case EntityType.SwapBlock:
                        spawnedEntity = new SwapBlockNoBg(spawnPosition, blockWidth, blockHeight, nodePosition, swapBlockTheme);
                        break;
                    case EntityType.TempleGate: // I'm not sure about this levelID thing but hopefully this works
                        spawnedEntity = new TempleGate(spawnPosition, blockHeight, templeGateType, templeGateSprite, level.Session.LevelData.Name);
                        break;
                    case EntityType.TheoCrystal:
                        spawnedEntity = new TheoCrystal(spawnPosition);
                        break;
                    case EntityType.Water:
                        spawnedEntity = new Water(spawnPosition, hasTop, hasBottom, blockWidth, blockHeight);
                        break;
                    case EntityType.ZipMover:
                        spawnedEntity = new ZipMover(spawnPosition, blockWidth, blockHeight, nodePosition, zipMoverTheme);
                        break;

                    case EntityType.CustomEntity:
                        spawnedEntity = GetEntityFromPath(spawnPosition, nodePosition, level.Session.LevelData);
                        break;
                    default:
                        break;
                }
                if (spawnedEntity != null && player != null && canSpawnThisFrame)
                { // If the entity has been spawned successfully:
                    Scene.Add(spawnedEntity);
                    if (despawnMethod == DespawnMethod.None || globalEntity)
                        spawnedEntity.AddTag(Tags.Global);
                    spawnedEntitiesWithTTL.Add(new EntityWithTTL(spawnedEntity, entityTTL, entityFTL));
                    spawnCooldown = spawnTime;
                    hasSpawnedFromSpeed = true; // defaults to true, we'll set to false later if condition is not met
                    level.Session.IncrementCounter("KoseiHelper_Total_" + spawnedEntity.ToString() + "s_Spawned");
                    level.Session.IncrementCounter("KoseiHelper_TotalEntitiesSpawned");
                    if (!string.IsNullOrEmpty(counterPositive))
                        level.Session.SetCounter(counterPositive, level.Session.GetCounter(counterPositive) - 1);
                    Audio.Play(appearSound, player.Position);
                }
            }
        }

        //Mouse functionality
        if (canDragAround && player != null)
        {
            float mouseX = MInput.Mouse.Position.X * (320f / 1920f);
            float mouseY = MInput.Mouse.Position.Y * (180f / 1080f);
            Vector2 mousePosition = new Vector2(level.Camera.Position.X + mouseX, level.Camera.Position.Y + mouseY);

            if (isDragging && draggedEntity != null)
                draggedEntity.Position = mousePosition - dragOffset;

            // Determine drag button state based on spawnCondition and oppositeDragButton
            bool dragButtonHeld =
                (spawnCondition == SpawnCondition.LeftClick && !oppositeDragButton && MInput.Mouse.CheckLeftButton) ||
                (spawnCondition == SpawnCondition.RightClick && !oppositeDragButton && MInput.Mouse.CheckRightButton) ||
                (spawnCondition == SpawnCondition.LeftClick && oppositeDragButton && MInput.Mouse.CheckRightButton) ||
                (spawnCondition == SpawnCondition.RightClick && oppositeDragButton && MInput.Mouse.CheckLeftButton);
            bool dragButtonReleased =
                (spawnCondition == SpawnCondition.LeftClick && !oppositeDragButton && MInput.Mouse.ReleasedLeftButton) ||
                (spawnCondition == SpawnCondition.RightClick && !oppositeDragButton && MInput.Mouse.ReleasedRightButton) ||
                (spawnCondition == SpawnCondition.LeftClick && oppositeDragButton && MInput.Mouse.ReleasedRightButton) ||
                (spawnCondition == SpawnCondition.RightClick && oppositeDragButton && MInput.Mouse.ReleasedLeftButton);

            if (spawnedEntitiesWithTTL != null && !isDragging && dragButtonHeld)
            {
                foreach (EntityWithTTL entityWrapper in spawnedEntitiesWithTTL)
                {
                    Entity entity = entityWrapper.Entity;
                    Rectangle entityCenterRect = new Rectangle((int)(entity.Center.X - 12), (int)(entity.Center.Y - 12), 20, 20);
                    if (entityCenterRect.Contains((int)mousePosition.X, (int)mousePosition.Y))
                    {
                        isDragging = true;
                        draggedEntity = entity;
                        dragOffset = mousePosition - entity.Position;
                        break;
                    }
                }
            }
            if (dragButtonReleased)
            {
                isDragging = false;
                draggedEntity = null;
            }

            if (mouseWheelMode && (spawnCondition == SpawnCondition.LeftClick || spawnCondition == SpawnCondition.RightClick))
            {
                int wheel = MInput.Mouse.Wheel;
                int movedWheel = wheel - lastWheel;
                if (movedWheel != 0)
                {
                    lastWheel = wheel;
                    int dir = Math.Sign(movedWheel);
                    wheelIndex = (wheelIndex + dir + WheelCycle.Length) % WheelCycle.Length;
                    entityToSpawn = currentWheelValue;
                }
            }
        }

        List<EntityWithTTL> toRemove = new List<EntityWithTTL>();
        EntityWithTTL previousEntity = null;
        foreach (EntityWithTTL wrapper in spawnedEntitiesWithTTL)
        {
            if (wrapper.TimeToLive > 0f && (despawnMethod == DespawnMethod.TTL || despawnMethod == DespawnMethod.TTLFlag))
            {
                wrapper.TimeToLive -= Engine.RawDeltaTime;
                level.Session.SetSlider("KoseiHelper_EntitySpawnerTTL" + wrapper.Entity.ToString(), wrapper.TimeToLive);
            }
            if (wrapper.Entity is MoveBlock moveBlock) // They would crash the game if they were too out of bounds, so...
            {
                if (wrapper.Entity.Bottom > (float)level.Bounds.Bottom + 16 || // ... We remove move blocks regardless of TTS if they are oob!
                    wrapper.Entity.Left < (float)level.Bounds.Left - 16 ||
                    wrapper.Entity.Right > (float)level.Bounds.Right + 16 ||
                    wrapper.Entity.Top < (float)level.Bounds.Top - 16)
                {
                    Scene.Remove(wrapper.Entity);
                    toRemove.Add(wrapper);
                }

            }
            switch (despawnMethod)
            {
                case DespawnMethod.None:
                    break;
                case DespawnMethod.Flag:
                    if (level.Session.GetFlag(wrapper.TTLFlag))
                        HandleDespawn(wrapper, player, level, toRemove, poofWhenDisappearing, disappearSound, poofParticle, dummyFix);
                    break;
                case DespawnMethod.TTLFlag:
                    if (wrapper.TimeToLive <= 0 || level.Session.GetFlag(wrapper.TTLFlag))
                        HandleDespawn(wrapper, player, level, toRemove, poofWhenDisappearing, disappearSound, poofParticle, dummyFix);
                    break;
                case DespawnMethod.OverriddenByNext:
                    if (previousEntity != null)
                        HandleDespawn(previousEntity, player, level, toRemove, poofWhenDisappearing, disappearSound, poofParticle, dummyFix);
                    break;
                default: // TTL
                    if (wrapper.TimeToLive <= 0)
                        HandleDespawn(wrapper, player, level, toRemove, poofWhenDisappearing, disappearSound, poofParticle, dummyFix);
                    break;
            }
            previousEntity = wrapper;
        }
        // Remove the entities from the list after they have been removed from the scene
        foreach (EntityWithTTL wrapper in toRemove)
        {
            spawnedEntitiesWithTTL.Remove(wrapper);
        }
        //Reset conditions on different spawn modes so they don't check once per frame:
        if (player != null)
        {
            if (Math.Abs(player.Speed.X) < spawnSpeed && hasSpawnedFromSpeed) // Resets speed check if speed falls below threshold
                hasSpawnedFromSpeed = false;
        }
        previousHasSpawnedFromFlag = currentHasSpawnedFromFlag; // used on Flag Spawn Condition mode
        canSpawnFromCassette = false;
        if (player != null)
        {
            if (player.StartedDashing)
            {
                dashCount++;
                if (dashCount >= everyXDashes)
                    dashCount = 0;
            }
        }
    }

    private void HandleDespawn(EntityWithTTL wrapper, Player player, Level level, List<EntityWithTTL> toRemove, bool poofWhenDisappearing, string disappearSound, ParticleType poofParticle, bool dummyFix)
    {
        Audio.Play(disappearSound, wrapper.Entity.Position);
        if (poofWhenDisappearing)
            level.ParticlesFG.Emit(poofParticle, 5, wrapper.Entity.Center, Vector2.One * 4f, 0 - (float)Math.PI / 2f);

        if (randomLocation && doNotRepeatSpots) // this spot becomes eligible again (sorry for the long line to whoever reads this)
            usedSpawnPositions.Remove(gridAligned
                ? new Vector2((float)Math.Floor((wrapper.Entity.Position - new Vector2(offsetX, offsetY)).X / gridSize) * gridSize,
                (float)Math.Floor((wrapper.Entity.Position - new Vector2(offsetX, offsetY)).Y / gridSize) * gridSize)
                : (wrapper.Entity.Position - new Vector2(offsetX, offsetY)).Floor());

        //Fixes being stuck in StDummy if the player touches Badeline as she's poofing
        if (wrapper.Entity is BadelineBoost && player != null && player.StateMachine.State == 11)
            player.StateMachine.State = 0;
        if (wrapper.Entity is SwapBlockNoBg swapBlockNoBg)
            Scene.Remove(swapBlockNoBg.path);
        if (wrapper.Entity is IceBlock iceBlock) // Removes solids from iceBlocks
        {
            Solid solidToRemove = iceBlock.solid;
            if (solidToRemove != null)
            {
                Scene.Remove(solidToRemove);
                iceBlock.solid = null;
            }
        }
        if (wrapper.Entity is FireBarrier fireBarrier) // Removes solids from fireBarriers
        {
            Solid solidToRemove = fireBarrier.solid;
            if (solidToRemove != null)
            {
                Scene.Remove(solidToRemove);
                fireBarrier.solid = null;
            }
        }
        Scene.Remove(wrapper.Entity);
        toRemove.Add(wrapper);
        if (dummyFix)
        {
            foreach (BadelineDummy badelineDummy in level.Entities.OfType<BadelineDummy>())
                Scene.Remove(badelineDummy);
        }
    }

    private Entity GetEntityFromPath(Vector2 spawn, Vector2 node, LevelData data)
    {
        Logger.Debug(nameof(KoseiHelperModule), $"Spawning entity at position: {spawn}");
        EntityData entityData;
        if (noNode)
        {
            entityData = new()
            {
                Position = spawn,
                Width = blockWidth,
                Height = blockHeight,
                Level = data,
                Values = new()
            };
        }
        else
        {
            entityData = new()
            {
                Position = spawn,
                Width = blockWidth,
                Height = blockHeight,
                Nodes = new Vector2[] { node },
                Level = data,
                Values = new()
            };
        }
        for (int i = 0; i < dictionaryKeys.Count; i++)
        {
            entityData.Values[dictionaryKeys[i]] = dictionaryValues.ElementAtOrDefault(i);
        }
        EntityID newID = new EntityID(data.Name, entityID++);
        Type entityType;
        try
        {
            entityType = FakeAssembly.GetFakeEntryAssembly().GetType(entityPath); // This is where it gets the type, like Celeste.Strawberry
        }
        catch
        {
            Logger.Log(LogLevel.Error, "KoseiHelper", "Failed to get entity: Requested type does not exist");
            return null;
        }

        ConstructorInfo[] ctors = entityType.GetConstructors();
        try
        {
            foreach (ConstructorInfo ctor in ctors)
            {
                ParameterInfo[] parameters = ctor.GetParameters();
                List<object> ctorParams = new List<object>();

                foreach (ParameterInfo param in parameters)
                {
                    if (param.ParameterType == typeof(EntityData))
                    {
                        ctorParams.Add(entityData);
                    }
                    else if (param.ParameterType == typeof(Vector2))
                    { //Needs to check if the entity has just the position or position + offset
                        if (param.Name.Contains("position", StringComparison.OrdinalIgnoreCase))
                        {
                            ctorParams.Add(spawn);
                        }
                        else
                        {
                            Vector2 vec = Vector2.Zero;
                            if (entityData.Values.TryGetValue(param.Name, out object val) && val is string s)
                            {
                                s = s.Trim();
                                if ((s.StartsWith("[") && s.EndsWith("]")) || (s.StartsWith("(") && s.EndsWith(")")))
                                    s = s.Substring(1, s.Length - 2);

                                string[] parts = s.Split(',');
                                if (parts.Length == 2 &&
                                    float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                                    float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                                {
                                    vec = new Vector2(x, y);
                                }
                            }
                            if (vec == Vector2.Zero)
                            {
                                float x = entityData.Float(param.Name + "X", 0f);
                                float y = entityData.Float(param.Name + "Y", 0f);
                                vec = new Vector2(x, y);
                            }

                            ctorParams.Add(vec);
                        }
                    }
                    else if (param.ParameterType.IsEnum)
                    {
                        if (entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value is string enumValue)
                        {
                            Type enumType = param.ParameterType;
                            Object enumParsed = Enum.Parse(enumType, enumValue);
                            ctorParams.Add(enumParsed);
                        }
                        else
                        {
                            ctorParams.Add(Enum.GetValues(param.ParameterType).GetValue(0));
                        }
                    }
                    else if (param.ParameterType == typeof(bool))
                    {
                        ctorParams.Add(entityData.Bool(param.Name, defaultValue: false));
                    }
                    else if (param.ParameterType == typeof(int))
                    {
                        ctorParams.Add(entityData.Int(param.Name, defaultValue: 0));
                    }
                    else if (param.ParameterType == typeof(float))
                    {
                        ctorParams.Add(entityData.Float(param.Name, defaultValue: 0f));
                    }
                    else if (param.ParameterType == typeof(EntityID))
                    {
                        ctorParams.Add(new EntityID(data.Name, entityID++));
                    }
                    else if (param.ParameterType == typeof(string))
                    {
                        ctorParams.Add(entityData.Attr(param.Name, ""));
                    }
                    else if (param.ParameterType == typeof(Vector2[])) // hope the naming is correct but idk might work for a few entities
                    {
                        ctorParams.Add(entityData.Nodes != null && entityData.Nodes.Length > 0
                            ? new Vector2[] { entityData.Position }.Concat(entityData.Nodes).ToArray() : new Vector2[] { entityData.Position });
                    }
                    else if (param.ParameterType == typeof(Hitbox)) // hope the naming is correct but idk might work for a few entities
                    {
                        string prefix = param.Name;

                        float width = entityData.Float($"{prefix}Width", 16f);
                        float height = entityData.Float($"{prefix}Height", 16f);
                        float x = entityData.Float($"{prefix}X", 0f);
                        float y = entityData.Float($"{prefix}Y", 0f);
                        if (!entityData.Values.ContainsKey($"{prefix}X") && entityData.Values.ContainsKey($"{prefix}XOffset"))
                            x = entityData.Float($"{prefix}XOffset", 0f);

                        if (!entityData.Values.ContainsKey($"{prefix}Y") && entityData.Values.ContainsKey($"{prefix}YOffset"))
                            y = entityData.Float($"{prefix}YOffset", 0f);

                        ctorParams.Add(new Hitbox(width, height, x, y));
                    }
                    else
                    {
                        Logger.Log(LogLevel.Warn, "KoseiHelper", $"Unhandled parameter type: {param.ParameterType}");
                        ctorParams.Add(param.ParameterType.IsValueType ? Activator.CreateInstance(param.ParameterType) : null);
                    }
                }
                return (Entity)ctor.Invoke(ctorParams.ToArray());
            }
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, "KoseiHelper", $"Failed to instantiate entity: {ex.Message}");
        }
        return null;
    }

    private void OnChangeMode(Session.CoreModes mode)
    {
        coreMode = mode;
    }

    private static void jumpCheck(Player player)
    {
        playerIsJumping = true;
    }

    private static void onPlayerJump(On.Celeste.Player.orig_Jump orig, Player self, bool particles, bool playSfx)
    {
        jumpCheck(self);
        orig(self, particles, playSfx);
    }

    private static void onPlayerSuperJump(On.Celeste.Player.orig_SuperJump orig, Player self)
    {
        jumpCheck(self);
        orig(self);
    }

    private static void onPlayerWallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir)
    {
        jumpCheck(self);
        orig(self, dir);
    }

    private static void onPlayerSuperWallJump(On.Celeste.Player.orig_SuperWallJump orig, Player self, int dir)
    {
        jumpCheck(self);
        orig(self, dir);
    }

    public override void Render()
    {
        if (mouseWheelMode && (spawnCondition == SpawnCondition.LeftClick || spawnCondition == SpawnCondition.RightClick))
        {
            base.Render();
            Player player = SceneAs<Level>().Tracker.GetEntity<Player>();
            Level level = SceneAs<Level>();
            if (level != null && player != null)
            {
                if (entityToSpawn == EntityType.CustomEntity)
                {
                    if (string.IsNullOrEmpty(entityPath))
                    {
                        if (!wheelIndicatorImage)
                            ActiveFont.Draw("[Custom entity not specified]", new Vector2(wheelIndicatorX, wheelIndicatorY), Vector2.Zero, new Vector2(0.5f, 0.5f), Color.White, 1f, Color.Black, 1f, Color.Black);
                        else
                        {
                            Image image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Empty"]);
                            image.Position = new Vector2(level.Camera.Position.X + wheelIndicatorX, level.Camera.Position.Y + wheelIndicatorY);
                            image.Render();
                        }
                    }
                    else
                    {
                        if (!wheelIndicatorImage)
                            ActiveFont.Draw(entityPath.ToString(), new Vector2(wheelIndicatorX, wheelIndicatorY), Vector2.Zero, new Vector2(0.5f, 0.5f), Color.White, 1f, Color.Black, 1f, Color.Black);
                        else
                        {
                            Image image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Broken"]);
                            image.Position = new Vector2(level.Camera.Position.X + wheelIndicatorX, level.Camera.Position.Y + wheelIndicatorY);
                            image.Render();
                        }
                    }
                }
                else
                {
                    if (!wheelIndicatorImage)
                        ActiveFont.Draw(entityToSpawn.ToString(), new Vector2(wheelIndicatorX, wheelIndicatorY), Vector2.Zero, new Vector2(0.5f, 0.5f), Color.White, 1f, Color.Black, 1f, Color.Black);
                    else
                    {
                        Image image = PreRenderWheelIndicator(level);
                        image.Render();
                    }
                }
            }
        }
    }

    public Image PreRenderWheelIndicator(Level level)
    {
        Image image;
        switch (entityToSpawn)
        {
            case EntityType.Blade:
                if (bladeStar)
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Star"]);
                else
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Blade"]);
                break;
            case EntityType.Booster:
                if (boosterRed)
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Booster"]);
                else
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/BoosterGreen"]);
                break;
            case EntityType.Cloud:
                if (cloudFragile)
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/CloudFragile"]);
                else
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Cloud"]);
                break;
            case EntityType.DashSwitch:
                switch (dashSwitchSide)
                {
                    case "Left":
                        if (dashSwitchSprite == "Mirror")
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchLeftMirror"]);
                        else
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchLeftDefault"]);
                        break;
                    case "Right":
                        if (dashSwitchSprite == "Mirror")
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchRightMirror"]);
                        else
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchRightDefault"]);
                        break;
                    case "Down":
                        if (dashSwitchSprite == "Mirror")
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchDownMirror"]);
                        else
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchDownDefault"]);
                        break;
                    default:
                        if (dashSwitchSprite == "Mirror")
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchUpMirror"]);
                        else
                            image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/DashSwitchUpDefault"]);
                        break;
                }
                break;
            case EntityType.Feather:
                if (featherShielded)
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/FeatherShielded"]);
                else
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Feather"]);
                break;
            case EntityType.Jellyfish:
                if (jellyfishBubble)
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/JellyfishFloating"]);
                else
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Jellyfish"]);
                break;
            case EntityType.Refill:
                if (refillTwoDashes)
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/RefillPink"]);
                else
                    image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Refill"]);
                break;
            case EntityType.Spinner:
                switch (crystalColor)
                {
                    case CrystalColor.Purple:
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpinnerPurple"]);
                        break;
                    case CrystalColor.Red:
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpinnerRed"]);
                        break;
                    case CrystalColor.Rainbow:
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpinnerWhite"]);
                        break;
                    default:
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpinnerBlue"]);
                        break;
                }
                break;
            case EntityType.Spring:
                switch (orientation)
                {
                    case "WallLeft":
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpringWallLeft"]);
                        break;
                    case "WallRight":
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpringWallRight"]);
                        break;
                    case "PlayerFacing":
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpringPlayerFacing"]);
                        break;
                    case "PlayerFacingOpposite":
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpringPlayerFacingOpposite"]);
                        break;
                    default:
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/SpringFloor"]);
                        break;
                }
                break;
            case EntityType.Strawberry:
                if (isMoon)
                {
                    if (isWinged)
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/StrawberryWingedMoon"]);
                    else
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/StrawberryMoon"]);
                }
                else
                {
                    if (isWinged)
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/StrawberryWinged"]);
                    else
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/Strawberry"]);
                }
                break;
            case EntityType.TempleGate:
                switch (templeGateSprite)
                {
                    case "Mirror":
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/TempleGateMirror"]);
                        break;
                    case "Theo":
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/TempleGateTheo"]);
                        break;
                    default:
                        image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/TempleGate"]);
                        break;
                }
                break;
            default:
                image = new Image(GFX.Game["objects/KoseiHelper/Controllers/SpawnController/" + entityToSpawn.ToString()]);
                break;
        }
        image.Position = new Vector2(level.Camera.Position.X + wheelIndicatorX, level.Camera.Position.Y + wheelIndicatorY);
        return image;
    }

    public static void Load()
    {
        On.Celeste.Player.Jump += onPlayerJump;
        On.Celeste.Player.SuperJump += onPlayerSuperJump;
        On.Celeste.Player.WallJump += onPlayerWallJump;
        On.Celeste.Player.SuperWallJump += onPlayerSuperWallJump;
    }

    public static void Unload()
    {
        On.Celeste.Player.Jump -= onPlayerJump;
        On.Celeste.Player.SuperJump -= onPlayerSuperJump;
        On.Celeste.Player.WallJump -= onPlayerWallJump;
        On.Celeste.Player.SuperWallJump -= onPlayerSuperWallJump;
    }
}