using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FluteController")]
[Tracked]
public class FluteController : Entity
{
    public bool swapMusicLayers;
    public string flagName, sound, notePath;

    private class FluteNote : Entity
    {
        public FluteNote(Player player, string imagePath) : base(player.TopCenter)
        {
            string suffix = new string[] { "A", "B", "C", "D" }[Calc.Random.Next(0, 4)];
            Depth = -1;
            Image img = new Image(GFX.Game[$"{imagePath}{suffix}"]);
            base.Add(img);
            SineWave sine = new SineWave(0.5f);
            base.Add(sine);
            Vector2 offset = new Vector2(-Math.Sign(player.Speed.X) * 4f, -12f);
            img.CenterOrigin();
            img.Position = offset;
            sine.Active = true;
            sine.OnUpdate = (s) => {
                img.Color *= 0.995F;
                img.Position.Y -= 0.1f;
                img.Position.X = offset.X + s * 8f;
            };
        }

    }

    public FluteController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        swapMusicLayers = data.Bool("swapMusicLayers", true);
        sound = data.Attr("sound", "event:/KoseiHelper/Crossover/YumeFlute");
        flagName = data.Attr("flagName", "KoseiHelper_isDreaming"); // whether Inventory has DreamDash
        notePath = data.Attr("imagePath", "objects/KoseiHelper/Crossover/FluteNotes/note_"); // A, B, C or D
    }


    public override void Update()
    {
        base.Update();
        if (KoseiHelperModule.Settings.FluteButton.Pressed)
        {
            Level level = (Engine.Scene as Level);
            Player player = level.Tracker.GetEntity<Player>();
            if (level != null && player != null)
            {
                bool isDreaming = level.Session.Inventory.DreamDash = !level.Session.Inventory.DreamDash;
                level.Session.SetFlag(flagName, isDreaming);
                Audio.Play(sound, player.Center);
                base.Add(new Coroutine(DisplayNote(level, player), true));

                AudioState audio = level.Session.Audio;

                if (isDreaming)
                {
                    foreach (DreamBlock dreamblock in level.Tracker.GetEntities<DreamBlock>())
                    {
                        dreamblock?.ActivateNoRoutine();
                    }
                    if (swapMusicLayers)
                    {
                        audio.Music.Layer(1, true);
                        audio.Music.Layer(2, false);
                    }
                }
                else
                {
                    foreach (DreamBlock dreamblock in level.Tracker.GetEntities<DreamBlock>())
                    {
                        dreamblock?.DeactivateNoRoutine();
                    }
                    if (swapMusicLayers)
                    {
                        audio.Music.Layer(1, false);
                        audio.Music.Layer(2, true);
                    }
                }
                if (swapMusicLayers)
                    level.Session.Audio.Apply(false);
            }
        }
    }

    private IEnumerator DisplayNote(Level level, Player player)
    {
        FluteNote note = new FluteNote(player, notePath);
        level.Add(note);
        yield return 3f;
        note.RemoveSelf();
        yield break;
    }

}