#if KAMGAM_REWIRED
using Rewired;
using Kamgam.UGUIComponentsForSettings;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// NOTICE: Rewired support is EXPERIMENTAL. If Rewired changes its internal values then it may break.
    /// </summary>
    public static class RewiredUtils
    {
        // These are the values from the ControllerTemplates.cs > GamepadTemplate class.
        // Why do we mirror this code here? Because the GamepadTemplate and IGamepadTemplate
        // are generated classes inside Assembly-CSharp and thus can not be access in other
        // assembly definitions unless we put all that Rewired code into and assembly definition
        // which we can't because it would risk breaking user code. That's why we re-implement
        // the GamepadTemplate constants here.

        #region Constants

        // Element identifier id for "Left Stick X".
        const int elementId_leftStickX = 0;

        // Element identifier id for "Left Stick Y".
        const int elementId_leftStickY = 1;

        // Element identifier id for "Right Stick X".
        const int elementId_rightStickX = 2;

        // Element identifier id for "Right Stick Y".
        const int elementId_rightStickY = 3;

        // Element identifier id for "Action Bottom Row 1".
        const int elementId_actionBottomRow1 = 4;

        // Element identifier id for "Action Bottom Row 2".
        const int elementId_actionBottomRow2 = 5;

        // Element identifier id for "Action Bottom Row 3".
        const int elementId_actionBottomRow3 = 6;

        // Element identifier id for "Action Top Row 1".
        const int elementId_actionTopRow1 = 7;
        
        // Element identifier id for "Action Top Row 2".
        const int elementId_actionTopRow2 = 8;

        // Element identifier id for "Action Top Row 3".
        const int elementId_actionTopRow3 = 9;

        // Element identifier id for "Left Shoulder 1".
        const int elementId_leftShoulder1 = 10;

        // Element identifier id for "Left Shoulder 2".
        const int elementId_leftShoulder2 = 11;

        // Element identifier id for "Right Shoulder 1".
        const int elementId_rightShoulder1 = 12;

        // Element identifier id for "Right Shoulder 2".
        const int elementId_rightShoulder2 = 13;

        // Element identifier id for "Center 1".
        const int elementId_center1 = 14;

        // Element identifier id for "Center 2".
        const int elementId_center2 = 15;

        // Element identifier id for "Center 3".
        const int elementId_center3 = 16;

        // Element identifier id for "Left Stick Button".
        const int elementId_leftStickButton = 17;

        // Element identifier id for "Right Stick Button".
        const int elementId_rightStickButton = 18;

        // Element identifier id for "D-Pad Up".
        const int elementId_dPadUp = 19;

        // Element identifier id for "D-Pad Right".
        const int elementId_dPadRight = 20;

        // Element identifier id for "D-Pad Down".
        const int elementId_dPadDown = 21;

        // Element identifier id for "D-Pad Left".
        const int elementId_dPadLeft = 22;

        // Element identifier id for "Left Stick".
        const int elementId_leftStick = 23;

        // Element identifier id for "Right Stick".
        const int elementId_rightStick = 24;

        // Element identifier id for "D-Pad".
        const int elementId_dPad = 25;

        #endregion

        public static ControllerType DefaultControllerType = ControllerType.Joystick;

        static IControllerTemplateButton getButtonTemplate(int playerId, ControllerType controllerType, int elementId)
        {
            Controller controller;
            if (playerId < 0)
                controller = ReInput.controllers.GetLastActiveController(controllerType);
            else
                controller = ReInput.players.GetPlayer(playerId).controllers.GetLastActiveController(controllerType);
            if (controller == null)
                return null;

            var template = controller.GetTemplate<IControllerTemplate>();
            if (template == null)
                return null;

            return template.GetElement<IControllerTemplateButton>(elementId);
        }

        /// <summary>
        /// Notice that StickUp/Down/Left/Right return the axis value and not a bool. You have to determine yourself if the value is big enough.
        /// <br /><br />
        /// Uses the 'Last Active Controller' and thus returns NULL if no controller has been used yet.
        /// </summary>
        /// <param name="universalKeyCode"></param>
        /// <param name="controllerType">Optional. If left at null then the value of 'RewiredUtils.DefaultControllerType' will be used.</param>
        /// <param name="playerId">Optional. If left at -1 then no player id will be used.</param>
        /// <returns></returns>
        public static System.Func<IControllerTemplateButton> UniversalKeyCodeToIControllerTemplateButtonFunc(
            UniversalKeyCode universalKeyCode, ControllerType? controllerType = null, int playerId = -1)
        {
            if (!controllerType.HasValue)
                controllerType = DefaultControllerType;

            // Control mapping see:
            // https://guavaman.com/projects/rewired/docs/ControllerTemplates.html
            switch (universalKeyCode)
            {
                case UniversalKeyCode.GamePadNorth:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionTopRow2);

                case UniversalKeyCode.GamePadSouth:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionBottomRow1);

                case UniversalKeyCode.GamePadWest:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionTopRow1);

                case UniversalKeyCode.GamePadEast:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionBottomRow2);

                case UniversalKeyCode.GamePadStart:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_center2);

                case UniversalKeyCode.GamePadSelect:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_center1);

                case UniversalKeyCode.GamePadLeftShoulder:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftShoulder1);

                case UniversalKeyCode.GamePadRightShoulder:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightShoulder1);

                case UniversalKeyCode.GamePadLeftTrigger:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftShoulder2);

                case UniversalKeyCode.GamePadRightTrigger:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftShoulder2);

                case UniversalKeyCode.GamePadDPadUp:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadUp);

                case UniversalKeyCode.GamePadDPadDown:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadDown);

                case UniversalKeyCode.GamePadDPadLeft:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadLeft);

                case UniversalKeyCode.GamePadDPadRight:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadRight);

                case UniversalKeyCode.GamePadLeftStickButton:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickButton);

                case UniversalKeyCode.GamePadRightStickButton:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickButton);

                case UniversalKeyCode.GamePadLeftStickUp:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickY);

                case UniversalKeyCode.GamePadLeftStickDown:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickY);

                case UniversalKeyCode.GamePadLeftStickLeft:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickX);

                case UniversalKeyCode.GamePadLeftStickRight:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickX);

                case UniversalKeyCode.GamePadRightStickUp:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickY);

                case UniversalKeyCode.GamePadRightStickDown:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickY);

                case UniversalKeyCode.GamePadRightStickLeft:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickX);

                case UniversalKeyCode.GamePadRightStickRight:
                    return () => RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickX);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Notice that StickUp/Down/Left/Right return the axis value and not a bool. You have to determine yourself if the value is big enough.
        /// <br /><br />
        /// Uses the 'Last Active Controller' and thus returns NULL if no controller has been used yet.
        /// </summary>
        /// <param name="universalKeyCode"></param>
        /// <param name="controllerType">Optional. If left at null then the value of 'RewiredUtils.DefaultControllerType' will be used.</param>
        /// <param name="playerId">Optional. If left at -1 then no player id will be used.</param>
        /// <returns></returns>
        public static IControllerTemplateButton UniversalKeyCodeToIControllerTemplateButton(
            UniversalKeyCode universalKeyCode, ControllerType? controllerType = null, int playerId = -1)
        {
            if (!controllerType.HasValue)
                controllerType = DefaultControllerType;

            // Control mapping see:
            // https://guavaman.com/projects/rewired/docs/ControllerTemplates.html
            switch (universalKeyCode)
            {
                case UniversalKeyCode.GamePadNorth:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionTopRow2);

                case UniversalKeyCode.GamePadSouth:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionBottomRow1);

                case UniversalKeyCode.GamePadWest:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionTopRow1);

                case UniversalKeyCode.GamePadEast:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_actionBottomRow2);

                case UniversalKeyCode.GamePadStart:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_center2);

                case UniversalKeyCode.GamePadSelect:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_center1);

                case UniversalKeyCode.GamePadLeftShoulder:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftShoulder1);

                case UniversalKeyCode.GamePadRightShoulder:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightShoulder1);

                case UniversalKeyCode.GamePadLeftTrigger:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftShoulder2);

                case UniversalKeyCode.GamePadRightTrigger:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftShoulder2);

                case UniversalKeyCode.GamePadDPadUp:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadUp);

                case UniversalKeyCode.GamePadDPadDown:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadDown);

                case UniversalKeyCode.GamePadDPadLeft:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadLeft);

                case UniversalKeyCode.GamePadDPadRight:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_dPadRight);

                case UniversalKeyCode.GamePadLeftStickButton:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickButton);

                case UniversalKeyCode.GamePadRightStickButton:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickButton);

                case UniversalKeyCode.GamePadLeftStickUp:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickY);

                case UniversalKeyCode.GamePadLeftStickDown:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickY);

                case UniversalKeyCode.GamePadLeftStickLeft:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickX);

                case UniversalKeyCode.GamePadLeftStickRight:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_leftStickX);

                case UniversalKeyCode.GamePadRightStickUp:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickY);

                case UniversalKeyCode.GamePadRightStickDown:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickY);

                case UniversalKeyCode.GamePadRightStickLeft:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickX);

                case UniversalKeyCode.GamePadRightStickRight:
                    return RewiredUtils.getButtonTemplate(playerId, controllerType.Value, elementId_rightStickX);

                default:
                    return null;
            }
        }
    }
}
#endif