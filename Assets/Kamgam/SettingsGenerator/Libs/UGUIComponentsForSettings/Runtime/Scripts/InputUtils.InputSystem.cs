using UnityEngine;
using System.Linq;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
#endif

namespace Kamgam.UGUIComponentsForSettings
{
    public static partial class InputUtils
    {
        // For the OLD InputSystem implementation see
        // InputUtils.Input.cs

#if ENABLE_INPUT_SYSTEM

        // Methods available in both

        public static Key[] Keys;

        static void buildKeyCache()
        {
            if (Keys == null)
            {
                Keys = Keyboard.current.allKeys.Select(k => k.keyCode).ToArray();
            }
        }

        public static void ResetStuckKeyStates()
        {
            // We have to reset leftAlt manually after tabbing out and back in.
            // See: https://forum.unity.com/threads/after-alt-tabbing-the-left-alt-key-state-is-faulty.1367766/
            UnityEngine.InputSystem.Keyboard.current.leftAltKey.QueueValueChange(0f);
            UnityEngine.InputSystem.Keyboard.current.tabKey.QueueValueChange(0f);
        }

        public static bool AnyKey()
        {
            if (Mouse.current != null &&
                (Mouse.current.leftButton.isPressed
                || Mouse.current.middleButton.isPressed
                || Mouse.current.rightButton.isPressed
                || Mouse.current.forwardButton.isPressed
                || Mouse.current.backButton.isPressed)
                )
            {
                return true;
            }

            if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
            {
                return true;
            }

            if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonSouth.isPressed
                 || Gamepad.current.buttonEast.isPressed
                 || Gamepad.current.buttonWest.isPressed
                 || Gamepad.current.buttonNorth.isPressed
                 || Gamepad.current.leftShoulder.isPressed
                 || Gamepad.current.rightShoulder.isPressed
                 || Gamepad.current.selectButton.isPressed
                 || Gamepad.current.startButton.isPressed
                 || Gamepad.current.leftStickButton.isPressed
                 || Gamepad.current.leftStick.left.isPressed
                 || Gamepad.current.leftStick.right.isPressed
                 || Gamepad.current.leftStick.up.isPressed
                 || Gamepad.current.leftStick.down.isPressed
                 || Gamepad.current.rightStickButton.isPressed
                 || Gamepad.current.rightStick.left.isPressed
                 || Gamepad.current.rightStick.right.isPressed
                 || Gamepad.current.rightStick.up.isPressed
                 || Gamepad.current.rightStick.down.isPressed
                 || Gamepad.current.leftTrigger.isPressed
                 || Gamepad.current.rightTrigger.isPressed
                 || Gamepad.current.dpad.left.isPressed
                 || Gamepad.current.dpad.right.isPressed
                 || Gamepad.current.dpad.up.isPressed
                 || Gamepad.current.dpad.down.isPressed)
                {
                    return true;
                }
            }

            if (Joystick.current != null && Joystick.current.wasUpdatedThisFrame)
            {
                var controls = Joystick.current.allControls;
                for (int i = 0; i < controls.Count; i++)
                {
                    var control = controls[i];

                    if (!(control is ButtonControl button))
                        continue;

                    if (button.isPressed)
                    {
                        return true;
                    }
                }
            }

            

            return false;
        }

        /// <summary>
        /// Returns true the first frame the user hits any key or mouse button.
        /// </summary>
        /// <returns></returns>
        public static bool AnyKeyDown()
        {
            if (Mouse.current != null &&
                (Mouse.current.leftButton.wasPressedThisFrame
                || Mouse.current.middleButton.wasPressedThisFrame
                || Mouse.current.rightButton.wasPressedThisFrame
                || Mouse.current.forwardButton.wasPressedThisFrame
                || Mouse.current.backButton.wasPressedThisFrame)
                )
            {
                return true;
            }

            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null)
            {
                if (Gamepad.current.buttonSouth.wasPressedThisFrame
                 || Gamepad.current.buttonEast.wasPressedThisFrame
                 || Gamepad.current.buttonWest.wasPressedThisFrame
                 || Gamepad.current.buttonNorth.wasPressedThisFrame
                 || Gamepad.current.leftShoulder.wasPressedThisFrame
                 || Gamepad.current.rightShoulder.wasPressedThisFrame
                 || Gamepad.current.selectButton.wasPressedThisFrame
                 || Gamepad.current.startButton.wasPressedThisFrame
                 || Gamepad.current.leftStickButton.wasPressedThisFrame
                 || Gamepad.current.leftStick.left.wasPressedThisFrame
                 || Gamepad.current.leftStick.right.wasPressedThisFrame
                 || Gamepad.current.leftStick.up.wasPressedThisFrame
                 || Gamepad.current.leftStick.down.wasPressedThisFrame
                 || Gamepad.current.rightStickButton.wasPressedThisFrame
                 || Gamepad.current.rightStick.left.wasPressedThisFrame
                 || Gamepad.current.rightStick.right.wasPressedThisFrame
                 || Gamepad.current.rightStick.up.wasPressedThisFrame
                 || Gamepad.current.rightStick.down.wasPressedThisFrame
                 || Gamepad.current.leftTrigger.wasPressedThisFrame
                 || Gamepad.current.rightTrigger.wasPressedThisFrame
                 || Gamepad.current.dpad.left.wasPressedThisFrame
                 || Gamepad.current.dpad.right.wasPressedThisFrame
                 || Gamepad.current.dpad.up.wasPressedThisFrame
                 || Gamepad.current.dpad.down.wasPressedThisFrame)
                {
                    return true;
                }
            }

            if (Joystick.current != null && Joystick.current.wasUpdatedThisFrame)
            {
                var controls = Joystick.current.allControls;
                for (int i = 0; i < controls.Count; i++)
                {
                    var control = controls[i];

                    if (!(control is ButtonControl button))
                        continue;

                    if (button.wasPressedThisFrame)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true during the frame the user releases any mouse button.
        /// </summary>
        /// <returns></returns>
        public static bool MouseUp()
        {
            return
               Mouse.current != null
            &&
             (Mouse.current.leftButton.wasReleasedThisFrame
            || Mouse.current.middleButton.wasReleasedThisFrame
            || Mouse.current.rightButton.wasReleasedThisFrame
            || Mouse.current.forwardButton.wasReleasedThisFrame
            || Mouse.current.backButton.wasReleasedThisFrame);
        }

        public static bool SubmitDown()
        {
            if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool SubmitUp()
        {
            if (Keyboard.current != null && Keyboard.current.enterKey.wasReleasedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.buttonSouth.wasReleasedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool CancelDown()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool CancelUp()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.selectButton.wasReleasedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool AnyDirection()
        {
            if (Keyboard.current != null &&
                (Keyboard.current.downArrowKey.isPressed
                || Keyboard.current.upArrowKey.isPressed
                || Keyboard.current.leftArrowKey.isPressed
                || Keyboard.current.rightArrowKey.isPressed)
               )
            {
                return true;
            }

            if (Gamepad.current != null)
            {
                if (Gamepad.current.leftStick.left.isPressed
                 || Gamepad.current.leftStick.right.isPressed
                 || Gamepad.current.leftStick.up.isPressed
                 || Gamepad.current.leftStick.down.isPressed)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool UpPressed()
        {
            if (Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.leftStick.up.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool DownPressed()
        {
            if (Keyboard.current != null && Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.leftStick.down.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool LeftPressed()
        {
            if (Keyboard.current != null && Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.leftStick.left.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool RightPressed()
        {
            if (Keyboard.current != null && Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                return true;
            }

            if (Gamepad.current != null && Gamepad.current.leftStick.right.wasPressedThisFrame)
            {
                return true;
            }

            return false;
        }

        public static bool LeftMouse()
        {
            return Mouse.current != null && (
                   Mouse.current.leftButton.wasReleasedThisFrame
                || Mouse.current.leftButton.wasPressedThisFrame
                || Mouse.current.leftButton.isPressed
                );
        }

        /// <summary>
        /// Returns true during the frame the user starts pressing any modifier key (Shift, Tab, Control, Command or Alt).
        /// </summary>
        /// <returns></returns>
        public static UniversalKeyCode GetModifierKeyDown()
        {
            if (Keyboard.current[Key.LeftShift].wasPressedThisFrame)
                return UniversalKeyCode.LeftShift;
            if (Keyboard.current[Key.RightShift].wasPressedThisFrame)
                return UniversalKeyCode.RightShift;
            if (Keyboard.current[Key.Tab].wasPressedThisFrame)
                return UniversalKeyCode.Tab;
            if (Keyboard.current[Key.LeftCtrl].wasPressedThisFrame)
                return UniversalKeyCode.LeftControl;
            if (Keyboard.current[Key.RightCtrl].wasPressedThisFrame)
                return UniversalKeyCode.RightControl;
            if (Keyboard.current[Key.LeftCommand].wasPressedThisFrame)
                return UniversalKeyCode.LeftCommand;
            if (Keyboard.current[Key.RightCommand].wasPressedThisFrame)
                return UniversalKeyCode.RightCommand;
            if (Keyboard.current[Key.LeftAlt].wasPressedThisFrame)
                return UniversalKeyCode.LeftAlt;
            if (Keyboard.current[Key.RightAlt].wasPressedThisFrame)
                return UniversalKeyCode.RightAlt;

            return UniversalKeyCode.None;
        }

        /// <summary>
        /// Returns true if the given key was pressed down within the last frame.
        /// </summary>
        /// <returns></returns>
        public static bool GetUniversalKeyDown(UniversalKeyCode universalKeyCode)
        {
            return GetUniversalKeyDown(excludeModifierKeys: false, excludeMouseButtons: false) == universalKeyCode;
        }

        /// <summary>
        /// Get the UniversalKeyCode for the key/mouse/button that was pressed down within the last frame.
        /// </summary>
        /// <param name="excludeModifierKeys"></param>
        /// <param name="excludeMouseButtons"></param>
        /// <returns></returns>
        public static UniversalKeyCode GetUniversalKeyDown(bool excludeModifierKeys, bool excludeMouseButtons)
        {
            if (!excludeMouseButtons && Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame) return UniversalKeyCode.MouseLeft;
                if (Mouse.current.middleButton.wasPressedThisFrame) return UniversalKeyCode.MouseMiddle;
                if (Mouse.current.rightButton.wasPressedThisFrame) return UniversalKeyCode.MouseRight;
                if (Mouse.current.backButton.wasPressedThisFrame) return UniversalKeyCode.MouseBack;
                if (Mouse.current.forwardButton.wasPressedThisFrame) return UniversalKeyCode.MouseForward;
            }

            if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
            {
                buildKeyCache();
                foreach (KeyControl key in Keyboard.current.allKeys)
                {
                    if (excludeModifierKeys)
                    {
                        if (key.keyCode == Key.LeftShift
                            || key.keyCode == Key.RightShift
                            || key.keyCode == Key.Tab
                            || key.keyCode == Key.LeftCtrl
                            || key.keyCode == Key.RightCtrl
                            || key.keyCode == Key.LeftCommand
                            || key.keyCode == Key.RightCommand
                            || key.keyCode == Key.LeftAlt
                            || key.keyCode == Key.RightAlt
                            )
                        {
                            continue;
                        }
                    }

                    if (key.wasPressedThisFrame)
                    {
                        var universalKeyCode = KeyToUniversalKeyCode(key.keyCode);
                        return universalKeyCode;
                    }
                }
            }

            if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            {
                // Conversions based on the Windows Xbox Controller layout
                // See: https://answers.unity.com/storage/temp/134371-xbox-one-controller-unity-windows-macos.jpg
                if (Gamepad.current.buttonSouth.wasPressedThisFrame) return UniversalKeyCode.GamePadSouth;
                if (Gamepad.current.buttonEast.wasPressedThisFrame) return UniversalKeyCode.GamePadEast;
                if (Gamepad.current.buttonWest.wasPressedThisFrame) return UniversalKeyCode.GamePadWest;
                if (Gamepad.current.buttonNorth.wasPressedThisFrame) return UniversalKeyCode.GamePadNorth;
                if (Gamepad.current.leftShoulder.wasPressedThisFrame) return UniversalKeyCode.GamePadLeftShoulder;
                if (Gamepad.current.rightShoulder.wasPressedThisFrame) return UniversalKeyCode.GamePadRightShoulder;
                if (Gamepad.current.selectButton.wasPressedThisFrame) return UniversalKeyCode.GamePadSelect;
                if (Gamepad.current.startButton.wasPressedThisFrame) return UniversalKeyCode.GamePadStart;
                if (Gamepad.current.leftStickButton.wasPressedThisFrame) return UniversalKeyCode.GamePadLeftStickButton;
                if (Gamepad.current.leftStick.up.wasPressedThisFrame) return UniversalKeyCode.GamePadLeftStickUp;
                if (Gamepad.current.leftStick.down.wasPressedThisFrame) return UniversalKeyCode.GamePadLeftStickDown;
                if (Gamepad.current.leftStick.left.wasPressedThisFrame) return UniversalKeyCode.GamePadLeftStickLeft;
                if (Gamepad.current.leftStick.right.wasPressedThisFrame) return UniversalKeyCode.GamePadLeftStickRight;
                if (Gamepad.current.rightStickButton.wasPressedThisFrame) return UniversalKeyCode.GamePadRightStickButton;
                if (Gamepad.current.rightStick.up.wasPressedThisFrame) return UniversalKeyCode.GamePadRightStickUp;
                if (Gamepad.current.rightStick.down.wasPressedThisFrame) return UniversalKeyCode.GamePadRightStickDown;
                if (Gamepad.current.rightStick.left.wasPressedThisFrame) return UniversalKeyCode.GamePadRightStickLeft;
                if (Gamepad.current.rightStick.right.wasPressedThisFrame) return UniversalKeyCode.GamePadRightStickRight;
                // These are "new" as in the old system the triggers were axis.
                if (Gamepad.current.leftTrigger.wasPressedThisFrame) return UniversalKeyCode.GamePadLeftTrigger;
                if (Gamepad.current.rightTrigger.wasPressedThisFrame) return UniversalKeyCode.GamePadRightTrigger;
                // These are "new" as in the old system the dpad buttons were axis
                if (Gamepad.current.dpad.left.wasPressedThisFrame) return UniversalKeyCode.GamePadDPadLeft;
                if (Gamepad.current.dpad.right.wasPressedThisFrame) return UniversalKeyCode.GamePadDPadRight;
                if (Gamepad.current.dpad.up.wasPressedThisFrame) return UniversalKeyCode.GamePadDPadUp;
                if (Gamepad.current.dpad.down.wasPressedThisFrame) return UniversalKeyCode.GamePadDPadDown;
            }

            if (Joystick.current != null && Joystick.current.wasUpdatedThisFrame)
            {
                var controls = Joystick.current.allControls;
                for (int i = 0; i < controls.Count; i++)
                {
                    var control = controls[i];

                    if (!(control is ButtonControl button))
                        continue;

                    if (button.wasPressedThisFrame)
                    {
                        if (i == 0) return UniversalKeyCode.JoystickButton0;
                        if (i == 1) return UniversalKeyCode.JoystickButton1;
                        if (i == 2) return UniversalKeyCode.JoystickButton2;
                        if (i == 3) return UniversalKeyCode.JoystickButton3;
                        if (i == 4) return UniversalKeyCode.JoystickButton4;
                        if (i == 5) return UniversalKeyCode.JoystickButton5;
                        if (i == 6) return UniversalKeyCode.JoystickButton6;
                        if (i == 7) return UniversalKeyCode.JoystickButton7;
                        if (i == 8) return UniversalKeyCode.JoystickButton8;
                        if (i == 9) return UniversalKeyCode.JoystickButton9;
                        if (i == 10) return UniversalKeyCode.JoystickButton10;
                    }
                }
            }

            return UniversalKeyCode.None;
        }

        /// <summary>
        /// Returns true if the given key was released down within the last frame.
        /// </summary>
        /// <returns></returns>
        public static bool GetUniversalKeyUp(UniversalKeyCode universalKeyCode)
        {
            return GetUniversalKeyUp(excludeModifierKeys: false, excludeMouseButtons: false) == universalKeyCode;
        }

        /// <summary>
        /// Get the UniversalKeyCode for the key/mouse/button that was pressed down within the last frame.
        /// </summary>
        /// <param name="excludeModifierKeys"></param>
        /// <param name="excludeMouseButtons"></param>
        /// <returns></returns>
        public static UniversalKeyCode GetUniversalKeyUp(bool excludeModifierKeys, bool excludeMouseButtons)
        {
            if (!excludeMouseButtons && Mouse.current != null)
            {
                if (Mouse.current.leftButton.wasReleasedThisFrame) return UniversalKeyCode.MouseLeft;
                if (Mouse.current.middleButton.wasReleasedThisFrame) return UniversalKeyCode.MouseMiddle;
                if (Mouse.current.rightButton.wasReleasedThisFrame) return UniversalKeyCode.MouseRight;
                if (Mouse.current.backButton.wasReleasedThisFrame) return UniversalKeyCode.MouseBack;
                if (Mouse.current.forwardButton.wasReleasedThisFrame) return UniversalKeyCode.MouseForward;
            }

            if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
            {
                buildKeyCache();
                foreach (Key key in Keys)
                {
                    if (excludeModifierKeys)
                    {
                        if (Keyboard.current[Key.LeftShift].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.RightShift].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.Tab].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.LeftCtrl].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.RightCtrl].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.LeftCommand].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.RightCommand].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.LeftAlt].wasReleasedThisFrame) continue;
                        if (Keyboard.current[Key.RightAlt].wasReleasedThisFrame) continue;
                    }

                    if (Keyboard.current[key].wasReleasedThisFrame)
                    {
                        var universalKeyCode = KeyToUniversalKeyCode(key);
                        return universalKeyCode;
                    }
                }
            }

            if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            {
                // Conversions based on the Windows Xbox Controller layout
                // See: https://answers.unity.com/storage/temp/134371-xbox-one-controller-unity-windows-macos.jpg
                if (Gamepad.current.buttonSouth.wasReleasedThisFrame) return UniversalKeyCode.GamePadSouth;
                if (Gamepad.current.buttonEast.wasReleasedThisFrame) return UniversalKeyCode.GamePadEast;
                if (Gamepad.current.buttonWest.wasReleasedThisFrame) return UniversalKeyCode.GamePadWest;
                if (Gamepad.current.buttonNorth.wasReleasedThisFrame) return UniversalKeyCode.GamePadNorth;
                if (Gamepad.current.leftShoulder.wasReleasedThisFrame) return UniversalKeyCode.GamePadLeftShoulder;
                if (Gamepad.current.rightShoulder.wasReleasedThisFrame) return UniversalKeyCode.GamePadRightShoulder;
                if (Gamepad.current.selectButton.wasReleasedThisFrame) return UniversalKeyCode.GamePadSelect;
                if (Gamepad.current.startButton.wasReleasedThisFrame) return UniversalKeyCode.GamePadStart;
                if (Gamepad.current.leftStickButton.wasReleasedThisFrame) return UniversalKeyCode.GamePadLeftStickButton;
                if (Gamepad.current.leftStick.up.wasReleasedThisFrame) return UniversalKeyCode.GamePadLeftStickUp;
                if (Gamepad.current.leftStick.down.wasReleasedThisFrame) return UniversalKeyCode.GamePadLeftStickDown;
                if (Gamepad.current.leftStick.left.wasReleasedThisFrame) return UniversalKeyCode.GamePadLeftStickLeft;
                if (Gamepad.current.leftStick.right.wasReleasedThisFrame) return UniversalKeyCode.GamePadLeftStickRight;
                if (Gamepad.current.rightStickButton.wasReleasedThisFrame) return UniversalKeyCode.GamePadRightStickButton;
                if (Gamepad.current.rightStick.up.wasReleasedThisFrame) return UniversalKeyCode.GamePadRightStickUp;
                if (Gamepad.current.rightStick.down.wasReleasedThisFrame) return UniversalKeyCode.GamePadRightStickDown;
                if (Gamepad.current.rightStick.left.wasReleasedThisFrame) return UniversalKeyCode.GamePadRightStickLeft;
                if (Gamepad.current.rightStick.right.wasReleasedThisFrame) return UniversalKeyCode.GamePadRightStickRight;
                // These are "new" as in the old system the triggers were axis.
                if (Gamepad.current.leftTrigger.wasReleasedThisFrame) return UniversalKeyCode.GamePadLeftTrigger;
                if (Gamepad.current.rightTrigger.wasReleasedThisFrame) return UniversalKeyCode.GamePadRightTrigger;
                // These are "new" as in the old system the dpad buttons were axis
                if (Gamepad.current.dpad.left.wasReleasedThisFrame) return UniversalKeyCode.GamePadDPadLeft;
                if (Gamepad.current.dpad.right.wasReleasedThisFrame) return UniversalKeyCode.GamePadDPadRight;
                if (Gamepad.current.dpad.up.wasReleasedThisFrame) return UniversalKeyCode.GamePadDPadUp;
                if (Gamepad.current.dpad.down.wasReleasedThisFrame) return UniversalKeyCode.GamePadDPadDown;
            }

            if (Joystick.current != null && Joystick.current.wasUpdatedThisFrame)
            {
                var controls = Joystick.current.allControls;
                for (int i = 0; i < controls.Count; i++)
                {
                    var control = controls[i];

                    if (!(control is ButtonControl button))
                        continue;

                    if (button.wasReleasedThisFrame)
                    {
                        if (i == 0) return UniversalKeyCode.JoystickButton0;
                        if (i == 1) return UniversalKeyCode.JoystickButton1;
                        if (i == 2) return UniversalKeyCode.JoystickButton2;
                        if (i == 3) return UniversalKeyCode.JoystickButton3;
                        if (i == 4) return UniversalKeyCode.JoystickButton4;
                        if (i == 5) return UniversalKeyCode.JoystickButton5;
                        if (i == 6) return UniversalKeyCode.JoystickButton6;
                        if (i == 7) return UniversalKeyCode.JoystickButton7;
                        if (i == 8) return UniversalKeyCode.JoystickButton8;
                        if (i == 9) return UniversalKeyCode.JoystickButton9;
                        if (i == 10) return UniversalKeyCode.JoystickButton10;
                    }
                }
            }

            return UniversalKeyCode.None;
        }

        /// <summary>
        /// Returns true while the user holds down the given key.
        /// </summary>
        /// <param name="universalKeyCode"></param>
        /// <returns></returns>
        public static bool GetUniversalKey(UniversalKeyCode universalKeyCode)
        {
            return GetPressedUniversalKey(excludeModifierKeys: false, excludeMouseButtons: false) == universalKeyCode;
        }

        /// <summary>
        /// Returns the currently held down universal key code.
        /// </summary>
        /// <param name="universalKeyCode"></param>
        /// <returns></returns>
        public static UniversalKeyCode GetPressedUniversalKey(bool excludeModifierKeys, bool excludeMouseButtons)
        {
            if (!excludeMouseButtons && Mouse.current != null)
            {
                if (Mouse.current.leftButton.isPressed) return UniversalKeyCode.MouseLeft;
                if (Mouse.current.middleButton.isPressed) return UniversalKeyCode.MouseMiddle;
                if (Mouse.current.rightButton.isPressed) return UniversalKeyCode.MouseRight;
                if (Mouse.current.backButton.isPressed) return UniversalKeyCode.MouseBack;
                if (Mouse.current.forwardButton.isPressed) return UniversalKeyCode.MouseForward;
            }

            if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
            {
                buildKeyCache();
                foreach (Key key in Keys)
                {
                    if (excludeModifierKeys)
                    {
                        if (Keyboard.current[Key.LeftShift].isPressed) continue;
                        if (Keyboard.current[Key.RightShift].isPressed) continue;
                        if (Keyboard.current[Key.Tab].isPressed) continue;
                        if (Keyboard.current[Key.LeftCtrl].isPressed) continue;
                        if (Keyboard.current[Key.RightCtrl].isPressed) continue;
                        if (Keyboard.current[Key.LeftCommand].isPressed) continue;
                        if (Keyboard.current[Key.RightCommand].isPressed) continue;
                        if (Keyboard.current[Key.LeftAlt].isPressed) continue;
                        if (Keyboard.current[Key.RightAlt].isPressed) continue;
                    }

                    if (Keyboard.current[key].isPressed)
                    {
                        var universalKeyCode = KeyToUniversalKeyCode(key);
                        return universalKeyCode;
                    }
                }
            }

            if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            {
                // Conversions based on the Windows Xbox Controller layout
                // See: https://answers.unity.com/storage/temp/134371-xbox-one-controller-unity-windows-macos.jpg
                if (Gamepad.current.buttonSouth.isPressed) return UniversalKeyCode.GamePadSouth;
                if (Gamepad.current.buttonEast.isPressed) return UniversalKeyCode.GamePadEast;
                if (Gamepad.current.buttonWest.isPressed) return UniversalKeyCode.GamePadWest;
                if (Gamepad.current.buttonNorth.isPressed) return UniversalKeyCode.GamePadNorth;
                if (Gamepad.current.leftShoulder.isPressed) return UniversalKeyCode.GamePadLeftShoulder;
                if (Gamepad.current.rightShoulder.isPressed) return UniversalKeyCode.GamePadRightShoulder;
                if (Gamepad.current.selectButton.isPressed) return UniversalKeyCode.GamePadSelect;
                if (Gamepad.current.startButton.isPressed) return UniversalKeyCode.GamePadStart;
                if (Gamepad.current.leftStickButton.isPressed) return UniversalKeyCode.GamePadLeftStickButton;
                if (Gamepad.current.leftStick.up.isPressed) return UniversalKeyCode.GamePadLeftStickUp;
                if (Gamepad.current.leftStick.down.isPressed) return UniversalKeyCode.GamePadLeftStickDown;
                if (Gamepad.current.leftStick.left.isPressed) return UniversalKeyCode.GamePadLeftStickLeft;
                if (Gamepad.current.leftStick.right.isPressed) return UniversalKeyCode.GamePadLeftStickRight;
                if (Gamepad.current.rightStickButton.isPressed) return UniversalKeyCode.GamePadRightStickButton;
                if (Gamepad.current.rightStick.up.isPressed) return UniversalKeyCode.GamePadRightStickUp;
                if (Gamepad.current.rightStick.down.isPressed) return UniversalKeyCode.GamePadRightStickDown;
                if (Gamepad.current.rightStick.left.isPressed) return UniversalKeyCode.GamePadRightStickLeft;
                if (Gamepad.current.rightStick.right.isPressed) return UniversalKeyCode.GamePadRightStickRight;
                // These are "new" as in the old system the triggers were axis.
                if (Gamepad.current.leftTrigger.isPressed) return UniversalKeyCode.GamePadLeftTrigger;
                if (Gamepad.current.rightTrigger.isPressed) return UniversalKeyCode.GamePadRightTrigger;
                // These are "new" as in the old system the dpad buttons were axis
                if (Gamepad.current.dpad.left.isPressed) return UniversalKeyCode.GamePadDPadLeft;
                if (Gamepad.current.dpad.right.isPressed) return UniversalKeyCode.GamePadDPadRight;
                if (Gamepad.current.dpad.up.isPressed) return UniversalKeyCode.GamePadDPadUp;
                if (Gamepad.current.dpad.down.isPressed) return UniversalKeyCode.GamePadDPadDown;
            }

            if (Joystick.current != null && Joystick.current.wasUpdatedThisFrame)
            {
                var controls = Joystick.current.allControls;
                for (int i = 0; i < controls.Count; i++)
                {
                    var control = controls[i];

                    if (!(control is ButtonControl button))
                        continue;

                    if (button.isPressed)
                    {
                        if (i == 0) return UniversalKeyCode.JoystickButton0;
                        if (i == 1) return UniversalKeyCode.JoystickButton1;
                        if (i == 2) return UniversalKeyCode.JoystickButton2;
                        if (i == 3) return UniversalKeyCode.JoystickButton3;
                        if (i == 4) return UniversalKeyCode.JoystickButton4;
                        if (i == 5) return UniversalKeyCode.JoystickButton5;
                        if (i == 6) return UniversalKeyCode.JoystickButton6;
                        if (i == 7) return UniversalKeyCode.JoystickButton7;
                        if (i == 8) return UniversalKeyCode.JoystickButton8;
                        if (i == 9) return UniversalKeyCode.JoystickButton9;
                        if (i == 10) return UniversalKeyCode.JoystickButton10;
                    }
                }
            }

            return UniversalKeyCode.None;
        }

        static Dictionary<UniversalKeyCode, string> keyNameDictionary = new Dictionary<UniversalKeyCode, string>();

        /// <summary>
        /// Retuns a human readable key name (it aims to return the text which is printed on the keyboard keys).
        /// Key codes are somewhat layout dependent (Z and Y switch correctly for EN / DE), others may not.
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="richText">Replace some keys with sprites?</param>
        /// <returns></returns>
        public static string UniversalKeyName(UniversalKeyCode keyCode)
        {
            // NOTICE: Sometimes in the editor the Keyboard.current is null (usually after a fresh start).

            // fill dictionary if necessary.
            if (keyNameDictionary.Count == 0)
            {
                keyNameDictionary.Add(UniversalKeyCode.None, "-");

                keyNameDictionary.Add(UniversalKeyCode.MouseLeft, "Left Mouse");
                keyNameDictionary.Add(UniversalKeyCode.MouseRight, "Right Mouse");
                keyNameDictionary.Add(UniversalKeyCode.MouseMiddle, "Middle Mouse");
                keyNameDictionary.Add(UniversalKeyCode.MouseBack, "Back Mouse");
                keyNameDictionary.Add(UniversalKeyCode.MouseForward, "Forward Mouse");

                keyNameDictionary.Add(UniversalKeyCode.Backspace, "Backspace");
                keyNameDictionary.Add(UniversalKeyCode.Tab, "Tab");
                keyNameDictionary.Add(UniversalKeyCode.Clear, "Clear");
                keyNameDictionary.Add(UniversalKeyCode.Return, "Return");
                keyNameDictionary.Add(UniversalKeyCode.Pause, "Pause");
                keyNameDictionary.Add(UniversalKeyCode.Escape, "Esc");
                keyNameDictionary.Add(UniversalKeyCode.Space, "Space");
                keyNameDictionary.Add(UniversalKeyCode.Exclaim, "!");
                keyNameDictionary.Add(UniversalKeyCode.DoubleQuote, "\"");
                keyNameDictionary.Add(UniversalKeyCode.Hash, "#");
                keyNameDictionary.Add(UniversalKeyCode.Dollar, "$");
                keyNameDictionary.Add(UniversalKeyCode.Percent, "%");
                keyNameDictionary.Add(UniversalKeyCode.Ampersand, "&");
                keyNameDictionary.Add(UniversalKeyCode.Quote, Keyboard.current == null ? "\"" : Keyboard.current.quoteKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.LeftParen, "(");
                keyNameDictionary.Add(UniversalKeyCode.RightParen, ")");
                keyNameDictionary.Add(UniversalKeyCode.Asterisk, "*");
                keyNameDictionary.Add(UniversalKeyCode.Plus, "+");
                keyNameDictionary.Add(UniversalKeyCode.Comma, Keyboard.current == null ? "," : Keyboard.current.commaKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Minus, Keyboard.current == null ? "-" : Keyboard.current.minusKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Period, Keyboard.current == null ? "." : Keyboard.current.periodKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Slash, Keyboard.current == null ? "/" : Keyboard.current.slashKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit0, Keyboard.current == null ? "0" : Keyboard.current.digit0Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit1, Keyboard.current == null ? "1" : Keyboard.current.digit1Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit2, Keyboard.current == null ? "2" : Keyboard.current.digit2Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit3, Keyboard.current == null ? "3" : Keyboard.current.digit3Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit4, Keyboard.current == null ? "4" : Keyboard.current.digit4Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit5, Keyboard.current == null ? "5" : Keyboard.current.digit5Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit6, Keyboard.current == null ? "6" : Keyboard.current.digit6Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit7, Keyboard.current == null ? "7" : Keyboard.current.digit7Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit8, Keyboard.current == null ? "8" : Keyboard.current.digit8Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Digit9, Keyboard.current == null ? "9" : Keyboard.current.digit9Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Colon, ":");
                keyNameDictionary.Add(UniversalKeyCode.Semicolon, Keyboard.current == null ? ";" : Keyboard.current.semicolonKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Less, "<");
                keyNameDictionary.Add(UniversalKeyCode.Equals, Keyboard.current == null ? "=" : Keyboard.current.equalsKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Greater, ">");
                keyNameDictionary.Add(UniversalKeyCode.Question, "?");
                keyNameDictionary.Add(UniversalKeyCode.At, "@");
                keyNameDictionary.Add(UniversalKeyCode.LeftBracket, Keyboard.current == null ? "(" : Keyboard.current.leftBracketKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Backslash, Keyboard.current == null ? "\\" : Keyboard.current.backslashKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.RightBracket, Keyboard.current == null ? ")" : Keyboard.current.rightBracketKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Caret, "^");
                keyNameDictionary.Add(UniversalKeyCode.Underscore, "_");
                keyNameDictionary.Add(UniversalKeyCode.BackQuote, Keyboard.current == null ? "\"" : Keyboard.current.backquoteKey.displayName);
                keyNameDictionary.Add(UniversalKeyCode.A, Keyboard.current == null ? "A" : Keyboard.current.aKey.name.ToUpper()); // Thanks to things like this: https://forum.unity.com/threads/uparrow-showing-display-name-as-num-8.1367685/
                keyNameDictionary.Add(UniversalKeyCode.B, Keyboard.current == null ? "B" : Keyboard.current.bKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.C, Keyboard.current == null ? "C" : Keyboard.current.cKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.D, Keyboard.current == null ? "D" : Keyboard.current.dKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.E, Keyboard.current == null ? "E" : Keyboard.current.eKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F, Keyboard.current == null ? "F" : Keyboard.current.fKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.G, Keyboard.current == null ? "G" : Keyboard.current.gKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.H, Keyboard.current == null ? "H" : Keyboard.current.hKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.I, Keyboard.current == null ? "I" : Keyboard.current.iKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.J, Keyboard.current == null ? "J" : Keyboard.current.jKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.K, Keyboard.current == null ? "K" : Keyboard.current.kKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.L, Keyboard.current == null ? "L" : Keyboard.current.lKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.M, Keyboard.current == null ? "M" : Keyboard.current.mKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.N, Keyboard.current == null ? "N" : Keyboard.current.nKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.O, Keyboard.current == null ? "O" : Keyboard.current.oKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.P, Keyboard.current == null ? "P" : Keyboard.current.pKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Q, Keyboard.current == null ? "Q" : Keyboard.current.qKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.R, Keyboard.current == null ? "R" : Keyboard.current.rKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.S, Keyboard.current == null ? "S" : Keyboard.current.sKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.T, Keyboard.current == null ? "T" : Keyboard.current.tKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.U, Keyboard.current == null ? "U" : Keyboard.current.uKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.V, Keyboard.current == null ? "V" : Keyboard.current.vKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.W, Keyboard.current == null ? "W" : Keyboard.current.wKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.X, Keyboard.current == null ? "X" : Keyboard.current.xKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Y, Keyboard.current == null ? "Y" : Keyboard.current.yKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Z, Keyboard.current == null ? "Z" : Keyboard.current.zKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.LeftCurlyBracket, "{");
                keyNameDictionary.Add(UniversalKeyCode.Pipe, "|");
                keyNameDictionary.Add(UniversalKeyCode.RightCurlyBracket, "}");
                keyNameDictionary.Add(UniversalKeyCode.Tilde, "~");
                keyNameDictionary.Add(UniversalKeyCode.Delete, Keyboard.current == null ? "Del" : Keyboard.current.deleteKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Numpad0, "0");
                keyNameDictionary.Add(UniversalKeyCode.Numpad1, "1");
                keyNameDictionary.Add(UniversalKeyCode.Numpad2, "2");
                keyNameDictionary.Add(UniversalKeyCode.Numpad3, "3");
                keyNameDictionary.Add(UniversalKeyCode.Numpad4, "4");
                keyNameDictionary.Add(UniversalKeyCode.Numpad5, "5");
                keyNameDictionary.Add(UniversalKeyCode.Numpad6, "6");
                keyNameDictionary.Add(UniversalKeyCode.Numpad7, "7");
                keyNameDictionary.Add(UniversalKeyCode.Numpad8, "8");
                keyNameDictionary.Add(UniversalKeyCode.Numpad9, "9");
                keyNameDictionary.Add(UniversalKeyCode.NumpadPeriod, ".");
                keyNameDictionary.Add(UniversalKeyCode.NumpadDivide, "/");
                keyNameDictionary.Add(UniversalKeyCode.NumpadMultiply, "*");
                keyNameDictionary.Add(UniversalKeyCode.NumpadMinus, "-");
                keyNameDictionary.Add(UniversalKeyCode.NumpadPlus, "+");
                keyNameDictionary.Add(UniversalKeyCode.NumpadEnter, "Enter");
                keyNameDictionary.Add(UniversalKeyCode.NumpadEquals, "=");
                keyNameDictionary.Add(UniversalKeyCode.UpArrow, "Up Arrow");
                keyNameDictionary.Add(UniversalKeyCode.DownArrow, "Down Arrow");
                keyNameDictionary.Add(UniversalKeyCode.RightArrow, "Right Arrow");
                keyNameDictionary.Add(UniversalKeyCode.LeftArrow, "Left Arrow");
                keyNameDictionary.Add(UniversalKeyCode.Insert, Keyboard.current == null ? "Ins" : Keyboard.current.insertKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.Home, "Home");
                keyNameDictionary.Add(UniversalKeyCode.End, "End");
                keyNameDictionary.Add(UniversalKeyCode.PageUp, Keyboard.current == null ? "PageUp" : Keyboard.current.pageUpKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.PageDown, Keyboard.current == null ? "PageDown" : Keyboard.current.pageDownKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F1, Keyboard.current == null ? "F1" : Keyboard.current.f1Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F2, Keyboard.current == null ? "F2" : Keyboard.current.f2Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F3, Keyboard.current == null ? "F3" : Keyboard.current.f3Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F4, Keyboard.current == null ? "F4" : Keyboard.current.f4Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F5, Keyboard.current == null ? "F5" : Keyboard.current.f5Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F6, Keyboard.current == null ? "F6" : Keyboard.current.f6Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F7, Keyboard.current == null ? "F7" : Keyboard.current.f7Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F8, Keyboard.current == null ? "F8" : Keyboard.current.f8Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F9, Keyboard.current == null ? "F9" : Keyboard.current.f9Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F10, Keyboard.current == null ? "F10" : Keyboard.current.f10Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F11, Keyboard.current == null ? "F11" : Keyboard.current.f11Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.F12, Keyboard.current == null ? "F12" : Keyboard.current.f12Key.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.NumLock, "NumLock");
                keyNameDictionary.Add(UniversalKeyCode.CapsLock, "CapsLock");
                keyNameDictionary.Add(UniversalKeyCode.ScrollLock, "ScrollLock");
                keyNameDictionary.Add(UniversalKeyCode.RightShift, "Shift");
                keyNameDictionary.Add(UniversalKeyCode.LeftShift, "Shift");
                keyNameDictionary.Add(UniversalKeyCode.RightControl, "Ctrl");
                keyNameDictionary.Add(UniversalKeyCode.LeftControl, "Ctrl");
                keyNameDictionary.Add(UniversalKeyCode.RightAlt, "Alt");
                keyNameDictionary.Add(UniversalKeyCode.LeftAlt, "Alt");
                keyNameDictionary.Add(UniversalKeyCode.RightCommand, "Cmd");
                keyNameDictionary.Add(UniversalKeyCode.LeftCommand, "Cmd");
                keyNameDictionary.Add(UniversalKeyCode.LeftWindows, "Win");
                keyNameDictionary.Add(UniversalKeyCode.RightWindows, Keyboard.current == null ? "Win" : Keyboard.current.rightWindowsKey.displayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.AltGr, "AltGr");
                keyNameDictionary.Add(UniversalKeyCode.Help, "Help");
                keyNameDictionary.Add(UniversalKeyCode.Print, "Print");
                keyNameDictionary.Add(UniversalKeyCode.SysReq, "SysReq");
                keyNameDictionary.Add(UniversalKeyCode.Break, "Break");
                keyNameDictionary.Add(UniversalKeyCode.Menu, "Menu");

                keyNameDictionary.Add(UniversalKeyCode.JoystickButton0, "Joy0");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton1, "Joy1");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton2, "Joy2");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton3, "Joy3");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton4, "Joy4");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton5, "Joy5");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton6, "Joy6");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton7, "Joy7");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton8, "Joy8");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton9, "Joy9");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton10, "Joy10");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton11, "Joy11");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton12, "Joy12");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton13, "Joy13");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton14, "Joy14");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton15, "Joy15");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton16, "Joy16");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton17, "Joy17");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton18, "Joy18");
                keyNameDictionary.Add(UniversalKeyCode.JoystickButton19, "Joy19");

                keyNameDictionary.Add(UniversalKeyCode.GamePadNorth,            Gamepad.current == null ? "Y" : Gamepad.current.buttonNorth.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadSouth,            Gamepad.current == null ? "A" : Gamepad.current.buttonSouth.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadWest,             Gamepad.current == null ? "X" : Gamepad.current.buttonWest.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadEast,             Gamepad.current == null ? "B" : Gamepad.current.buttonEast.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadStart,            "Start");
                keyNameDictionary.Add(UniversalKeyCode.GamePadSelect,           "Select");
                keyNameDictionary.Add(UniversalKeyCode.GamePadLeftShoulder,     Gamepad.current == null ? "LB" : Gamepad.current.leftShoulder.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadRightShoulder,    Gamepad.current == null ? "RB" : Gamepad.current.rightShoulder.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadLeftTrigger,      Gamepad.current == null ? "LT" : Gamepad.current.leftTrigger.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadRightTrigger,     Gamepad.current == null ? "RT" : Gamepad.current.rightTrigger.shortDisplayName.ToUpper());
                keyNameDictionary.Add(UniversalKeyCode.GamePadDPadUp,           "DPad Up");
                keyNameDictionary.Add(UniversalKeyCode.GamePadDPadDown,         "DPad Down");
                keyNameDictionary.Add(UniversalKeyCode.GamePadDPadLeft,         "DPad Left");
                keyNameDictionary.Add(UniversalKeyCode.GamePadDPadRight,        "DPad Right");
                keyNameDictionary.Add(UniversalKeyCode.GamePadLeftStickButton,  "Left Stick");
                keyNameDictionary.Add(UniversalKeyCode.GamePadRightStickButton, "Left Stick");

                keyNameDictionary.Add(UniversalKeyCode.GamePadLeftStickUp, "L-Stick Up");
                keyNameDictionary.Add(UniversalKeyCode.GamePadLeftStickDown, "L-Stick Down");
                keyNameDictionary.Add(UniversalKeyCode.GamePadLeftStickLeft, "L-Stick Left");
                keyNameDictionary.Add(UniversalKeyCode.GamePadLeftStickRight, "L-Stick Right");

                keyNameDictionary.Add(UniversalKeyCode.GamePadRightStickUp, "R-Stick Up");
                keyNameDictionary.Add(UniversalKeyCode.GamePadRightStickDown, "R-Stick Down");
                keyNameDictionary.Add(UniversalKeyCode.GamePadRightStickLeft, "R-Stick Left");
                keyNameDictionary.Add(UniversalKeyCode.GamePadRightStickRight, "R-Stick Right");
            }

            if (keyNameDictionary.ContainsKey(keyCode))
            {
                return keyNameDictionary[keyCode];
            }
            else
            {
                return keyCode.ToString();
            }
        }


        // Methods exclusive to the new InputSystem

        static List<(Key, UniversalKeyCode)> _inputSystemKeyMap = new List<(Key, UniversalKeyCode)>()
        {
                (Key.None              ,UniversalKeyCode.None                 ),
                (Key.Space             ,UniversalKeyCode.Space                ),
                (Key.Enter             ,UniversalKeyCode.Return               ),
                (Key.Tab               ,UniversalKeyCode.Tab                  ),
                (Key.Backquote         ,UniversalKeyCode.BackQuote            ),
                (Key.Quote             ,UniversalKeyCode.Quote                ),
                (Key.Semicolon         ,UniversalKeyCode.Semicolon            ),
                (Key.Comma             ,UniversalKeyCode.Comma                ),
                (Key.Period            ,UniversalKeyCode.Period               ),
                (Key.Slash             ,UniversalKeyCode.Slash                ),
                (Key.Backslash         ,UniversalKeyCode.Backslash            ),
                (Key.LeftBracket       ,UniversalKeyCode.LeftBracket          ),
                (Key.RightBracket      ,UniversalKeyCode.RightBracket         ),
                (Key.Minus             ,UniversalKeyCode.Minus                ),
                (Key.Equals            ,UniversalKeyCode.Equals               ),
                (Key.A                 ,UniversalKeyCode.A                    ),
                (Key.B                 ,UniversalKeyCode.B                    ),
                (Key.C                 ,UniversalKeyCode.C                    ),
                (Key.D                 ,UniversalKeyCode.D                    ),
                (Key.E                 ,UniversalKeyCode.E                    ),
                (Key.F                 ,UniversalKeyCode.F                    ),
                (Key.G                 ,UniversalKeyCode.G                    ),
                (Key.H                 ,UniversalKeyCode.H                    ),
                (Key.I                 ,UniversalKeyCode.I                    ),
                (Key.J                 ,UniversalKeyCode.J                    ),
                (Key.K                 ,UniversalKeyCode.K                    ),
                (Key.L                 ,UniversalKeyCode.L                    ),
                (Key.M                 ,UniversalKeyCode.M                    ),
                (Key.N                 ,UniversalKeyCode.N                    ),
                (Key.O                 ,UniversalKeyCode.O                    ),
                (Key.P                 ,UniversalKeyCode.P                    ),
                (Key.Q                 ,UniversalKeyCode.Q                    ),
                (Key.R                 ,UniversalKeyCode.R                    ),
                (Key.S                 ,UniversalKeyCode.S                    ),
                (Key.T                 ,UniversalKeyCode.T                    ),
                (Key.U                 ,UniversalKeyCode.U                    ),
                (Key.V                 ,UniversalKeyCode.V                    ),
                (Key.W                 ,UniversalKeyCode.W                    ),
                (Key.X                 ,UniversalKeyCode.X                    ),
                (Key.Y                 ,UniversalKeyCode.Y                    ),
                (Key.Z                 ,UniversalKeyCode.Z                    ),
                (Key.Digit0            ,UniversalKeyCode.Digit0               ),
                (Key.Digit1            ,UniversalKeyCode.Digit1               ),
                (Key.Digit2            ,UniversalKeyCode.Digit2               ),
                (Key.Digit3            ,UniversalKeyCode.Digit3               ),
                (Key.Digit4            ,UniversalKeyCode.Digit4               ),
                (Key.Digit5            ,UniversalKeyCode.Digit5               ),
                (Key.Digit6            ,UniversalKeyCode.Digit6               ),
                (Key.Digit7            ,UniversalKeyCode.Digit7               ),
                (Key.Digit8            ,UniversalKeyCode.Digit8               ),
                (Key.Digit9            ,UniversalKeyCode.Digit9               ),
                (Key.LeftShift         ,UniversalKeyCode.LeftShift            ),
                (Key.RightShift        ,UniversalKeyCode.RightShift           ),
                (Key.LeftAlt           ,UniversalKeyCode.LeftAlt              ),
                (Key.RightAlt          ,UniversalKeyCode.RightAlt             ),
                (Key.LeftCtrl          ,UniversalKeyCode.LeftControl          ),
                (Key.RightCtrl         ,UniversalKeyCode.RightControl         ),
                (Key.LeftCommand       ,UniversalKeyCode.LeftCommand          ),
                (Key.RightCommand      ,UniversalKeyCode.RightCommand         ),
                (Key.ContextMenu       ,UniversalKeyCode.Menu                 ),
                (Key.Escape            ,UniversalKeyCode.Escape               ),
                (Key.LeftArrow         ,UniversalKeyCode.LeftArrow            ),
                (Key.RightArrow        ,UniversalKeyCode.RightArrow           ),
                (Key.UpArrow           ,UniversalKeyCode.UpArrow              ),
                (Key.DownArrow         ,UniversalKeyCode.DownArrow            ),
                (Key.Backspace         ,UniversalKeyCode.Backspace            ),
                (Key.PageDown          ,UniversalKeyCode.PageDown             ),
                (Key.PageUp            ,UniversalKeyCode.PageUp               ),
                (Key.Home              ,UniversalKeyCode.Home                 ),
                (Key.End               ,UniversalKeyCode.End                  ),
                (Key.Insert            ,UniversalKeyCode.Insert               ),
                (Key.Delete            ,UniversalKeyCode.Delete               ),
                (Key.CapsLock          ,UniversalKeyCode.CapsLock             ),
                (Key.NumLock           ,UniversalKeyCode.NumLock              ),
                (Key.PrintScreen       ,UniversalKeyCode.Print                ),
                (Key.ScrollLock        ,UniversalKeyCode.ScrollLock           ),
                (Key.Pause             ,UniversalKeyCode.Pause                ),
                (Key.NumpadEnter       ,UniversalKeyCode.NumpadEnter          ),
                (Key.NumpadDivide      ,UniversalKeyCode.NumpadDivide         ),
                (Key.NumpadMultiply    ,UniversalKeyCode.NumpadMultiply       ),
                (Key.NumpadPlus        ,UniversalKeyCode.NumpadPlus           ),
                (Key.NumpadMinus       ,UniversalKeyCode.NumpadMinus          ),
                (Key.NumpadPeriod      ,UniversalKeyCode.NumpadPeriod         ),
                (Key.NumpadEquals      ,UniversalKeyCode.NumpadEquals         ),
                (Key.Numpad0           ,UniversalKeyCode.Numpad0              ),
                (Key.Numpad1           ,UniversalKeyCode.Numpad1              ),
                (Key.Numpad2           ,UniversalKeyCode.Numpad2              ),
                (Key.Numpad3           ,UniversalKeyCode.Numpad3              ),
                (Key.Numpad4           ,UniversalKeyCode.Numpad4              ),
                (Key.Numpad5           ,UniversalKeyCode.Numpad5              ),
                (Key.Numpad6           ,UniversalKeyCode.Numpad6              ),
                (Key.Numpad7           ,UniversalKeyCode.Numpad7              ),
                (Key.Numpad8           ,UniversalKeyCode.Numpad8              ),
                (Key.Numpad9           ,UniversalKeyCode.Numpad9              ),
                (Key.F1                ,UniversalKeyCode.F1                   ),
                (Key.F2                ,UniversalKeyCode.F2                   ),
                (Key.F3                ,UniversalKeyCode.F3                   ),
                (Key.F4                ,UniversalKeyCode.F4                   ),
                (Key.F5                ,UniversalKeyCode.F5                   ),
                (Key.F6                ,UniversalKeyCode.F6                   ),
                (Key.F7                ,UniversalKeyCode.F7                   ),
                (Key.F8                ,UniversalKeyCode.F8                   ),
                (Key.F9                ,UniversalKeyCode.F9                   ),
                (Key.F10               ,UniversalKeyCode.F10                  ),
                (Key.F11               ,UniversalKeyCode.F11                  ),
                (Key.F12               ,UniversalKeyCode.F12                  ),
                (Key.OEM1              ,UniversalKeyCode.Unknown              ),
                (Key.OEM2              ,UniversalKeyCode.Unknown              ),
                (Key.OEM3              ,UniversalKeyCode.Unknown              ),
                (Key.OEM4              ,UniversalKeyCode.Unknown              ),
                (Key.OEM5              ,UniversalKeyCode.Unknown              ),
                (Key.IMESelected       ,UniversalKeyCode.Unknown              )
        };

        public static UniversalKeyCode KeyToUniversalKeyCode(Key key)
        {
            foreach ((Key, UniversalKeyCode) link in _inputSystemKeyMap)
            {
                if(link.Item1 == key)
                {
                    return link.Item2;
                }
            }

            return UniversalKeyCode.Unknown;
        }

        public static Key? UniversalKeyCodeToKey(UniversalKeyCode universalKeyCode)
        {
            foreach ((Key, UniversalKeyCode) link in _inputSystemKeyMap)
            {
                if (link.Item2 == universalKeyCode)
                {
                    return link.Item1;
                }
            }

            return null;
        }

#endif

    }
}
