using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    [SelectionBase]
    public class InputKeyUGUI : MonoBehaviour
    {
        public delegate void OnChangedDelegate(UniversalKeyCode key, UniversalKeyCode modifierKey);

        /// <summary>
        /// Set this to override the default KeyCode to KeyName conversion.<br />
        /// Useful for text modifications (translations, sprite inserts, ...).
        /// </summary>
        public System.Func<UniversalKeyCode, string> KeyCodeToKeyNameFunc;

        [SerializeField]
        protected UniversalKeyCode _key = UniversalKeyCode.None;
        public UniversalKeyCode Key
        {
            get => _key;
            set
            {
                if (value == _key)
                    return;

                _key = value;
                UpdateKeyName();
            }
        }

        [SerializeField]
        protected UniversalKeyCode _modifierKey = UniversalKeyCode.None;
        public UniversalKeyCode ModifierKey
        {
            get => _modifierKey;
            set
            {
                if (value == _modifierKey)
                    return;

                _modifierKey = value;
                UpdateKeyName();
            }
        }

        public bool AllowMouseButtons = false;
        public bool AllowKeyCombinations = false;
        public bool AllowAbortWithCancelButton = false;

        /// <summary>
        /// The first key code is the normal key (like A, SPACE, ENTER, ...). The second key code is the modifier key (CTRL, SHIFT, COMMAND or TAB).
        /// </summary>
        public UnityEvent<UniversalKeyCode, UniversalKeyCode> OnChangedEvent;
        public OnChangedDelegate OnChanged;

        public Button Button;
        public GameObject Normal;
        public GameObject Active;
        public TextMeshProUGUI TextTf;
        public TextMeshProUGUI KeyNameTf;
        public TextMeshProUGUI ActiveTextTf;

        public bool IsActive => Active.activeSelf;

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

        public string KeyName
        {
            get => KeyNameTf.text;
            set
            {
                if (value == KeyName)
                    return;

                KeyNameTf.text = value;
            }
        }

        public string ActiveText
        {
            get => ActiveTextTf.text;
            set
            {
                if (value == ActiveText)
                    return;

                ActiveTextTf.text = value;
            }
        }

        protected bool waitForKeyRelease;

        public void SetActive(bool active)
        {
            bool willBeActivated = IsActive != active && active;
            bool willBeDeactivated = IsActive != active && !active;

            // Fix stuck key in new input system after tabbing in and out.
#if ENABLE_INPUT_SYSTEM
            if (willBeActivated)
            {
                // See: https://forum.unity.com/threads/after-alt-tabbing-the-left-alt-key-state-is-faulty.1367766/
                InputUtils.ResetStuckKeyStates();
            }
#endif

            // Submit is triggered in onKeyDOWN, thus it is possible that
            // a key is still being held down when this happends.
            // To avoid this key to trigger the key selection (which happens
            // onKeyUP) we have to remember to ignore this key.
            if (willBeActivated && InputUtils.AnyKey())
            {
                waitForKeyRelease = true;
            }

            // Reselect Button after input was finished.
            if(willBeDeactivated && EventSystem.current != null)
            {
                SelectionUtils.SetSelected(Button.gameObject);
            }

            Normal.SetActive(!active);
            Active.SetActive(active);
            Button.interactable = !active;

            if (active)
            {
                _modifierKeyWhileActive = UniversalKeyCode.None;
                _keyWhileActive = UniversalKeyCode.None;
                _aKeyWasPressedWhileActive = false;
            }
        }

        public void UpdateKeyName()
        {
            if (ModifierKey != UniversalKeyCode.None)
            {
                if(KeyCodeToKeyNameFunc == null)
                    KeyName = InputUtils.UniversalKeyName(ModifierKey) + " + " + InputUtils.UniversalKeyName(Key);
                else
                    KeyName = KeyCodeToKeyNameFunc(ModifierKey) + " + " + KeyCodeToKeyNameFunc(Key);
            }
            else
            {
                if (KeyCodeToKeyNameFunc == null)
                    KeyName = InputUtils.UniversalKeyName(Key);
                else
                    KeyName = KeyCodeToKeyNameFunc(Key);
            }
        }

        public bool IsCancelKeyPressed()
        {
            return InputUtils.CancelDown();
        }

        public void OnEnable()
        {
            Refresh();
        }

        public void OnDisable()
        {
            if (IsActive)
            {
                waitForKeyRelease = false;
                SetActive(false);
            }
        }

        public void Refresh()
        {
            UpdateKeyName();
        }

        protected UniversalKeyCode _modifierKeyWhileActive;
        protected UniversalKeyCode _keyWhileActive;
        protected bool _aKeyWasPressedWhileActive;

        public void Update()
        {
            if (!InputUtils.AnyKey())
            {
                waitForKeyRelease = false;
            }

            if (IsActive && !waitForKeyRelease)
            {
                if (AllowAbortWithCancelButton && IsCancelKeyPressed())
                {
                    SetActive(false);
                }

                // If no more keys are pressed then analyze the results and end editing this key.
                bool keyPressStopped = InputUtils.GetUniversalKeyUp(excludeModifierKeys: false, excludeMouseButtons: true) != UniversalKeyCode.None;
                bool mouseClicked = InputUtils.MouseUp();
                if (_aKeyWasPressedWhileActive && (keyPressStopped || mouseClicked))
                {
                    SetActive(false);

                    // Don't set key if mouse was pressed yet mouse is ignored.
                    if (!mouseClicked || AllowMouseButtons)
                    {
                        // analyze pressed keys
                        if (_modifierKeyWhileActive != UniversalKeyCode.None && _keyWhileActive == UniversalKeyCode.None)
                        {
                            ModifierKey = UniversalKeyCode.None;
                            Key = _modifierKeyWhileActive;
                        }
                        else
                        {
                            if (AllowKeyCombinations)
                            {
                                ModifierKey = _modifierKeyWhileActive;
                            } else {
                                ModifierKey = UniversalKeyCode.None;
                            }
                            Key = _keyWhileActive;
                        }

                        OnChanged?.Invoke(Key, ModifierKey);
                        OnChangedEvent.Invoke(Key, ModifierKey);
                    }
                }

                if (InputUtils.AnyKeyDown())
                {
                    // record pressed keys only if they are not NONE.
                    _aKeyWasPressedWhileActive = true;
                    var mKey = InputUtils.GetModifierKeyDown();
                    if (mKey != UniversalKeyCode.None)
                        _modifierKeyWhileActive = mKey;

                    var key = InputUtils.GetUniversalKeyDown(excludeModifierKeys: true, !AllowMouseButtons);
                    if (key != UniversalKeyCode.None)
                        _keyWhileActive = key;
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            UpdateKeyName();
        }
#endif
    }
}
