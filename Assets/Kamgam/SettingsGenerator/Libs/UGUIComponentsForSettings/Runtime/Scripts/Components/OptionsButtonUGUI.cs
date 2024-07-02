using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class OptionsButtonUGUI : MonoBehaviour
    {
        public static string UndefinedText = "-";

        public TextMeshProUGUI TextTf;

        [Tooltip("Loop the options if either of the ends is reached?")]
        public bool Loop = true;

        [SerializeField]
        [Tooltip("If enabled and if this is selected (has focus) then the prev/next action will be triggered by keyboard/controller navigation too.\n" +
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

        public delegate void OnValueChangedDelegate(int optionIndex);

        /// <summary>
        /// Set this to override the default option to text conversion.<br />
        /// Useful for text modifications (translations, sprite inserts, ...).
        /// </summary>
        public System.Func<string, string> OptionToTextFunc;

        /// <summary>
        /// The index of the selected option.
        /// </summary>
        public UnityEvent<int> OnValueChangedEvent;
        public OnValueChangedDelegate OnValueChanged;

        [SerializeField]
        protected List<string> _options = new List<string>();

        /// <summary>
        /// Cache for the output of GetOptions().
        /// </summary>
        protected List<string> _getOptionsCache = new List<string>();

        protected int _value;
        public int SelectedIndex
        {
            get => _value;
            set
            {
                if (value == _value)
                    return;

                if (_options == null || _options.Count == 0)
                {
                    _value = 0;
                    return;
                }

                _value = value % _options.Count;
                if (_value < 0)
                    _value = _options.Count + _value;
                UpdateText();

                OnValueChangedEvent?.Invoke(_value);
                OnValueChanged?.Invoke(_value);
            }
        }

        public int NumOfOptions => _options.Count;

        public void Start()
        {
            EnableButtonControls = _enableButtonControls;
            UpdateText();
        }

        public virtual void Update()
        {
            if (EnableButtonControls && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == Selectable.gameObject)
            {
                if (InputUtils.LeftPressed())
                    Prev();
                else if (InputUtils.RightPressed())
                    Next();
            }
        }

        public void SetOptions(IList<string> options)
        {
            _options.Clear();
            _options.AddRange(options);

            UpdateText();
        }

        public List<string> GetOptions()
        {
            _getOptionsCache.Clear();
            foreach (var option in _options)
            {
                _getOptionsCache.Add(option);
            }
            return _getOptionsCache;
        }

        public void UpdateText()
        {
            if (_options.Count == 0 || _options.Count >= _value)
                TextTf.text = UndefinedText;

            if (OptionToTextFunc == null)
                TextTf.text = _options[_value];
            else
                TextTf.text = OptionToTextFunc(_options[_value]);
        }

        public void ClearOptions()
        {
            _options.Clear();
            UpdateText();
        }

        public void Prev()
        {
            if (_options.Count == 0)
                return;

            if (SelectedIndex == 0 && !Loop)
                return;

            SelectedIndex = SelectedIndex - 1;
        }

        public void Next()
        {
            if (_options.Count == 0)
                return;

            if (SelectedIndex == NumOfOptions-1 && !Loop)
                return;

            SelectedIndex = SelectedIndex + 1;
        }

        // Used by the prev and next click target (they block the button, therefore
        // we have to select it manually (see the inspector OnClick on the prefab).
        public void SetSelected()
        {
            if (Selectable != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(Selectable.gameObject);
        }
    }
}
