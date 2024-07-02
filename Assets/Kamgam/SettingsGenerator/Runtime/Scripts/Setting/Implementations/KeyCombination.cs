using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public struct KeyCombination
    {
        /// <summary>
        /// A universal key code that covers the old AND the new input system.<br />
        /// NOTICE: There is a InputUtils class to convert from universal (UniversalKeyCode) to old key codes (KeyCode).
        /// </summary>
        public UniversalKeyCode Key;

        /// <summary>
        /// Used for modifier keys (CTRL, ALT, SHIFT, TAB) in key combinations.
        /// </summary>
        public UniversalKeyCode ModifierKey;

        public KeyCombination(UniversalKeyCode key)
        {
            Key = key;
            ModifierKey = UniversalKeyCode.None;
        }

        public KeyCombination(UniversalKeyCode key, UniversalKeyCode modifierKey)
        {
            Key = key;
            ModifierKey = modifierKey;
        }

        public bool Equals(KeyCombination combination)
        {
            return Key == combination.Key && ModifierKey == combination.ModifierKey;
        }

        public override string ToString()
        {
            return $"KeyCombination: (mod: {ModifierKey}, key: {Key})";
        }

#if !ENABLE_INPUT_SYSTEM
        public UnityEngine.KeyCode GetKeyCode()
        {
            return InputUtils.UniversalKeyCodeToKeyCode(Key);
        }

        public UnityEngine.KeyCode GetModifierKeyCode()
        {
            return InputUtils.UniversalKeyCodeToKeyCode(ModifierKey);
        }
#endif
    }
}
