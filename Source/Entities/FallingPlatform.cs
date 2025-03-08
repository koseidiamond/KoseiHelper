using Celeste.Mod.Entities;
using Monocle;
using System.Collections;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FallingPlatform")]
[Tracked]
public class FallingPlatform : JumpthruPlatform
{
    private bool hasFallen;
    private float fallDelay, originalFallTime;
    public bool triggered;
    public float customFallSpeed;
    public bool blockImage;
    public string sound;
    public bool fallHorizontally;
    public bool activatedByActors;

    public FallingPlatform(EntityData data, Vector2 offset) : base(data, offset)
    {
        overrideTexture = data.Attr("texture", "default");
        fallDelay = originalFallTime = data.Float("fallDelay", 0.2f);
        customFallSpeed = data.Float("fallSpeed", 150f);
        blockImage = data.Bool("blockImage", false);
        sound = data.Attr("fallingSound", "event:/none");
        fallHorizontally = data.Bool("fallHorizontally", false);
        activatedByActors = data.Bool("activatedByActors", false);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);

        if (blockImage)
        {
            List<Image> imagesToRemove = new List<Image>();
            foreach (var component in Components)
            {
                if (component is Image image)
                {
                    imagesToRemove.Add(image);
                }
            }
            foreach (var image in imagesToRemove)
            {
                Remove(image);
            }
            MTexture mTexture = GFX.Game["objects/jumpthru/KoseiHelper/DonutBlock"];
            int numBlocks = (int)(Width / 16);

            for (int i = 0; i < numBlocks; i++)
            {
                MTexture subTexture = mTexture.GetSubtexture(0, 0, 16, 16);
                Image image = new Image(subTexture);
                image.X = i * 16;
                Add(image);
            }
        }
    }


    public override void Update()
    {
        base.Update();
        if (!hasFallen)
        {
            if (HasRider() && activatedByActors || HasPlayerRider() && !activatedByActors)
            {
                // If the someone is on the platform, start the fall process after a delay
                if (fallDelay > 0f)
                {
                    fallDelay -= Engine.DeltaTime; // Countdown to start falling
                }
                else
                {
                    if (!triggered)
                    {
                        StartFalling(); // Start falling if someone has stood for too long
                    }
                }
            }
            if (!HasRider() && activatedByActors || !HasPlayerRider() && !activatedByActors)
            {
                fallDelay = originalFallTime;
            }
        }
    }
    public void StartFalling()
    {
        triggered = true;
        Audio.Play(sound, Center);
        Add(new Coroutine(FallSequence()));
    }
    private IEnumerator FallSequence()
    {
        Level level = SceneAs<Level>();
        // Continue falling until hitting a solid
        while (true)
        {
            if (!fallHorizontally) // Vertical movement
            {
                MoveV(customFallSpeed * Engine.DeltaTime);
                if (CollideCheck<Solid>(TopCenter + Vector2.UnitY))
                {
                    // We hit something solid (ground or another platform), stop falling
                    break;
                }
            }
            else // Horizontal movement
            {
                MoveH(customFallSpeed * Engine.DeltaTime);
                if (customFallSpeed > 0 && CollideCheck<Solid>(CenterLeft + Vector2.UnitX) ||
                    customFallSpeed < 0 && CollideCheck<Solid>(CenterLeft + Vector2.UnitX))
                {
                    MoveV(customFallSpeed * Engine.DeltaTime);
                    if (CollideCheck<Solid>(TopCenter + Vector2.UnitY))
                    {
                        // We hit something solid (ground or another platform), stop falling
                        break; //TODO make them fall
                    }
                }
            }

            yield return null;
        }
        // When hitting something
        Collidable = false;
        yield return 0.2f;
        RemoveSelf();
    }
}