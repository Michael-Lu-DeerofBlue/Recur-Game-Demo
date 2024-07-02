using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    public interface IConnectionWithOptions<TOption> : IConnection<int>
    {
        bool HasOptions();
        List<TOption> GetOptionLabels();
        void SetOptionLabels(List<TOption> optionLabels);

        /// <summary>
        /// Fetches the options labels from the connection.
        /// </summary>
        void RefreshOptionLabels();
    }
}
