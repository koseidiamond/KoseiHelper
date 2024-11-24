using Celeste.Mod.Entities;
using Monocle;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.IO;
using System.Linq;

namespace Celeste.Mod.KoseiHelper.Triggers
{
    [CustomEntity("KoseiHelper/FlagsFromFileTrigger")]
    public class FlagsFromFileTrigger : Trigger
    {
        private List<string> flags = new List<string>();
        private string filePath;
        private bool writeToFile; // If this is true, write new flags to the file
        private string flagsToWrite; // Comma-separated list of flags to add to the file

        public FlagsFromFileTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            filePath = data.Attr("filePath");
            writeToFile = data.Bool("writeToFile", false);
            flagsToWrite = data.Attr("flagsToWrite", "");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            Level level = SceneAs<Level>();

            if (player != null && !string.IsNullOrEmpty(filePath))
            {
                if (Everest.Content.TryGet(filePath, out var modAsset))
                {
                    try
                    {
                        if (writeToFile && !string.IsNullOrEmpty(flagsToWrite))
                        {
                            AppendFlagsToFile();
                        }
                        using (StreamReader reader = new StreamReader(modAsset.Stream))
                        {
                            string content = reader.ReadToEnd();
                            flags = content.Split(',').Select(flag => flag.Trim()).ToList();
                            foreach (var flag in flags)
                            {
                                Logger.Debug(nameof(KoseiHelperModule), $"Flag loaded: {flag}");
                                level.Session.SetFlag(flag);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(nameof(KoseiHelperModule), $"Error reading flags file: {ex.Message}");
                    }
                }
                else
                    Logger.Debug(nameof(KoseiHelperModule), $"File not found at {filePath}");
                RemoveSelf();
            }
            else
                Logger.Debug(nameof(KoseiHelperModule), "File path is empty or invalid.");
        }
        private void AppendFlagsToFile()
        {
            if (string.IsNullOrEmpty(flagsToWrite))
            {
                Logger.Debug(nameof(KoseiHelperModule), "No flags to write.");
                return;
            }
            try
            {
                string[] newFlags = flagsToWrite.Split(',').Select(flag => flag.Trim()).ToArray();
                string existingContent = "";
                if (File.Exists(filePath))
                {

                    Logger.Debug(nameof(KoseiHelperModule), $"The file exists");
                    existingContent = File.ReadAllText(filePath);
                }
                var combinedFlags = existingContent.Split(',')
                    .Select(flag => flag.Trim())
                    .Concat(newFlags)
                    .Distinct()
                    .ToList();
                File.WriteAllText(filePath, string.Join(",", combinedFlags));
                Logger.Debug(nameof(KoseiHelperModule), $"Flags written to file: {string.Join(",", combinedFlags)}");
            }
            catch (Exception ex)
            {
                Logger.Error(nameof(KoseiHelperModule), $"Error writing flags to file: {ex.Message}");
            }
        }
    }
}
