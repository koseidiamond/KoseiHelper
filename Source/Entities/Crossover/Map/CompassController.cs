using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities.Crossover;

[CustomEntity("KoseiHelper/CompassController")]
[Tracked]
public class CompassController : Entity
{
    public Vector2 compassRoomCoordinates;
    public int berryCount;

    public CompassController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        compassRoomCoordinates.X = data.Int("roomCoordinatesX", 0);
        compassRoomCoordinates.Y = data.Int("roomCoordinatesY", 0);
        berryCount = data.Int("berryCount", 0);
    }
}