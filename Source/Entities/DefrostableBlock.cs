using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;
using System.Reflection.Metadata;
using static Celeste.Session;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/DefrostableBlock")]
[Tracked]
public class DefrostableBlock : Solid
{
    public bool defrosting;
    public bool big;
    public static ParticleType defrostParticle = Cloud.P_Cloud;
    public string sound;
    public Sprite sprite;
    public string spriteID;
    private bool fireMode;
    public DefrostableBlock(EntityData data, Vector2 offset) : base(data.Position + offset - new Vector2(data.Bool("big")? 16 : 8),
        data.Bool("big", false) ? 32 : 16, data.Bool("big", false) ? 32 : 16, false)
    {
        Depth = -10050;
        sound = data.Attr("sound", "event:/none");
        big = data.Bool("big", false);
        spriteID = data.Attr("sprite", "koseiHelper_DefrostableBlock");
        Add(sprite = GFX.SpriteBank.Create(spriteID));
        if (big)
            sprite.Play("Big");
        else
            sprite.Play("Small");
        Add(new CoreModeListener(OnChangeMode));
    }

    public override void Update()
    {
        Level level = SceneAs<Level>();
        if (CollideCheck<Shot>() || fireMode)
            defrosting = true;
        if (defrosting)
            Add(new Coroutine(Defrost()));
        base.Update();
    }

    private IEnumerator Defrost()
    {
        Audio.Play(sound);
        if (big)
            sprite.Play("BigMelt");
        else
            sprite.Play("SmallMelt");
        yield return 0.2f;
        if (big)
            SceneAs<Level>().ParticlesFG.Emit(defrostParticle, 5, Center, Vector2.One * 12f);
        else
            SceneAs<Level>().ParticlesFG.Emit(defrostParticle, 5, Center, Vector2.One * 6f);
        RemoveSelf();
        yield break;
    }

    public void OnChangeMode(Session.CoreModes coreMode)
    {
        fireMode = coreMode == Session.CoreModes.Hot;
    }
}