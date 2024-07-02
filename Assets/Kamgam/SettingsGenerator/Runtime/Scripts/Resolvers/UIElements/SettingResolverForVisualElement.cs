// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// The purpose of a SettingResolverForVisualElement is to connect a UIElement in the VisualTree.<br />
    /// It's supposed to be used added as a component to a child of the UIDocument.<br />
    /// You need to extend this class with a concrete implementation.
    /// </summary>
    public abstract class SettingResolverForVisualElement : SettingResolver, ISettingResolver
    {
        public static string SettingsClassNamePrefix = "set_";

        /// <summary>
        /// Separator between setting name and some additionl parts on the class name.<br />
        /// Example: "set_bossFights__1": Setting ID = "bossFights", additional info = "1".
        /// </summary>
        public static string SettingsClassNameSeparator = "__";

        public static bool HasSettingClass(VisualElement element)
        {
            return GetSettingClassName(element) != null;
        }

        public static string GetSettingClassName(VisualElement element)
        {
            var classes = element.GetClasses();

            foreach (var className in classes)
            {
                // Use first found.
                if (className.StartsWith(SettingsClassNamePrefix))
                {
                    return className;
                }
            }

            return null;
        }

        /// <summary>
        /// The UNIQUE class name of the VisualElement which we want this
        /// resolver to be bound to.<br />
        /// Upon creation the first class name starting with "set_" will be
        /// used (see SettingsClassNamePrefix).
        /// <br /><br />
        /// The first word after "set_" will be used to prefill the setting ID.
        /// However you can change it later at any time in the resolver component.
        /// <br />
        /// Example: "set_bossFights" will set the used Settings
        /// ID to "bossFights" upon initial resolver creation.
        /// <br /><br />
        /// If you happen to have two "set_bossFights" classes then you can make
        /// them unique by appending "__" + a custom text like:<br />
        /// "set_bossFights__1" and "set_bossFights__2". Both will still bind to 
        /// the "bossFights" ID.
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

        protected VisualElement _visualElement;
        public VisualElement VisualElement
        {
            get
            {
                if (_visualElement == null && !string.IsNullOrEmpty(BindingClass))
                {
                    if (Document != null)
                    {
                        _visualElement = Document.rootVisualElement.Q(className: BindingClass);
                        if(_visualElement != null)
                        {
                            _visualElement.RegisterCallback<DetachFromPanelEvent>(detachFromPanel);
                        }
                        else
                        {
                            Logger.LogWarning("No element with binding class '" + BindingClass + "' found.");
                        }
                    }
                }

                return _visualElement;
            }

            set
            {
                _visualElement = value;
                if (value == null)
                {
                    BindingClass = null;
                }
            }
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
            Refresh();
        }

        public void BindTo(VisualElement element)
        {
            _document = null;
            _visualElement = null;

            if (element == null)
            {
                BindingClass = null;
                return;
            }

            var classes = element.GetClasses();

            foreach (var className in classes)
            {
                // Use first found.
                if (className.StartsWith(SettingsClassNamePrefix))
                {
                    // Extract settings ID (if there is one).
                    string[] parts = Regex.Split(className, SettingsClassNameSeparator);
                    if (parts.Length > 0)
                    {
                        ID = parts[0].Substring(SettingsClassNamePrefix.Length);
                    }

                    BindingClass = className;

#if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(this);
                    UnityEditor.EditorUtility.SetDirty(gameObject);
#endif

                    break;
                }
            }
        }

        public void Unbind()
        {
            // Reset guid
            resetUIElements();
            BindingClass = null;
        }

        public override void OnDisable()
        {
            // Rest to trigger fetching the label again OnEnable.
            resetUIElements();
            StopAllCoroutines();
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

        public override void OnDestroy()
        {
            Unbind();
            StopAllCoroutines();
            base.OnDestroy();
        }
    }
}
#endif
