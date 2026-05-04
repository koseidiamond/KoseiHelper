using Celeste.Mod.Registry.DecalRegistryHandlers;
using Monocle;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Celeste.Mod.KoseiHelper.DecalRegistry;

internal class CounterSwapDecalRegistryHandler : DecalRegistryHandler
{
    private string counter;
    private List<(int value, string path)> equals = new();

    private int? lessThan;
    private string pathLess;

    private int? greaterThan;
    private string pathMore;

    public override string Name => "koseihelper.counterSwap";

    public override void Parse(XmlAttributeCollection xml)
    {
        counter = Get(xml, "counter", "");

        equals.Clear();

        int i = 1;
        while (true)
        {
            int? value = Get(xml, $"valueForPath{i}", 0); // Value for path of same i
            string path = Get(xml, $"path{i}", "");

            if (value == null || string.IsNullOrEmpty(path))
                break;

            equals.Add((value.Value, path));
            i++;
        }

        lessThan = Get(xml, "lessThan", 0);
        pathLess = Get(xml, "pathLessThan", "");

        greaterThan = Get(xml, "greaterThan", 0);
        pathMore = Get(xml, "pathGreaterThan", "");
    }

    public override void ApplyTo(Decal decal)
    {
        decal.Add(new CounterSwapComponent(
            counter,
            new List<(int, string)>(equals),
            lessThan,
            pathLess,
            greaterThan,
            pathMore
        ));
    }

    internal class CounterSwapComponent : Component
    {
        private readonly string counter;
        private readonly List<(int value, List<MTexture> textures)> equals;

        private readonly int? lessThan;
        private readonly List<MTexture> lessTextures;

        private readonly int? greaterThan;
        private readonly List<MTexture> moreTextures;

        private List<MTexture> defaultTexture;

        public CounterSwapComponent(
            string counter,
            List<(int value, string path)> equalsRaw,
            int? lessThan,
            string pathLess,
            int? greaterThan,
            string pathMore
        ) : base(active: true, visible: false)
        {
            this.counter = counter;

            equals = [];
            foreach (var (value, path) in equalsRaw)
            {
                equals.Add((value, GFX.Game.GetAtlasSubtextures("decals/" + path)));
            }

            this.lessThan = lessThan;
            if (!string.IsNullOrEmpty(pathLess))
                lessTextures = GFX.Game.GetAtlasSubtextures("decals/" + pathLess);

            this.greaterThan = greaterThan;
            if (!string.IsNullOrEmpty(pathMore))
                moreTextures = GFX.Game.GetAtlasSubtextures("decals/" + pathMore);
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
            if (!level.Session.Counters.Any(c => c.Key == counter))
                return; // make sure the counter exists so it doesn't assume it's 0

            int value = level.Session.GetCounter(counter);

            // exact matches (highest priority)
            for (int i = 0; i < equals.Count; i++)
            {
                if (value == equals[i].value)
                {
                    decal.textures = equals[i].textures;
                    return;
                }
            }
            // fallbacks
            if (lessThan.HasValue && value < lessThan.Value && lessTextures != null)
            {
                decal.textures = lessTextures;
                return;
            }
            if (greaterThan.HasValue && value > greaterThan.Value && moreTextures != null)
            {
                decal.textures = moreTextures;
                return;
            }
            // else: just use the default decal texture
            if (defaultTexture != null)
                decal.textures = defaultTexture;
        }
    }
}