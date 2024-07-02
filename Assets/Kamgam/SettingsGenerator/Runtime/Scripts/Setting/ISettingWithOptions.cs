using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    public interface ISettingWithOptions<TOption> : ISettingWithConnection<int>
    {
        bool HasOptions();
        List<TOption> GetOptionLabels();
        void SetOptionLabels(List<TOption> options);

        bool GetOverrideConnectionLabels();
        void SetOverrideConnectionLabels(bool overrideLabels);
    }
}
