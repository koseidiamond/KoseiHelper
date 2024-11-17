using Celeste.Mod.Entities;
using Monocle;
using System;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/IRLController")]
public class IRLController : Entity
{

    private Level level;
    private int currentHour;
    private bool changesDarknessLevel;
    public IRLController (EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        changesDarknessLevel = data.Bool("changesDarknessLevel", false);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (changesDarknessLevel)
        {
            float lightLevel = GetLightLevelForHour(currentHour);
            level.BaseLightingAlpha = lightLevel;
        }
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        level = SceneAs<Level>();
        int currentMonth = DateTime.Now.Month;
        currentHour = DateTime.Now.Hour;
        for (int month = 1; month <= 12; month++)
        {
            string kosei_irlMonth = $"kosei_irlMonth{month}";

            if (month == currentMonth)
            {
                level.Session.SetFlag(kosei_irlMonth, true);
            }
            else
            {
                level.Session.SetFlag(kosei_irlMonth, false);
            }
        }
    }

    public override void Update()
    {
        if (Scene.OnInterval(1f)) // We don't need to get the current hour every frame
            currentHour = DateTime.Now.Hour;

        if (changesDarknessLevel)
        {
            float lightLevel = GetLightLevelForHour(currentHour);
            level.BaseLightingAlpha = lightLevel;
        }
        for (int hour = 0; hour < 24; hour++)
        {
            string kosei_irlHour = $"kosei_irlHour{hour}";
            if (hour == currentHour)
                level.Session.SetFlag(kosei_irlHour, true);
            else
                level.Session.SetFlag(kosei_irlHour, false);
        }

        if (currentHour >= 6 && currentHour < 12) // Morning
        {
            level.Session.SetFlag("kosei_irlMorning", true);
            level.Session.SetFlag("kosei_irlAfternoon", false);
            level.Session.SetFlag("kosei_irlNight", false);
        }
        else if (currentHour >= 12 && currentHour < 19) // Afternoon
        {
            level.Session.SetFlag("kosei_irlMorning", false);
            level.Session.SetFlag("kosei_irlAfternoon", true);
            level.Session.SetFlag("kosei_irlNight", false);
        }
        else // Night
        {
            level.Session.SetFlag("kosei_irlMorning", false);
            level.Session.SetFlag("kosei_irlAfternoon", false);
            level.Session.SetFlag("kosei_irlNight", true);
        }

        int currentDay = DateTime.Now.Day;
        for (int day = 1; day <= 31; day++)
        {
            string kosei_irlDay = $"kosei_irlDay{day:D2}";
            if (day == currentDay)
                level.Session.SetFlag(kosei_irlDay, true);
            else
                level.Session.SetFlag(kosei_irlDay, false);
        }
    }

    private float GetLightLevelForHour(int hour)
    {
        if (hour >= 9 && hour <= 19)
        {
            return 0;
        }
        else
        {
            if (hour == 19 || hour == 8)
                return 0.1f;
            else if (hour == 20 || hour == 7)
                return 0.25f;
            else if (hour == 21 || hour == 6)
                return 0.4f;
            else
                return 0.8f;
        }
    }
}