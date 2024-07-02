// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using UnityEngine.UIElements;
#endif

using UnityEngine;
using UnityEngine.Events;

namespace Kamgam.UIToolkitComponentsForSettings
{
    /// <summary>
    /// Add this as a CHILD to a UIDocument.
    /// This allows you to add event listeners via Unity Events.
    /// </summary>
    public class UIElementEvents : UIElementClickEvent
    {
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
        [Header("Events")]

        public UnityEvent<PointerDownEvent> OnPointerDown;
        public UnityEvent<PointerUpEvent> OnPointerUp;
        public UnityEvent<PointerEnterEvent> OnPointerEnter;
        public UnityEvent<PointerLeaveEvent> OnPointerLeave;
        public UnityEvent<FocusEvent> OnFocus;
        public UnityEvent<BlurEvent> OnBlur;
        public UnityEvent<KeyDownEvent> OnKeyDown;
        public UnityEvent<KeyUpEvent> OnKeyUp;
        /*
        public UnityEvent<DragEnterEvent> OnDragEnter;
        public UnityEvent<DragExitedEvent> OnDragExit;
        */
        public UnityEvent<ChangeEvent<bool>> OnChangeBool;
        public UnityEvent<ChangeEvent<int>> OnChangeInt;
        public UnityEvent<ChangeEvent<float>> OnChangeFloat;
        public UnityEvent<ChangeEvent<string>> OnChangeString;

        public override void RegisterEvents()
        {
            if (Elements.Count == 0)
                return;

            foreach (var ele in Elements)
            {
                if (OnPointerDown != null) ele.RegisterCallback<PointerDownEvent>(onPointerDown);
                if (OnPointerUp != null) ele.RegisterCallback<PointerUpEvent>(onPointerUp);
                if (OnPointerEnter != null) ele.RegisterCallback<PointerEnterEvent>(onPointerEnter);
                if (OnPointerLeave != null) ele.RegisterCallback<PointerLeaveEvent>(onPointerLeave);
                if (OnFocus != null) ele.RegisterCallback<FocusEvent>(onFocus);
                if (OnBlur != null) ele.RegisterCallback<BlurEvent>(onBlur);
                if (OnKeyDown != null) ele.RegisterCallback<KeyDownEvent>(onKeyDown);
                if (OnKeyUp != null) ele.RegisterCallback<KeyUpEvent>(onKeyUp);
                //if (OnDragEnter != null) ele.RegisterCallback<DragEnterEvent>(onDragEnter);
                //if (OnDragExit != null) ele.RegisterCallback<DragExitedEvent>(onDragExit);
                if (OnChangeBool != null) ele.RegisterCallback<ChangeEvent<bool>>(onChangeBool);
                if (OnChangeInt != null) ele.RegisterCallback<ChangeEvent<int>>(onChangeInt);
                if (OnChangeFloat != null) ele.RegisterCallback<ChangeEvent<float>>(onChangeFloat);
                if (OnChangeString != null) ele.RegisterCallback<ChangeEvent<string>>(onChangeString);
            }

            base.RegisterEvents();
        }

        public override void UnregisterEvents()
        {
            if (Elements.Count == 0)
                return;

            foreach (var ele in Elements)
            {
                if (OnPointerDown != null) ele.UnregisterCallback<PointerDownEvent>(onPointerDown);
                if (OnPointerUp != null) ele.UnregisterCallback<PointerUpEvent>(onPointerUp);
                if (OnPointerEnter != null) ele.UnregisterCallback<PointerEnterEvent>(onPointerEnter);
                if (OnPointerLeave != null) ele.UnregisterCallback<PointerLeaveEvent>(onPointerLeave);
                if (OnFocus != null) ele.UnregisterCallback<FocusEvent>(onFocus);
                if (OnBlur != null) ele.UnregisterCallback<BlurEvent>(onBlur);
                if (OnKeyDown != null) ele.UnregisterCallback<KeyDownEvent>(onKeyDown);
                if (OnKeyUp != null) ele.UnregisterCallback<KeyUpEvent>(onKeyUp);
                //if (OnDragEnter != null) ele.UnregisterCallback<DragEnterEvent>(onDragEnter);
                //if (OnDragExit != null) ele.UnregisterCallback<DragExitedEvent>(onDragExit);
                if (OnChangeBool != null) ele.UnregisterCallback<ChangeEvent<bool>>(onChangeBool);
                if (OnChangeInt != null) ele.UnregisterCallback<ChangeEvent<int>>(onChangeInt);
                if (OnChangeFloat != null) ele.UnregisterCallback<ChangeEvent<float>>(onChangeFloat);
                if (OnChangeString != null) ele.UnregisterCallback<ChangeEvent<string>>(onChangeString);
            }

            base.UnregisterEvents();
        }

        protected virtual void onPointerDown(PointerDownEvent evt)
        {
            OnPointerDown?.Invoke(evt);
        }

        protected virtual void onPointerUp(PointerUpEvent evt)
        {
            OnPointerUp?.Invoke(evt);
        }

        protected virtual void onPointerEnter(PointerEnterEvent evt)
        {
            OnPointerEnter?.Invoke(evt);
        }

        protected virtual void onPointerLeave(PointerLeaveEvent evt)
        {
            OnPointerLeave?.Invoke(evt);
        }

        protected virtual void onFocus(FocusEvent evt)
        {
            OnFocus?.Invoke(evt);
        }

        protected virtual void onBlur(BlurEvent evt)
        {
            OnBlur?.Invoke(evt);
        }

        protected virtual void onKeyDown(KeyDownEvent evt)
        {
            OnKeyDown?.Invoke(evt);
        }

        protected virtual void onKeyUp(KeyUpEvent evt)
        {
            OnKeyUp?.Invoke(evt);
        }

        /*
        protected virtual void onDragEnter(DragEnterEvent evt)
        {
            OnDragEnter?.Invoke(evt);
        }

        protected virtual void onDragExit(DragExitedEvent evt)
        {
            OnDragExit?.Invoke(evt);
        }
        */

        protected virtual void onChangeBool(ChangeEvent<bool> evt)
        {
            OnChangeBool?.Invoke(evt);
        }

        protected virtual void onChangeInt(ChangeEvent<int> evt)
        {
            OnChangeInt?.Invoke(evt);
        }

        protected virtual void onChangeFloat(ChangeEvent<float> evt)
        {
            OnChangeFloat?.Invoke(evt);
        }

        protected virtual void onChangeString(ChangeEvent<string> evt)
        {
            OnChangeString?.Invoke(evt);
        }
#endif
    }
}
