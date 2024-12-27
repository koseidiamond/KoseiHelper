using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomTrapdoor")]
[Tracked]
public class CustomTrapdoor : Entity
{
    private Sprite sprite;
    private PlayerCollider playerCollider;
    private LightOcclude occluder;
    private string sfxTop, sfxBottom;
    private bool occludesLight = true;

    public CustomTrapdoor(EntityData data, Vector2 offset)
    {
        Position = data.Position + offset;
        base.Depth = data.Int("depth", 8999);
        sfxTop = data.Attr("sfxTop", "event:/game/03_resort/trapdoor_fromtop");
        sfxBottom = data.Attr("sfxBottom", "event:/game/03_resort/trapdoor_frombottom");
        occludesLight = data.Bool("occludesLight", true);
        Add(sprite = GFX.SpriteBank.Create(data.Attr("spriteID", "trapdoor")));
        sprite.Play("idle");
        sprite.Y = 6f;
        base.Collider = new Hitbox(24f, 4f, 0f, 6f);
        Add(playerCollider = new PlayerCollider(Open));
        if (occludesLight)
            Add(occluder = new LightOcclude(new Rectangle(0, 6, 24, 2)));
    }
    private void Open(Player player)
    {
        Collidable = false;
        occluder.Visible = false;
        if (player.Speed.Y >= 0f)
        {
            Audio.Play(sfxTop, Position);
            sprite.Play("open");
        }
        else
        {
            Audio.Play(sfxBottom, Position);
            Add(new Coroutine(OpenFromBottom()));
        }
    }

    private IEnumerator OpenFromBottom()
    {
        sprite.Scale.Y = -1f;
        yield return sprite.PlayRoutine("open_partial");
        yield return 0.1f;
        sprite.Rate = -1f;
        yield return sprite.PlayRoutine("open_partial", restart: true);
        sprite.Scale.Y = 1f;
        sprite.Rate = 1f;
        sprite.Play("open", restart: true);
    }
}