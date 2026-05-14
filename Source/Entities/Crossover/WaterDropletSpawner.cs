using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/WaterDropletSpawner")]
[Tracked]
// Original code by Popax21! Thank you for making this!
public class WaterDropletSpawner : Entity
{
    private class Droplet : Entity
    {
        private float speed, lifetime, maxSpeed;
        private Color dropletColor;
        private string sound;
        private bool ignoreSolids;

        public Droplet(Vector2 position, Color dropletColor, string sound, int depth, float maxSpeed,
            bool ignoreSolids, float lifetime)
        {
            Position = position;
            speed = 0;
            this.lifetime = lifetime;
            this.dropletColor = dropletColor;
            this.sound = sound;
            this.maxSpeed = maxSpeed;
            this.ignoreSolids = ignoreSolids;
            Depth = depth;
        }

        public override void Update()
        {
            base.Update();
            speed = Calc.Clamp(speed + Engine.DeltaTime * 3.7f, 0, maxSpeed); // Accelerate to terminal velocity
            Position.Y += speed; // Update position
            if (!ignoreSolids) // If in contact with a solid or water, play SFX and disappear
            {
                Solid solid = (Solid)Scene.Tracker.Entities[typeof(Solid)].FirstOrDefault(e => Collide.CheckPoint(e, Position));
                if (solid != null)
                {
                    if (string.IsNullOrEmpty(sound))
                        Audio.Play("event:/char/madeline/landing", Position, "surface_index",
                            solid.GetLandSoundIndex(this)).setVolume(0.075f);
                    else
                        Audio.Play(sound, Position);
                    RemoveSelf();
                    // Make particle splash
                    Position.Y = solid.Top;
                    speed = 0;
                }

                if (Scene.Tracker.Entities[typeof(Water)].Any(e => Collide.CheckPoint(e, Position)))
                {
                    Audio.Play("event:/char/madeline/water_in", Position).setVolume(0.2f);
                    RemoveSelf();
                }
            }

            // If we exited the bounds of the level or lifetime is over, disappear
            if (Scene is Level level && !level.IsInBounds(Position) || (lifetime -= Engine.DeltaTime) <= 0)
                RemoveSelf();
        }

        public override void Render()
        {
            base.Render();
            Draw.Pixel.Draw(Position, Vector2.Zero, dropletColor);
        }
    }

    private Vector2 size;
    private float spawnTimer, spawnInterval, maxSpeed;
    private Color dropletColor;
    private string sound;
    private new int depth;
    private bool ignoreSolids;
    private float lifetime;

    public WaterDropletSpawner(EntityData data, Vector2 offset)
    {
        Position = data.Position + offset;
        size = new Vector2(data.Width, data.Height);
        spawnInterval = data.Float("interval", 1f);
        sound = data.Attr("sound", "");
        depth = data.Int("depth", Depths.FGParticles);
        dropletColor = data.HexColor("dropletColor", Color.FromNonPremultiplied(28, 79, 161, 242));
        spawnTimer = Calc.Random.Range(0, spawnInterval);
        maxSpeed = data.Float("maxSpeed", 10f);
        ignoreSolids = data.Bool("ignoreSolids", false);
        lifetime = data.Float("lifetime", 5f);
    }

    public override void Update()
    {
        base.Update();
        // Update spawn timer
        if ((spawnTimer += Engine.DeltaTime) > spawnInterval)
        {
            spawnTimer = Calc.Random.Range(-spawnInterval / 5, spawnInterval / 5);
            // Spawn new droplet
            Scene.Add(new Droplet(Position + new Vector2(Calc.Random.Range(0, size.X - 1),
                Calc.Random.Range(0, size.Y - 1)), dropletColor, sound, depth, maxSpeed, ignoreSolids, lifetime));
        }
    }
}