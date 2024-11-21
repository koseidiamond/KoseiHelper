using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using static Celeste.Session;

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
    SwapBlock,
    ZipMover
}

[CustomEntity("KoseiHelper/SpawnController")]
public class SpawnController : Entity
{

    private Level level;
    private Player player;
    public int offsetY;
    public int offsetX;
    public bool removeDash;
    public bool removeStamina;
    public EntityType entityToSpawn;
    public float spawnCooldown, spawnTime;
    public bool relativeToPlayerFacing;
    private int entityID = 7388544; // Very high value so it doesn't conflict with other ids (hopefully)
    private List<EntityWithTTL> spawnedEntitiesWithTTL = new List<EntityWithTTL>();
    private List<Solid> spawnedSolids = new List<Solid>(); // So lava/ice solids can be removed
    public static ParticleType poofParticle = TouchSwitch.P_FireWhite;
    private List<Entity> spawnedEntities = new List<Entity>();
    public Entity spawnedEntity = null;

    //entity specific data

    public int nodeX, nodeY;
    public bool cloudFragile;

    public bool boosterRed;

    public bool featherShielded;
    public bool featherSingleUse;

    public float iceballSpeed;
    public bool iceballAlwaysIce;

    public int iceBlockWidth, iceBlockHeight = 16;
    private CoreModes coreMode;

    public char dashBlockTileType;
    public int dashBlockWidth, dashBlockHeight = 16;
    public bool dashBlockCanDash;

    public char fallingBlockTile;
    public int fallingBlockWidth, fallingBlockHeight;
    public bool fallingBlockBadeline, fallingBlockClimbFall;

    public int moveBlockWidth, moveBlockHeight;
    public bool moveBlockCanSteer, moveBlockFast;
    public MoveBlock.Directions moveBlockDirection;

    public int swapBlockWidth, swapBlockHeight;
    public SwapBlock.Themes swapBlockTheme;

    public int zipMoverWidth, zipMoverHeight;
    public ZipMover.Themes zipMoverTheme;

    public string flag;
    public bool flagValue;

    public float timeToLive;
    public string appearSound;
    public string disappearSound;

    public SpawnController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        // General attributes
        offsetX = data.Int("offsetX", 0);
        offsetY = data.Int("offsetY", 8);
        entityToSpawn = data.Enum("entityToSpawn", EntityType.Puffer);
        spawnCooldown = spawnTime = data.Float("spawnCooldown", 0f);
        removeDash = data.Bool("removeDash", false);
        removeStamina = data.Bool("removeStamina", false);
        relativeToPlayerFacing = data.Bool("relativeToPlayerFacing", true);
        timeToLive = data.Float("timeToLive", -1f);
        appearSound = data.Attr("appearSound", "event:/none");
        disappearSound = data.Attr("disappearSound", "event:/KoseiHelper/spawn");
        flag = data.Attr("flag", "");
        flagValue = data.Bool("flagValue", true);

        //Entity specific attributes


        nodeX = data.Int("zipMoverNodeX", 16);
        nodeY = data.Int("zipMoverNodeY", 16);

        boosterRed = data.Bool("boosterRed", false);

        iceBlockWidth = data.Int("iceBlockWidth", 16);
        iceBlockHeight = data.Int("iceBlockHeight", 16);

        cloudFragile = data.Bool("cloudFragile", true);

        featherShielded = data.Bool("featherShielded", false);
        featherSingleUse = data.Bool("featherSingleUse", true);

        dashBlockTileType = data.Char("dashBlockTileType", '3');
        dashBlockWidth = data.Int("dashBlockWidth", 16);
        dashBlockHeight = data.Int("dashBlockHeight", 16);
        dashBlockCanDash = data.Bool("dashBlockCanDash", true);

        moveBlockWidth = data.Int("moveBlockWidth", 16);
        moveBlockHeight = data.Int("moveBlockHeight", 16);
        moveBlockCanSteer = data.Bool("moveBlockCanSteer", false);
        moveBlockFast = data.Bool("moveBlockFast", true);
        moveBlockDirection = data.Enum("moveBlockDirection", MoveBlock.Directions.Down);

        iceballSpeed = data.Float("iceballSpeed", 1f);
        iceballAlwaysIce = data.Bool("iceballAlwaysIce", false);

        moveBlockDirection = data.Enum("moveBlockDirection", MoveBlock.Directions.Down);

        swapBlockWidth = data.Int("swapBlockWidth", 16);
        swapBlockHeight = data.Int("swapBlockHeight", 16);
        swapBlockTheme = data.Enum("swapBlockTheme", SwapBlock.Themes.Normal);

        fallingBlockTile = data.Char("fallingBlockTile", '3');
        fallingBlockWidth = data.Int("fallingBlockWidth", 16);
        fallingBlockHeight = data.Int("fallingBlockHeight", 16);
        fallingBlockBadeline = data.Bool("fallingBlockBadeline", false);
        fallingBlockClimbFall = data.Bool("fallingBlockClimbFall", false);

        zipMoverWidth = data.Int("zipMoverWidth", 16);
        zipMoverHeight = data.Int("zipMoverHeight", 16);
        zipMoverTheme = data.Enum("zipMoverTheme", ZipMover.Themes.Normal);
        Add(new CoreModeListener(OnChangeMode));
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();

    }

    public override void Update()
    {
        base.Update();
        Logger.Debug(nameof(KoseiHelperModule), $"Current core mode is: {coreMode}");
        if (spawnCooldown > 0)
            spawnCooldown -= Engine.RawDeltaTime;
        else
            spawnCooldown = 0f;
        if ((flagValue && level.Session.GetFlag(flag)) || string.IsNullOrEmpty(flag) || (!flagValue && !level.Session.GetFlag(flag)))
        { // If the flag is true, or if no flag is required, spawn the entity
            if (KoseiHelperModule.Settings.SpawnButton.Pressed && spawnCooldown == 0 && Scene.Tracker.GetEntity<Player>() != null)
            {
                player = Scene.Tracker.GetEntity<Player>();
                if (removeDash && Scene.Tracker.GetEntity<Player>().Dashes > 0)
                    player.Dashes -= 1;
                if (removeStamina)
                    player.Stamina = 0;
                var spawnPosition = new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY);
                bool useNegativeOffset = relativeToPlayerFacing && player.Facing == Facings.Left;
                if (useNegativeOffset)
                    spawnPosition = new Vector2(player.Position.X - offsetX, player.Position.Y + offsetY);
                else
                    spawnPosition = new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY);

                int adjustedNodeX = player.Facing == Facings.Left ? (int)(player.Position.X - nodeX) : (int)(player.Position.X + nodeX);
                int adjustedNodeY = (int)(player.Position.Y + nodeY);

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
                        spawnedEntity = new BadelineBoost(new Vector2[] { new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY), new Vector2(player.Position.X + offsetX, level.Bounds.Top - 200) }, false, false, false, false, false);
                        break;
                    case EntityType.Booster:
                        spawnedEntity = new Booster(spawnPosition, boosterRed);
                        break;
                    case EntityType.Bumper:
                        spawnedEntity = new Bumper(spawnPosition, null);
                        break;
                    case EntityType.IceBlock: //It also works for lava actually
                        if (coreMode == CoreModes.Hot)
                            spawnedEntity = new FireBarrier(spawnPosition, iceBlockWidth, iceBlockHeight);
                        else if (coreMode == CoreModes.Cold)
                            spawnedEntity = new IceBlock(spawnPosition, iceBlockWidth, iceBlockHeight);
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
                    case EntityType.DashBlock: //TODO fix tiletype selection in their plugin
                        spawnedEntity = new NoFreezeDashBlock(spawnPosition, dashBlockTileType, dashBlockWidth, dashBlockHeight, false, true, dashBlockCanDash, new EntityID("koseiHelper_spawnedDashBlock", entityID));
                        entityID += 1;
                        break;
                    case EntityType.Feather:
                        spawnedEntity = new FlyFeather(spawnPosition, featherShielded, featherSingleUse);
                        break;
                    case EntityType.Iceball: //TODO implement nodeX/nodeY
                        spawnedEntity = new FireBall(new Vector2[] { spawnPosition }, 1, 1, 0, iceballSpeed, iceballAlwaysIce);
                        break;
                    case EntityType.MoveBlock: // TODO THEY ARE JANK
                        spawnedEntity = new MoveBlock(spawnPosition, moveBlockWidth, moveBlockHeight, moveBlockDirection, moveBlockCanSteer, moveBlockFast);
                        break;
                    case EntityType.Seeker:
                        spawnedEntity = new Seeker(spawnPosition, new Vector2[] { spawnPosition });
                        break;
                    case EntityType.SwapBlock:
                        spawnedEntity = new SwapBlock(spawnPosition, swapBlockWidth, swapBlockHeight, new Vector2(adjustedNodeX, adjustedNodeY), swapBlockTheme);
                        break;
                    case EntityType.ZipMover:
                        spawnedEntity = new ZipMover(spawnPosition, zipMoverWidth, zipMoverHeight, new Vector2(adjustedNodeX, adjustedNodeY), zipMoverTheme);
                        break;
                    case EntityType.FallingBlock:
                        spawnedEntity = new FallingBlock(spawnPosition, fallingBlockTile, fallingBlockWidth, fallingBlockHeight, fallingBlockBadeline, false, fallingBlockClimbFall);
                        break;
                    default:
                        break;
                }
                if (spawnedEntity != null)
                {
                    Scene.Add(spawnedEntity);
                    spawnedEntitiesWithTTL.Add(new EntityWithTTL(spawnedEntity, entityTTL));
                }
                spawnCooldown = spawnTime;
                Audio.Play(appearSound, player.Position);
            }
        }
        List<EntityWithTTL> toRemove = new List<EntityWithTTL>();
        foreach (var wrapper in spawnedEntitiesWithTTL)
        {
            wrapper.TimeToLive -= Engine.RawDeltaTime;

            if (wrapper.Entity is MoveBlock moveBlock) //Fix: They crash the game when they're too out of bounds, so...
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
                level.ParticlesFG.Emit(poofParticle, 5, wrapper.Entity.Position, Vector2.One * 4f, 0 - (float)Math.PI / 2f);
                if (wrapper.Entity is BadelineBoost && player.StateMachine.State == 11) //FIX: being stuck in StDummy if the player touches Badeline as she's poofing
                    player.StateMachine.State = 0;
                if (wrapper.Entity is IceBlock iceBlock) // FIX: Remove solids from iceBlocks
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
            }
        }
        // Remove the entities from the list after they have been removed from the scene
        foreach (var wrapper in toRemove)
        {
            spawnedEntitiesWithTTL.Remove(wrapper);
        }
    }
    private void OnChangeMode(Session.CoreModes mode)
    {
        coreMode = mode;
    }
}