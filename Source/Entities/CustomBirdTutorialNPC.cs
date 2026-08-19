using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomBirdTutorial")]
[TrackedAs(typeof(CustomBirdTutorial))]
public class CustomBirdTutorialNPC : BirdNPC // i had to call it differently because my ide was confused
{
    public string BirdId;
    private new bool onlyOnce;
    private bool caw;
    private bool triggered;
    private bool flewAway;
    private new CustomBirdTutorialGui gui;
    public string flag;

    private static readonly Dictionary<string, Vector2> Directions = new()
    {
        ["Left"] = new Vector2(-1f, 0f),
        ["Right"] = new Vector2(1f, 0f),
        ["Up"] = new Vector2(0f, -1f),
        ["Down"] = new Vector2(0f, 1f),
        ["UpLeft"] = new Vector2(-1f, -1f),
        ["UpRight"] = new Vector2(1f, -1f),
        ["DownLeft"] = new Vector2(-1f, 1f),
        ["DownRight"] = new Vector2(1f, 1f)
    };

    public CustomBirdTutorialNPC(EntityData data, Vector2 offset) : base(data, offset)
    {
        BirdId = data.Attr("birdId", "");
        onlyOnce = data.Bool("onlyOnce", false);
        caw = data.Bool("caw", false);
        Facing = data.Bool("faceLeft", false) ? Facings.Left : Facings.Right;
        Sprite.Scale.X = (float)Facing;
        string infoText = data.Attr("info", "");
        object info = string.IsNullOrEmpty(infoText) ? "" : GFX.Gui.Has(infoText) ? GFX.Gui[infoText] : Dialog.Clean(infoText);
        string controlString = data.Attr("controls", "");
        object[] controls = ParseControls(controlString);

        Sprite.Visible = !data.Bool("noBird", false);
        flag = data.Attr("flag", "");

        gui = new CustomBirdTutorialGui(this, new Vector2(0f, data.Float("verticalOffset", -16f)), info, controls);
        gui.titleColor = KoseiHelperUtils.ParseHexColor(data.Attr("titleColor", null), Color.White);
        gui.bgColor = KoseiHelperUtils.ParseHexColor(data.Attr("bgColor", null), Calc.HexToColor("061526"));
        gui.lineColor = KoseiHelperUtils.ParseHexColor(data.Attr("lineColor", null), Color.White);
        gui.textColor = KoseiHelperUtils.ParseHexColor(data.Attr("textColor", null), Color.White);
        gui.secondaryTextColor = KoseiHelperUtils.ParseHexColor(data.Attr("secondaryTextColor", null), Calc.HexToColor("6179e2"));
        gui.directionColor = KoseiHelperUtils.ParseHexColor(data.Attr("directionColor", null), Color.White);
        gui.buttonColor = KoseiHelperUtils.ParseHexColor(data.Attr("buttonColor", null), Color.White);
        gui.imageColor = KoseiHelperUtils.ParseHexColor(data.Attr("imageColor", null), Color.White);
        gui.buttonPadding = data.Float("buttonPadding", 8f);
        gui.sizeMultiplier = data.Float("sizeMultiplier", 1f);
        gui.renderTriangleBelow = data.Bool("renderTriangleBelow", true);
        gui.rectangleShape = data.Bool("rectangleShape", true);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (scene.Tracker.GetEntities<CustomBirdTutorialTrigger>().OfType<CustomBirdTutorialTrigger>().All(trigger => !trigger.ShowTutorial || trigger.BirdId != BirdId) && KoseiHelperUtils.CheckFlag(scene as Level, flag))
            TriggerShowTutorial();
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (KoseiHelperUtils.CheckFlag(level, flag))
            TriggerShowTutorial();
        else if (triggered)
            TriggerHideTutorial();
    }

    public void TriggerShowTutorial()
    {
        if (triggered)
            return;
        triggered = true;
        Add(new Coroutine(ShowTutorial(gui, caw)));
    }

    public void TriggerHideTutorial()
    {
        if (flewAway)
            return;
        flewAway = true;
        if (triggered)
            Add(new Coroutine(HideTutorial()));
        triggered = true;
        Add(new Coroutine(StartleAndFlyAway()));
        if (onlyOnce)
            SceneAs<Level>().Session.DoNotLoad.Add(EntityID);
    }

    public static CustomBirdTutorialNPC FindById(Level level, string birdId)
    {
        return level.Tracker.GetEntities<CustomBirdTutorialNPC>().OfType<CustomBirdTutorialNPC>().FirstOrDefault(bird => bird.BirdId == birdId);
    }

    private static object[] ParseControls(string controlString)
    {
        if (string.IsNullOrWhiteSpace(controlString))
            return Array.Empty<object>();

        string[] commands = controlString.Split(',');
        object[] result = new object[commands.Length];
        for (int i = 0; i < commands.Length; i++)
        {
            result[i] = ParseControl(commands[i].Trim());
        }

        return result;
    }

    private static object ParseControl(string command)
    {

        if (GFX.Gui.Has(command))
            return GFX.Gui[command];

        if (typeof(Input).GetField(command, BindingFlags.Static | BindingFlags.Public)?.GetValue(null) is VirtualButton button)
            return button;

        if (command.StartsWith("mod:", StringComparison.Ordinal))
        {
            string[] autoBinding = command.Substring(4).Split('/');
            if (autoBinding.Length >= 2)
            {
                EverestModule module = Everest.Modules.FirstOrDefault(m => m.Metadata.Name == autoBinding[0]);
                if (module?.SettingsType != null)
                {
                    var property = module.SettingsType.GetProperty(autoBinding[1]);
                    if (property?.GetGetMethod() != null)
                    {
                        ButtonBinding binding = property.GetGetMethod().Invoke(module._Settings, null) as ButtonBinding;
                        if (binding?.Button != null)
                            return binding.Button;
                    }
                }
            }
        }

        if (command.StartsWith("dialog:", StringComparison.Ordinal))
            return Dialog.Clean(command.Substring("dialog:".Length));

        if (Directions.TryGetValue(command, out Vector2 direction))
            return direction;

        return command;
    }
}