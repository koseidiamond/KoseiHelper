using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Triggers;


[CustomEntity("KoseiHelper/DetachFollowerTrigger")]
public class DetachFollowerTrigger : Trigger
{
    public Vector2 Target;
    public bool global, affectBerries, affectKeys, affectOthers, onlyOnce;
    public string sound, flag;
    public TriggerMode triggerMode;
    public string easer;

    public DetachFollowerTrigger(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        Vector2[] array = data.NodesOffset(offset);
        if (array.Length != 0)
        {
            Target = array[0];
        }
        global = data.Bool("global", defaultValue: true);
        affectBerries = data.Bool("affectBerries", true);
        affectKeys = data.Bool("affectKeys", false);
        affectOthers = data.Bool("affectOthers", false);
        sound = data.Attr("sound", "event:/new_content/game/10_farewell/strawberry_gold_detach");
        triggerMode = data.Enum("triggerMode", TriggerMode.OnEnter);
        flag = data.Attr("flag", "");
        onlyOnce = data.Bool("onlyOnce", false);
        easer = data.Attr("easer", "CubeInOut");
    }
    public override void OnEnter(Player player)
    {
        base.OnEnter(player);
        if (triggerMode == TriggerMode.OnEnter)
            Detach(player);
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        if (triggerMode == TriggerMode.OnLeave)
            Detach(player);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (triggerMode == TriggerMode.OnFlagEnabled && !string.IsNullOrEmpty(flag) && SceneAs<Level>().Session.GetFlag(flag))
            Detach(player);
    }

    private void Detach(Player player)
    {
        for (int num = player.Leader.Followers.Count - 1; num >= 0; num--)
        {
            if (player.Leader.Followers[num].Entity is Strawberry)
            {
                if (affectBerries)
                    Add(new Coroutine(DetachFollower(player.Leader.Followers[num])));
            }
            if (player.Leader.Followers[num].Entity is Key)
            {
                if (affectKeys)
                    Add(new Coroutine(DetachFollower(player.Leader.Followers[num])));
            }
            else
            {
                if (affectOthers)
                    Add(new Coroutine(DetachFollower(player.Leader.Followers[num])));
            }
        }
    }
    private IEnumerator DetachFollower(Follower follower)
    {
        if (follower.HasLeader)
        {
            yield return new SwapImmediately(DetachedFollower(follower));
        }
    }
    private IEnumerator DetachedFollower(Follower follower)
    {
        Leader leader = follower.Leader;
        Entity entity = follower.Entity;
        float num = (entity.Position - Target).Length();
        float time = num / 200f;
        if (entity is Strawberry strawberry)
        {
            strawberry.ReturnHomeWhenLost = false;
        }
        leader.LoseFollower(follower);
        entity.Active = false;
        entity.Collidable = false;
        if (global)
        {
            entity.AddTag(Tags.Global);
            follower.OnGainLeader = (Action)Delegate.Combine(follower.OnGainLeader, (Action)(() =>
            {
                entity.RemoveTag(Tags.Global);
            }));
        }
        else
            entity.AddTag(Tags.Persistent);
        Audio.Play(sound, entity.Position);
        Vector2 position = entity.Position;
        SimpleCurve curve = new SimpleCurve(position, Target, position + (Target - position) * 0.5f + new Vector2(0f, -64f));
        for (float p = 0f; p < 1f; p += Engine.DeltaTime / time)
        {
            entity.Position = curve.GetPoint(KoseiHelperUtils.Easer(easer, Ease.CubeInOut)(p));
            yield return null;
        }
        entity.Active = true;
        entity.Collidable = true;
        if (onlyOnce)
            RemoveSelf();
    }
}