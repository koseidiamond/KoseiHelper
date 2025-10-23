using Celeste.Mod.Entities;
using FrostHelper;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/SelfieTrigger")]
public class SelfieTrigger : Trigger
{

    private Entities.Selfie selfie;
    public bool flash, oneUse;
    private string image, buttonTexture, photoInSound, photoOutSound, inputSound, flag;
    private float timeToOpen;
    private TriggerMode triggerMode;
    private bool interactable;
    private bool showingRoutine = false;
    private string openEaser, endEaser;

    public SelfieTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        interactable = data.Bool("interactable", false);
        image = data.Attr("image", "selfieCampfire");
        photoInSound = data.Attr("photoInSound", "event:/game/02_old_site/theoselfie_photo_in");
        photoOutSound = data.Attr("photoOutSound", "event:/game/02_old_site/theoselfie_photo_out");
        buttonTexture = data.Attr("buttonTexture", "textboxbutton");
        inputSound = data.Attr("inputSound", "event:/ui/main/button_lowkey");
        flash = data.Bool("flash", true);
        timeToOpen = data.Float("timeToOpen", 0.5f);
        flag = data.Attr("flag", "");
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        oneUse = data.Bool("oneUse", true);
        openEaser = data.Attr("openEaser", "CubeOut");
        endEaser = data.Attr("endEaser", "BackIn");

        if (interactable)
        {
            Add(new TalkComponent(new Rectangle(0, 0, (int)Width, (int)Height), new Vector2(data.Int("talkBubbleX", (int)Width / 2),
                data.Int("talkBubbleY", 0)), (player) => { })
            { PlayerMustBeFacing = false });
        }
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (!interactable && triggerMode == TriggerMode.OnEnter && player.Scene != null && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
            Add(new Coroutine(AddSelfie()));
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (!interactable && triggerMode == TriggerMode.OnLeave && player.Scene != null && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
            Add(new Coroutine(AddSelfie()));
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (interactable && Input.Talk.Pressed && !showingRoutine)
            Add(new Coroutine(AddSelfie()));
    }

    public IEnumerator AddSelfie()
    {
        showingRoutine = true;
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            player.DummyAutoAnimate = true;
            player.StateMachine.State = 11;
            player.StateMachine.Locked = true;
            selfie = new Entities.Selfie(SceneAs<Level>());
            Scene.Add(selfie);
            yield return selfie.PictureRoutine(image, buttonTexture, photoInSound, photoOutSound, inputSound, timeToOpen, flash, openEaser, endEaser);
            selfie = null;
            player.StateMachine.Locked = false;
            player.StateMachine.State = 0;
        }

        if (interactable && oneUse)
            base.RemoveSelf();
        yield return 0.2f;
        showingRoutine = false;
        if (oneUse)
            RemoveSelf();
    }
}