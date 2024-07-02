using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A ConnectionWithOptions can GET and SET the SELECTED INDEX of some OPTIONS.<br />
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ConnectionWithOptions<TOption> : Connection<int>, IConnectionWithOptions<TOption>
    {
        public bool HasOptions()
        {
            var options = GetOptionLabels();
            return options != null && options.Count > 0;
        }

        public abstract List<TOption> GetOptionLabels();
        public abstract void SetOptionLabels(List<TOption> optionLabels);
        public abstract void RefreshOptionLabels();
    }
}
