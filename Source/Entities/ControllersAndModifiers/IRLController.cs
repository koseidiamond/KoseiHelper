using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;

namespace Celeste.Mod.KoseiHelper.Entities;

[CustomEntity("KoseiHelper/IRLController")]
public class IRLController : Entity
{
    private int currentHour, currentMinute;
    private bool changesDarknessLevel;
    public IRLController(EntityData data, Vector2 offset) : base(data.Position + offset)
    {
        if (data.Bool("persistent", true))
            base.Tag = Tags.Persistent;
        changesDarknessLevel = data.Bool("changesDarknessLevel", false);
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        Level level = SceneAs<Level>();
        if (changesDarknessLevel)
        {
            float lightLevel = GetLightLevelForHour(currentHour);
            level.BaseLightingAlpha = lightLevel;
        }
    }

    public override void Added(Scene scene)
    {
        base.Added(scene);
        Level level = SceneAs<Level>();
        int currentMonth = DateTime.Now.Month;

        // day of week
        level.Session.SetFlag("kosei_irl" + DateTime.Now.DayOfWeek.ToString());
        // system username
        level.Session.SetFlag("kosei_irlName" + SaveData.Instance?.Name); // used to be Environment.UserName but i got a complaints form
        // if a gamepad is plugged to the pc or not
        level.Session.SetFlag("kosei_irlGamePad" + (GamePad.GetState(PlayerIndex.One).IsConnected ? "True" : "False"));

        currentHour = DateTime.Now.Hour;
        currentMinute = DateTime.Now.Minute;
        for (int month = 1; month <= 12; month++)
        {
            string kosei_irlMonth = $"kosei_irlMonth{month}";

            if (month == currentMonth)
                level.Session.SetFlag(kosei_irlMonth, true);
            else
                level.Session.SetFlag(kosei_irlMonth, false);
        }
    }

    public override void Update()
    {
        base.Update();
        Level level = SceneAs<Level>();
        if (Scene.OnInterval(1f))
        {// We don't need to get the current hour/minute every frame
            currentHour = DateTime.Now.Hour;
            currentMinute = DateTime.Now.Minute;
        }

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
        for (int minute = 0; minute < 60; minute++)
        {
            string kosei_irlMinute = $"kosei_irlMinute{minute:D2}";
            if (minute == currentMinute)
                level.Session.SetFlag(kosei_irlMinute, true);
            else
                level.Session.SetFlag(kosei_irlMinute, false);
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