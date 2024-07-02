using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class StepperUGUI : MonoBehaviour
    {
        public delegate void OnValueChangedDelegate(float value);

        /// <summary>
        /// The index of the selected option.
        /// </summary>
        public UnityEvent<float> OnValueChangedEvent;
        public OnValueChangedDelegate OnValueChanged;

        public float MinValue = 0f;
        public float MaxValue = 100f;
        public float StepSize = 10f;
        public bool WholeNumbers = true;

        // All step properties are optional.
        // If you set them then steps will be instantiated if not, then not.
        public GameObject StepTemplate;
        public GameObject StepsContainer;
        public bool ShowSteps => StepsContainer != null && StepTemplate != null;
        [System.NonSerialized]
        protected List<StepperStepConsoleUGUI> _steps = new List<StepperStepConsoleUGUI>();

        public float StepCountFloat => (MaxValue - MinValue) / StepSize;
        /// <summary>
        /// Mathf.Ceil(range / stepSize).
        /// </summary>
        public int StepCount => Mathf.CeilToInt(((MaxValue - MinValue) - 0.001f) / StepSize);

        /// <summary>
        /// Number format in string. See: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        /// </summary>
        public string ValueFormat = "{0:N0} %";
        [Tooltip("Should the buttons be disabled if the limits (min,max) are reached?")]
        public bool DisableButtons = true;

        public Button DecreaseButton;
        public Button IncreaseButton;

        protected AutoNavigationOverrides decreaseButtonNavigationOverrides;
        public AutoNavigationOverrides DecreaseButtonNavigationOverrides
        {
            get
            {
                if (DecreaseButton == null)
                    return null;

                if (decreaseButtonNavigationOverrides == null)
                {
                    decreaseButtonNavigationOverrides = DecreaseButton.GetComponent<AutoNavigationOverrides>();
                }
                return decreaseButtonNavigationOverrides;
            }
        }

        protected AutoNavigationOverrides increaseButtonNavigationOverrides;
        public AutoNavigationOverrides IncreaseButtonNavigationOverrides
        {
            get
            {
                if (IncreaseButton == null)
                    return null;

                if (increaseButtonNavigationOverrides == null)
                {
                    increaseButtonNavigationOverrides = IncreaseButton.GetComponent<AutoNavigationOverrides>();
                }
                return increaseButtonNavigationOverrides;
            }
        }

        protected float _value;
        public float Value
        {
            get => WholeNumbers ? Mathf.Round(_value) : _value;
            set
            {
                if (Mathf.Abs(_value - value) <= Mathf.Epsilon)
                    return;

                updateValue(value);
                updateButtons();
            }
        }

        public int IntValue
        {
            get => Mathf.RoundToInt(_value);
        }

        public TextMeshProUGUI TextTf;
        public TextMeshProUGUI ValueTf;

        public string Text
        {
            get => TextTf.text;
            set
            {
                if (value == Text)
                    return;

                updateText(value);
                updateButtons();
            }
        }

        [SerializeField]
        [Tooltip("If enabled and if this is selected (has focus) then the descrease/increase action will be triggered by keyboard/controller navigation too.\n" +
            "NOTICE: This also means it will deny left/right selection navigation away from this is object. Useful for console type UIs.")]
        protected bool _enableButtonControls = false;
        public bool EnableButtonControls
        {
            get => _enableButtonControls;
            set
            {
                if (AutoNavigationOverrides != null)
                {
                    AutoNavigationOverrides.BlockLeft = EnableButtonControls;
                    AutoNavigationOverrides.BlockRight = EnableButtonControls;
                }
            }
        }

        protected AutoNavigationOverrides _autoNavigationOverrides;
        public AutoNavigationOverrides AutoNavigationOverrides
        {
            get
            {
                if (_autoNavigationOverrides == null)
                {
                    _autoNavigationOverrides = this.GetComponent<AutoNavigationOverrides>();
                }
                return _autoNavigationOverrides;
            }
        }

        protected Selectable _selectable;
        public Selectable Selectable
        {
            get
            {
                if (_selectable == null)
                {
                    _selectable = this.GetComponent<Selectable>();
                }
                return _selectable;
            }
        }

        protected void updateValue(float value)
        {
            float newValue = WholeNumbers ? Mathf.Round(value) : value;
            newValue = ConvertToStepValue(newValue);

            _value = Mathf.Clamp(newValue, MinValue, MaxValue);
            ValueTf.text = string.Format(ValueFormat, _value);

            if (ShowSteps)
            {
                if (!hasValidSteps())
                    _steps = StepperStepConsoleUGUI.CreateSteps(StepsContainer.transform, StepTemplate, StepCount);

                int stepsToActivate = GetStepToDisplay(Value);
                StepperStepConsoleUGUI.SetActive(_steps, stepsToActivate);
            }
        }

        public void Refresh()
        {
            updateValue(Value);
        }

        protected bool hasValidSteps()
        {
            if (_steps == null || _steps.Count != StepCount)
                return false;

            // Check for user deleted steps in the editor.
#if UNITY_EDITOR
            if (_steps.Count > 0 && _steps[0] == null || _steps[0].gameObject == null)
                return false;
#endif

            return true;
        }

        protected void updateText(string text)
        {
            TextTf.text = text;
        }

        public void OnEnable()
        {
            EnableButtonControls = _enableButtonControls;
            updateText(Text);
            updateValue(Value);
            updateButtons();
        }

        public virtual void Update()
        {
            if (EnableButtonControls && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == Selectable.gameObject)
            {
                if (InputUtils.LeftPressed())
                    Decrease();
                else if (InputUtils.RightPressed())
                    Increase();
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

        public int GetStepToDisplay(float value)
        {
            if (value > MaxValue - StepSize * 0.5f)
            {
                int steps = Mathf.CeilToInt((MaxValue - MinValue) / StepSize);
                return steps;
            }
            else
                return Mathf.RoundToInt((value - MinValue + 0.499f * StepSize) / StepSize);
        }

        public void Increase()
        {
            Step(1);
        }

        public void IncreaseLooped()
        {
            if (_value > MaxValue - StepSize * 0.1f)
            {
                Step(-Mathf.CeilToInt(StepCountFloat));
            }
            else
            {
                Increase();
            }
        }

        public void Decrease()
        {
            Step(-1);
        }

        public void Step(int steps)
        {
            Value = Mathf.Clamp(_value + steps * StepSize, MinValue, MaxValue);
            
            if(steps != 0)
            {
                OnValueChangedEvent?.Invoke(Value);
                OnValueChanged?.Invoke(Value);
            }
        }

        protected void updateButtons()
        {
            if (!DisableButtons)
            {
                if (DecreaseButton != null) DecreaseButton.enabled = true;
                if (IncreaseButton != null) IncreaseButton.enabled = true;
                return;
            }

            bool enableIncrease = Mathf.Abs(_value - MaxValue) > float.Epsilon;
            if (IncreaseButton != null) IncreaseButton.interactable = enableIncrease;

            bool enableDecrease = Mathf.Abs(_value - MinValue) > float.Epsilon;
            if (DecreaseButton != null) DecreaseButton.interactable = enableDecrease;
        }

        // Used by the prev and next click target (they block the button, therefore
        // we have to select it manually (see the inspector OnClick on the prefab).
        public void SetSelected()
        {
            if (Selectable != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(Selectable.gameObject);
        }

#if UNITY_EDITOR
        public void _EditorUpdateValue()
        {
            updateValue(Value);
        }
#endif
    }
}
