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

    public TalkComponentCustomizator(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        highlightTexturePath = data.Attr("highlightTexture", "hover/highlight");
        idleTexturePath = data.Attr("idleTexture", "hover/idle");
        sfxIn = data.Attr("sfxIn", "event:/ui/game/hotspot_main_in");
        sfxOut = data.Attr("sfxOut", "event:/ui/game/hotspot_main_out");
        floatiness = data.Float("floatiness", 1f);
        textStroke = data.Float("textStroke", 2f);
        tint = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("tint", out object tintColor) ? tintColor.ToString() : null, Calc.HexToColor("FFFFFF"));
        talkTextColor = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("talkTextColor", out object talkTextC) ? talkTextC.ToString() : null, Calc.HexToColor("FFFFFF"));
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
        {
            CustomizeTalkComponent(closestTalkComponent);
        }
    }

    private void CustomizeTalkComponent(TalkComponent talkComponent)
    {
        TalkComponentCustomization customization = new()
        {
            HighlightTexture = GFX.Gui[highlightTexturePath],
            IdleTexture = GFX.Gui[idleTexturePath],
            SfxIn = sfxIn,
            SfxOut = sfxOut,
            Floatiness = floatiness,
            Tint = tint,
            TalkTextColor = talkTextColor,
            TextStroke = textStroke
        };

        talkComponent.HoverUI.Texture = customization.HighlightTexture;
        talkComponent.HoverUI.SfxIn = customization.SfxIn;
        talkComponent.HoverUI.SfxOut = customization.SfxOut;

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
        MTexture textureToDraw = self.Highlighted ? custom.HighlightTexture : custom.IdleTexture;
        textureToDraw.DrawJustified(vector2, new Vector2(0.5f, 1f), drawColor, scale);
        if (self.Highlighted)
        {
            Vector2 inputPos = vector2 + Handler.HoverUI.InputPosition * scale;

            if (Input.GuiInputController(Input.PrefixMode.Latest))
            {
                Input.GuiButton(Input.Talk, "controls/keyboard/oemquestion").DrawJustified(inputPos, new Vector2(0.5f), custom.TalkTextColor * visibility, scale);
            }
            else
            {
                ActiveFont.DrawOutline(Input.FirstKey(Input.Talk).ToString().ToUpper(), inputPos,
                    new Vector2(0.5f), new Vector2(scale), custom.TalkTextColor * visibility, custom.TextStroke, Color.Black);
            }
        }
    }
}