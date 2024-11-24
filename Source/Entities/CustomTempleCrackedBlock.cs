using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomTempleCrackedBlock")]
[TrackedAs(typeof(TempleCrackedBlock))]
public class CustomTempleCrackedBlock : TempleCrackedBlock
{
    private EntityID eid;
    public int health;
    public static  bool persistent;
    public Color tint;
    public string breakSound, texture;
    public char debris;
    public MTexture[,,] tiles;
    public float frame;
    public int frames;
    public static void Load()
    {
        On.Celeste.TempleCrackedBlock.Break += onBreak;
    }

    public static void Unload()
    {
        On.Celeste.TempleCrackedBlock.Break -= onBreak;
    }

    public CustomTempleCrackedBlock(EntityData data, Vector2 offset, EntityID id) : base(id, data.Position + offset, data.Width, data.Height, persistent)
    {
        persistent = data.Bool("persistent", false);
        tint = data.HexColor("tint", Color.White);
        breakSound = data.Attr("breakSound", "event:/game/05_mirror_temple/crackedwall_vanish");
        health = data.Int("health", 1); //Number of times it needs to be hit until it breaks
        texture = data.Attr("texture","objects/KoseiHelper/CustomTempleCrackedBlock/breakBlock");
        debris = data.Char("debris", '1');

        int num = (int)(data.Width / 8f);
        int num2 = (int)(data.Height / 8f);
        List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures(texture);
        tiles = new MTexture[num, num2, atlasSubtextures.Count];
        frames = atlasSubtextures.Count;
        for (int i = 0; i < num; i++)
        {
            for (int j = 0; j < num2; j++)
            {
                int num3 = ((i < num / 2 && i < 2) ? i : ((i < num / 2 || i < num - 2) ? (2 + i % 2) : (5 - (num - i - 1))));
                int num4 = ((j < num2 / 2 && j < 2) ? j : ((j < num2 / 2 || j < num2 - 2) ? (2 + j % 2) : (5 - (num2 - j - 1))));
                for (int k = 0; k < atlasSubtextures.Count; k++)
                {
                    tiles[i, j, k] = atlasSubtextures[k].GetSubtexture(num3 * 8, num4 * 8, 8, 8);
                }
            }
        }
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        if (CollideCheck<Player>())
        {
            if (persistent)
            {
                SceneAs<Level>().Session.DoNotLoad.Add(eid);
            }
            RemoveSelf();
        }
        else
        {
            Collidable = (Visible = true);
        }
    }

    public override void Render()
    {
        int num = (int)frame;
        if (num >= frames)
        {
            return;
        }
        for (int i = 0; (float)i < base.Width / 8f; i++)
        {
            for (int j = 0; (float)j < base.Height / 8f; j++)
            {
                tiles[i, j, num].Draw(Position + new Vector2(i, j) * 8f, Vector2.Zero,tint);
            }
        }
    }

    private static void onBreak(On.Celeste.TempleCrackedBlock.orig_Break orig, TempleCrackedBlock self, Vector2 from)
    {
        throw new NotImplementedException();
    }

    public void Break(Vector2 from) //TODO it's not working
    {
        if (persistent)
        {
            SceneAs<Level>().Session.DoNotLoad.Add(eid);
        }
        health -= 1;
        if (health == 0)
        {
            Logger.Debug(nameof(KoseiHelperModule), $"A Custom Temple Cracked Block was broken.");
            Audio.Play(breakSound, base.Center);
            broken = true;
            Collidable = false;
            for (int i = 0; (float)i < base.Width / 8f; i++)
            {
                for (int j = 0; (float)j < base.Height / 8f; j++)
                {
                    base.Scene.Add(Engine.Pooler.Create<Debris>().Init(Position + new Vector2(i * 8 + 4, j * 8 + 4), debris, playSound: true).BlastFrom(from));
                }
            }
        }
    }
}