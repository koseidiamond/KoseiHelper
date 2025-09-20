using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;


public class TalkComponentCustomization : Entity
{
    public MTexture HighlightTexture;
    public MTexture IdleTexture;
    public string SfxIn;
    public string SfxOut;
    public float Floatiness = 1f;
    public Color Tint = Color.White;
    public Color TalkTextColor = Color.White;
    public float TextStroke = 2f;
    public int AnimationFrames = 0;
    public string Text = "Talk";
    public float TextScaleX = 1f, TextScaleY = 1f, IconScaleX = 1f, IconScaleY = 1f;

    //Animation variables
    public float AnimationTimer = 0f;
    public float AnimationSpeed = 0.1f;
    public int CurrentFrame = 0;
    public string HighlightBaseName, HighlightFrameFormat;
    public string IdleBaseName, IdleFrameFormat;

    public List<object> ParsedTextChunks = new();
}

[CustomEntity("KoseiHelper/TalkComponentCustomizator")]
[Tracked]
public class TalkComponentCustomizator : Entity
{
    public static Dictionary<TalkComponent, TalkComponentCustomization> Customizations = new();

    private readonly string highlightTexturePath, idleTexturePath;
    private readonly string sfxIn, sfxOut;
    private readonly float floatiness;
    private Color tint, talkTextColor;
    private readonly float textStroke;
    private readonly int animationFrames;
    private readonly float animationSpeed;
    private readonly string text;
    private readonly float textScaleX, textScaleY, iconScaleX, iconScaleY;
    private readonly bool allEntities;

    public TalkComponentCustomizator(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        highlightTexturePath = data.Attr("highlightTexture", "hover/highlight");
        idleTexturePath = data.Attr("idleTexture", "hover/idle");
        sfxIn = data.Attr("sfxIn", "event:/ui/game/hotspot_main_in");
        sfxOut = data.Attr("sfxOut", "event:/ui/game/hotspot_main_out");
        floatiness = data.Float("floatiness", 1f);
        textStroke = data.Float("textStroke", 2f);
        animationFrames = data.Int("animationFrames", 0);
        animationSpeed = data.Float("animationSpeed", 0.1f);
        text = data.Attr("text", "Talk");
        textScaleX = data.Float("textScaleX", 1f);
        textScaleY = data.Float("textScaleY", 1f);
        iconScaleX = data.Float("iconScaleX", 1f);
        iconScaleY = data.Float("iconScaleY", 1f);
        tint = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("tint", out object tintColor) ? tintColor.ToString() : null, Calc.HexToColor("FFFFFF"));
        talkTextColor = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("talkTextColor", out object talkTextC) ? talkTextC.ToString() : null, Calc.HexToColor("FFFFFF"));
        allEntities = data.Bool("allEntities", false);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        Tracker.AddTypeToTracker(typeof(TalkComponent));
        Tracker.Refresh(scene);
        List<Component> components = level.Tracker.GetComponents<TalkComponent>();
        if (components.Count == 0) return;

        TalkComponent closestTalkComponent = null;
        float closestDistance = float.MaxValue;
        if (allEntities)
        {
            foreach (Component component in components)
            {
                if (component.Entity != null)
                {
                    TalkComponent talkComponent = (TalkComponent)component;
                    CustomizeTalkComponent(talkComponent);
                }
            }
        }
        else
        {
            foreach (Component component in components)
            {
                if (component.Entity == null)
                    continue;

                float distance = Vector2.DistanceSquared(Position, component.Entity.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTalkComponent = (TalkComponent)component;
                }
            }
            if (closestTalkComponent != null)
                CustomizeTalkComponent(closestTalkComponent);
        }
    }

    private static readonly Dictionary<string, Vector2> DirectionMap = new()
    {
        { "Left", new Vector2(-1, 0) },
        { "Right", new Vector2(1, 0) },
        { "Up", new Vector2(0, -1) },
        { "Down", new Vector2(0, 1) },
        { "UpLeft", new Vector2(-1, -1) },
        { "UpRight", new Vector2(1, -1) },
        { "DownLeft", new Vector2(-1, 1) },
        { "DownRight", new Vector2(1, 1) },
    };

    private void CustomizeTalkComponent(TalkComponent talkComponent)
    {
        TalkComponentCustomization customization = new()
        {
            SfxIn = sfxIn,
            SfxOut = sfxOut,
            Floatiness = floatiness,
            Tint = tint,
            TalkTextColor = talkTextColor,
            TextStroke = textStroke,
            AnimationFrames = animationFrames,
            AnimationSpeed = animationSpeed,
            Text = text,
            TextScaleX = textScaleX,
            TextScaleY = textScaleY,
            IconScaleX = iconScaleX,
            IconScaleY = iconScaleY
        };

        ExtractBaseNameAndFormat(highlightTexturePath, out customization.HighlightBaseName, out customization.HighlightFrameFormat);
        ExtractBaseNameAndFormat(idleTexturePath, out customization.IdleBaseName, out customization.IdleFrameFormat);
        //if no anim we'll just use these textures:
        customization.HighlightTexture = GFX.Gui[highlightTexturePath];
        customization.IdleTexture = GFX.Gui[idleTexturePath];

        talkComponent.HoverUI.Texture = customization.HighlightTexture;
        talkComponent.HoverUI.SfxIn = customization.SfxIn;
        talkComponent.HoverUI.SfxOut = customization.SfxOut;

        // parse text chunks (like BirdTutorialGui)
        foreach (string textChunk in text.Split(','))
        {
            string trimmed = textChunk.Trim();

            if (Enum.TryParse(trimmed, out BirdTutorialGui.ButtonPrompt prompt))
                customization.ParsedTextChunks.Add(Input.GuiButton(BirdTutorialGui.orig_ButtonPromptToVirtualButton(prompt), "controls/keyboard/oemquestion"));
            else if (DirectionMap.TryGetValue(trimmed, out Vector2 dir))
                customization.ParsedTextChunks.Add(Input.GuiDirection(dir));
            else if (GFX.Gui.Has(trimmed))
                customization.ParsedTextChunks.Add(GFX.Gui[trimmed]);
            else if (trimmed.StartsWith("dialog:"))
            {
                string dialogKey = trimmed.Substring("dialog:".Length);
                customization.ParsedTextChunks.Add(Dialog.Get(dialogKey));
            }
            else
                customization.ParsedTextChunks.Add(trimmed);
        }
        Customizations[talkComponent] = customization;
    }

    public static void Load()
    {
        On.Celeste.TalkComponent.TalkComponentUI.Render += TalkComponentUI_Render;
    }

    public static void Unload()
    {
        On.Celeste.TalkComponent.TalkComponentUI.Render -= TalkComponentUI_Render;
        Customizations.Clear();
    }

    private static void TalkComponentUI_Render(On.Celeste.TalkComponent.TalkComponentUI.orig_Render orig, TalkComponent.TalkComponentUI self)
    { // TalkComponent Render copypaste because it's hardcoded
        TalkComponent Handler = self.Handler;

        if (!Customizations.TryGetValue(Handler, out TalkComponentCustomization custom))
        {
            orig(self);
            return;
        }

        Level level = self.Scene as Level;
        if (level.FrozenOrPaused || !(self.slide > 0f) || Handler.Entity == null)
            return;
        Vector2 vector = level.Camera.Position.Floor();
        Vector2 vector2 = Handler.Entity.Position + Handler.DrawAt - vector;
        if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
            vector2.X = 320f - vector2.X;
        vector2.X *= 6f;
        vector2.Y *= 6f;

        float timer = (float)typeof(TalkComponent.TalkComponentUI).GetField("timer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(self);
        float alpha = (float)typeof(TalkComponent.TalkComponentUI).GetField("alpha", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(self);

        float wigglerValue = self.Get<Wiggler>()?.Value ?? 0f;

        float scale = self.Highlighted ? (1f - wigglerValue * 0.5f) : (1f + wigglerValue * 0.5f);
        float slideEase = Ease.CubeOut(self.slide);
        float visibility = Ease.CubeInOut(self.slide) * alpha;
        vector2.Y += (float)Math.Sin(timer * 4f) * 12f * custom.Floatiness + 64f * (1f - slideEase);
        Color drawColor = custom.Tint * visibility;

        custom.AnimationTimer += Engine.DeltaTime;
        if (custom.AnimationFrames > 1)
        {
            if (custom.AnimationTimer >= custom.AnimationSpeed)
            {
                custom.AnimationTimer -= custom.AnimationSpeed;
                custom.CurrentFrame = (custom.CurrentFrame + 1) % custom.AnimationFrames;
            }
        }

        MTexture textureToDraw;
        if (custom.AnimationFrames > 1)
        {
            int frame = custom.CurrentFrame;

            if (self.Highlighted && custom.HighlightFrameFormat != null)
            {
                string tex = $"{custom.HighlightBaseName}{frame.ToString(custom.HighlightFrameFormat)}";
                textureToDraw = GFX.Gui.Has(tex) ? GFX.Gui[tex] : custom.HighlightTexture;
            }
            else if (!self.Highlighted && custom.IdleFrameFormat != null)
            {
                string tex = $"{custom.IdleBaseName}{frame.ToString(custom.IdleFrameFormat)}";
                textureToDraw = GFX.Gui.Has(tex) ? GFX.Gui[tex] : custom.IdleTexture;
            }
            else
                textureToDraw = self.Highlighted ? custom.HighlightTexture : custom.IdleTexture;
        }
        else
            textureToDraw = self.Highlighted ? custom.HighlightTexture : custom.IdleTexture;

        textureToDraw.DrawJustified(vector2, new Vector2(0.5f, 1f), drawColor, new Vector2(scale * custom.IconScaleX, scale * custom.IconScaleY));

        if (self.Highlighted)
        {
            Vector2 inputPos = vector2 + Handler.HoverUI.InputPosition * scale;

            if (custom.Text == "Default")
            {
                // Use original TalkComponent behavior
                if (Input.GuiInputController(Input.PrefixMode.Latest))
                {
                    Input.GuiButton(Input.Talk, "controls/keyboard/oemquestion").DrawJustified(
                        inputPos, new Vector2(0.5f), custom.TalkTextColor * visibility,
                        new Vector2(scale * custom.TextScaleX, scale * custom.TextScaleY));
                }
                else
                {
                    ActiveFont.DrawOutline(Input.FirstKey(Input.Talk).ToString().ToUpper(), inputPos,
                        new Vector2(0.5f), new Vector2(scale * custom.TextScaleX, scale * custom.TextScaleY),
                        custom.TalkTextColor * visibility, custom.TextStroke, Color.Black);
                }
            }
            else
            {
                Vector2 basePos = inputPos;
                float spacing = 4f;
                float totalWidth = 0f;

                // first we calculate the total width
                foreach (object chunk in custom.ParsedTextChunks)
                {
                    if (chunk is MTexture tex)
                    {
                        totalWidth += tex.Width * scale * custom.TextScaleX + spacing;
                    }
                    else if (chunk is string textChunk)
                    {
                        Vector2 size = ActiveFont.Measure(textChunk) * scale * new Vector2(custom.TextScaleX, custom.TextScaleY);
                        totalWidth += size.X + spacing;
                    }
                }
                if (custom.ParsedTextChunks.Count > 0)
                    totalWidth -= spacing;

                // then we draw centered
                Vector2 drawPos = basePos - new Vector2(totalWidth / 2f, 0f);
                foreach (object chunk in custom.ParsedTextChunks)
                {
                    if (chunk is MTexture tex)
                    {
                        tex.DrawJustified(drawPos, new Vector2(0f, 0.5f), custom.TalkTextColor * visibility,
                            new Vector2(scale * custom.TextScaleX, scale * custom.TextScaleY));
                        drawPos.X += tex.Width * scale * custom.TextScaleX + spacing;
                    }
                    else if (chunk is string textChunk)
                    {
                        Vector2 size = ActiveFont.Measure(textChunk) * scale * new Vector2(custom.TextScaleX, custom.TextScaleY);
                        ActiveFont.DrawOutline(textChunk, drawPos + new Vector2(1f, 2f), new Vector2(0f, 0.5f),
                            new Vector2(scale * custom.TextScaleX, scale * custom.TextScaleY), custom.TalkTextColor * visibility, custom.TextStroke, Color.Black);
                        drawPos.X += size.X + spacing;
                    }
                }
            }
        }
    }

    // For animations
    private static void ExtractBaseNameAndFormat(string path, out string baseName, out string format)
    {
        path = path.Replace('\\', '/');
        string fileName = System.IO.Path.GetFileName(path);
        int i = fileName.Length - 1;

        while (i >= 0 && char.IsDigit(fileName[i])) i--;

        int digitStart = i + 1;
        string digits = (digitStart < fileName.Length) ? fileName.Substring(digitStart) : "";
        string baseFile = fileName.Substring(0, digitStart);

        string directory = System.IO.Path.GetDirectoryName(path);
        baseName = (!string.IsNullOrEmpty(directory) ? directory.Replace('\\', '/') + "/" : "") + baseFile;
        format = (digits.Length > 0) ? $"D{digits.Length}" : null;
    }
}