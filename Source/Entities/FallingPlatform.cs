using Celeste.Mod.Entities;
using Monocle;
using System.Collections;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/FallingPlatform")]
[Tracked]
public class FallingPlatform : JumpthruPlatform
{
    private bool hasFallen;
    private float fallDelay;
    private bool triggered;
    public float customFallSpeed;

    public FallingPlatform(EntityData data, Vector2 offset) : base(data, offset)
    {
        overrideTexture = data.Attr("texture", "default");
        fallDelay = data.Float("fallDelay", 0.04f);
        customFallSpeed = data.Float("fallSpeed", 150f);
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
        StartShaking();
        Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        Audio.Play("event:/none", Center);
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
        StopShaking();
        SceneAs<Level>().Shake();
        Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        Collidable = false;
        yield return 0.2f;
        RemoveSelf();
    }
}
