// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.SettingsGenerator
{
    public class TextFieldUIElementResolver : SettingResolverForVisualElement, ISettingResolver
    {
        protected TextField _textfield;
        public TextField Textfield
        {
            get
            {
                if ((_textfield == null && VisualElement != null) || _textfield != VisualElement)
                {
                    _textfield = VisualElement as TextField;

                    if (_textfield != null)
                        _textfield.RegisterValueChangedCallback(onValueChanged);
                }
                return _textfield;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.String };

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
            // Rest to trigger fetching the element again OnEnable.
            _textfield = null;
            base.OnDisable();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (Textfield != null)
                Textfield.UnregisterValueChangedCallback(onValueChanged);
        }

        protected void onValueChanged(ChangeEvent<string> evt)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            var setting = SettingsProvider.Settings.GetString(ID);
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

                var setting = SettingsProvider.Settings.GetString(ID);
                if (setting != null)
                {
                    Textfield.value = setting.GetValue();
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
