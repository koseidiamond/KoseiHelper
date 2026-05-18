using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/MapController")]
[Tracked]
public class MapController : Entity
{
    public string pinMadeline, pinBerry;
    public string[] mapLayers;
    public string rootPath;
    public bool onlyOnSafeGround;
    public string mapDisabledFlag;
    public MapController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        base.Tag = Tags.Global;
        pinMadeline = data.Attr("pinMadeline", "KoseiHelper/map/pins/pin_SingleDash");
        pinBerry = data.Attr("pinBerry", "collectables/strawberry");
        mapLayers = data.Attr("mapLayers", "filenames,separated,by,commas").Split(',');
        rootPath = data.Attr("rootPath", "");
        onlyOnSafeGround = data.Bool("onlyOnSafeGround", false);
        mapDisabledFlag = data.Attr("mapDisabledFlag", "KoseiHelper_CantOpenMap");
    }
}