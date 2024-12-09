using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/SleepyController")]
public class SleepyController : Entity
{
    private float minTime, maxTime, sleepTimer, napTime;
    public Random sleepRandom;
    private Sprite sleepSprite;
    private bool adjustHairColor;
    private bool cassetteSleep, canSleep;
    private CassetteBlockManager cassetteManager;
    private int currentCassetteIndex, previousCassetteIndex;
    public SleepyController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        minTime = data.Float("minTime", 1.2f);
        maxTime = data.Float("maxTime", 1.8f);
        napTime = data.Float("napTime", 0.15f);
        cassetteSleep = data.Bool("cassetteSleep", false);
        adjustHairColor = data.Bool("adjustHairColor", true);
        Add(sleepSprite = GFX.SpriteBank.Create("koseiHelper_player"));
        sleepSprite.Visible = true;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        cassetteManager = scene.Tracker.GetEntity<CassetteBlockManager>();
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        Player player = level.Tracker.GetEntity<Player>();
        if (player != null)
        {
            Position = player.Position;
            sleepSprite.FlipX = player.Facing == Facings.Left ? true : false;
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
                if (currentCassetteIndex != cassetteManager.currentIndex)
                {
                    previousCassetteIndex = currentCassetteIndex;
                    currentCassetteIndex = cassetteManager.currentIndex;
                    canSleep = true;
                }
                if (canSleep)
                    Add(new Coroutine(Sleep(player)));
            }
            previousCassetteIndex = currentCassetteIndex;
            canSleep = false;
        }
    }

    private IEnumerator Sleep(Player player)
    {

        int previousState = player.StateMachine.State;
        player.StateMachine.State = 15;
        player.ForceCameraUpdate = true;
        if (adjustHairColor)
        {
            sleepSprite.Visible = true;
            player.Visible = false;
            switch (player.Dashes)
            {
                case 0:
                    sleepSprite.Play("sleepy_zeroDashes");
                    break;
                case 2:
                    sleepSprite.Play("sleepy_twoDashes");
                    break;
                case 3:
                    sleepSprite.Play("sleepy_threeDashes");
                    break;
                case 4:
                    sleepSprite.Play("sleepy_fourDashes");
                    break;
                case 5:
                    sleepSprite.Play("sleepy_fiveDashes");
                    break;
                default:
                    sleepSprite.Visible = false;
                    player.Visible = true;
                    break;
            }
        }
        yield return napTime;
        if (adjustHairColor)
        {
            player.Visible = true;
            sleepSprite.Visible = false;
        }
        player.StateMachine.State = 0;
        player.ForceCameraUpdate = false;
        yield return null;
    }
}