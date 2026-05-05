using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/Chest")]
[Tracked]
public class Chest : Entity
{
    private string path;
    private string flagSet;
    private string flagRequired; // can be empty
    private string flagForExtra;
    private int minCoins, maxCoins;
    private string currencyName; // If no message is provided the default one will appear:
    // openMessage = "You found You found {#F94A4A}" ..tostring(coinsfoundinchest).. "{#}
    // terracoins inside the chest.{n}You have {#F94A4A}"..tostring(GetFishamount("terracoins")).. "{#} terracoins now.
    private string openedMessage;
    private string lockedMessage;
    private string openSound, paymentSound;

    private int coinsGiven;
    private Image image;
    private enum ChestState
    {
        closed,
        opening,
        opened
    };
    private ChestState state;
    private Coroutine talkRoutine;
    public string counterName;

    public Chest(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        Depth = data.Int("depth", 8999);
        Collider = new Hitbox(16, 16, -8, -8);
        path = data.Attr("path", "objects/KoseiHelper/Crossover/Chest/normalchest_");
        flagSet = data.Attr("flagSet", "normalchestopened");
        flagRequired = data.Attr("flagRequired", "");
        flagForExtra = data.Attr("flagForExtra", "Midas");
        minCoins = data.Int("minCoins", 50);
        maxCoins = data.Int("maxCoins", 60);
        currencyName = data.Attr("currencyName", "coins");
        openedMessage = data.Attr("openedMessage", "This chest is empty.");
        lockedMessage = data.Attr("lockedMessage", "This chest is locked!");
        openSound = data.Attr("openSound", "event:/KoseiHelper/Crossover/Chest");
        paymentSound = data.Attr("paymentSound", "event:/CC/CC_KoseiDiamond_sounds/terracoin");
        counterName = data.Attr("counterName", "KoseiHelper_coins");
        Add(new TalkComponent(new Rectangle(-12, -12, 24, 24), new Vector2(0f, -18f), OnTalk));
    }

    public void OnTalk(Player player)
    {
        Level level = SceneAs<Level>();
        Session session = level.Session;
        if (player != null && level != null)
        {
            if (session.GetFlag(flagSet))
            {// If the chest was already opened
                Add(talkRoutine = new Coroutine(Talk(player, openedMessage)));
            }
            else
            { // (try to) open chest (if not already opened) and give coins
                if (state == ChestState.closed)
                    Add(new Coroutine(OpenCoroutine(player)));
            }
        }
    }

    private IEnumerator Talk(Player player, string message)
    {
        player.StateMachine.State = 11;
        player.StateMachine.Locked = true;
        yield return player.DummyWalkToExact((int)Center.X);
        yield return 0.1f;
        yield return BetterTextbox.Say(message);
        player.StateMachine.Locked = false;
        player.StateMachine.State = 0;
        talkRoutine.Cancel();
        talkRoutine.RemoveSelf();
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        Session session = level.Session;
        if (session.GetFlag(flagSet))
            state = ChestState.opened;
        else
            state = ChestState.closed;
    }

    private IEnumerator OpenCoroutine(Player player)
    {
        Level level = SceneAs<Level>();
        Session session = level.Session;
        if (string.IsNullOrEmpty(flagRequired) || session.GetFlag(flagRequired))
        {
            Random deterministicRandom = new Random(level.Session.Deaths * 37 + SourceId.ID * 13 + (int)player.X * 101 + (int)player.Y);
            if (level.Session.GetFlag("CelesteTAS_TAS_Was_Run"))
            {
                // TAS info
                Logger.Log(LogLevel.Info, "CrossoverCollab", $"Chest RNG seed: Deaths: {level.Session.Deaths}, EntityID: {SourceId.ID}, PlayerPos: {player.Position}");
            }
            Audio.Play(openSound);
            state = ChestState.opening;
            yield return 0.15f;
            Audio.Play(paymentSound);
            if (!string.IsNullOrEmpty(flagForExtra) && session.GetFlag(flagForExtra))
                coinsGiven = (int)(deterministicRandom.NextFloat() * (maxCoins - minCoins) + minCoins) * 2;
            else
                coinsGiven = (int)(deterministicRandom.NextFloat() * (maxCoins - minCoins) + minCoins);
            session.SetCounter(counterName, coinsGiven);
                Add(talkRoutine = new Coroutine(Talk(player, "You found {#F94A4A}" + coinsGiven + "{#} " + currencyName + " inside!{n}" +
                "You have {#F94A4A}" + session.GetCounter(counterName) + "{#} " + currencyName + " now.")));
            session.SetFlag(flagSet, true);
        } // Can't be opened:
        if (!string.IsNullOrEmpty(flagRequired) && !session.GetFlag(flagRequired))
            Add(talkRoutine = new Coroutine(Talk(player, lockedMessage)));
        yield return null;
    }

    public override void Render()
    {
        base.Render();
        image = new Image(GFX.Game[path + state.ToString()]);
        image.Position = Position - new Vector2(image.Width / 2, image.Height / 2);
        image.Render();
    }
}