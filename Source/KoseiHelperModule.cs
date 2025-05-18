using MonoMod.ModInterop;
using System;
using Celeste.Mod.KoseiHelper.Entities;
using Celeste.Mod.KoseiHelper.Triggers;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModule : EverestModule {
    public static KoseiHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(KoseiHelperModuleSettings);
    public static KoseiHelperModuleSettings Settings => (KoseiHelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(KoseiHelperModuleSession);
    public static KoseiHelperModuleSession Session => (KoseiHelperModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(KoseiHelperModuleSaveData);
    public static KoseiHelperModuleSaveData SaveData => (KoseiHelperModuleSaveData) Instance._SaveData;

    public bool frostHelperLoaded = false;
    public bool collabUtils2Loaded = false;
    public bool helpingHandLoaded = false;

    public KoseiHelperModule() {
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
        public static Action<string, float, bool> TriggerFloatVariant;
        public static Action<string, float, bool> TriggerBooleanVariant;
        public static Action<string, float, bool> TriggerIntegerVariant;
        public static Func<string, object> GetCurrentVariantValue;
    }

    public override void Load() {
        typeof(ExtendedVariantImports).ModInterop();
        //TopDownViewController.Load();
        PufferBall.Load();
        CustomTempleCrackedBlock.Load();
        PufferRefill.Load();
        FlagRefill.Load();
        CounterRefill.Load();
        On.Celeste.Player.ClimbBoundsCheck += KoseiHelperModule.ClimbCheck;
        DebugMapController.Load();
        ClimbJumpController.Load();
        SpawnController.Load();
        GameDataController.Load();
        Everest.Events.Player.OnRegisterStates += MaryBlock.RegisterBaldState;

        if (!frostHelperLoaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "FrostHelper", Version = new Version(1, 66, 0) }))
            frostHelperLoaded = true;
        if (!collabUtils2Loaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "CollabUtils2", Version = new Version(1, 10, 0) }))
            collabUtils2Loaded = true;
        if (!helpingHandLoaded && Everest.Loader.DependencyLoaded(new EverestModuleMetadata { Name = "MaxHelpingHand", Version = new Version(1, 33, 6) }))
            helpingHandLoaded = true;
    }

    public override void Unload() {
        //TopDownViewController.Unload();
        PufferBall.Unload();
        CustomTempleCrackedBlock.Unload();
        PufferRefill.Unload();
        FlagRefill.Unload();
        CounterRefill.Unload();
        On.Celeste.Player.ClimbBoundsCheck -= KoseiHelperModule.ClimbCheck;
        DebugMapController.Unload();
        ClimbJumpController.Unload();
        SpawnController.Unload();
        GameDataController.Unload();
        Everest.Events.Player.OnRegisterStates -= MaryBlock.RegisterBaldState;

            frostHelperLoaded = false;
            helpingHandLoaded = false;
            collabUtils2Loaded = false;
    }

    private static bool ClimbCheck(On.Celeste.Player.orig_ClimbBoundsCheck orig, Player self, int dir)
    {
        if (KoseiHelperModule.Session.oobClimbFix)
            return true;
        else
        {
            if (self.Left + (float)(dir * 2) >= (float)self.level.Bounds.Left)
                return self.Right + (float)(dir * 2) < (float)self.level.Bounds.Right;
        }
        return false;
    }
}