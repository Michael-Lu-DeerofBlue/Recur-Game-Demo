using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Slider;

namespace Kamgam.UGUIComponentsForSettings
{
    [SelectionBase]
    public class SliderUGUI : MonoBehaviour
    {
        public float MinValue{ get => Slider == null ? 0f : Slider.minValue; set { if (Slider != null) Slider.minValue = value; } }
        public float MaxValue { get => Slider == null ? 1f : Slider.maxValue; set { if (Slider != null) Slider.maxValue = value; } }

        [Tooltip("How big should one step be. Keep in mind that very small step sizes are cumbersome if used with a controller or keyboard.")]
        public float StepSize = 10f;

        public bool WholeNumbers
        {
            get
            {
                if (Slider == null)
                    return false;

                return Slider.wholeNumbers;
            }

            set
            {
                if (Slider != null)
                    Slider.wholeNumbers = value;
            }
        }

        /// <summary>
        /// Number format in string. See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        /// </summary>
        public string ValueFormat = "{0:N0} %";

        [Tooltip("Should the default move left/right input commands be used to change the value of the slider\n" +
            "Disable if you want to ship your own controller input. You will have to call Increase() and Decrease() manually.")]
        public bool UseMoveCommandToChangeValue = true;

        public SliderWithEventOverridesUGUI Slider;

        [SerializeField]
        private SliderEvent OnValueChangedEvent;
        public delegate void ValueChangedDelegate(float value);
        public ValueChangedDelegate OnValueChanged;

        [SerializeField]
        protected float _value;
        public float Value
        {
            get
            {
                if (Slider == null)
                    return 0f;

                return WholeNumbers ? Mathf.Round(Slider.value) : Slider.value;
            }

            set
            {
                if (Slider == null)
                    return;

                if (Mathf.Abs(_value - value) <= Mathf.Epsilon)
                    return;

                float newValue = WholeNumbers ? Mathf.Round(value) : value;
                newValue = ConvertToStepValue(newValue);
                newValue = Mathf.Clamp(newValue, MinValue, MaxValue);

                if (Mathf.Abs(newValue) < 0.0001f){
                    newValue = 0f;
                }

                // Update slider only if necessary or else we would end up with an
                // endless loop since UpdateFromSlider is being called by Slider.OnValueChanged().
                if (Mathf.Abs(Slider.value - newValue) > float.Epsilon) {
                    Slider.value = newValue;
                }
                _value = newValue;

                UpdateText();
            }
        }

        protected void UpdateText()
        {
            if (ValueTf == null)
                return;

            if (!string.IsNullOrEmpty(ValueFormat))
                ValueTf.text = string.Format(ValueFormat, Slider.value);
            else
                ValueTf.text = Slider.value.ToString();
        }

        public int IntValue
        {
            get => Mathf.RoundToInt(Slider.value);
        }

        public TextMeshProUGUI TextTf;
        public TextMeshProUGUI ValueTf;

        public string Text
        {
            get
            {
                if (TextTf == null)
                    return null;

                return TextTf.text;
            }

            set
            {
                if (value == Text || TextTf == null)
                    return;

                TextTf.text = value;
            }
        }

        public void Start()
        {
            Slider.onValueChanged.AddListener(onValueChangedHandler);
            Slider.OnMoveOverride = onMove;

            Slider.minValue = MinValue;
            Slider.maxValue = MaxValue;
            Slider.wholeNumbers = WholeNumbers;
            Slider.value = Value;

            UpdateText();
        }

        private void onValueChangedHandler(float value)
        {
            Value = value;
            OnValueChangedEvent?.Invoke(Value);
            OnValueChanged?.Invoke(Value);
        }

        public bool onMove(AxisEventData eventData)
        {
            if (!gameObject.activeInHierarchy)
            {
                return false;
            }

            switch (eventData.moveDir)
            {
                case MoveDirection.Left:
                    if (UseMoveCommandToChangeValue)
                    {
                        Decrease();
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                case MoveDirection.Right:
                    if (UseMoveCommandToChangeValue)
                    {
                        Increase();
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                default:
                    return true;
            }
        }

        public float ConvertToStepValue(float value)
        {
            // set the new value to the closest stepped value;
            float minDelta = float.MaxValue;
            float minDeltaValue = value;
            float refValue = MinValue;
            int steps = Mathf.CeilToInt((MaxValue - MinValue) / StepSize) + 1;
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
            Value = Slider.value + steps * StepSize;
        }

#if UNITY_EDITOR
        public void Reset()
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode && Slider == null)
            {
                Slider = GetComponent<SliderWithEventOverridesUGUI>();

                // If there is no SliderWithEventOverridesUGUI then create a new one.
                // Take existing sliders into account.
                if (Slider == null)
                {
                    var nativeSlider = GetComponent<Slider>();
                    
                    // Since we can not have two slider components on the same object
                    // we have to do a little tmp game object swap.
                    var tmpGO = new GameObject("tmp");
                    tmpGO.hideFlags = HideFlags.HideAndDontSave;

                    var tmpNativeSlider = tmpGO.AddComponent<Slider>();
                    tmpNativeSlider.hideFlags = HideFlags.HideAndDontSave;
                    UnityEditor.EditorUtility.CopySerialized(nativeSlider, tmpNativeSlider);
                    DestroyImmediate(nativeSlider);

                    Slider = gameObject.AddComponent<SliderWithEventOverridesUGUI>();
                    UnityEditor.EditorUtility.CopySerialized(tmpNativeSlider, Slider);

                    if (tmpNativeSlider.wholeNumbers)
                        StepSize = 1;

                    DestroyImmediate(tmpNativeSlider);
                    DestroyImmediate(tmpGO);
                }
            }
        }
#endif
    }
}
