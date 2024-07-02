using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class ToggleUGUI : MonoBehaviour
    {
        public TextMeshProUGUI TextTf;
        public Toggle Toggle;

        public bool Value
        {
            get => Toggle.isOn;
            set => Toggle.isOn = value;
        }

        public string Text
        {
            get => TextTf.text;
            set
            {
                if (value == Text)
                    return;

                TextTf.text = value;
            }
        }

        [SerializeField]
        public Toggle.ToggleEvent OnValueChangedEvent;

        public delegate void ValueChangedDelegate(bool value);
        public ValueChangedDelegate OnValueChanged;

        public void Start()
        {
            Toggle.onValueChanged.AddListener(onValueChanged);
        }

        private void onValueChanged(bool isOn)
        {
            OnValueChangedEvent?.Invoke(isOn);
            OnValueChanged?.Invoke(isOn);
        }

#if UNITY_EDITOR
        public void Reset()
        {
            if (Toggle == null && !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Toggle = GetComponentInChildren<Toggle>(includeInactive: true);
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.EditorUtility.SetDirty(this.gameObject);
            }
        }
#endif
    }
}
