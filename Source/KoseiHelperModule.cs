using MonoMod.ModInterop;
using System;
using Celeste.Mod.KoseiHelper.Entities;

namespace Celeste.Mod.KoseiHelper;

public class KoseiHelperModule : EverestModule {
    public static KoseiHelperModule Instance { get; private set; }

    public override Type SettingsType => typeof(KoseiHelperModuleSettings);
    public static KoseiHelperModuleSettings Settings => (KoseiHelperModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(KoseiHelperModuleSession);
    public static KoseiHelperModuleSession Session => (KoseiHelperModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(KoseiHelperModuleSaveData);
    public static KoseiHelperModuleSaveData SaveData => (KoseiHelperModuleSaveData) Instance._SaveData;

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
    // I can do this to debug stuff:
    //private static void ExampleMethod()
    //{
    //    Logger.Debug(nameof(KoseiHelperModule), "hello, world!");
    //    Logger.Debug($"{nameof(KoseiHelperModule)}/{nameof(ExampleMethod)}", "hello, world, yet again!");
    //}

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
        TopDownViewController.Load();
        PufferBall.Load();
        CustomTempleCrackedBlock.Load();
        PufferRefill.Load();
    }

    public override void Unload() {
        TopDownViewController.Unload();
        PufferBall.Unload();
        CustomTempleCrackedBlock.Unload();
        PufferRefill.Unload();
    }
}