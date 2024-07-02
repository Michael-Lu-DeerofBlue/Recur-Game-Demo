using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class DropDownUGUI : MonoBehaviour
    {
        public delegate void OnSelectionChangedDelegate(int optionIndex);

        /// <summary>
        /// The index of the selected option.
        /// </summary>
        public UnityEvent<int> OnSelectionChangedEvent;
        public OnSelectionChangedDelegate OnSelectionChanged;

        public TMP_Dropdown DropDown;

        /// <summary>
        /// Cache for the output of GetOptions().
        /// </summary>
        protected List<string> _getOptionsCache = new List<string>();

        public int SelectedIndex
        {
            get => DropDown.value;
            set
            {
                if (DropDown.value == value)
                    return;

                DropDown.value = value;
            }
        }

        public void Start()
        {
            DropDown.onValueChanged.AddListener(onValueChanged);
        }

        protected void onValueChanged(int index)
        {
            OnSelectionChangedEvent?.Invoke(index);
            OnSelectionChanged?.Invoke(index);
        }

        public void SetOptions(IList<string> options)
        {
            if (options == null || options.Count == 0)
                return;

            var tmpOptions = new List<TMP_Dropdown.OptionData>();
            foreach (var option in options)
            {
                tmpOptions.Add(new TMP_Dropdown.OptionData(option));
            }

            DropDown.ClearOptions();
            DropDown.AddOptions(tmpOptions);
        }

        public List<string> GetOptions()
        {
            _getOptionsCache.Clear();
            foreach (var option in DropDown.options)
            {
                _getOptionsCache.Add(option.text);
            }
            return _getOptionsCache;
        }

        public void ClearOptions()
        {
            DropDown.ClearOptions();
        }

        public void AddOptions(List<Sprite> options)
        {
            AddOptions(options);
        }

        public void AddOptions(List<string> options)
        {
            AddOptions(options);
        }

        public void AddOptions(List<TMP_Dropdown.OptionData> options)
        {
            DropDown.AddOptions(options);
        }

#if UNITY_EDITOR
        public void Reset()
        {
            if (DropDown == null && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                DropDown = GetComponentInChildren<TMP_Dropdown>(includeInactive: true);
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.EditorUtility.SetDirty(this.gameObject);
            }
        }
#endif
    }
}
