using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.UGUIComponentsForSettings
{
    public class InputAxisButtonEvents : MonoBehaviour
    {
        // The new Input system has this built in with GamePad.* .

#if !ENABLE_INPUT_SYSTEM

        // These axis are added automatically by the InputAxisButtonEventsEditor class.
        // They are interpreted according to the windows setup described here:
        // https://answers.unity.com/storage/temp/134371-xbox-one-controller-unity-windows-macos.jpg
        public const string OldInputAxisX = "KamgamInputAxisX"; // Left Stick horizontal
        public const string OldInputAxisY = "KamgamInputAxisY"; // Left Stick vertical
        public const string OldInputAxis4 = "KamgamInputAxis4"; // Right Stick horizontal
        public const string OldInputAxis5 = "KamgamInputAxis5"; // Right Stick vertical
        public const string OldInputAxis6 = "KamgamInputAxis6"; // DPad horizontal
        public const string OldInputAxis7 = "KamgamInputAxis7"; // DPad vertical
        public const string OldInputAxis9 = "KamgamInputAxis9"; // DPad horizontal
        public const string OldInputAxis10 = "KamgamInputAxis10"; // DPad vertical

        private static InputAxisButtonEvents _instance;
        public static InputAxisButtonEvents Instance
        {
            get
            {
                if (!_instance)
                {
#if UNITY_EDITOR
                    // Keep the instance null outside of play mode to avoid leaking
                    // instances into the scene.
                    if (!UnityEditor.EditorApplication.isPlaying)
                    {
                        return null;
                    }
#endif

                    _instance = new GameObject().AddComponent<InputAxisButtonEvents>();
                    _instance.name = _instance.GetType().ToString();
#if UNITY_EDITOR
                    _instance.hideFlags = HideFlags.DontSave;
                    if (UnityEditor.EditorApplication.isPlaying)
                    {
#endif
                        DontDestroyOnLoad(_instance.gameObject);
#if UNITY_EDITOR
                    }
#endif
                }
                return _instance;
            }
        }

        float _previousDownDPadHorizontal = 0f;
        float _previousDownDPadVertical = 0f;
        float _previousDownLeftStickHorizontal = 0f;
        float _previousDownLeftStickVertical = 0f;
        float _previousDownRightStickHorizontal = 0f;
        float _previousDownRightStickVertical = 0f;
        float _previousDownLeftTrigger = 0f;
        float _previousDownRightTrigger = 0f;

        float _previousUpDPadHorizontal = 0f;
        float _previousUpDPadVertical = 0f;
        float _previousUpLeftStickHorizontal = 0f;
        float _previousUpLeftStickVertical = 0f;
        float _previousUpRightStickHorizontal = 0f;
        float _previousUpRightStickVertical = 0f;
        float _previousUpLeftTrigger = 0f;
        float _previousUpRightTrigger = 0f;

        List<UniversalKeyCode> downButtons = new List<UniversalKeyCode>(5);
        List<UniversalKeyCode> upButtons = new List<UniversalKeyCode>(5);
        List<UniversalKeyCode> heldButtons = new List<UniversalKeyCode>(5);

        protected int lastFrame = 0;

        public void Update()
        {
            updateStateIfNeeded();
        }

        protected void updateStateIfNeeded()
        {
            if (Time.frameCount - lastFrame <= 0)
                return;

            downButtons.Clear();
            upButtons.Clear();
            heldButtons.Clear();

            lastFrame = Time.frameCount;

            detectDown(OldInputAxisX, UniversalKeyCode.GamePadLeftStickLeft, UniversalKeyCode.GamePadLeftStickRight, ref _previousDownLeftStickHorizontal);
            detectDown(OldInputAxisY, UniversalKeyCode.GamePadLeftStickUp, UniversalKeyCode.GamePadLeftStickDown, ref _previousDownLeftStickVertical);
            detectDown(OldInputAxis4, UniversalKeyCode.GamePadRightStickLeft, UniversalKeyCode.GamePadRightStickRight, ref _previousDownRightStickHorizontal);
            detectDown(OldInputAxis5, UniversalKeyCode.GamePadRightStickUp, UniversalKeyCode.GamePadRightStickDown, ref _previousDownRightStickVertical);
            detectDown(OldInputAxis6, UniversalKeyCode.GamePadDPadLeft, UniversalKeyCode.GamePadDPadRight, ref _previousDownDPadHorizontal);
            detectDown(OldInputAxis7, UniversalKeyCode.GamePadDPadDown, UniversalKeyCode.GamePadDPadUp, ref _previousDownDPadVertical);
            detectDown(OldInputAxis9, UniversalKeyCode.None, UniversalKeyCode.GamePadLeftTrigger, ref _previousDownLeftTrigger);
            detectDown(OldInputAxis10, UniversalKeyCode.None, UniversalKeyCode.GamePadRightTrigger, ref _previousDownRightTrigger);

            foreach (var downBtn in downButtons)
            {
                if (!heldButtons.Contains(downBtn))
                    heldButtons.Add(downBtn);
            }

            detectUp(OldInputAxisX, UniversalKeyCode.GamePadLeftStickLeft, UniversalKeyCode.GamePadLeftStickRight, ref _previousUpLeftStickHorizontal);
            detectUp(OldInputAxisY, UniversalKeyCode.GamePadLeftStickUp, UniversalKeyCode.GamePadLeftStickDown, ref _previousUpLeftStickVertical);
            detectUp(OldInputAxis4, UniversalKeyCode.GamePadRightStickLeft, UniversalKeyCode.GamePadRightStickRight, ref _previousUpRightStickHorizontal);
            detectUp(OldInputAxis5, UniversalKeyCode.GamePadRightStickUp, UniversalKeyCode.GamePadRightStickDown, ref _previousUpRightStickVertical);
            detectUp(OldInputAxis6, UniversalKeyCode.GamePadDPadLeft, UniversalKeyCode.GamePadDPadRight, ref _previousUpDPadHorizontal);
            detectUp(OldInputAxis7, UniversalKeyCode.GamePadDPadDown, UniversalKeyCode.GamePadDPadUp, ref _previousUpDPadVertical);
            detectUp(OldInputAxis9, UniversalKeyCode.None, UniversalKeyCode.GamePadLeftTrigger, ref _previousUpLeftTrigger);
            detectUp(OldInputAxis10, UniversalKeyCode.None, UniversalKeyCode.GamePadRightTrigger, ref _previousUpRightTrigger);

            foreach (var upBtn in upButtons)
            {
                heldButtons.Remove(upBtn);
            }
        }

        protected void addIfNotNone(List<UniversalKeyCode> buttons, UniversalKeyCode button)
        {
            if (button == UniversalKeyCode.None)
                return;

            buttons.Add(button);
        }

        public static bool GetButtonDown(UniversalKeyCode button)
        {
            if (Instance == null)
                return false;

            Instance.updateStateIfNeeded();
            return Instance.downButtons.Contains(button);
        }

        public static bool GetButtonUp(UniversalKeyCode button)
        {
            if (Instance == null)
                return false;

            Instance.updateStateIfNeeded();
            return Instance.upButtons.Contains(button);
        }

        public static bool GetButton(UniversalKeyCode button)
        {
            if (Instance == null)
                return false;

            Instance.updateStateIfNeeded();
            return Instance.heldButtons.Contains(button);
        }

        public static bool AnyButton()
        {
            if (Instance == null)
                return false;

            Instance.updateStateIfNeeded();
            return Instance.downButtons.Count > 0 || Instance.downButtons.Count > 0;
        }

        public static bool AnyButtonDown()
        {
            if (Instance == null)
                return false;

            Instance.updateStateIfNeeded();
            return Instance.downButtons.Count > 0;
        }

        public static UniversalKeyCode FirstButtonDown()
        {
            if (Instance == null)
                return UniversalKeyCode.None;

            Instance.updateStateIfNeeded();
            if (Instance.downButtons.Count > 0)
                return Instance.downButtons[0];
            else
                return UniversalKeyCode.None;
        }

        public static bool AnyButtonUp()
        {
            if (Instance == null)
                return false;

            Instance.updateStateIfNeeded();
            return Instance.upButtons.Count > 0;
        }

        public static UniversalKeyCode FirstButtonUp()
        {
            if (Instance == null)
                return UniversalKeyCode.None;

            Instance.updateStateIfNeeded();
            if (Instance.upButtons.Count > 0)
                return Instance.upButtons[0];
            else
                return UniversalKeyCode.None;
        }

        void detectDown(string axisName, UniversalKeyCode keyCodeNegative, UniversalKeyCode keyCodePositive, ref float previousValue, float threshold = 0.5f)
        {
            float value = Input.GetAxis(axisName);
            if (value <= -threshold && previousValue > -threshold)
            {
                previousValue = value;
                if (!downButtons.Contains(keyCodeNegative) && keyCodeNegative != UniversalKeyCode.None)
                    downButtons.Add(keyCodeNegative);
            }
            else if (value >= threshold && previousValue < threshold)
            {
                previousValue = value;
                if (!downButtons.Contains(keyCodePositive) && keyCodePositive != UniversalKeyCode.None)
                    downButtons.Add(keyCodePositive);
            }
            else
            {
                previousValue = value;
                downButtons.Remove(keyCodePositive);
                downButtons.Remove(keyCodeNegative);
            }
        }

        void detectUp(string axisName, UniversalKeyCode keyCodeNegative, UniversalKeyCode keyCodePositive, ref float previousValue, float threshold = 0.5f)
        {
            float value = Input.GetAxis(axisName);
            if (value >= -threshold && previousValue < -threshold)
            {
                previousValue = value;
                if (!upButtons.Contains(keyCodeNegative) && keyCodeNegative != UniversalKeyCode.None)
                    upButtons.Add(keyCodeNegative);
            }
            else if (value <= threshold && previousValue > threshold)
            {
                previousValue = value;
                if (!upButtons.Contains(keyCodePositive) && keyCodePositive != UniversalKeyCode.None)
                    upButtons.Add(keyCodePositive);
            }
            else
            {
                previousValue = value;
                upButtons.Remove(keyCodePositive);
                upButtons.Remove(keyCodeNegative);
            }
        }
#endif
    }
}
