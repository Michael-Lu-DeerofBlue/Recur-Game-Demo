using UnityEngine;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    [AddComponentMenu("UI/Settings/RandomColorUGUIResolver")]
    [RequireComponent(typeof(RandomColorUGUI))]
    public class RandomColorUGUIResolver : SettingResolver, ISettingResolver
    {
        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.Color };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected RandomColorUGUI randomColorUGUI;
        public RandomColorUGUI RandomColorUGUI
        {
            get
            {
                if (randomColorUGUI == null)
                {
                    randomColorUGUI = this.GetComponent<RandomColorUGUI>();
                }
                return randomColorUGUI;
            }
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();
            
            RandomColorUGUI.OnColorChanged += onColorChanged;

            if (HasValidSettingForID(ID, GetSupportedDataTypes()) && HasActiveSettingForID(ID))
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                setting.AddPulledFromConnectionListener(Refresh);

                Refresh();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (RandomColorUGUI != null)
                RandomColorUGUI.OnColorChanged -= onColorChanged;
        }

        private void onColorChanged(Color color)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetColor(ID);
            setting.SetValue(color);
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetColor(ID);

            if (setting == null)
                return;

            try
            {
                stopPropagation = true;

                RandomColorUGUI.Color = setting.GetValue();
            }
            finally
            {
                stopPropagation = false;
            }
        }
    }
}
