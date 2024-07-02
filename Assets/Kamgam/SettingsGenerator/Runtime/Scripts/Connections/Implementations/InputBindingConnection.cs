#if ENABLE_INPUT_SYSTEM
using UnityEngine;

using UnityEngine.InputSystem;

namespace Kamgam.SettingsGenerator
{
    public class InputBindingConnection : Connection<string>
    {
        public static bool LogErrorOnBindingFail = true;

        /// <summary>
        /// The input action asset. An override will be added to the binding in this asset if Set(KeyCombination keyCombination) is called.
        /// </summary>
        protected InputActionAsset _inputActionAsset;

        /// <summary>
        /// A string uniquely identifying the binding in the InputSystem (GUID).
        /// </summary>
        protected string _bindingId;

        public InputBindingConnection() 
        {
        }

        public void SetBindingId(string id)
        {
            _bindingId = id;
        }

        public string GetBindingId()
        {
            return _bindingId;
        }

        public void SetInputActionAsset(InputActionAsset asset)
        {
            _inputActionAsset = asset;
        }

        public InputActionAsset GetInputActionAsset()
        {
            return _inputActionAsset;
        }

        public void ClearOverride()
        {
            if (_inputActionAsset == null)
            {
                return;
            }

            _inputActionAsset.ClearOverride(_bindingId);
        }

        public override string Get()
        {
            if (_inputActionAsset == null)
            {
                logNoInputAssetError();
                return null;
            }

            InputBinding binding;
            bool found = _inputActionAsset.FindBinding(_bindingId, out binding);
            if(found)
            {
                return binding.effectivePath;
            }
            else
            {
                logNoBindingError();
                return null;
            }
        }

        public override string GetDefault()
        {
            if (_inputActionAsset == null)
            {
                logNoInputAssetError();
                return null;
            }

            InputBinding binding;
            bool found = _inputActionAsset.FindBinding(_bindingId, out binding);
            if (found)
            {
                return binding.path;
            }
            else
            {
                logNoBindingError();
                return null;
            }
        }

        private static void logNoInputAssetError()
        {
            if (LogErrorOnBindingFail)
            {
                Debug.LogError("The InputActionAsset is NULL.");
            }
        }

        private void logNoBindingError()
        {
            if (LogErrorOnBindingFail)
            {
                Debug.LogError($"No binding for ID '{_bindingId}' found.");
            }
        }

        public override void Set(string overridePath)
        {
            if (_inputActionAsset == null)
            {
                if (LogErrorOnBindingFail)
                {
                    Debug.LogError("The InputActionAsset is NULL.");
                }
                return;
            }

            var result = _inputActionAsset.ApplyBindingOverrideWithResult(_bindingId, overridePath);
            if (!result && LogErrorOnBindingFail)
            {
                Debug.LogError($"No binding for ID '{_bindingId}' found.");
            }

            NotifyListenersIfChanged(overridePath);
        }
    }
}

#endif
