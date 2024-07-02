using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public partial class Settings : ScriptableObject
    {
        /// <summary>
        /// List of input elements which have been shown at least once.
        /// We use this list to update the UIs after a reset.
        /// Input elements register once they are made visible for the first time.
        /// </summary>
        public List<ISettingResolver> RegisteredResolvers = new List<ISettingResolver>();

        public void RegisterResolver(ISettingResolver resolver)
        {
            if (resolver == null)
                return;
            
            // Ignore elements which are not part of this settings object.
            if (!HasID(resolver.GetID()))
                return;

            RegisteredResolvers.Add(resolver);
            DefragRegisteredResolvers();
        }

        public void UnregisterResolver(ISettingResolver input)
        {
            if (input == null)
                return;

            RegisteredResolvers.Remove(input);
            DefragRegisteredResolvers();
        }

        public void DefragRegisteredResolvers()
        {
            for (int i = RegisteredResolvers.Count-1; i >= 0; i--)
            {
                if (RegisteredResolvers[i] == null)
                    RegisteredResolvers.RemoveAt(i);
            }
        }

        /// <summary>
        /// Triggers all registered resolvers to update themselves.
        /// </summary>
        public void RefreshRegisteredResolvers()
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                resolver.Refresh();
            }
        }

        /// <summary>
        /// Refreshes resolvers registered to the given ID.
        /// </summary>
        /// <param name="id"></param>
        public void RefreshRegisteredResolvers(string id)
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                if(resolver.GetID() == id)
                    resolver.Refresh();
            }
        }

        /// <summary>
        /// Refreshes resolvers registered to the given setting.
        /// </summary>
        /// <param name="setting"></param>
        public void RefreshRegisteredResolvers(ISetting setting)
        {
            RefreshRegisteredResolvers(setting.GetID());
        }

        /// <summary>
        /// Refreshes resolvers that are connected to a setting with the given Connection type T.
        /// </summary>
        public void RefreshRegisteredResolversWithConnection<T>() where T : IConnection
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                var id = resolver.GetID();
                if (!string.IsNullOrEmpty(id))
                {
                    var setting = GetSetting(id);
                    if (setting != null && setting.HasConnection() && setting.GetConnectionInterface() is T)
                    {
                        resolver.Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Refreshes resolvers that are connected to a setting with the given connection object.
        /// </summary>
        public void RefreshRegisteredResolversWithConnection(IConnection connection)
        {
            if (RegisteredResolvers == null || RegisteredResolvers.Count == 0)
                return;

            DefragRegisteredResolvers();

            foreach (var resolver in RegisteredResolvers)
            {
                var id = resolver.GetID();
                if (!string.IsNullOrEmpty(id))
                {
                    var setting = GetSetting(id);
                    if (setting != null && setting.HasConnection() && setting.GetConnectionInterface() == connection)
                    {
                        resolver.Refresh();
                    }
                }
            }
        }

        public void Reset()
        {
            // Reset all
            foreach (var field in _settingsCache)
            {
                field.ResetToDefault();
            }

            // Notify inputs
            DefragRegisteredResolvers();
            foreach (var input in RegisteredResolvers)
            {
                input.Refresh();
            }
        }

        public void Reset(params string[] ids)
        {
            if (ids == null || ids.Length == 0)
                return;

            // Reset fields
            foreach (var setting in _settingsCache)
            {
                // Ignore all which do not match the paths
                if (!ids.Contains(setting.GetID()))
                    continue;

                setting.ResetToDefault();
            }

            // Notify inputs
            DefragRegisteredResolvers();
            foreach (var input in RegisteredResolvers)
            {
                input.Refresh();
            }
        }

        public void ResetGroups(params string[] groups)
        {
            if (groups == null || groups.Length == 0)
                return;

            // Reset fields
            foreach (var setting in _settingsCache)
            {
                // Ignore all which do not match the tags
                if (!setting.MatchesAnyGroup(groups))
                    continue;

                setting.ResetToDefault();
            }

            // Notify inputs
            DefragRegisteredResolvers();
            foreach (var input in RegisteredResolvers)
            {
                input.Refresh();
            }
        }
    }
}
