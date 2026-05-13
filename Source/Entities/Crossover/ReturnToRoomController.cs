using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/ReturnToRoomController")]
[Tracked]
// Actual behavior in the KoseiHelperModule
public class ReturnToRoomController(EntityData data, Vector2 offset) : Entity(data.Position + offset)
{
    public string roomName = data.Attr("roomName", "");
    public string dialogID = data.Attr("dialogID", "ReturnToRoom");
    public string flagsToUnset = data.Attr("flagsToUnset", "");
    public string preventionFlag = data.Attr("preventionFlag", "");
    public Player.IntroTypes introType = data.Enum("introType", Player.IntroTypes.None);
    public float closestSpawnX = data.Float("closestSpawnX", 0f), closestSpawnY = data.Float("closestSpawnY", 0f);
    public string sound = data.Attr("sound", "event:/none");
    public bool loseFollowers = data.Bool("loseFollowers", true);
    public int menuIndex = data.Int("menuIndex", 0);
}