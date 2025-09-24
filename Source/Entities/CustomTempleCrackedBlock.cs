using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/CustomTempleCrackedBlock")]
[TrackedAs(typeof(TempleCrackedBlock))]
public class CustomTempleCrackedBlock : TempleCrackedBlock
{
    public int health;
    public static new bool persistent;
    public Color tint;
    public string breakSound, prebreakSound, texture;
    public char debris;
    public string debrisTexture;
    public new MTexture[,,] tiles;
    public new float frame;
    public new int frames;
    private bool destroyStaticMovers;
    private EntityID id;
    public bool legacy;
    public CustomTempleCrackedBlock(EntityData data, Vector2 offset, EntityID id) : base(id, data.Position + offset, data.Width, data.Height, persistent)
    {
        Depth = data.Int("depth", -9000);
        this.id = id;
        persistent = data.Bool("persistent", false);
        tint = KoseiHelperUtils.ParseHexColor(data.Values.TryGetValue("tint", out object c1) ? c1.ToString() : null, Color.White);
        breakSound = data.Attr("breakSound", "event:/none");
        prebreakSound = data.Attr("prebreakSound", "event:/none");
        health = data.Int("health", 1); //Number of times it needs to be hit until it breaks
        texture = data.Attr("texture", "objects/KoseiHelper/CustomTempleCrackedBlock/breakBlock");
        debris = data.Char("debris", '1');
        debrisTexture = data.Attr("debrisTexture", "debris/KoseiHelper/tintableDebris");
        destroyStaticMovers = data.Bool("destroyStaticMovers", false);
        legacy = data.Bool("legacy", true);

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

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (persistent && KoseiHelperModule.Session.SoftDoNotLoad.Contains(id))
        {
            DestroyStaticMovers();
            RemoveSelf();
        }
    }

    public static void Load()
    {
        On.Celeste.TempleCrackedBlock.Break += modBreak;
    }

    public static void Unload()
    {
        On.Celeste.TempleCrackedBlock.Break -= modBreak;
    }

    private static void modBreak(On.Celeste.TempleCrackedBlock.orig_Break orig, TempleCrackedBlock self, Vector2 from) // Thanks for everything Snip
    {
        if (self is CustomTempleCrackedBlock customTempleCrackedBlock) // == checks if its exactly the same instance; is checks if its of that type
        {
            if (persistent)
                KoseiHelperModule.Session.SoftDoNotLoad.Add(self.eid);
            //self.SceneAs<Level>().Session.DoNotLoad.Add(self.eid);
            customTempleCrackedBlock.health--;
            if (customTempleCrackedBlock.health > 0)
                Audio.Play(customTempleCrackedBlock.breakSound, self.Center);
            if (customTempleCrackedBlock.health == 0)
            {
                //Logger.Debug(nameof(KoseiHelperModule), $"A Custom Temple Cracked Block was broken.");
                Audio.Play(customTempleCrackedBlock.breakSound, self.Center);
                self.broken = true;
                self.Collidable = false;
                if (customTempleCrackedBlock.destroyStaticMovers)
                    customTempleCrackedBlock.DestroyStaticMovers();
                for (int i = 0; (float)i < self.Width / 8f; i++)
                {
                    for (int j = 0; (float)j < self.Height / 8f; j++)
                    {
                        if (customTempleCrackedBlock.legacy)
                            self.Scene.Add(Engine.Pooler.Create<Debris>().Init(self.Position + new Vector2(i * 8 + 4, j * 8 + 4),
                                customTempleCrackedBlock.debris, playSound: true).BlastFrom(from));
                        else
                        {
                            CustomDebris debris = Engine.Pooler.Create<CustomDebris>().Init(self.Position + new Vector2(i * 8 + 4, j * 8 + 4),
                                customTempleCrackedBlock.debris, true).BlastFrom(from).SetTint(customTempleCrackedBlock.tint);
                            debris.SetTexture(customTempleCrackedBlock.debrisTexture);
                            self.Scene.Add(debris);
                        }
                    }
                }
            }
        }
        else
            orig(self, from);
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        if (CollideCheck<Player>())
        {
            if (persistent)
            {
                KoseiHelperModule.Session.SoftDoNotLoad.Add(eid);
                //SceneAs<Level>().Session.DoNotLoad.Add(eid);
            }
            RemoveSelf();
        }
        else
        {
            Collidable = (Visible = true);
        }
    }

    public override void Update()
    {
        base.Update();
        if (broken)
        {
            frame += Engine.DeltaTime * 15f;
            if (frame >= (float)frames)
            {
                RemoveSelf();
            }
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
                tiles[i, j, num].Draw(Position + new Vector2(i, j) * 8f, Vector2.Zero, tint);
            }
        }
    }
}