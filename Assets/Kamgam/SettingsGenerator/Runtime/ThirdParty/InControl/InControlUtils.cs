#if KAMGAM_INCONTROL
using InControl;
using Kamgam.UGUIComponentsForSettings;

namespace Kamgam.SettingsGenerator
{
    public static class InControlUtils
    {
        public static System.Func<IInputControl> UniversalKeyCodeToControlFunc(UniversalKeyCode universalKeyCode)
        {
            // Control mapping see: https://www.gallantgames.com/pages/incontrol-standardized-controls
            switch (universalKeyCode)
            {
                case UniversalKeyCode.GamePadNorth:
                    return () => InputManager.ActiveDevice.Action4;

                case UniversalKeyCode.GamePadSouth:
                    return () => InputManager.ActiveDevice.Action1;

                case UniversalKeyCode.GamePadWest:
                    return () => InputManager.ActiveDevice.Action3;

                case UniversalKeyCode.GamePadEast:
                    return () => InputManager.ActiveDevice.Action2;

                case UniversalKeyCode.GamePadStart:
                    return () => InputManager.ActiveDevice.RightCommand;

                case UniversalKeyCode.GamePadSelect:
                    return () => InputManager.ActiveDevice.LeftCommand;

                case UniversalKeyCode.GamePadLeftShoulder:
                    return () => InputManager.ActiveDevice.LeftBumper;

                case UniversalKeyCode.GamePadRightShoulder:
                    return () => InputManager.ActiveDevice.RightBumper;

                case UniversalKeyCode.GamePadLeftTrigger:
                    return () => InputManager.ActiveDevice.LeftTrigger;

                case UniversalKeyCode.GamePadRightTrigger:
                    return () => InputManager.ActiveDevice.RightTrigger;

                case UniversalKeyCode.GamePadDPadUp:
                    return () => InputManager.ActiveDevice.DPadUp;

                case UniversalKeyCode.GamePadDPadDown:
                    return () => InputManager.ActiveDevice.DPadDown;

                case UniversalKeyCode.GamePadDPadLeft:
                    return () => InputManager.ActiveDevice.DPadLeft;

                case UniversalKeyCode.GamePadDPadRight:
                    return () => InputManager.ActiveDevice.DPadRight;

                case UniversalKeyCode.GamePadLeftStickButton:
                    return () => InputManager.ActiveDevice.LeftStick;

                case UniversalKeyCode.GamePadRightStickButton:
                    return () => InputManager.ActiveDevice.RightStick;

                case UniversalKeyCode.GamePadLeftStickUp:
                    return () => InputManager.ActiveDevice.LeftStickUp;

                case UniversalKeyCode.GamePadLeftStickDown:
                    return () => InputManager.ActiveDevice.LeftStickDown;

                case UniversalKeyCode.GamePadLeftStickLeft:
                    return () => InputManager.ActiveDevice.LeftStickLeft;

                case UniversalKeyCode.GamePadLeftStickRight:
                    return () => InputManager.ActiveDevice.LeftStickRight;

                case UniversalKeyCode.GamePadRightStickUp:
                    return () => InputManager.ActiveDevice.RightStickUp;

                case UniversalKeyCode.GamePadRightStickDown:
                    return () => InputManager.ActiveDevice.RightStickDown;

                case UniversalKeyCode.GamePadRightStickLeft:
                    return () => InputManager.ActiveDevice.RightStickLeft;

                case UniversalKeyCode.GamePadRightStickRight:
                    return () => InputManager.ActiveDevice.RightStickRight;

                default:
                    return null;
            }
        }

        public static IInputControl UniversalKeyCodeToControl(UniversalKeyCode universalKeyCode)
        {
            // Control mapping see: https://www.gallantgames.com/pages/incontrol-standardized-controls
            switch (universalKeyCode)
            {
                case UniversalKeyCode.GamePadNorth:
                    return InputManager.ActiveDevice.Action4;

                case UniversalKeyCode.GamePadSouth:
                    return InputManager.ActiveDevice.Action1;

                case UniversalKeyCode.GamePadWest:
                    return InputManager.ActiveDevice.Action3;

                case UniversalKeyCode.GamePadEast:
                    return InputManager.ActiveDevice.Action2;

                case UniversalKeyCode.GamePadStart:
                    return InputManager.ActiveDevice.RightCommand;

                case UniversalKeyCode.GamePadSelect:
                    return InputManager.ActiveDevice.LeftCommand;

                case UniversalKeyCode.GamePadLeftShoulder:
                    return InputManager.ActiveDevice.LeftBumper;

                case UniversalKeyCode.GamePadRightShoulder:
                    return InputManager.ActiveDevice.RightBumper;

                case UniversalKeyCode.GamePadLeftTrigger:
                    return InputManager.ActiveDevice.LeftTrigger;

                case UniversalKeyCode.GamePadRightTrigger:
                    return InputManager.ActiveDevice.RightTrigger;

                case UniversalKeyCode.GamePadDPadUp:
                    return InputManager.ActiveDevice.DPadUp;

                case UniversalKeyCode.GamePadDPadDown:
                    return InputManager.ActiveDevice.DPadDown;

                case UniversalKeyCode.GamePadDPadLeft:
                    return InputManager.ActiveDevice.DPadLeft;

                case UniversalKeyCode.GamePadDPadRight:
                    return InputManager.ActiveDevice.DPadRight;

                case UniversalKeyCode.GamePadLeftStickButton:
                    return InputManager.ActiveDevice.LeftStick;

                case UniversalKeyCode.GamePadRightStickButton:
                    return InputManager.ActiveDevice.RightStick;

                case UniversalKeyCode.GamePadLeftStickUp:
                    return InputManager.ActiveDevice.LeftStickUp;

                case UniversalKeyCode.GamePadLeftStickDown:
                    return InputManager.ActiveDevice.LeftStickDown;

                case UniversalKeyCode.GamePadLeftStickLeft:
                    return InputManager.ActiveDevice.LeftStickLeft;

                case UniversalKeyCode.GamePadLeftStickRight:
                    return InputManager.ActiveDevice.LeftStickRight;

                case UniversalKeyCode.GamePadRightStickUp:
                    return InputManager.ActiveDevice.RightStickUp;

                case UniversalKeyCode.GamePadRightStickDown:
                    return InputManager.ActiveDevice.RightStickDown;

                case UniversalKeyCode.GamePadRightStickLeft:
                    return InputManager.ActiveDevice.RightStickLeft;

                case UniversalKeyCode.GamePadRightStickRight:
                    return InputManager.ActiveDevice.RightStickRight;

                default:
                    return null;
            }
        }
    }
}
#endif