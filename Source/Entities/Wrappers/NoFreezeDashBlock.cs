using Celeste.Mod.Entities;
using Monocle;
using MonoMod;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

// By Kalobi!
[TrackedAs(typeof(DashBlock))]
public class NoFreezeDashBlock : DashBlock
{
    public NoFreezeDashBlock(Vector2 position, char tiletype, float width, float height, bool blendIn, bool permanent, bool canDash, EntityID id) : base(position, tiletype, width, height, blendIn, permanent, canDash, id)
    {
    }

    [MonoModLinkTo("Monocle.Entity", "System.Void Removed(Monocle.Scene)")]
    public void base_Removed(Scene scene)
    {
    }

    public override void Removed(Scene scene)
    {
        base_Removed(scene);
    }
}