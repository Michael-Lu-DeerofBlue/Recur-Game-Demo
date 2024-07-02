using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class SettingCollectionExtensions
    {
        public static IList<ISetting> PullFromConnection(this IList<ISetting> settings)
        {
            foreach (var setting in settings)
            {
                setting.PullFromConnection();
            }

            return settings;
        }

        public static IList<ISetting> PushToConnection(this IList<ISetting> settings)
        {
            foreach (var setting in settings)
            {
                setting.PullFromConnection();
            }

            return settings;
        }

        public static IList<ISetting> RefreshRegisteredResolvers(this IList<ISetting> settings, Settings settingsObj)
        {
            foreach (var setting in settings)
            {
                settingsObj.RefreshRegisteredResolvers(setting.GetID());
            }

            return settings;
        }

        public static IList<ISetting> RefreshRegisteredResolvers(this IList<ISetting> settings, SettingsProvider provider)
        {
            if (!provider.HasSettings())
                return settings;

            foreach (var setting in settings)
            {
                provider.Settings.RefreshRegisteredResolvers(setting.GetID());
            }

            return settings;
        }
    }
}
