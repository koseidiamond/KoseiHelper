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
    private float fallDelay;
    private bool triggered;
    public float customFallSpeed;
    public bool blockImage;
    public string sound;

    public FallingPlatform(EntityData data, Vector2 offset) : base(data, offset)
    {
        overrideTexture = data.Attr("texture", "default");
        fallDelay = data.Float("fallDelay", 0.2f);
        customFallSpeed = data.Float("fallSpeed", 150f);
        blockImage = data.Bool("blockImage", false);
        sound = data.Attr("fallingSound", "event:/none");
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
        if (HasRider() && !hasFallen)
        {
            // If the player is on the platform, start the fall process after a delay
            if (fallDelay > 0f)
            {
                fallDelay -= Engine.DeltaTime; // Countdown to start falling
            }
            else
            {
                if (!triggered)
                {
                    StartFalling(); // Start falling if the player has stood for too long
                }
            }
        }
    }
    private void StartFalling()
    {
        triggered = true;
        Audio.Play(sound, Center);
        Add(new Coroutine(FallSequence()));
    }
    private IEnumerator FallSequence()
    {
        float fallSpeed = customFallSpeed;
        Level level = SceneAs<Level>();

        // Continue falling until hitting a solid
        while (true)
        {
            MoveV(fallSpeed * Engine.DeltaTime);
            if (CollideCheck<Solid>(Position + Vector2.UnitY))
            {
                // We hit something solid (ground or another platform), stop falling
                break;
            }

            yield return null;
        }
        // When hitting something
        Collidable = false;
        yield return 0.2f;
        RemoveSelf();
    }
}