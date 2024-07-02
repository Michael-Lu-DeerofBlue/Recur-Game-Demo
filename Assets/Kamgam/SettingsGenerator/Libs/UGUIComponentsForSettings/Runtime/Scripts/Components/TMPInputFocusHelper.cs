using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kamgam.UGUIComponentsForSettings
{
    [RequireComponent(typeof(TMP_InputField))]
    public class TMPInputFocusHelper : MonoBehaviour, ISelectHandler, ISubmitHandler
    {
        [SerializeField]
        [Header("Touch Settings")]
        [Tooltip("Which keyboard type should be opened on touch devices?")]
        TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;

        protected TMP_InputField inputTf;
        public TMP_InputField InputTf
        {
            get
            {
                if (inputTf == null)
                {
                    inputTf = this.GetComponent<TMP_InputField>();
                }
                return inputTf;
            }
        }
        
        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(UnFocusByDefault());
        }

        IEnumerator UnFocusByDefault()
        {
            yield return new WaitForEndOfFrame();
            InputTf.DeactivateInputField();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            TouchScreenKeyboard.Open(InputTf.text, keyboardType);
        }

        public void Update()
        {
            // Sadly while entering text the "Submit" action does not
            // end the input if done with a controller (keyboard ENTER
            // works just fine).
            
            // Catch Submit directly and end editing
            if (InputUtils.SubmitDown() && InputTf.isFocused)
            {
                if (InputTf.isFocused
#if ENABLE_INPUT_SYSTEM
                    && Keyboard.current != null
                    && !Keyboard.current.enterKey.wasPressedThisFrame
                    && !Keyboard.current.numpadEnterKey.wasPressedThisFrame
#else
                    && !Input.GetKeyDown(KeyCode.Return)
                    && !Input.GetKeyDown(KeyCode.KeypadEnter)
#endif
                    )
                {
                    InputTf.DeactivateInputField();
                }
            }
        }
    }
}
