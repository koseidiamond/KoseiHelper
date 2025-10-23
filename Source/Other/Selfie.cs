using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/Selfie")]
public class Selfie : Entity
{
    private Level level;
    private Image image;
    private bool waitForKeyPress;
    private float timer;
    private Ease.Easer openEaser, endEaser;
    //private Tween tween;

    private string buttonIcon;

    public Selfie(Level level)
    {
        base.Tag = Tags.HUD;
        this.level = level;
    }

    public IEnumerator PictureRoutine(string photo, string textboxbutton, string photoInSound, string photoOutSound, string inputSound,
        float timeToOpen, bool flash, string openEaser, string endEaser)
    {
        buttonIcon = textboxbutton;
        if (flash)
            level.Flash(Color.White);
        yield return timeToOpen;
        yield return OpenRoutine(photo, photoInSound, openEaser);
        yield return WaitForInput(inputSound);
        yield return EndRoutine(photoOutSound, endEaser);
    }

    public IEnumerator OpenRoutine(string selfie, string photoInSound, string openEaser)
    {
        Audio.Play(photoInSound);
        image = new Image(GFX.Portraits[selfie]);
        image.CenterOrigin();
        float percent = 0f;
        while (percent < 1f)
        {
            percent += Engine.DeltaTime;
            image.Position = Vector2.Lerp(new Vector2(992f, 1080f + image.Height / 2f), new Vector2(960f, 540f),
                KoseiHelperUtils.Easer(openEaser, Ease.CubeOut)(percent));
            image.Rotation = MathHelper.Lerp(0.5f, 0f, Ease.BackOut(percent));
            yield return null;
        }
    }

    public IEnumerator WaitForInput(string inputSound)
    {
        waitForKeyPress = true;
        while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed)
            yield return null;
        Audio.Play(inputSound);
        waitForKeyPress = false;
    }

    public IEnumerator EndRoutine(string photoOutSound, string endEaser)
    {
        Audio.Play(photoOutSound);
        float percent = 0f;
        while (percent < 1f)
        {
            percent += Engine.DeltaTime * 2f;
            image.Position = Vector2.Lerp(new Vector2(960f, 540f), new Vector2(928f, (0f - image.Height) / 2f),
                KoseiHelperUtils.Easer(endEaser, Ease.BackIn)(percent));
            image.Rotation = MathHelper.Lerp(0f, -0.15f, Ease.BackIn(percent));
            yield return null;
        }
        yield return null;
        level.Remove(this);
    }

    public override void Update()
    {
        //if (tween != null && tween.Active)
        //    tween.Update();
        if (waitForKeyPress)
            timer += Engine.DeltaTime;
    }

    public override void Render()
    {
        if (base.Scene is Level level && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
            return;
        if (image != null && image.Visible)
            image.Render();
        if (waitForKeyPress)
            GFX.Gui[buttonIcon].DrawCentered(image.Position + new Vector2(image.Width / 2f + 40f, image.Height / 2f + (float)((timer % 1f < 0.25f) ? 6 : 0)));
    }
}
