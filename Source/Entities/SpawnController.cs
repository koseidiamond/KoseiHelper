using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using static Celeste.Session;
using System.Linq;
using Celeste.Mod.Helpers;
using System.Reflection;

namespace Celeste.Mod.KoseiHelper.Entities;

public enum EntityType
{
    Puffer,
    Cloud,
    BadelineBoost,
    Booster,
    Bumper,
    IceBlock,
    Heart,
    DashBlock,
    FallingBlock,
    Feather,
    Iceball,
    MoveBlock,
    Seeker,
    SwapBlockNoBg,
    ZipMover,
    CrumblePlatform,
    DreamBlock,
    BounceBlock,
    Refill,
    GlassBlock,
    JumpthruPlatform,
    FloatySpaceBlock,
    StarJumpBlock,
    CrushBlock,
    Water,
    Strawberry,
    Decal,
    Flag,
    Counter,
    //The following entities are just alternate names so the name from the plugin is renamed:
    SwapBlock,
    Kevin,
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
    OnClimb = 7
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
    public string spawnFlag;
    public bool spawnFlagValue;
    public float spawnSpeed;
    public int spawnLimit;
    public bool persistency;
    public float timeToLive;
    public string appearSound;
    public string disappearSound;
    public int dashCount, everyXDashes;
#pragma warning disable CS0414
    private bool isBlock = false;
#pragma warning restore CS0414
    private bool hasSpawnedFromSpeed = false;
    private bool previousHasSpawnedFromFlag, currentHasSpawnedFromFlag;
    private int currentCassetteIndex, previousCassetteIndex;
    private bool canSpawnFromCassette = false;
    public int flagCount = 1;
    public int flagCycleAt = 9999;
    public bool flagStop;
    public bool poofWhenDisappearing;
    public bool absoluteCoords = false;
    //other important variables
    private int entityID = 7388544; // Very high value so it doesn't conflict with other ids (hopefully)
    private List<EntityWithTTL> spawnedEntitiesWithTTL = new List<EntityWithTTL>();
    private List<Solid> spawnedSolids = new List<Solid>(); // So lava/ice solids can be removed
    public static ParticleType poofParticle = TouchSwitch.P_FireWhite;
    private List<Entity> spawnedEntities = new List<Entity>();
    public Entity spawnedEntity = null;

    //entity specific data

    public int nodeX, nodeY;
    public int blockWidth, blockHeight = 16;
    public char blockTileType;

    public bool cloudFragile;

    public bool boosterRed;

    public bool dummyFix;

    public bool featherShielded;
    public bool featherSingleUse;

    public float iceballSpeed;
    public bool iceballAlwaysIce;

    public bool dashBlockCanDash;

    public bool fallingBlockBadeline, fallingBlockClimbFall;

    public bool moveBlockCanSteer, moveBlockFast;
    public MoveBlock.Directions moveBlockDirection;

    public SwapBlockNoBg.Themes swapBlockNoBgTheme;
    public ZipMover.Themes zipMoverTheme;

    public string flagToEnableSpawner;
    public bool flagValue;

    public bool refillTwoDashes;

    public string jumpthruTexture;
    public int soundIndex;

    public bool blockSinks;

    public CrushBlock.Axes crushBlockAxe;
    public bool crushBlockChillout;

    public bool hasTop, hasBottom;

    public string decalTexture;
    public int decalDepth;

    private CoreModes coreMode;
    private CassetteBlockManager cassetteBlockManager;

    public int minCounterCap = 0, maxCounterCap = 9999;
    public bool decreaseCounter;

    private bool isWinged;
    private bool isMoon;

    //Custom Entity Fields
    private string entityPath;
    private List<string> dictionaryKeys;
    private List<string> dictionaryValues;

    public SpawnController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Add(new PostUpdateHook(() => { }));
        Collider = new Hitbox(8, 8, -4, -4);
        // General attributes
        offsetX = data.Int("offsetX", 0);
        offsetY = data.Int("offsetY", 8);
        absoluteCoords = data.Bool("absoluteCoords", false);
        entityToSpawn = data.Enum("entityToSpawn", EntityType.Puffer);
        if (entityToSpawn == EntityType.SwapBlock)
            entityToSpawn = EntityType.SwapBlockNoBg;
        if (entityToSpawn == EntityType.Kevin)
            entityToSpawn = EntityType.CrushBlock;
        spawnCooldown = spawnTime = data.Float("spawnCooldown", 0f);
        removeDash = data.Bool("removeDash", false);
        removeStamina = data.Bool("removeStamina", false);
        relativeToPlayerFacing = data.Bool("relativeToPlayerFacing", true);
        nodeRelativeToPlayerFacing = data.Bool("nodeRelativeToPlayerFacing", true);
        timeToLive = data.Float("timeToLive", 0f);
        appearSound = data.Attr("appearSound", "event:/KoseiHelper/spawn");
        disappearSound = data.Attr("disappearSound", "event:/game/general/assist_dash_aim");
        flagToEnableSpawner = data.Attr("flag", "");
        flagValue = data.Bool("flagValue", true);
        spawnCondition = data.Enum("spawnCondition", SpawnCondition.OnCustomButtonPress);
        spawnFlag = data.Attr("spawnFlag", "koseiHelper_spawn");
        spawnFlagValue = data.Bool("spawnFlagValue", true);
        spawnSpeed = data.Float("spawnSpeed", 300f);
        spawnLimit = data.Int("spawnLimit", 3);
        persistency = data.Bool("persistent", false);
        everyXDashes = data.Int("everyXDashes", 1);
        poofWhenDisappearing = data.Bool("poofWhenDisappearing", true);
        dashCount = 0;
        if (persistency)
            base.Tag = Tags.Persistent;

        //Entity specific attributes


        nodeX = data.Int("nodeX", 0);
        nodeY = data.Int("nodeY", 0);
        blockWidth = data.Int("blockWidth", 16);
        blockHeight = data.Int("blockHeight", 16);
        blockTileType = data.Char("blockTileType", '3');

        boosterRed = data.Bool("boosterRed", false);

        dummyFix = data.Bool("dummyFix", true);

        cloudFragile = data.Bool("cloudFragile", true);

        featherShielded = data.Bool("featherShielded", false);
        featherSingleUse = data.Bool("featherSingleUse", true);

        refillTwoDashes = data.Bool("refillTwoDashes", false);

        dashBlockCanDash = data.Bool("dashBlockCanDash", true);

        moveBlockCanSteer = data.Bool("moveBlockCanSteer", false);
        moveBlockFast = data.Bool("moveBlockFast", true);
        moveBlockDirection = data.Enum("moveBlockDirection", MoveBlock.Directions.Down);

        iceballSpeed = data.Float("iceballSpeed", 1f);
        iceballAlwaysIce = data.Bool("iceballAlwaysIce", false);

        swapBlockNoBgTheme = data.Enum("swapBlockTheme", SwapBlockNoBg.Themes.Normal);

        fallingBlockBadeline = data.Bool("fallingBlockBadeline", false);
        fallingBlockClimbFall = data.Bool("fallingBlockClimbFall", false);

        zipMoverTheme = data.Enum("zipMoverTheme", ZipMover.Themes.Normal);

        jumpthruTexture = data.Attr("jumpthruTexture", "wood");
        soundIndex = data.Int("soundIndex", -1);
        blockSinks = data.Bool("blockSinks", false);
        crushBlockAxe = data.Enum("crushBlockAxe", CrushBlock.Axes.Both);
        crushBlockChillout = data.Bool("crushBlockChillout", false);

        hasTop = data.Bool("hasTop", true);
        hasBottom = data.Bool("hasBottom", false);

        decalTexture = data.Attr("decalTexture", "10-farewell/creature_f00");
        decalDepth = data.Int("decalDepth", 9000);

        flagCycleAt = data.Int("flagCycleAt", 9999);
        flagStop = data.Bool("flagStop", false);

        minCounterCap = data.Int("minCounterCap", 0);
        maxCounterCap = data.Int("maxCounterCap", 9999);
        decreaseCounter = data.Bool("decreaseCounter", false);

        isWinged = data.Bool("winged", false);
        isMoon = data.Bool("moon", false);

        //Custom Entities
        entityPath = data.Attr("entityPath");
        dictionaryKeys = data.Attr("dictKeys").Replace(" ", string.Empty).Split(',').ToList();
        dictionaryValues = data.Attr("dictValues").Split(',').ToList();

        Add(new CoreModeListener(OnChangeMode));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        cassetteBlockManager = scene.Tracker.GetEntity<CassetteBlockManager>();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
        Player player = Scene.Tracker.GetEntity<Player>();
        Level level = SceneAs<Level>();
        if (spawnCooldown > 0)
            spawnCooldown -= Engine.RawDeltaTime;
        else
            spawnCooldown = 0f;
        //This HashSet contains not only blocks, but all entities with width. Used to fix spawn positions on relative mode.
        HashSet<EntityType> blockEntities = new HashSet<EntityType>
        {
        EntityType.MoveBlock,
        EntityType.GlassBlock,
        EntityType.CrushBlock,
        EntityType.FallingBlock,
        EntityType.DashBlock,
        EntityType.JumpthruPlatform,
        EntityType.BounceBlock,
        EntityType.FloatySpaceBlock,
        EntityType.StarJumpBlock,
        EntityType.CrumblePlatform,
        EntityType.SwapBlockNoBg,
        EntityType.ZipMover
        };
        bool isBlock = blockEntities.Contains(entityToSpawn);
        // If the flag flagToEnableSpawner is true, or if no flag is required, check if the spawn conditions are met
        // (Not to be confused with spawnFlag, this one is just a common requirement, the other is for the Flag Mode)
        if (((flagValue && level.Session.GetFlag(flagToEnableSpawner)) || string.IsNullOrEmpty(flagToEnableSpawner) ||
            (!flagValue && !level.Session.GetFlag(flagToEnableSpawner))) && player != null && !player.JustRespawned)
        {
            if (cassetteBlockManager != null)
            {
                if (currentCassetteIndex != cassetteBlockManager.currentIndex && // Make sure the player is not too close to the bounds of the level to prevent transition jank
                player.Bottom < (float)level.Bounds.Bottom && player.Top > (float)level.Bounds.Top + 4 &&
                player.Left > (float)level.Bounds.Left + 8 && player.Right < (float)level.Bounds.Right - 8)
                {
                    previousCassetteIndex = currentCassetteIndex;
                    currentCassetteIndex = cassetteBlockManager.currentIndex;
                    canSpawnFromCassette = true;
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
                SpawnCondition.OnClimb => player.StateMachine.state == 1,
                _ => false
            };
            if (conditionMet && spawnLimit != 0 && spawnCooldown == 0 && player != null)
            {
                if (entityToSpawn != EntityType.CustomEntity)
                    Logger.Debug(nameof(KoseiHelperModule), $"An entity is going to spawn: {entityToSpawn}");
                else //Logs the parameters used in Lönn + the original constructor
                    Logger.Debug(nameof(KoseiHelperModule), $"An entity ({entityPath}) is going to spawn, with the attributes: " +
                    $"{string.Join(", ", dictionaryKeys.Zip(dictionaryValues, (key, value) => $"{key}={value}"))}, with parameters: " +
                    $"{string.Join(", ", FakeAssembly.GetFakeEntryAssembly().GetType(entityPath).GetConstructors().Select(constructor =>
                    $"{constructor.Name}({string.Join(", ", constructor.GetParameters().Select(param => $"{param.ParameterType.Name} {param.Name}"))})"))}.");

                if (removeDash && Scene.Tracker.GetEntity<Player>().Dashes > 0)
                    player.Dashes -= 1;
                if (removeStamina)
                    player.Stamina = 0;
                if (spawnLimit > 0)
                    spawnLimit -= 1;
                //Calculate spawn position
                var spawnPosition = new Vector2(X + offsetX, Y + offsetY);
                if (!absoluteCoords)
                {
                    spawnPosition = new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY);
                    if (relativeToPlayerFacing && player.Facing == Facings.Left)
                        spawnPosition = new Vector2(player.Position.X - offsetX, player.Position.Y + offsetY);
                    if (relativeToPlayerFacing && player.Facing == Facings.Right && isBlock)
                        spawnPosition = new Vector2(player.Position.X + offsetX - blockWidth, player.Position.Y + offsetY);
                }
                //Calculate node position
                var nodePosition = new Vector2(X + nodeX, Y + nodeY);
                if (!absoluteCoords)
                {
                    nodePosition = new Vector2(player.Position.X + nodeX, player.Position.Y + nodeY);
                    if (nodeRelativeToPlayerFacing && player.Facing == Facings.Left)
                        nodePosition = new Vector2(player.Position.X - nodeX, player.Position.Y + nodeY);
                    if (relativeToPlayerFacing && player.Facing == Facings.Right && isBlock)
                        nodePosition = new Vector2(player.Position.X + nodeX - blockWidth, player.Position.Y + nodeY);
                }
                float entityTTL = timeToLive;
                switch (entityToSpawn)
                { // If relativeToPlayerFacing is true, a positive X value will spawn in front of the player, and a negative X value will spawn behind the player
                    case EntityType.Puffer:
                        spawnedEntity = new Puffer(spawnPosition, player.Facing == Facings.Right);
                        break;
                    case EntityType.Cloud:
                        spawnedEntity = new Cloud(spawnPosition, true);
                        break;
                    case EntityType.BadelineBoost:
                        spawnedEntity = new BadelineBoost(new Vector2[] { spawnPosition, new Vector2(player.Position.X + offsetX, level.Bounds.Top - 200) }, false, false, false, false, false);
                        break;
                    case EntityType.Booster:
                        spawnedEntity = new BoosterNoOutline(spawnPosition, boosterRed); // I had to make a new class so their outline doesn't stay after they poof
                        break;
                    case EntityType.Bumper:
                        spawnedEntity = new Bumper(spawnPosition, null);
                        break;
                    case EntityType.IceBlock: //It also works for lava actually
                        if (coreMode == CoreModes.Hot)
                            spawnedEntity = new FireBarrier(spawnPosition, blockWidth, blockHeight);
                        else if (coreMode == CoreModes.Cold)
                            spawnedEntity = new IceBlock(spawnPosition, blockWidth, blockHeight);
                        if (spawnedEntity is IceBlock iceBlock)
                        {
                            var solid = iceBlock.solid;
                            if (solid != null)
                                spawnedSolids.Add(solid);
                        }
                        else if (spawnedEntity is FireBarrier fireBarrier)
                        {
                            var solid = fireBarrier.solid;
                            if (solid != null)
                                spawnedSolids.Add(solid);
                        }
                        break;
                    case EntityType.Heart:
                        spawnedEntity = new FakeHeart(spawnPosition);
                        break;
                    case EntityType.DashBlock:
                        spawnedEntity = new NoFreezeDashBlock(spawnPosition, blockTileType, blockWidth, blockHeight, false, true, dashBlockCanDash, new EntityID("koseiHelper_spawnedDashBlock", entityID));
                        entityID++;
                        break;
                    case EntityType.Feather:
                        spawnedEntity = new FlyFeather(spawnPosition, featherShielded, featherSingleUse);
                        break;
                    case EntityType.Iceball:
                        spawnedEntity = new FireBall(new Vector2[] { spawnPosition, nodePosition }, 1, 1, 0, iceballSpeed, iceballAlwaysIce);
                        break;
                    case EntityType.MoveBlock:
                        spawnedEntity = new MoveBlock(spawnPosition, blockWidth, blockHeight, moveBlockDirection, moveBlockCanSteer, moveBlockFast);
                        break;
                    case EntityType.Seeker:
                        spawnedEntity = new Seeker(spawnPosition, new Vector2[] { spawnPosition });
                        break;
                    case EntityType.SwapBlockNoBg:
                        spawnedEntity = new SwapBlockNoBg(spawnPosition, blockWidth, blockHeight, nodePosition, swapBlockNoBgTheme);
                        break;
                    case EntityType.ZipMover:
                        spawnedEntity = new ZipMover(spawnPosition, blockWidth, blockHeight, nodePosition, zipMoverTheme);
                        break;
                    case EntityType.FallingBlock:
                        spawnedEntity = new FallingBlock(spawnPosition, blockTileType, blockWidth, blockHeight, fallingBlockBadeline, false, fallingBlockClimbFall);
                        break;
                    case EntityType.CrumblePlatform:
                        spawnedEntity = new CrumblePlatform(spawnPosition, blockWidth);
                        break;
                    case EntityType.DreamBlock:
                        spawnedEntity = new DreamBlock(spawnPosition, blockWidth, blockHeight, null, false, true);
                        break;
                    case EntityType.BounceBlock:
                        spawnedEntity = new BounceBlock(spawnPosition, blockWidth, blockHeight);
                        break;
                    case EntityType.Refill:
                        spawnedEntity = new Refill(spawnPosition, refillTwoDashes, true);
                        break;
                    case EntityType.GlassBlock:
                        spawnedEntity = new GlassBlock(spawnPosition, blockWidth, blockHeight, blockSinks);
                        break;
                    case EntityType.JumpthruPlatform:
                        spawnedEntity = new JumpthruPlatform(spawnPosition, blockWidth, jumpthruTexture, soundIndex);
                        break;
                    case EntityType.FloatySpaceBlock:
                        spawnedEntity = new FloatySpaceBlock(spawnPosition, blockWidth, blockHeight, blockTileType, true);
                        break;
                    case EntityType.StarJumpBlock:
                        spawnedEntity = new StarJumpBlock(spawnPosition, blockWidth, blockHeight, blockSinks);
                        break;
                    case EntityType.CrushBlock:
                        spawnedEntity = new CrushBlock(spawnPosition, blockWidth, blockHeight, crushBlockAxe, crushBlockChillout);
                        break;
                    case EntityType.Water:
                        spawnedEntity = new Water(spawnPosition,hasTop,hasBottom,blockWidth,blockHeight);
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
                        spawnedEntity = new Strawberry(
                            strawberryData,
                            Vector2.Zero,
                            strawberryID
                        );
                        break;
                    case EntityType.Decal:
                        if (player.Facing == Facings.Left)
                            spawnedEntity = new Decal(decalTexture, spawnPosition, new Vector2(-1, 1), decalDepth);
                        else
                            spawnedEntity = new Decal(decalTexture, spawnPosition, new Vector2(1, 1), decalDepth);
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
                            level.Session.SetFlag("koseiFlag" + (flagCount-1), false);
                        Audio.Play(appearSound, player.Position); //Audio is here because it doesn't actually spawn anything
                        if (spawnCondition == SpawnCondition.OnInterval)
                            spawnCooldown = spawnTime;
                        flagCount++;
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
                    case EntityType.CustomEntity:
                        spawnedEntity = GetEntityFromPath(spawnPosition, nodePosition, level.Session.LevelData);
                        break;
                    default:
                        break;
                }
                if (spawnedEntity != null && player != null)
                { // If the entity has been spawned successfully:
                    Scene.Add(spawnedEntity);
                    spawnedEntitiesWithTTL.Add(new EntityWithTTL(spawnedEntity, entityTTL));
                    spawnCooldown = spawnTime;
                    hasSpawnedFromSpeed = true;
                    Audio.Play(appearSound, player.Position);
                }
            }
        }
        List<EntityWithTTL> toRemove = new List<EntityWithTTL>();
        foreach (var wrapper in spawnedEntitiesWithTTL)
        {
            wrapper.TimeToLive -= Engine.RawDeltaTime;

            if (wrapper.Entity is MoveBlock moveBlock) //Fix: They would crash the game if they were too out of bounds, so...
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
            if (wrapper.TimeToLive <= 0) // The instance of the entity will (literally) make poof
            {
                Audio.Play(disappearSound, wrapper.Entity.Position);
                if (timeToLive > 0 && poofWhenDisappearing)
                    level.ParticlesFG.Emit(poofParticle, 5, wrapper.Entity.Center, Vector2.One * 4f, 0 - (float)Math.PI / 2f);
                if (wrapper.Entity is BadelineBoost && player.StateMachine.State == 11) //Fixes being stuck in StDummy if the player touches Badeline as she's poofing
                    player.StateMachine.State = 0;
                if (wrapper.Entity is IceBlock iceBlock) // Removes solids from iceBlocks
                {
                    var solidToRemove = iceBlock.solid;
                    if (solidToRemove != null)
                    {
                        Scene.Remove(solidToRemove);
                        iceBlock.solid = null;
                    }
                }
                if (wrapper.Entity is FireBarrier fireBarrier) // FIX: Remove solids from fireBarriers
                {
                    var solidToRemove = fireBarrier.solid;
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
                    foreach (var badelineDummy in level.Entities.OfType<BadelineDummy>())
                        Scene.Remove(badelineDummy);
                }
            }
        }
        // Remove the entities from the list after they have been removed from the scene
        foreach (var wrapper in toRemove)
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
        previousCassetteIndex = currentCassetteIndex; // used on Cassette Beat Condition mode
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

    private Entity GetEntityFromPath(Vector2 spawn, Vector2 node, LevelData data)
    {
        Logger.Debug(nameof(KoseiHelperModule), $"Spawning entity at position: {spawn}");
        EntityData entityData = new()
        {
            Position = spawn,
            Width = blockWidth,
            Height = blockHeight,
            Nodes = new Vector2[] { node },
            Level = data,
            Values = new()
        };
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
        catch (ArgumentNullException)
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

                foreach (var param in parameters)
                {
                    if (param.ParameterType == typeof(EntityData))
                    {
                        ctorParams.Add(entityData);
                    }
                    else if (param.ParameterType == typeof(Vector2))
                    { //Needs to check if the entity has just the position or position + offset
                        if (param.Name.Contains("position", StringComparison.OrdinalIgnoreCase))
                            ctorParams.Add(spawn);
                        else
                            ctorParams.Add(Vector2.Zero);
                    }
                    else if (param.ParameterType.IsEnum)
                    {
                        string enumValue = entityData.Values.FirstOrDefault(kv => kv.Key == param.Name).Value as string;
                        if (enumValue != null)
                        {
                            var enumType = param.ParameterType;
                            var enumParsed = Enum.Parse(enumType, enumValue);
                            ctorParams.Add(enumParsed);
                        }
                        else
                            ctorParams.Add(Enum.GetValues(param.ParameterType).GetValue(0));
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
                    else
                    {
                        Logger.Log(LogLevel.Warn, "KoseiHelper", $"Unhandled parameter type {param.ParameterType}");
                        ctorParams.Add(null);
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
}