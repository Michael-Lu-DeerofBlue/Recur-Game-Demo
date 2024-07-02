using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// Makes a copy of the QualitySetting. Use this to reset the QualitySettings
    /// after making changes to individual fields.
    /// </summary>
    public class QualityPresets
    {
        public static Dictionary<int,QualityPreset> Presets = new Dictionary<int, QualityPreset>();

        /// <summary>
        /// Goes through all available QualitySettings and generates a copy for each.<br />
        /// Keep in mind that all of the are applies once in order for them the be readable.
        /// </summary>
        public static void AddAllLevels()
        {
            // Remember level
            int level = QualitySettings.GetQualityLevel();

            // Go through all levels and copy each
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, applyExpensiveChanges: false);
                AddCurrentLevel();
            }

            // Restore level
            QualitySettings.SetQualityLevel(level);
        }

        /// <summary>
        /// Makes a copy of the current QualitySettings IF no such copy exists yet.<br />
        /// Does nothing if it already has a copy stored for the current quality level.
        /// </summary>
        public static void AddCurrentLevel()
        {
            int level = QualitySettings.GetQualityLevel();
            if (!Presets.ContainsKey(level))
            {
                Presets.Add(level, QualityPreset.CreateFromCurrentLevel());
            }
        }

        /// <summary>
        /// Updates the CURRENT QualitySetting from a stored copy.<br />
        /// Does nothing if it has not copy matching the current quality level.
        /// </summary>
        public static void RestoreCurrentLevel()
        {
            RestoreCurrentFrom(QualitySettings.GetQualityLevel());
        }

        /// <summary>
        /// Updates the CURRENT QualitySetting from a stored copy.<br />
        /// Does nothing if it has not copy matching the given level.
        /// </summary>
        /// <param name="level"></param>
        public static void RestoreCurrentFrom(int level)
        {
            if (Presets.ContainsKey(level))
            {
                Presets[level].ApplyToCurrentLevel();
            }
        }

        /// <summary>
        /// Returns the Preset of the given quality level.<br />
        /// Will return NULL if no copy of the given level is available.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static QualityPreset GetPreset(int level)
        {
            if (Presets.ContainsKey(level))
            {
                return Presets[level];
            }

            return null;
        }

        /// <summary>
        /// Returns a new list generated from the copied QualitySetting presets.<br />
        /// The indices match the QualityLevel.<br />
        /// <br />
        /// NOTICE: The list may contain NULL values depending on whether or not all
        /// QualityLevels have been added already. Use AddAllLevels() before this to
        /// ensure each level has a preset.
        /// </summary>
        /// <returns></returns>
        public static List<QualityPreset> GetPresetList()
        {
            var list = new List<QualityPreset>();
            int maxLevel = Presets.Max(kv => kv.Key);
            for (int i = 0; i < maxLevel; i++)
            {
                if (Presets.ContainsKey(i))
                {
                    list.Add(GetPreset(i));
                }
            }
            return list;
        }
    }
}
