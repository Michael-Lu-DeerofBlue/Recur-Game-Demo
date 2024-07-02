using System.IO;
using UnityEngine;

namespace Kamgam.SettingsGenerator.Examples
{
    // Make sure this runs before any normal Awake code.
    [DefaultExecutionOrder(-10)]
    public class SettingsInitializerSaveLoadToFile : MonoBehaviour
    {
        void Awake()
        {
            Settings.CustomLoadMethod = load;
            Settings.CustomSaveMethod = save;
            Settings.CustomDeleteMethod = delete;
        }

        // Just for reference, this is how the default "save-to-player-prefs" is implemented.
        /*
        void delete(string key, Settings settings)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        void save(string key, Settings settings)
        {
            var json = SettingsSerializer.ToJson(settings);
            if (!string.IsNullOrEmpty(json))
            {
                PlayerPrefs.SetString(key, json);
                PlayerPrefs.Save();
            }
        }

        void load(string key, Settings settings)
        {
            var json = PlayerPrefs.GetString(key, null);
            if (!string.IsNullOrEmpty(json))
            {
                SettingsSerializer.FromJson(json, settings);
            }
        }
        */


        // Let's save the json data into a file instead of Player Prefs.
        string getFilePath(string key)
        {
            return Application.dataPath + "/" + key + ".json";
        }

        void delete(string key, Settings settings)
        {
            var filePath = getFilePath(key);

            // Delete file
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Delete .meta file in editor
#if UNITY_EDITOR
            if (File.Exists(filePath + ".meta"))
            {
                File.Delete(filePath + ".meta");
                UnityEditor.AssetDatabase.Refresh();
            }
#endif
        }

        void save(string key, Settings settings)
        {
            var json = SettingsSerializer.ToJson(settings);
            if (!string.IsNullOrEmpty(json))
            {
                var filePath = getFilePath(key);

                // Write to tmp file first.
                File.WriteAllText(filePath + ".tmp", json, System.Text.Encoding.UTF8);

                // Check if tmp file now exists.
                // If you want to be extra sure you could do a validity check of .tmp.
                if (File.Exists(filePath + ".tmp"))
                {
                    // Delete existing
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    // Finally move the tmp file.
                    File.Move(filePath + ".tmp", filePath);

                    // Log path (for testing)
                    Debug.Log("Saved to: " + filePath);

#if UNITY_EDITOR
                    UnityEditor.AssetDatabase.Refresh();
#endif
                }
            }
        }

        void load(string key, Settings settings)
        {
            var filePath = getFilePath(key);

            // Abort if not found
            if (!File.Exists(filePath))
                return;

            // Read file text
            var json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);

            // Apply to settings
            if (!string.IsNullOrEmpty(json))
            {
                SettingsSerializer.FromJson(json, settings);
            }
        }
    }
}
