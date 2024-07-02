#if ENABLE_INPUT_SYSTEM
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

namespace Kamgam.SettingsGenerator
{
    public static class InputActionRebindingExtensionsExtensions
    {
        /// <summary>
        /// Find the binding by GUI in the given input actions asset.<br />
        /// Returns true if found, false otherwise.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        /// <param name="binding">The out variable taking the found binding.</param>
        /// <returns>Returns true if found, false otherwise.</returns>
        public static bool FindBinding(this InputActionAsset inputActionAsset, string bindingId, out InputBinding binding)
        {
            if (string.IsNullOrEmpty(bindingId))
            {
                binding = default;
                return false;
            }

            int mapCount = inputActionAsset.actionMaps.Count;
            for (int m = 0; m < mapCount; m++)
            {
                var map = inputActionAsset.actionMaps[m];

                int bindingCount = map.bindings.Count;
                for (int b = 0; b < bindingCount; b++)
                {
                    if (map.bindings[b].id.ToString() == bindingId)
                    {
                        binding = map.bindings[b];
                        return true;
                    }
                }
            }

            binding = default;
            return false;
        }

        /// <summary>
        /// Finds the action of the given binding id in the given input actions asset.<br />
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        /// <returns></returns>
        public static InputAction GetActionOfBinding(this InputActionAsset inputActionAsset, string bindingId)
        {
            if (string.IsNullOrEmpty(bindingId))
            {
                return null;
            }

            int mapCount = inputActionAsset.actionMaps.Count;
            for (int m = 0; m < mapCount; m++)
            {
                var map = inputActionAsset.actionMaps[m];

                int actionCount = map.actions.Count;
                for (int a = 0; a < actionCount; a++)
                {
                    var action = map.actions[a];

                    int bindingCount = action.bindings.Count;
                    for (int b = 0; b < bindingCount; b++)
                    {
                        if (action.bindings[b].id.ToString() == bindingId)
                        {
                            return action;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the action map which the given binding id is part of.<br />
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        /// <returns></returns>
        public static InputActionMap GetActionMapOfBinding(this InputActionAsset inputActionAsset, string bindingId)
        {
            if (string.IsNullOrEmpty(bindingId))
            {
                return null;
            }

            int mapCount = inputActionAsset.actionMaps.Count;
            for (int m = 0; m < mapCount; m++)
            {
                var map = inputActionAsset.actionMaps[m];

                int actionCount = map.actions.Count;
                for (int a = 0; a < actionCount; a++)
                {
                    var action = map.actions[a];

                    int bindingCount = action.bindings.Count;
                    for (int b = 0; b < bindingCount; b++)
                    {
                        if (action.bindings[b].id.ToString() == bindingId)
                        {
                            return map;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The binding index within the given input actions asset.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        /// <returns>The index or -1 if not found.</returns>
        public static int GetBindingIndexWithinActionMap(this InputActionAsset inputActionAsset, string bindingId)
        {
            if (string.IsNullOrEmpty(bindingId))
            {
                return -1;
            }

            int mapCount = inputActionAsset.actionMaps.Count;
            for (int m = 0; m < mapCount; m++)
            {
                var map = inputActionAsset.actionMaps[m];

                // Index of all bindings in the action map.
                // See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/api/UnityEngine.InputSystem.InputActionRebindingExtensions.html
                int bindingCount = map.bindings.Count;
                for (int b = 0; b < bindingCount; b++)
                {
                    if (map.bindings[b].id.ToString() == bindingId)
                    {
                        return b;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// The binding index within the ACTION of the binding.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        /// <returns>The index or -1 if not found.</returns>
        public static int GetBindingIndexWithinAction(this InputActionAsset inputActionAsset, string bindingId)
        {
            if (string.IsNullOrEmpty(bindingId))
            {
                return -1;
            }

            int mapCount = inputActionAsset.actionMaps.Count;
            for (int m = 0; m < mapCount; m++)
            {
                var map = inputActionAsset.actionMaps[m];

                int actionCount = map.actions.Count;
                for (int a = 0; a < actionCount; a++)
                {
                    var action = map.actions[a];

                    int bindingCount = action.bindings.Count;
                    for (int b = 0; b < bindingCount; b++)
                    {
                        if (action.bindings[b].id.ToString() == bindingId)
                        {
                            return b;
                        }
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Overrides the binding based on the given input action asset and binding id.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        /// <param name="overridePath"></param>
        /// <param name="overrideInteractions"></param>
        /// <param name="overrideProcessors"></param>
        public static void ApplyBindingOverride(this InputActionAsset inputActionAsset, string bindingId, string overridePath, string overrideInteractions = null, string overrideProcessors = null)
        {
            inputActionAsset.ApplyBindingOverrideWithResult(bindingId, overridePath, overrideInteractions, overrideProcessors);
        }

        /// <summary>
        /// Overrides the binding based on the given input action asset and binding id.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        /// <param name="overridePath"></param>
        /// <param name="overrideInteractions"></param>
        /// <param name="overrideProcessors"></param>
        /// <returns>True if override succeeded, false otherwise.</returns>
        public static bool ApplyBindingOverrideWithResult(this InputActionAsset inputActionAsset, string bindingId, string overridePath, string overrideInteractions = null, string overrideProcessors = null)
        {
            int bindingIndex = inputActionAsset.GetBindingIndexWithinActionMap(bindingId);
            if (bindingIndex >= 0)
            {
                var actionMap = inputActionAsset.GetActionMapOfBinding(bindingId);
                if (actionMap != null)
                {
                    InputBinding overrideBinding = new InputBinding();
                    overrideBinding.overridePath = overridePath;
                    overrideBinding.overrideInteractions = overrideInteractions;
                    overrideBinding.overrideProcessors = overrideProcessors;

                    actionMap.ApplyBindingOverride(bindingIndex, overrideBinding);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the override on the binding.
        /// </summary>
        /// <param name="inputActionAsset"></param>
        /// <param name="bindingId"></param>
        public static void ClearOverride(this InputActionAsset inputActionAsset, string bindingId)
        {
            int bindingIndex = inputActionAsset.GetBindingIndexWithinAction(bindingId);
            if (bindingIndex >= 0)
            {
                var action = inputActionAsset.GetActionOfBinding(bindingId);
                if (action != null)
                {
                    action.RemoveBindingOverride(bindingIndex);
                }
            }
        }
    }
}

#endif
