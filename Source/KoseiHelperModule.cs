using Celeste.Mod.KoseiHelper.DecalRegistry;
using Celeste.Mod.KoseiHelper.Entities;
using MonoMod.ModInterop;
using System;
using Microsoft.Xna.Framework;
using Celeste.Mod.KoseiHelper.Entities.Crossover;
using Monocle;
using System.Collections;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModule : EverestModule
{
    public static KoseiHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(KoseiHelperModuleSettings);
    public static KoseiHelperModuleSettings Settings => (KoseiHelperModuleSettings)Instance._Settings;

    public override Type SessionType => typeof(KoseiHelperModuleSession);
    public static KoseiHelperModuleSession Session => (KoseiHelperModuleSession)Instance._Session;

    public override Type SaveDataType => typeof(KoseiHelperModuleSaveData);
    public static KoseiHelperModuleSaveData SaveData => (KoseiHelperModuleSaveData)Instance._SaveData;

    public bool frostHelperLoaded = false;
    public bool collabUtils2Loaded = false;
    public bool helpingHandLoaded = false;
    public bool doonvHelperLoaded = false;
    public bool femtoHelperLoaded = false;

    public static TextMenu ReturnToGDMenu;

    public KoseiHelperModule()
    {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(KoseiHelperModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(KoseiHelperModule), LogLevel.Info);
#endif
    }

    [ModImportName("ExtendedVariantMode")]
    public static class ExtendedVariantImports
    {
        // The string is the variant name, the second argument is the value, the third argument is whether the change is temporary / revertible
        public static Action<string, float, bool> TriggerFloatVariant;
        public static Action<string, bool, bool> TriggerBooleanVariant;
        public static Action<string, int, bool> TriggerIntegerVariant;
        public static Func<string, object> GetCurrentVariantValue;
    }

    public override void Load()
    {
        typeof(ExtendedVariantImports).ModInterop();
        //TopDownViewController.Load();
        On.Celeste.PlayerSprite.CreateFramesMetadata += PlayerSpriteOnCreateFramesMetadata;
        //On.Celeste.LevelLoader.ctor += LevelLoaderOnctor;
        Everest.Events.Level.OnCreatePauseMenuButtons += OnCreatePauseMenuButtons;
        MoveBlockCrystal.Load();
        MapCutscene.Load();
        PufferBall.Load();
        CustomTempleCrackedBlock.Load();
        PufferRefill.Load();
        FlagRefill.Load();
        CounterRefill.Load();
        On.Celeste.Player.ClimbBoundsCheck += KoseiHelperModule.ClimbCheck;
        DebugMapController.Load();
        ClimbJumpController.Load();
        ShadowCloakController.Load();
        SpawnController.Load();
        GameDataController.Load();
        TalkComponentCustomizator.Load();
        SetFlagPrefixOnSpawnController.Load();
        Ladder.Load();
        Everest.Events.Player.OnRegisterStates += Ladder.RegisterLadderState;
        Everest.Events.Player.OnRegisterStates += MaryBlock.RegisterBaldState;

        if (!frostHelperLoaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "FrostHelper", Version = new Version(1, 66, 0) }))
            frostHelperLoaded = true;
        if (!collabUtils2Loaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "CollabUtils2", Version = new Version(1, 10, 0) }))
            collabUtils2Loaded = true;
        if (!helpingHandLoaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "MaxHelpingHand", Version = new Version(1, 33, 6) }))
            helpingHandLoaded = true;
        if (!doonvHelperLoaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "DoonvHelper", Version = new Version(1, 0, 0) }))
            doonvHelperLoaded = true;
        if (!femtoHelperLoaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "FemtoHelper", Version = new Version(1, 15, 12) }))
            femtoHelperLoaded = true;

        Mod.DecalRegistry.AddPropertyHandler<KillDecalRegistryHandler>();
        Mod.DecalRegistry.AddPropertyHandler<MovingDecalRegistryHandler>();
        Mod.DecalRegistry.AddPropertyHandler<TrailDecalRegistryHandler>();
        Mod.DecalRegistry.AddPropertyHandler<MultiFlagSwapDecalRegistryHandler>();
        Mod.DecalRegistry.AddPropertyHandler<CounterSwapDecalRegistryHandler>();
    }

    public override void Unload()
    {
        //TopDownViewController.Unload();
        On.Celeste.PlayerSprite.CreateFramesMetadata -= PlayerSpriteOnCreateFramesMetadata;
        //On.Celeste.LevelLoader.ctor -= LevelLoaderOnctor;
        Everest.Events.Level.OnCreatePauseMenuButtons -= OnCreatePauseMenuButtons;
        MoveBlockCrystal.Unload();
        MapCutscene.Unload();
        PufferBall.Unload();
        CustomTempleCrackedBlock.Unload();
        PufferRefill.Unload();
        FlagRefill.Unload();
        CounterRefill.Unload();
        On.Celeste.Player.ClimbBoundsCheck -= KoseiHelperModule.ClimbCheck;
        DebugMapController.Unload();
        ClimbJumpController.Unload();
        ShadowCloakController.Unload();
        SpawnController.Unload();
        GameDataController.Unload();
        TalkComponentCustomizator.Unload();
        SetFlagPrefixOnSpawnController.Unload();
        Ladder.Unload();
        Everest.Events.Player.OnRegisterStates -= Ladder.RegisterLadderState;
        Everest.Events.Player.OnRegisterStates -= MaryBlock.RegisterBaldState;

        frostHelperLoaded = false;
        helpingHandLoaded = false;
        collabUtils2Loaded = false;
        doonvHelperLoaded = false;
        femtoHelperLoaded = false;
    }

    private static bool ClimbCheck(On.Celeste.Player.orig_ClimbBoundsCheck orig, Player self, int dir)
    {
        if (KoseiHelperModule.Session.oobClimbFix)
            return true;
        else
            return orig(self, dir);
    }

    private void OnCreatePauseMenuButtons(Level level, TextMenu menu, bool minimal)
    {
        if (level == null) return;

        ReturnToRoomController controller = level.Tracker.GetEntity<ReturnToRoomController>();
        Player player = level.Tracker.GetEntity<Player>();
        if (controller != null && player != null &&
            (string.IsNullOrEmpty(controller.preventionFlag) || !level.Session.GetFlag(controller.preventionFlag)))
        {
            int index = menu.Items.FindIndex(item =>
                item is TextMenu.Button button && button.Label == Dialog.Clean("menu_pause_savequit")
            );
            if (index >= 0)
            {
                ReturnToGDMenu = menu;
                menu.Insert(index + controller.menuIndex, new TextMenu.Button(Dialog.Clean(controller.dialogID))
                {
                    OnPressed = delegate {
                        if (!string.IsNullOrEmpty(controller.flagsToUnset))
                        {
                            string[] flags = controller.flagsToUnset.Split(',');
                            foreach (string flag in flags)
                            {
                                if (!string.IsNullOrEmpty(flag))
                                {
                                    level.Session.SetFlag(flag, false);
                                }
                            }
                        }
                        Audio.Play(controller.sound, player.Center);
                        if (controller.loseFollowers)
                        {
                            player.Leader.LoseFollowers();
                        }
                        level.OnEndOfFrame += () => {
                            level.TeleportTo(player, controller.roomName, controller.introType,
                                new Vector2(controller.closestSpawnX, controller.closestSpawnY));
                        };
                        level.Paused = false;
                    }
                });
            }
        }
    }

    private static void PlayerSpriteOnCreateFramesMetadata(On.Celeste.PlayerSprite.orig_CreateFramesMetadata orig, string sprite)
    {   // This hook can be used to add animations specifically to player sprites.
        orig(sprite);
        // Maybe load and store this SpriteBank on game startup instead of creating it every time.
        SpriteBank modBank = new SpriteBank(GFX.Game, "Graphics/KoseiHelperXML/CustomPlayerSprites.xml");
        if (!modBank.SpriteData.ContainsKey(sprite))
            return;

        SpriteData modSpriteData = modBank.SpriteData[sprite];
        SpriteData origSpriteData = GFX.SpriteBank.SpriteData[sprite];

        IDictionary animsOrig = origSpriteData.Sprite.Animations;
        IDictionary animsCustom = modSpriteData.Sprite.Animations;

        Logger.Log(LogLevel.Info, "KoseiHelper", $"Adding missing custom animations to sprite '{sprite}'.");
        foreach (DictionaryEntry kvpNewAnim in animsCustom)
        {
            if (!animsOrig.Contains(kvpNewAnim.Key))
                animsOrig[kvpNewAnim.Key] = kvpNewAnim.Value;
        }
    }

    /*private static void LevelLoaderOnctor(On.Celeste.LevelLoader.orig_ctor orig, LevelLoader self, Session session, Vector2? startPosition)
    {   // This hook can be used to add missing animations to any sprite.
        orig(self, session, startPosition);

        // Maybe load and store this SpriteBank on game startup instead of creating it every time.
        SpriteBank modBank = new SpriteBank(GFX.Game, "Graphics/KoseiHelperXML/CustomSprites.xml");
        SpriteBank origBank = GFX.SpriteBank;

        foreach ((string spriteName, SpriteData modSpriteData) in modBank.SpriteData)
        {
            if (origBank.SpriteData.TryGetValue(spriteName, out SpriteData origSpriteData))
            {
                IDictionary animsOrig = origSpriteData.Sprite.Animations;
                IDictionary animsCustom = modSpriteData.Sprite.Animations;

                Logger.Log(LogLevel.Info, "KoseiHelper", $"Adding missing custom animations to sprite '{spriteName}'.");
                foreach (DictionaryEntry kvpNewAnim in animsCustom)
                {
                    if (!animsOrig.Contains(kvpNewAnim.Key))
                        animsOrig[kvpNewAnim.Key] = kvpNewAnim.Value;
                }
            }
            else
            {
                throw new Exception($"Tried to add a new sprite '{spriteName}' using the merging spritebank.\n" +
                    "Use \"Graphics/Sprites.xml\" instead to make the sprite reskinnable in maps.");
            }
        }
    }*/
}