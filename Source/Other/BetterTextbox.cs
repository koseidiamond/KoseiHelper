using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using Color = Microsoft.Xna.Framework.Color;

namespace Celeste.Mod.KoseiHelper;

public class BetterTextbox : Entity // This is entirely copied from Crossover Collab! Code by @lunasssy (Luna)
{
    public static string FormatText(string inputText)
    {
        Regex portraitRegex = new Regex(@"\[(.*?) (.*?)\]");
        Regex dialogRegex = new Regex(@"\[(.*?)\](.*?)\s*");
        string formattedText = portraitRegex.Replace(inputText, "{portrait $1 $2}");
        formattedText = dialogRegex.Replace(formattedText, "$2{break}");

        return formattedText;
    }


    private MTexture textbox = GFX.Portraits["textbox/default"];

    private MTexture textboxOverlay;

    private const int textboxInnerWidth = 1688;

    private const int textboxInnerHeight = 272;

    private const float portraitPadding = 16f;

    private const float tweenDuration = 0.4f;

    private const float switchToIdleAnimationDelay = 0.5f;

    private readonly float innerTextPadding;

    private readonly float maxLineWidthNoPortrait;

    private readonly float maxLineWidth;

    private readonly int linesPerPage;

    private const int stopVoiceCharactersEarly = 4;

    private float ease;

    private FancyText.Text text;

    private Func<IEnumerator>[] events;

    private Coroutine runRoutine;

    private Coroutine skipRoutine;

    private float lineHeight;

    private FancyText.Anchors anchor;

    private FancyText.Portrait portrait;

    private int index;

    private bool waitingForInput;

    private bool disableInput;

    private int shakeSeed;

    private float timer;

    private float gradientFade;

    private bool isInTrigger;

    private bool canSkip = true;

    private bool easingClose;

    private bool easingOpen;

    public Vector2 RenderOffset;

    private bool autoPressContinue;

    private char lastChar;

    private Sprite portraitSprite = new Sprite(null, null);

    private bool portraitExists;

    private bool portraitIdling;

    private float portraitScale = 1.5f;

    private Wiggler portraitWiggle;

    private Sprite portraitGlitchy;

    private bool isPortraitGlitchy;

    private Dictionary<string, SoundSource> talkers = new Dictionary<string, SoundSource>();

    private SoundSource activeTalker;

    private SoundSource phonestatic;

    public bool Opened { get; private set; }

    public int Page { get; private set; }

    public List<FancyText.Node> Nodes => text.Nodes;

    public bool UseRawDeltaTime
    {
        set
        {
            runRoutine.UseRawDeltaTime = value;
        }
    }

    public int Start { get; private set; }

    public string PortraitName
    {
        get
        {
            if (portrait == null || portrait.Sprite == null)
            {
                return "";
            }

            return portrait.Sprite;
        }
    }

    public string PortraitAnimation
    {
        get
        {
            if (portrait == null || portrait.Sprite == null)
            {
                return "";
            }

            return portrait.Animation;
        }
    }

    public BetterTextbox(string dialog, params Func<IEnumerator>[] events)
        : this(dialog, null, events)
    {
    }

    public BetterTextbox(string dialog, Language language, params Func<IEnumerator>[] events)
    {
        base.Tag = (int)Tags.PauseUpdate | (int)Tags.HUD;
        Opened = true;
        //font = Dialog.Language.Font;
        lineHeight = Dialog.Language.FontSize.LineHeight - 1;
        portraitSprite.UseRawDeltaTime = true;
        Add(portraitWiggle = Wiggler.Create(0.4f, 4f));
        this.events = events;
        linesPerPage = (int)(240f / lineHeight);
        innerTextPadding = (272f - lineHeight * (float)linesPerPage) / 2f;
        maxLineWidthNoPortrait = 1688f - innerTextPadding * 2f;
        maxLineWidth = maxLineWidthNoPortrait - 240f - 32f;
        text = FancyText.Parse(FormatText(dialog), (int)maxLineWidth, linesPerPage, 0f, null, language);
        index = 0;
        Start = 0;
        skipRoutine = new Coroutine(SkipDialog());
        runRoutine = new Coroutine(RunRoutine());
        runRoutine.UseRawDeltaTime = true;
        if (Level.DialogSnapshot == null)
        {
            Level.DialogSnapshot = Audio.CreateSnapshot("snapshot:/dialogue_in_progress", start: false);
        }

        Audio.ResumeSnapshot(Level.DialogSnapshot);
        Add(phonestatic = new SoundSource());
    }

    public void SetStart(int value)
    {
        int num2 = (index = (Start = value));
    }

    private IEnumerator RunRoutine()
    {
        FancyText.Node last = null;
        float delayBuildup = 0f;
        while (index < Nodes.Count)
        {
            FancyText.Node current = Nodes[index];
            float delay = 0f;
            if (current is FancyText.Anchor)
            {
                if (RenderOffset == Vector2.Zero)
                {
                    FancyText.Anchors next2 = (current as FancyText.Anchor).Position;
                    if (ease >= 1f && next2 != anchor)
                    {
                        yield return EaseClose(final: false);
                    }

                    anchor = next2;
                }
            }
            else if (current is FancyText.Portrait)
            {
                FancyText.Portrait next = current as FancyText.Portrait;
                phonestatic.Stop();
                if (ease >= 1f && (this.portrait == null || next.Sprite != this.portrait.Sprite || next.Side != this.portrait.Side))
                {
                    yield return EaseClose(final: false);
                }

                textbox = GFX.Portraits["textbox/default"];
                textboxOverlay = null;
                portraitExists = false;
                activeTalker = null;
                isPortraitGlitchy = false;
                XmlElement xmlElement = null;
                if (!string.IsNullOrEmpty(next.Sprite))
                {
                    if (GFX.PortraitsSpriteBank.Has(next.SpriteId))
                    {
                        xmlElement = GFX.PortraitsSpriteBank.SpriteData[next.SpriteId].Sources[0].XML;
                    }

                    portraitExists = xmlElement != null;
                    isPortraitGlitchy = next.Glitchy;
                    if (isPortraitGlitchy && portraitGlitchy == null)
                    {
                        portraitGlitchy = new Sprite(GFX.Portraits, "noise/");
                        portraitGlitchy.AddLoop("noise", "", 0.1f);
                        portraitGlitchy.Play("noise");
                    }
                }

                if (portraitExists)
                {
                    if (this.portrait == null || next.Sprite != this.portrait.Sprite)
                    {
                        GFX.PortraitsSpriteBank.CreateOn(portraitSprite, next.SpriteId);
                        portraitScale = 240f / (float)xmlElement.AttrInt("size", 160);
                        if (!talkers.ContainsKey(next.SfxEvent))
                        {
                            SoundSource soundSource = new SoundSource().Play(next.SfxEvent);
                            talkers.Add(next.SfxEvent, soundSource);
                            Add(soundSource);
                        }
                    }

                    if (talkers.ContainsKey(next.SfxEvent))
                    {
                        activeTalker = talkers[next.SfxEvent];
                    }

                    string text = "textbox/" + xmlElement.Attr("textbox", "default");
                    textbox = GFX.Portraits[text];
                    if (GFX.Portraits.Has(text + "_overlay"))
                    {
                        textboxOverlay = GFX.Portraits[text + "_overlay"];
                    }

                    string text2 = xmlElement.Attr("phonestatic", "");
                    if (!string.IsNullOrEmpty(text2))
                    {
                        if (text2 == "ex")
                        {
                            phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_ex");
                        }
                        else if (text2 == "mom")
                        {
                            phonestatic.Play("event:/char/dialogue/sfx_support/phone_static_mom");
                        }
                    }

                    canSkip = false;
                    FancyText.Portrait portrait = this.portrait;
                    this.portrait = next;
                    if (next.Pop)
                    {
                        portraitWiggle.Start();
                    }

                    if (portrait == null || portrait.Sprite != next.Sprite || portrait.Animation != next.Animation)
                    {
                        if (portraitSprite.Has(next.BeginAnimation))
                        {
                            portraitSprite.Play(next.BeginAnimation, restart: true);
                            yield return EaseOpen();
                            while (portraitSprite.CurrentAnimationID == next.BeginAnimation && portraitSprite.Animating)
                            {
                                yield return null;
                            }
                        }

                        if (portraitSprite.Has(next.IdleAnimation))
                        {
                            portraitIdling = true;
                            portraitSprite.Play(next.IdleAnimation, restart: true);
                        }
                    }

                    yield return EaseOpen();
                    canSkip = true;
                }
                else
                {
                    this.portrait = null;
                    yield return EaseOpen();
                }
            }
            else if (current is FancyText.NewPage)
            {
                PlayIdleAnimation();
                if (ease >= 1f)
                {
                    waitingForInput = true;
                    yield return 0.1f;
                    while (!ContinuePressed())
                    {
                        yield return null;
                    }

                    waitingForInput = false;
                }

                Start = index + 1;
                Page++;
            }
            else if (current is FancyText.Wait)
            {
                PlayIdleAnimation();
                delay = (current as FancyText.Wait).Duration;
            }
            else if (current is FancyText.Trigger)
            {
                isInTrigger = true;
                PlayIdleAnimation();
                FancyText.Trigger trigger = current as FancyText.Trigger;
                if (!trigger.Silent)
                {
                    yield return EaseClose(final: false);
                }

                int num = trigger.Index;
                if (events != null && num >= 0 && num < events.Length)
                {
                    yield return events[num]();
                }

                isInTrigger = false;
            }
            else if (current is FancyText.Char)
            {
                FancyText.Char ch = current as FancyText.Char;
                lastChar = (char)ch.Character;
                if (ease < 1f)
                {
                    yield return EaseOpen();
                }

                bool flag = false;
                if (index - 5 > Start)
                {
                    for (int i = index; i < Math.Min(index + 4, Nodes.Count); i++)
                    {
                        if (Nodes[i] is FancyText.NewPage)
                        {
                            flag = true;
                            PlayIdleAnimation();
                        }
                    }
                }

                if (!flag && !ch.IsPunctuation)
                {
                    PlayTalkAnimation();
                }

                if (last != null && last is FancyText.NewPage)
                {
                    index--;
                    yield return 0.2f;
                    index++;
                }

                delay = ch.Delay + delayBuildup;
            }

            last = current;
            index++;
            if (delay < 0.016f)
            {
                delayBuildup += delay;
                continue;
            }

            delayBuildup = 0f;
            if (delay > 0.5f)
            {
                PlayIdleAnimation();
            }

            yield return delay;
        }

        PlayIdleAnimation();
        if (ease > 0f)
        {
            waitingForInput = true;
            while (!ContinuePressed())
            {
                yield return null;
            }

            waitingForInput = false;
            Start = Nodes.Count;
            yield return EaseClose(final: true);
        }

        Close();
    }

    private void PlayIdleAnimation()
    {
        StopTalker();
        if (!portraitIdling && portraitSprite != null && portrait != null && portraitSprite.Has(portrait.IdleAnimation))
        {
            portraitSprite.Play(portrait.IdleAnimation);
            portraitIdling = true;
        }
    }

    private void StopTalker()
    {
        if (activeTalker != null)
        {
            activeTalker.Param("dialogue_portrait", 0f);
            activeTalker.Param("dialogue_end", 1f);
        }
    }

    private void PlayTalkAnimation()
    {
        StartTalker();
        if (portraitIdling && portraitSprite != null && portrait != null && portraitSprite.Has(portrait.TalkAnimation))
        {
            portraitSprite.Play(portrait.TalkAnimation);
            portraitIdling = false;
        }
    }

    private void StartTalker()
    {
        if (activeTalker != null)
        {
            activeTalker.Param("dialogue_portrait", (portrait == null) ? 1 : portrait.SfxExpression);
            activeTalker.Param("dialogue_end", 0f);
        }
    }

    private IEnumerator EaseOpen()
    {
        if (ease < 1f)
        {
            easingOpen = true;
            if (portrait != null && portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                Audio.Play("event:/ui/game/textbox_madeline_in");
            }
            else
            {
                Audio.Play("event:/ui/game/textbox_other_in");
            }

            while ((ease += (runRoutine.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime) / 0.4f) < 1f)
            {
                gradientFade = Math.Max(gradientFade, ease);
                yield return null;
            }

            ease = (gradientFade = 1f);
            easingOpen = false;
        }
    }

    private IEnumerator EaseClose(bool final)
    {
        easingClose = true;
        if (portrait != null && portrait.Sprite.IndexOf("madeline", StringComparison.InvariantCultureIgnoreCase) >= 0)
        {
            Audio.Play("event:/ui/game/textbox_madeline_out");
        }
        else
        {
            Audio.Play("event:/ui/game/textbox_other_out");
        }

        while ((ease -= (runRoutine.UseRawDeltaTime ? Engine.RawDeltaTime : Engine.DeltaTime) / 0.4f) > 0f)
        {
            if (final)
            {
                gradientFade = ease;
            }

            yield return null;
        }

        ease = 0f;
        easingClose = false;
    }

    private IEnumerator SkipDialog()
    {
        while (true)
        {
            if (!waitingForInput && canSkip && !easingOpen && !easingClose && ContinuePressed())
            {
                StopTalker();
                disableInput = true;
                while (!waitingForInput && canSkip && !easingOpen && !easingClose && !isInTrigger && !runRoutine.Finished)
                {
                    runRoutine.Update();
                }
            }

            yield return null;
            disableInput = false;
        }
    }

    public bool SkipToPage(int page)
    {
        autoPressContinue = true;
        while (Page != page && !runRoutine.Finished)
        {
            Update();
        }

        autoPressContinue = false;
        Update();
        while (Opened && ease < 1f)
        {
            Update();
        }

        if (Page == page)
        {
            return Opened;
        }

        return false;
    }

    public void Close()
    {
        Opened = false;
        if (base.Scene != null)
        {
            base.Scene.Remove(this);
        }
    }

    private bool ContinuePressed()
    {
        if (!autoPressContinue)
        {
            if (Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed)
            {
                return !disableInput;
            }

            return false;
        }

        return true;
    }

    public override void Update()
    {
        Level level = base.Scene as Level;
        if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null))
        {
            return;
        }

        if (!autoPressContinue)
        {
            skipRoutine.Update();
        }

        runRoutine.Update();
        if (base.Scene != null && base.Scene.OnInterval(0.05f))
        {
            shakeSeed = Calc.Random.Next();
        }

        if (portraitSprite != null && ease >= 1f)
        {
            portraitSprite.Update();
        }

        if (portraitGlitchy != null && ease >= 1f)
        {
            portraitGlitchy.Update();
        }

        timer += Engine.DeltaTime;
        portraitWiggle.Update();
        int num = Math.Min(index, Nodes.Count);
        for (int i = Start; i < num; i++)
        {
            if (Nodes[i] is FancyText.Char)
            {
                FancyText.Char @char = Nodes[i] as FancyText.Char;
                if (@char.Fade < 1f)
                {
                    @char.Fade = Calc.Clamp(@char.Fade + 8f * Engine.DeltaTime, 0f, 1f);
                }
            }
        }
    }

    public override void Render()
    {
        Level level = base.Scene as Level;
        if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
        {
            return;
        }

        float num = Ease.CubeInOut(ease);
        if (num < 0.05f)
        {
            return;
        }

        float num2 = 116f;
        Vector2 vector = new Vector2(num2, num2 / 2f) + RenderOffset;
        if (RenderOffset == Vector2.Zero)
        {
            if (anchor == FancyText.Anchors.Bottom)
            {
                vector = new Vector2(num2, 1080f - num2 / 2f - 272f);
            }
            else if (anchor == FancyText.Anchors.Middle)
            {
                vector = new Vector2(num2, 404f);
            }

            vector.Y += (int)(136f * (1f - num));
        }

        textbox.DrawCentered(vector + new Vector2(1688f, 272f * num) / 2f, Color.White, new Vector2(1f, num));
        if (waitingForInput)
        {
            float num3 = ((portrait == null || PortraitSide(portrait) < 0) ? 1688f : 1432f);
            Vector2 position = new Vector2(vector.X + num3, vector.Y + 272f) + new Vector2(-48f, -40 + ((timer % 1f < 0.25f) ? 6 : 0));
            GFX.Gui["textboxbutton"].DrawCentered(position);
        }

        if (portraitExists)
        {
            if (PortraitSide(portrait) > 0)
            {
                portraitSprite.Position = new Vector2(vector.X + 1688f - 240f - 16f, vector.Y);
                portraitSprite.Scale.X = 0f - portraitScale;
            }
            else
            {
                portraitSprite.Position = new Vector2(vector.X + 16f, vector.Y);
                portraitSprite.Scale.X = portraitScale;
            }

            portraitSprite.Scale.X *= ((!portrait.Flipped) ? 1 : (-1));
            portraitSprite.Scale.Y = portraitScale * ((272f * num - 32f) / 240f) * (float)((!portrait.UpsideDown) ? 1 : (-1));
            portraitSprite.Scale *= 0.9f + portraitWiggle.Value * 0.1f;
            portraitSprite.Position += new Vector2(120f, 272f * num * 0.5f);
            portraitSprite.Color = Microsoft.Xna.Framework.Color.White * num;
            if (Math.Abs(portraitSprite.Scale.Y) > 0.05f)
            {
                portraitSprite.Render();
                if (isPortraitGlitchy && portraitGlitchy != null)
                {
                    portraitGlitchy.Position = portraitSprite.Position;
                    portraitGlitchy.Origin = portraitSprite.Origin;
                    portraitGlitchy.Scale = portraitSprite.Scale;
                    portraitGlitchy.Color = Color.White * 0.2f * num;
                    portraitGlitchy.Render();
                }
            }
        }

        if (textboxOverlay != null)
        {
            int num4 = 1;
            if (portrait != null && PortraitSide(portrait) > 0)
            {
                num4 = -1;
            }

            textboxOverlay.DrawCentered(vector + new Vector2(1688f, 272f * num) / 2f, Color.White, new Vector2(num4, num));
        }

        Calc.PushRandom(shakeSeed);
        int num5 = 1;
        for (int i = Start; i < text.Nodes.Count; i++)
        {
            if (text.Nodes[i] is FancyText.NewLine)
            {
                num5++;
            }
            else if (text.Nodes[i] is FancyText.NewPage)
            {
                break;
            }
        }

        Vector2 vector2 = new Vector2(innerTextPadding + ((portrait != null && PortraitSide(portrait) < 0) ? 256f : 0f), innerTextPadding);
        Vector2 vector3 = new Vector2((portrait == null) ? maxLineWidthNoPortrait : maxLineWidth, (float)linesPerPage * lineHeight * num) / 2f;
        float num6 = ((num5 >= 4) ? 0.75f : 1f);
        text.Draw(vector + vector2 + vector3, new Vector2(0.5f, 0.5f), new Vector2(1f, num) * num6, num, Start);
        Calc.PopRandom();
    }

    public int PortraitSide(FancyText.Portrait portrait)
    {
        if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
        {
            return -portrait.Side;
        }

        return portrait.Side;
    }

    public override void Removed(Scene scene)
    {
        Audio.EndSnapshot(Level.DialogSnapshot);
        base.Removed(scene);
    }

    public override void SceneEnd(Scene scene)
    {
        Audio.EndSnapshot(Level.DialogSnapshot);
        base.SceneEnd(scene);
    }

    public static IEnumerator Say(string dialog, params Func<IEnumerator>[] events)
    {
        BetterTextbox textbox = new BetterTextbox(dialog, events);
        Engine.Scene.Add(textbox);
        while (textbox.Opened)
        {
            yield return null;
        }
    }

    public static IEnumerator SayWhileFrozen(string dialog, params Func<IEnumerator>[] events)
    {
        BetterTextbox textbox = new BetterTextbox(dialog, events);
        textbox.Tag |= Tags.FrozenUpdate;
        Engine.Scene.Add(textbox);
        while (textbox.Opened)
        {
            yield return null;
        }
    }
}