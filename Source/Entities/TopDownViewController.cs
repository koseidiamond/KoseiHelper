using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Microsoft.Xna.Framework.Graphics;
using System;



namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/TopDownViewController")]
[Tracked]
public class TopDownViewController : Entity
{
    private Level level;
    public TopDownViewController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
    }

    public static void Load()
    {
        On.Celeste.Player.UnderwaterMusicCheck += Player_UnderwaterMusicCheck;
        IL.Celeste.Player.CallDashEvents += Player_CallDashEvents;
    }

    public static void Unload()
    {
        On.Celeste.Player.UnderwaterMusicCheck -= Player_UnderwaterMusicCheck;
        IL.Celeste.Player.CallDashEvents -= Player_CallDashEvents;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        KoseiHelperModule.ExtendedVariantImports.TriggerFloatVariant("UnderwaterSpeedX", (float)KoseiHelperModule.ExtendedVariantImports.GetCurrentVariantValue("UnderwaterSpeedY"), true);
        Water water = new Water(new Vector2(level.Bounds.Left, level.Bounds.Top - 20), false, false, level.Bounds.Width, level.Bounds.Height + 20);
        water.Visible = false;
        Scene.Add(water);
        level.Displacement.Enabled = false;
        
        // TODO block the displacement, make the controller load globally, add proper sprites, and prevent tech
        // Maybe add an option for grid movement too.
    }

    private static bool Player_UnderwaterMusicCheck(On.Celeste.Player.orig_UnderwaterMusicCheck orig, Player self)
    {
        return false;
    }

    private static void Player_CallDashEvents(ILContext il) // From CommunalHelper (coloursofnoise)
    {
        ILCursor cursor = new(il);
        if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("SwimCheck")))
        {
            cursor.EmitDelegate<Func<bool, bool>>(s =>
            {
                TopDownViewController controller = (Engine.Scene as Level)?.Tracker?.GetEntity<TopDownViewController>();
                return s;
            });
        }
    }
}