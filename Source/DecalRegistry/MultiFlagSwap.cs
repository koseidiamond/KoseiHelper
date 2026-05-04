using Celeste.Mod.Registry.DecalRegistryHandlers;
using Monocle;
using System.Collections.Generic;
using System.Xml;

namespace Celeste.Mod.KoseiHelper.DecalRegistry;

internal class MultiFlagSwapDecalRegistryHandler : DecalRegistryHandler
{
    private List<string> flags = new();
    private List<string> paths = new();

    public override string Name => "koseihelper.multiflagSwap";

    public override void Parse(XmlAttributeCollection xml)
    {
        flags.Clear();
        paths.Clear();

        int i = 1;
        while (true)
        {
            string flag = Get(xml, $"flag{i}", "");
            string path = Get(xml, $"path{i}", "");
            if (string.IsNullOrEmpty(flag) || string.IsNullOrEmpty(path))
                break;
            flags.Add(flag);
            paths.Add(path);
            i++;
        }
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new MultiFlagSwapComponent(flags, paths));
    }

    internal class MultiFlagSwapComponent : Component
    {
        private readonly List<string> flags;
        private readonly List<List<MTexture>> textures;
        private List<MTexture> defaultTexture;

        public MultiFlagSwapComponent(List<string> flags, List<string> paths)
            : base(active: true, visible: false)
        {
            this.flags = flags;
            textures = new();

            foreach (string path in paths)
            {
                textures.Add(GFX.Game.GetAtlasSubtextures("decals/" + path));
            }
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            if (entity is Decal decal)
                defaultTexture = decal.textures;
        }

        public override void Update()
        {
            base.Update();

            if (Entity is not Decal decal || decal.Scene is not Level level)
                return;

            for (int i = 0; i < flags.Count; i++)
            {
                if (level.Session.GetFlag(flags[i]))
                {
                    decal.textures = textures[i];
                    return;
                }
            }

            if (defaultTexture != null)
                decal.textures = defaultTexture;
        }
    }
}