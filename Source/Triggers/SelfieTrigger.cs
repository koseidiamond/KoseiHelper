using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System.Collections;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/SelfieTrigger")]
public class SelfieTrigger : Trigger
{

    private Entities.Selfie selfie;
    public bool flash, oneUse;
    private string image, buttonTexture, photoInSound, photoOutSound, inputSound, flag;
    private float timeToOpen;
    private TriggerMode triggerMode;

    public SelfieTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
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
    }

    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnEnter && player.Scene != null && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
            Add(new Coroutine(AddSelfie()));
    }

    public override void OnLeave(Player player)
    {
        base.OnEnter(player);
        Level level = SceneAs<Level>();
        if (triggerMode == TriggerMode.OnLeave && player.Scene != null && (string.IsNullOrEmpty(flag) || level.Session.GetFlag(flag)))
            Add(new Coroutine(AddSelfie()));
    }

    public IEnumerator AddSelfie()
    {
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        player.DummyAutoAnimate = true;
        player.StateMachine.State = 11;
        player.StateMachine.Locked = true;
        selfie = new Entities.Selfie(SceneAs<Level>());
        Scene.Add(selfie);
        yield return selfie.PictureRoutine(image, buttonTexture, photoInSound, photoOutSound, inputSound, timeToOpen, flash);
        selfie = null;
        player.StateMachine.Locked = false;
        player.StateMachine.State = 0;
        yield return 0.2f;
        if (oneUse)
            RemoveSelf();
    }
}