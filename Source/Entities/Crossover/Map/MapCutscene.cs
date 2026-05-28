using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/MapCutscene")]
[Tracked]
// Original entity by EllaTAS!
public class MapCutscene(Player player) : CutsceneEntity
{
    private readonly Player player = player;
    private MapPage map;

    public override void OnBegin(Level level)
    {
        Add(new Coroutine(Routine()));
    }

    private IEnumerator Routine()
    {
        SceneAs<Level>().FormationBackdrop.Display = true;
        SceneAs<Level>().FormationBackdrop.Alpha = 0.7f;
        player.StateMachine.State = 11;
        player.StateMachine.Locked = true;
        Scene.Add(map = new());
        yield return map.EaseIn();
        yield return map.Wait(player);
        yield return map.EaseOut();
        map = null;
        EndCutscene(Level);
    }

    public override void OnEnd(Level level)
    {
        if (SceneAs<Level>()?.FormationBackdrop?.Display != null)
        {
            SceneAs<Level>().FormationBackdrop.Display = false;
        }
        player.StateMachine.Locked = false;
        player.StateMachine.State = 0;
        map?.RemoveSelf();
    }

    private class MapPage : Entity
    {
        private string[] textures = [""];
        private List<MTexture> segments = new();
        private MTexture currentLocation = GFX.Gui["KoseiHelper/map/pins/pin_SingleDash"];
        private MTexture berryLocations = GFX.Gui["collectables/strawberry"];
        private int berryCount = 0;

        private VirtualRenderTarget target;
        private float alpha = 1f;
        private float quickEaseOut = 1f;
        private Vector2 madelinePosition = new Vector2(-32f, -32f);

        public MapPage()
        {
            Tag = Tags.HUD;
            Add(new BeforeRenderHook(BeforeRender));
        }

        public IEnumerator EaseIn()
        {
            Audio.Play("event:/game/03_resort/memo_in");

            Position = new(960, 0);
            for (float p = 0f; p < 1f; p += Engine.DeltaTime)
            {
                alpha = Ease.CubeOut(p);
                yield return null;
            }
        }

        public IEnumerator Wait(Player player)
        {
            while (!Input.MenuCancel.Pressed && !Input.MenuConfirm.Pressed &&
                !KoseiHelperModule.Settings.MapButton.Pressed && player.onGround)
            {
                yield return null;
            }
            if (!player.onGround)
            {
                quickEaseOut = 2.5f;
            }
            Audio.Play("event:/ui/main/button_lowkey");
        }

        public IEnumerator EaseOut()
        {
            Audio.Play("event:/game/03_resort/memo_out");
            for (float p = 0f; p < 1f; p += Engine.DeltaTime * 1.5f * quickEaseOut)
            {
                alpha = 1f - Ease.CubeIn(p);
                yield return null;
            }
            RemoveSelf();
        }

        public void BeforeRender()
        {
            Level level = SceneAs<Level>();
            target ??= VirtualContent.CreateRenderTarget("oshiro-memo", 1920, 1080);
            Engine.Graphics.GraphicsDevice.SetRenderTarget(target);
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            foreach (MTexture texture in segments)
            {
                texture.Draw(Vector2.Zero, Vector2.Zero, Color.White, 0.5f);
            }
            if (level.Session.GetFlag("map_CollectorsMap"))
            {
                for (int i = 0; i < berryCount; i++)
                {
                    berryLocations.Draw(new Vector2(1846 - i * 60, 994), Vector2.Zero, Color.White, 1f);
                }
            }
            if (level.Session.GetFlag("map_WaywardCompass"))
            {
                currentLocation.Draw(Vector2.Zero + madelinePosition, Vector2.Zero, Color.White, 0.5f);
            }
            Draw.SpriteBatch.End();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = SceneAs<Level>();
            MapController mapController = level.Entities.FindFirst<MapController>();
            if (mapController != null)
            {
                currentLocation = GFX.Gui[mapController.pinMadeline];
                berryLocations = GFX.Gui[mapController.pinBerry];
                textures = mapController.mapLayers;

            }
            foreach (string name in textures)
            {
                if (SceneAs<Level>().Session.GetFlag("map_" + name))
                    segments.Add(GFX.Gui[mapController.rootPath + name]);
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            CompassController compass = scene.Tracker.GetEntity<CompassController>();
            if (compass != null)
            {
                madelinePosition = compass.compassRoomCoordinates;
            }
            foreach (Entity berry in scene)
            {
                if (berry is Strawberry)
                    berryCount++;
            }
        }

        public override void Update()
        {
            // If there's more than one compass in this room, we make sure the correct one is used
            base.Update();
            Level level = SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            CompassController compass = level.Tracker.GetEntity<CompassController>();
            if (player != null && level.Tracker.GetEntities<CompassController>().Count > 1)
            {
                float closestDist = float.MaxValue;
                foreach (CompassController compasses in level.Tracker.GetEntities<CompassController>())
                {
                    float dist = Vector2.DistanceSquared(player.Center, compasses.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        compass = compasses;
                    }
                }
                madelinePosition = compass.compassRoomCoordinates;
            }
            if (compass != null)
                madelinePosition = compass.compassRoomCoordinates;
        }

        public override void Removed(Scene scene)
        {
            target?.Dispose();
            target = null;
            base.Removed(scene);
        }

        public override void SceneEnd(Scene scene)
        {
            target?.Dispose();
            target = null;
            base.SceneEnd(scene);
        }

        public override void Render()
        {
            Level level = Scene as Level;
            if (level?.RetryPlayerCorpse == null && target != null) // Idk why I substracted -2 to the X's origin but it works
                Draw.SpriteBatch.Draw(target, Position, target.Bounds, Color.White * alpha, 0, new Vector2(target.Width - 2f, 0f) / 2f, 1f, SpriteEffects.None, 0f);
        }
    }

    public static void Load()
    {
        On.Celeste.Player.Update += PlayerUpdate;
    }

    public static void Unload()
    {
        On.Celeste.Player.Update -= PlayerUpdate;
    }

    public static void PlayerUpdate(On.Celeste.Player.orig_Update orig, Player player)
    {
        orig(player);
        Level level = player.SceneAs<Level>();
        MapController mapController = level.Entities.FindFirst<MapController>();
        if (mapController != null && KoseiHelperModule.Settings.MapButton.Pressed && level.Entities.FindFirst<MapCutscene>() == null &&
        !level.Session.GetFlag(mapController.mapDisabledFlag) && player.StateMachine.State == 0 && !player.Ducking)
        {
            if ((mapController.onlyOnSafeGround && player.OnSafeGround) || (!mapController.onlyOnSafeGround && player.onGround))
                level.Add(new MapCutscene(player));
        }
    }
}