using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/SleepyController")]
[Tracked]
public class SleepyController : Entity
{
    private float minTime, maxTime, sleepTimer, napTime;
    public Random sleepRandom;
    private bool adjustHairColor;
    private bool cassetteSleep, sleeping;
    private CassetteBlockManager cassetteManager;
    private int cassetteIndex;
    public SleepyController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        minTime = data.Float("minTime", 1.2f);
        maxTime = data.Float("maxTime", 1.8f);
        napTime = data.Float("napTime", 0.15f);
        cassetteSleep = data.Bool("cassetteSleep", false);
        adjustHairColor = data.Bool("adjustHairColor", true);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        cassetteManager = scene.Tracker.GetEntity<CassetteBlockManager>();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            Position = player.Position;
            if (!cassetteSleep)
            {
                if (sleepRandom == null)
                    sleepRandom = new Random(level.Session.Area.ID * 77 + (int)level.Session.Area.Mode * 999);
                if (sleepTimer <= 0f)
                    sleepTimer = sleepRandom.Range(minTime, maxTime);
                sleepTimer -= Engine.DeltaTime;
                if (sleepTimer <= 0f && player != null)
                    Add(new Coroutine(Sleep(player)));
            }
            else
            {
                if (cassetteIndex != cassetteManager.currentIndex)
                {
                    cassetteIndex = cassetteManager.currentIndex;
                    sleeping = true;
                }
                if (sleeping)
                    Add(new Coroutine(Sleep(player)));
            }
        }
    }

    private IEnumerator Sleep(Player player)
    {

        int previousState = player.StateMachine.State;
        player.StateMachine.State = 11;
        player.ForceCameraUpdate = true;
        player.DummyAutoAnimate = false;
        if (!player.JustRespawned)
        {
            if (adjustHairColor)
            {
                switch (player.Dashes)
                {
                    case 0:
                        KoseiHelperUtils.PlayIfNot(player, "sleepy_zeroDashes");
                        break;
                    case 2:
                        KoseiHelperUtils.PlayIfNot(player, "sleepy_twoDashes");
                        break;
                    case 3:
                        KoseiHelperUtils.PlayIfNot(player, "sleepy_threeDashes");
                        break;
                    case 4:
                        KoseiHelperUtils.PlayIfNot(player, "sleepy_fourDashes");
                        break;
                    case 5:
                        KoseiHelperUtils.PlayIfNot(player, "sleepy_fiveDashes");
                        break;
                    default:
                        KoseiHelperUtils.PlayIfNot(player, "asleep");
                        break;
                }
            }
            else
                KoseiHelperUtils.PlayIfNot(player, "asleep");
        }
        yield return napTime;
        player.StateMachine.State = 0;
        player.ForceCameraUpdate = false;
        sleeping = false;
        yield return null;
    }
}