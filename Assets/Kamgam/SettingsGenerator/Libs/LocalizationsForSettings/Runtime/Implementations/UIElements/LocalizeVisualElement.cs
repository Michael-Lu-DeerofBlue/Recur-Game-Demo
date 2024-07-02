// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using System.Collections;
using UnityEngine.UIElements;

namespace Kamgam.LocalizationForSettings.UIElements
{
    public abstract class LocalizeVisualElement : LocalizeBase
    {
        public static string LocalizationClassNamePrefix = "loc_";

        public static bool HasLocalizationClass(VisualElement element)
        {
            return GetLocalizationClassName(element) != null;
        }

        public static string GetLocalizationClassName(VisualElement element)
        {
            var classes = element.GetClasses();

            foreach (var className in classes)
            {
                // Use first found.
                if (className.StartsWith(LocalizationClassNamePrefix))
                {
                    return className;
                }
            }

            return null;
        }

        /// <summary>
        /// The UNIQUE class name of the VisualElement which we want this
        /// localizer to be bound to.<br />
        /// Upon creation the first class name starting with "loc_" will be
        /// used (prefix). However you can change it
        /// manually later.<br />
        /// Example: "loc_bossFights"<br />
        /// <br />
        /// NOTICE: The element you bind to may not be the final element. If
        /// the type does not match the first child matching the type will
        /// be used.
        /// </summary>
        public string BindingClass;

        protected UIDocument _document;
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = transform.GetComponentInParent<UIDocument>();
                }
                return _document;
            }
        }

        public abstract System.Type GetElementType();

        protected VisualElement _visualElement;
        public VisualElement VisualElement
        {
            get
            {
                if (_visualElement == null && !string.IsNullOrEmpty(BindingClass))
                {
                    if (Document != null)
                    {
                        var ele = GetBindingClassElement();
                        _visualElement = getFinalElement(ele);

                        if (_visualElement != null)
                        {
                            _visualElement.RegisterCallback<DetachFromPanelEvent>(detachFromPanel);
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning("No element with binding class '" + BindingClass + "' found.");
                        }
                    }
                }

                return _visualElement;
            }
        }

        public VisualElement GetBindingClassElement()
        {
            return Document.rootVisualElement.Q(className: BindingClass);
        }

        protected virtual void detachFromPanel(DetachFromPanelEvent evt)
        {
            resetUIElements();
            if (this != null && isActiveAndEnabled)
            {
                StartCoroutine(RefreshDelayedAsync());
            }
        }

        protected virtual IEnumerator RefreshDelayedAsync()
        {
            // Wait 1 frame for UI Toolkit to reload.
            // TODO: Investigate, maybe there is an event we can listen to?
            yield return null;
            Localize(); 
        }

        protected virtual VisualElement getFinalElement(VisualElement ele)
        {
            if (ele == null)
                return null;

            // If the element found by class name is NOT yet the correct type then try to
            // find it in the children.
            var type = GetElementType();
            if (ele.GetType() == type)
            {
                return ele;
            }
            else
            {
                // Use the first matching the requested type.
                // TODO: Introduce a more robust solution as this
                // one will fail if there are two or more elements
                // of the same type.
                foreach (var child in ele.Query<VisualElement>().Build())
                {
                    if (child.GetType() == type)
                    {
                        return child;
                    }
                }

                return ele;
            }
        }

        public virtual void BindTo(VisualElement element)
        {
            _document = null;
            _visualElement = null;

            if (element == null)
            {
                BindingClass = null;
                return;
            }

            var classes = element.GetClasses();
            BindingClass = GetLocalizationClassName(element);
            if (BindingClass != null)
            {
                Term = GetText();

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
            }
        }

        public virtual void Unbind()
        {
            // Reset
            resetUIElements();
            BindingClass = null;
        }

        public override void OnDisable()
        {
            // Rest to trigger fetching the label again OnEnable.
            resetUIElements();

            base.OnDisable();
        }

        protected virtual void resetUIElements()
        {
            _document = null;

            if (_visualElement != null)
            {
                _visualElement.UnregisterCallback<DetachFromPanelEvent>(detachFromPanel);
            }
            _visualElement = null;
        }

        public void OnDestroy()
        {
            Unbind();
        }
    }
}
#endif
