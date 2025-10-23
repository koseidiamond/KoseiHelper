using Celeste.Mod.Core;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/MultinodeCameraTrigger")]
[Tracked]
public class MultinodeCameraTrigger : Trigger
{
    public bool onlyOnce;
    public string flag;

    private List<Vector2> nodes;
    private float lerpStrength;
    private bool xOnly;
    private bool yOnly;
    private float maxSmoothingDistance;
    private float smoothingStrength;
    public MultinodeCameraTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        flag = data.Attr("flag", "");
        onlyOnce = data.Bool("onlyOnce", false);

        lerpStrength = data.Float("lerpStrength", 1f);
        xOnly = data.Bool("xOnly", false);
        yOnly = data.Bool("yOnly", false);
        smoothingStrength = data.Float("smoothingStrength", 0.5f);
        maxSmoothingDistance = data.Float("maxSmoothingDistance", 200f);

        Vector2[] arr = data.NodesOffset(offset);
        if (arr != null && arr.Length > 0)
            nodes = new List<Vector2>(arr);
        else
            nodes = new List<Vector2>() { Position };
    }

    public override void OnLeave(Player player)
    {
        base.OnLeave(player);
        ResetCamera(player);
    }

    public override void OnStay(Player player)
    {
        base.OnStay(player);
        if (!KoseiHelperModule.Settings.MotionSicknessPrevention &&
            ((!string.IsNullOrEmpty(flag) && SceneAs<Level>().Session.GetFlag(flag)) || string.IsNullOrEmpty(flag)))
        {
            NodedCameraUpdate(player);
        }
    }

    public void NodedCameraUpdate(Player player)
    {
        Level level = SceneAs<Level>();
        Vector2 playerPos = player.Center;
        Vector2 weightedPosition = Vector2.Zero;
        float totalWeight = 0f;

        foreach (Vector2 node in nodes)
        {
            float dist = Vector2.Distance(playerPos, node);
            if (dist <= maxSmoothingDistance)
            {
                float weight = 1f / (dist + 0.1f);

                weightedPosition += node * weight;
                totalWeight += weight;
            }
        }
        if (totalWeight > 0)
        {
            weightedPosition /= totalWeight;
            Vector2 target = weightedPosition - new Vector2(160f, 90f);
            player.CameraAnchor = Vector2.Lerp(player.CameraAnchor, target, MathHelper.Clamp(lerpStrength * smoothingStrength, 0f, 1f));
        }

        player.CameraAnchorLerp = Vector2.One * MathHelper.Clamp(lerpStrength, 0f, 1f);
        player.CameraAnchorIgnoreX = yOnly;
        player.CameraAnchorIgnoreY = xOnly;

        if (onlyOnce)
            RemoveSelf();
    }

    private void ResetCamera(Player player)
    {
        bool anyTrigger = false;
        foreach (MultinodeCameraTrigger nodedCameraTrigger in Scene.Tracker.GetEntities<MultinodeCameraTrigger>())
        {
            if (nodedCameraTrigger.PlayerIsInside)
            {
                anyTrigger = true;
                break;
            }
        }
        if (!anyTrigger)
        {
            player.CameraAnchorLerp = Vector2.Zero;
        }
    }
}