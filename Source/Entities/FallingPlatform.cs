using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections;
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

    private MTexture mTexture;

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
        string jumpthru = AreaData.Get(scene).Jumpthru;
        if (!string.IsNullOrEmpty(overrideTexture) && !overrideTexture.Equals("default"))
        {
            jumpthru = overrideTexture;
        }
        mTexture = GFX.Game["objects/jumpthru/" + jumpthru];
        if (blockImage)
        {
            List<Image> imagesToRemove = new List<Image>();
            foreach (Component component in Components)
            {
                if (component is Image image)
                {
                    imagesToRemove.Add(image);
                }
            }
            foreach (Image image in imagesToRemove)
            {
                Remove(image);
            }
            mTexture = GFX.Game["objects/jumpthru/KoseiHelper/DonutBlock"];
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
        hasFallen = true;
    }
    private IEnumerator FallSequence()
    {
        Level level = SceneAs<Level>();
        while (true)
        {
            if (!fallHorizontally) // Vertical movement
            {
                MoveV(customFallSpeed * Engine.DeltaTime);
                if (CollideCheck<Solid>())
                    break;
            }
            else // Horizontal movement
            {
                MoveH(customFallSpeed * Engine.DeltaTime);
                if (customFallSpeed > 0 && CollideCheck<Solid>(CenterLeft + Vector2.UnitX) ||
                    customFallSpeed < 0 && CollideCheck<Solid>(CenterLeft + Vector2.UnitX))
                {
                    MoveV(customFallSpeed * Engine.DeltaTime);
                    if (CollideCheck<Solid>())
                        break;
                }
            }

            yield return null;
        }
        // When hitting something
        Collidable = false;
        /*SceneAs<Level>().Add(new DisperseImage(Position,
            new Vector2(fallHorizontally ? (customFallSpeed > 0 ? 1 : -1) : 0, fallHorizontally ? 0 : (customFallSpeed > 0 ? 1 : -1)),
            Center, Vector2.One * 8, mTexture));*/
        Audio.Play("event:/char/madeline/stand", Center);
        Add(new Coroutine(FadeAndDisappear(0.08f)));
    }

    private IEnumerator FadeAndDisappear(float duration)
    {
        float t = 0f;
        List<Image> images = new List<Image>();
        Dictionary<Image, Color> originalColors = new Dictionary<Image, Color>();

        foreach (Image img in Components.GetAll<Image>())
        {
            images.Add(img);
            originalColors[img] = img.Color;
        }

        while (t < duration)
        {
            t += Engine.DeltaTime;
            float progress = t / duration;
            float fade = 1f - progress;

            foreach (Image img in images)
            {
                img.Color = originalColors[img] * fade;
            }

            yield return null;
        }
        foreach (Image img in images)
        {
            img.Color = Color.Transparent;
        }

        RemoveSelf();
    }
}