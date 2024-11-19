using Celeste.Mod.Entities;
using Monocle;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using System.Linq;
using Mono.Cecil.Cil;


namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/TopDownViewController")]
[Tracked]
public class TopDownViewController : Entity
{
    private Level level;
    public TopDownViewController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        // TODO Attributes: sound when walking, 4/8 directions
    }

    public static void Load()
    {
        On.Celeste.Player.UnderwaterMusicCheck += Player_UnderwaterMusicCheck;
        IL.Celeste.Player.CallDashEvents += Player_CallDashEvents;
        IL.Celeste.TalkComponent.Update += ModTalkComponentUpdate;
    }

    public static void Unload()
    {
        On.Celeste.Player.UnderwaterMusicCheck -= Player_UnderwaterMusicCheck;
        IL.Celeste.Player.CallDashEvents -= Player_CallDashEvents;
        IL.Celeste.TalkComponent.Update -= ModTalkComponentUpdate;
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        Water water = new Water(new Vector2(level.Bounds.Left, level.Bounds.Top - 20), false, false, level.Bounds.Width, level.Bounds.Height + 20);
        KoseiHelperModule.ExtendedVariantImports.TriggerFloatVariant("UnderwaterSpeedX", (float)KoseiHelperModule.ExtendedVariantImports.GetCurrentVariantValue("UnderwaterSpeedY"), true);
        water.Visible = false;
        Scene.Add(water);
        level.Displacement.Enabled = false;

        // TODO Make an option to load the controller globally, add proper sprites, underwater interactions, prevent tech,
        // Reset player st to always swim on update (unless dashing or unless dummy but in that case it shouldnt have gravity)
    }

    public override void Update()
    {
        base.Update();
        if (Scene.OnInterval(0.5f)) // Makes sure to preserve the same speed if you press F5 or something
            KoseiHelperModule.ExtendedVariantImports.TriggerFloatVariant("UnderwaterSpeedX", (float)KoseiHelperModule.ExtendedVariantImports.GetCurrentVariantValue("UnderwaterSpeedY"), true);
        Player player = Scene.Tracker.GetEntity<Player>();
        int playerState = player.StateMachine.state;
        if (playerState == 11)
        {
            player.DummyGravity = false;
            player.DummyMoving = false;
        }
        if (playerState != 3 && playerState != 2 && playerState != 11 && playerState != 14) // If it's a state other than dashing/dummy/restarting, force swimming
            Scene.Tracker.GetEntity<Player>().StateMachine.ForceState(3);
        foreach (SolidTiles solid in level.Entities.FindAll<SolidTiles>())
        {
            if (solid.Position.Y < Scene.Tracker.GetEntity<Player>().Position.Y)
                solid.actualDepth = 1;
            solid.Depth = 1;
        }
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

    private static void ModTalkComponentUpdate(ILContext il) // From PrismaticHelper (luna deer)
    {
        ILCursor cursor = new(il);
        // OnGround() => OnGround() | swimming; outside of water, it checks anyways for non-swim state
        // callvirt instance bool Celeste.Actor::OnGround(int32)
        if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Actor>("OnGround")))
        {
            cursor.Emit(OpCodes.Ldloc_0); // player
            cursor.EmitDelegate<Func<bool, Player, bool>>((old, player) => old || player.StateMachine.State == Player.StSwim);
        }
        else
        {
            return;
        }

        // swap state 0 (normal) for swim
        // ldfld class Monocle.StateMachine Celeste.Player::StateMachine
        // callvirt instance int32 Monocle.StateMachine::get_State()
        if (cursor.TryGotoNext(MoveType.After,
               instr => instr.MatchLdfld<Player>("StateMachine"),
               instr => instr.MatchCallvirt<StateMachine>("get_State")))
        {
            // short-circuits on true
            cursor.Emit(OpCodes.Ldloc_0); // player
            cursor.EmitDelegate<Func<bool, Player, bool>>((old, player) => {
                var canActivateWet = player.Scene.Tracker.GetEntities<TopDownViewController>()
                    .Cast<TopDownViewController>()
                    .Any(x => true);
                return canActivateWet ? player.StateMachine.State != Player.StSwim : old;
            });
        }
        else
        {
            return;
        }
        // and again
        // ldfld class Monocle.StateMachine Celeste.Player::StateMachine
        // call int32 Monocle.StateMachine::op_Implicit(class Monocle.StateMachine)
        int count = 0;
        while (cursor.TryGotoNext(MoveType.After,
               instr => instr.MatchLdfld<Player>("StateMachine"),
               instr => instr.MatchCall<StateMachine>("op_Implicit")))
        {
            // short-circuits on true
            cursor.Emit(OpCodes.Ldloc_0); // player
            cursor.EmitDelegate<Func<bool, Player, bool>>((old, player) => {
                var canActivateWet = player.Scene.Tracker.GetEntities<TopDownViewController>()
                    .Cast<TopDownViewController>()
                    .Any(x => true);
                return canActivateWet ? player.StateMachine.State != Player.StSwim : old;
            });
            count++;
        }
    }
}