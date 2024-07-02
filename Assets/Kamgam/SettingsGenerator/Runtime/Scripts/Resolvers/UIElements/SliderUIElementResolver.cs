// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Kamgam.SettingsGenerator
{
    public class SliderUIElementResolver : SettingResolverForVisualElement, ISettingResolver
    {
        [Tooltip("How big should one step be. Keep in mind that very small step sizes are cumbersome if used with a controller or keyboard.")]
        public float StepSize = 10f;

        [Tooltip("Should the value be rounded to integers?" +
            "\nNOTICE: If connected to an Integer settings then this will always be true.")]
        public bool WholeNumbers = false;

        [System.NonSerialized]
        protected float _value = 0f;
        [SerializeField]
        public float Value
        {
            get
            {
                return WholeNumbers? Mathf.Round(_value) : _value;
            }
            set
            {
                if (Slider == null)
                {
                    _value = Mathf.Round(value / StepSize) * StepSize;
                    if (WholeNumbers)
                        _value = Mathf.Round(_value);
                    return;
                }

                if (Mathf.Abs(_value - value) <= Mathf.Epsilon)
                    return;

                float newValue = WholeNumbers ? Mathf.Round(value) : value;
                newValue = ConvertToStepValue(newValue);
                newValue = Mathf.Clamp(newValue, Slider.lowValue, Slider.highValue);

                if (Mathf.Abs(newValue) < 0.0001f)
                {
                    newValue = 0f;
                }

                // Update slider only if necessary or else we would end up with an
                // endless loop since this is being called by Slider -> OnValueChanged event.
                if (Mathf.Abs(Slider.value - newValue) > float.Epsilon)
                {
                    Slider.value = newValue;
                }

                _value = newValue;
            }
        }

        /// <summary>
        /// Number format in string. See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        /// </summary>
        public string ValueFormat = "{0:N0} %";

        [Tooltip("Should the default move left/right input commands be used to change the value of the slider\n" +
            "Disable if you want to ship your own controller input. You will have to call Increase() and Decrease() manually.")]
        public bool UseMoveCommandToChangeValue = true;

        protected Slider _slider;
        public Slider Slider
        {
            get
            {
                if ((_slider == null && VisualElement != null) || _slider != VisualElement)
                {
                    _slider = VisualElement as Slider;

                    if (_slider != null)
                    {
                        _slider.RegisterValueChangedCallback(onValueChanged);
                        _slider.RegisterCallback<NavigationMoveEvent>(onMove);
                    }
                }
                return _slider;
            }
        }

        protected TextField _valueTf;
        public TextField ValueTf
        {
            get
            {
                if (_valueTf == null)
                {
                    _valueTf = VisualElement.Q<TextField>();
                }
                return _valueTf;
            }
        }

        protected SettingData.DataType[] supportedDataTypes = new SettingData.DataType[] { SettingData.DataType.Int, SettingData.DataType.Float };

        public override SettingData.DataType[] GetSupportedDataTypes()
        {
            return supportedDataTypes;
        }

        protected bool stopPropagation = false;

        public override void OnEnable()
        {
            base.OnEnable();

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
            _slider = null;
            base.OnDisable();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (Slider != null)
                Slider.UnregisterValueChangedCallback(onValueChanged);
        }

        protected void onValueChanged(ChangeEvent<float> evt)
        {
            if (stopPropagation)
                return;

            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            Value = evt.newValue;

            // float
            var settingFloat = SettingsProvider.Settings.GetFloat(ID);
            if (settingFloat != null)
            {
                settingFloat.SetValue(Value);
            }
            else
            {
                // int
                var settingInt = SettingsProvider.Settings.GetInt(ID);
                if (settingInt != null)
                {
                    settingInt.SetValue(Mathf.RoundToInt(Value));
                }
            }
        }

        public void onMove(NavigationMoveEvent evt)
        {
            // Abort if not focused.
            if (!isFocused(Slider))
            {
                return;
            }

            switch (evt.direction)
            {
                case NavigationMoveEvent.Direction.Left:
                    if (UseMoveCommandToChangeValue)
                    {
                        Decrease();
#if UNITY_2021_2_OR_NEWER
                        evt.StopPropagation();
#else
                        evt.PreventDefault();
#endif
                    }
                    break;

                case NavigationMoveEvent.Direction.Right:
                    if (UseMoveCommandToChangeValue)
                    {
                        Increase();
#if UNITY_2021_2_OR_NEWER
                        evt.StopPropagation();
#else
                        evt.PreventDefault();
#endif
                    }
                    break;
            }
        }

        protected bool isFocused(VisualElement ele)
        {
            return ele == ele.panel.focusController.focusedElement;
        }

        public float ConvertToStepValue(float value)
        {
            // set the new value to the closest stepped value;
            float minDelta = float.MaxValue;
            float minDeltaValue = value;
            float refValue = Slider.lowValue;
            int steps = Mathf.CeilToInt((Slider.highValue - Slider.lowValue) / StepSize) + 1;
            for (int i = 0; i < steps; i++)
            {
                float delta = Mathf.Abs(value - refValue);
                if (delta < minDelta)
                {
                    minDelta = delta;
                    minDeltaValue = refValue;
                }
                refValue += StepSize;
            }

            if (WholeNumbers)
                minDeltaValue = Mathf.Round(minDeltaValue);

            return minDeltaValue;
        }

        public void Increase()
        {
            Step(1);
        }

        public void Decrease()
        {
            Step(-1);
        }

        public void Step(int steps)
        {
            Value = Value + steps * StepSize;
        }

        public override void Refresh()
        {
            if (!HasValidSettingForID(ID, GetSupportedDataTypes()) || !HasActiveSettingForID(ID))
                return;

            try
            {
                stopPropagation = true;

                // float
                var settingFloat = SettingsProvider.Settings.GetFloat(ID);
                if (settingFloat != null)
                {
                    Value = settingFloat.GetValue();
                }
                else
                {
                    // int
                    var settingInt = SettingsProvider.Settings.GetInt(ID);
                    if (settingInt != null)
                    {
                        Value = settingInt.GetValue();
                    }
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
