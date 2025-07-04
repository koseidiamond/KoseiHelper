using System.Collections.Generic;
using Celeste.Mod.CelesteNet;
using Celeste.Mod.CelesteNet.Client;
using Celeste.Mod.CelesteNet.Client.Entities;
using Celeste.Mod.CelesteNet.DataTypes;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.NemesisGun
{
    public class BulletComponent : CelesteNetGameComponent
    {
        private static CelesteNetClientContext clientCtx;

        public static Dictionary<uint, Vector2> Queue = new Dictionary<uint, Vector2>();

        public BulletComponent(CelesteNetClientContext ctx, Game game)
            : base(ctx, game)
        {
            clientCtx = ctx;
        }

        public static void HitGhost(Vector2 bulletDir)
        {
            if (clientCtx?.Client?.PlayerInfo != null)
            {
                clientCtx.Client.Send<BulletData>(new BulletData(clientCtx.Client.PlayerInfo, bulletDir));
            }
        }

        public void Handle(CelesteNetConnection con, BulletData data)
        {
            CelesteNetClientModule.Instance.Context.Main.Ghosts.TryGetValue(data.Player.ID, out Ghost ghost);
            if (data.Player != clientCtx.Client?.PlayerInfo)
            {
                Queue.TryAdd(data.Player.ID, data.BulletDir);
            }
        }

        public static void Update()
        {
            if (Queue.Count != 0)
            {
                foreach (Ghost entity in Engine.Scene.Tracker.GetEntities<Ghost>())
                {
                    Ghost val = entity;
                    val.Dead = true;
                    if (Queue.TryGetValue(val.PlayerInfo.ID, out var value))
                    {
                        //((Entity)(object)val)?.Add((Component)new Bullet(0.1f, value));
                        Queue.Remove(val.PlayerInfo.ID);
                    }
                    if (Queue.Count == 0)
                    {
                        return;
                    }
                }
            }
            Queue = new Dictionary<uint, Vector2>();
        }
    }

}