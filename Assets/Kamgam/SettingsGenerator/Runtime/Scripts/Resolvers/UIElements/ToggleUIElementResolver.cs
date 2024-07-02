// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.SettingsGenerator
{
    public class ToggleUIElementResolver : SettingResolverForVisualElement, ISettingResolver
    {
        protected Toggle _toggle;
        public Toggle Toggle
        {
            get
            {
                if ((_toggle == null && VisualElement != null) || _toggle != VisualElement)
                {
                    _toggle = VisualElement as Toggle;

                    if (_toggle != null)
                        _toggle.RegisterValueChangedCallback(onValueChanged);
                }
                return _toggle;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.Bool };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void Start()
        {
            base.Start();

            if (HasValidSettingForID(ID, GetSupportedDataTypes()))
            {
                var setting = SettingsProvider.Settings.GetSetting(ID);
                setting.AddPulledFromConnectionListener(Refresh);

                Refresh();
            }
        }

        public override void OnDisable()
        {
            // Rest to trigger fetching the label again OnEnable.
            _toggle = null;
            base.OnDisable();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (Toggle != null)
                Toggle.UnregisterValueChangedCallback(onValueChanged);
        }

        protected void onValueChanged(ChangeEvent<bool> evt)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetBool(ID);
            if (setting != null)
            {
                setting.SetValue(evt.newValue);
            }
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            try
            {
                stopPropagation = true;

                var setting = SettingsProvider.Settings.GetBool(ID);
                if (setting != null)
                {
                    Toggle.value = setting.GetValue();
                }
            }
            finally
            {
                stopPropagation = false;
            }
        }
    }
}
#endif
