using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    [SelectionBase]
    public class InputBindingUGUI : MonoBehaviour
    {
        public delegate void OnChangedDelegate(string bindingPath);

        /// <summary>
        /// Set this to override the default path to display name conversion.<br />
        /// Usually this is an InputControlPath from the new InputSystem, see:
        /// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/api/UnityEngine.InputSystem.InputControlPath.html
        /// <br /><br />
        /// Useful for text modifications (translations, sprite inserts, ...).
        /// </summary>
        public System.Func<string, string> PathToDisplayNameFunc;

       public InputBindingForInputSystem InputBinding;

        /// <summary>
        /// The string is a path. Most likely an InputControlPath from the InputSystem.
        /// </summary>
        public UnityEvent<string> OnChangedEvent;
        public OnChangedDelegate OnChanged;

        public Button Button;
        public GameObject Normal;
        public GameObject Active;

        /// <summary>
        /// The label textfield of this input binding.
        /// </summary>
        public TextMeshProUGUI TextTf;

        /// <summary>
        /// The textfield that shows the binding name or key as a string.
        /// </summary>
        public TextMeshProUGUI DisplayNameTf;

        /// <summary>
        /// Textfield that contains the "press no to change" text is change mode is active.
        /// </summary>
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

        public virtual string DisplayName
        {
            get
            {
                if (DisplayNameTf == null)
                    return null;

                return DisplayNameTf.text;
            }
            set
            {
                if (value == DisplayName)
                    return;

                DisplayNameTf.text = value;
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

        public virtual void SetActive(bool active)
        {
            bool willBeActivated = IsActive != active && active;
            bool willBeDeactivated = IsActive != active && !active;

            if (willBeActivated && InputBinding != null)
            {
                InputBinding.AddOnCompleteCallback(onBindingComplete);
                InputBinding.AddOnCanceledCallback(onBindingCanceled);
                InputBinding.StartListening();
            }

            // Reselect Button after input was finished.
            if (willBeDeactivated && EventSystem.current != null)
            {
                SelectionUtils.SetSelected(Button.gameObject);
            }

            Normal.SetActive(!active);
            Active.SetActive(active);
            Button.interactable = !active;
        }

        protected void onBindingComplete()
        {
            InputBinding.RemoveOnCompleteCallback(onBindingComplete);
            InputBinding.RemoveOnCanceledCallback(onBindingCanceled);

            UpdateDisplayName();

            SetActive(false);

            OnChanged?.Invoke(InputBinding.GetBindingPath());
            OnChangedEvent?.Invoke(InputBinding.GetBindingPath());
        }

        protected virtual void onBindingCanceled()
        {
            InputBinding.RemoveOnCompleteCallback(onBindingComplete);
            InputBinding.RemoveOnCanceledCallback(onBindingCanceled);

            SetActive(false);
        }

        public virtual void UpdateDisplayName()
        {
            if (InputBinding == null)
                return;

            if (PathToDisplayNameFunc != null)
                DisplayName = PathToDisplayNameFunc(InputBinding.GetBindingPath());
            else
                DisplayName = InputBinding.GetBindingPath();
        }

        public virtual bool IsCancelKeyPressed()
        {
            return InputUtils.CancelDown();
        }

        public void OnEnable()
        {
            Refresh();
            InputBinding.OnEnable();
        }

        public void OnDisable()
        {
            InputBinding.OnDisable();

            if (IsActive)
            {
                SetActive(false);
            }
        }

        public virtual void Refresh()
        {
            UpdateDisplayName();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlaying && !UnityEditor.BuildPipeline.isBuildingPlayer)
            {
                UpdateDisplayName();
            }
        }
#endif
    }
}
