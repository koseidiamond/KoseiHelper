using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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
    public EntityType entityToSpawn;
    public float spawnCooldown, spawnTime;
    public bool relativeToPlayerFacing;
    private int entityID = 2388544;

    //entity specific data
    public bool cloudFragile;
    public bool boosterRed;
    public int iceBlockWidth, iceBlockHeight = 16;
    public bool featherShielded;
    public bool featherSingleUse;
    public char dashBlockTileType;
    public int dashBlockWidth, dashBlockHeight = 16;
    public bool dashBlockCanDash;
    public float iceballSpeed;
    public bool iceballAlwaysIce;
    public int moveBlockWidth, moveBlockHeight;
    public bool moveBlockCanSteer, moveBlockFast;
    public MoveBlock.Directions moveBlockDirection;
    public int swapBlockWidth, swapBlockHeight, swapBlockNodeX, swapBlockNodeY;
    public SwapBlock.Themes swapBlockTheme;
    public int zipMoverWidth, zipMoverHeight, zipMoverNodeX, zipMoverNodeY;
    public ZipMover.Themes zipMoverTheme;
    public string flag;
    public bool flagValue;
    private List<Entity> spawnedEntities = new List<Entity>();
    public Entity spawnedEntity = null;

    public SpawnController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        offsetX = data.Int("offsetX", 0);
        offsetY = data.Int("offsetY", 8);
        entityToSpawn = data.Enum("entityToSpawn", EntityType.Puffer);
        spawnCooldown = spawnTime = data.Float("spawnCooldown", 0f);
        removeDash = data.Bool("removeDash", true);
        relativeToPlayerFacing = data.Bool("relativeToPlayerFacing", true);
        timeToLive = data.Float("timeToLive", -1f);

        //entity specific data
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
        iceballSpeed = data.Float("iceballSpeed", 1f);
        iceballAlwaysIce = data.Bool("iceballAlwaysIce", false);
        moveBlockDirection = data.Enum("moveBlockDirection", MoveBlock.Directions.Down);
        flag = data.Attr("flag", "");
        flagValue = data.Bool("flagValue", true);
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
        if (spawnCooldown > 0)
            spawnCooldown -= Engine.RawDeltaTime;
        else
            spawnCooldown = 0f;
        player = Scene.Tracker.GetEntity<Player>();
        if ((flagValue && level.Session.GetFlag(flag)) || string.IsNullOrEmpty(flag) || (!flagValue && !level.Session.GetFlag(flag)))
        { // If the flag is true, or if no flag is required, spawn the entity
            if (KoseiHelperModule.Settings.SpawnButton.Pressed && spawnCooldown == 0)
            {
                if (removeDash && player.Dashes > 0)
                    player.Dashes -= 1;
                var spawnPosition = new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY);
                bool useNegativeOffset = relativeToPlayerFacing && player.Facing == Facings.Left;
                if (useNegativeOffset)
                    spawnPosition = new Vector2(player.Position.X - offsetX, player.Position.Y + offsetY);
                else
                    spawnPosition = new Vector2(player.Position.X + offsetX, player.Position.Y + offsetY);
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
                    case EntityType.IceBlock:
                        spawnedEntity = new IceBlock(spawnPosition, 8, 8);
                        break;
                    case EntityType.Heart:
                        spawnedEntity = new FakeHeart(spawnPosition);
                        break;
                    case EntityType.DashBlock:
                        spawnedEntity = new DashBlock(spawnPosition, dashBlockTileType, dashBlockWidth, dashBlockHeight, false, true, dashBlockCanDash, new EntityID("koseiHelper_spawnedDashBlock", entityID));
                        entityID += 1;
                        break;
                    case EntityType.Feather:
                        spawnedEntity = new FlyFeather(spawnPosition, featherShielded, featherSingleUse);
                        break;
                    case EntityType.Iceball:
                        spawnedEntity = new FireBall(new Vector2[] { spawnPosition }, 1, 1, 0, iceballSpeed, iceballAlwaysIce);
                        break;
                    case EntityType.MoveBlock:
                        spawnedEntity = new MoveBlock(spawnPosition, moveBlockWidth, moveBlockHeight, moveBlockDirection, moveBlockCanSteer, moveBlockFast);
                        break;
                    case EntityType.Seeker:
                        spawnedEntity = new Seeker(spawnPosition, new Vector2[] { spawnPosition });
                        break;
                    case EntityType.SwapBlock:
                        spawnedEntity = new SwapBlock(spawnPosition, swapBlockWidth, swapBlockHeight, new Vector2(player.Position.X + swapBlockNodeX, player.Position.Y + swapBlockNodeY), swapBlockTheme);
                        break;
                    case EntityType.ZipMover:
                        spawnedEntity = new ZipMover(spawnPosition, zipMoverWidth, zipMoverHeight, new Vector2(player.Position.X + zipMoverNodeX, player.Position.Y + zipMoverNodeY), zipMoverTheme);
                        break;
                    default:
                        break;
                }
                if (spawnedEntity != null)
                {
                    Scene.Add(spawnedEntity);
                    spawnedEntities.Add(spawnedEntity);
                }
                spawnCooldown = spawnTime;
                Audio.Play("event:/KoseiHelper/spawn", player.Position);
            }
        }
    }
    public void RemoveEntity(Entity entity)
    {
        if (spawnedEntities.Contains(entity))
        {
            Scene.Remove(entity);
            spawnedEntities.Remove(entity);
        }
    }
}