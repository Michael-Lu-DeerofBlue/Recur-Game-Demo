// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using UnityEngine.UIElements;
#endif

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kamgam.UIToolkitComponentsForSettings
{
    /// <summary>
    /// Add this as a CHILD to a UIDocument.
    /// This allows you to add event listeners via Unity Events.
    /// </summary>
    public class UIElementClickEvent : MonoBehaviour
    {
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
        [Header("Query Criteria")]

        [Tooltip("Only elements of this type are used.")]
        public UIElementType Type;

        [Tooltip("If set then the element will be search by the class name.\nIf an element name is set then both have to match.")]
        public string BindingClass;

        [Tooltip("If set then the element will be search by the element name.\nIf a class name is set then both have to match.")]
        public string BindingName;

        [Tooltip("If enabled then all elements matching the criteria are used.")]
        public bool MultipleResults = false;


        [Header("Events")]

        public UnityEvent<ClickEvent> OnClick;

        protected UIDocument _document;
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = this.GetComponentInParent<UIDocument>();
                }

#if UNITY_EDITOR
                if (_document == null)
                {
                    Debug.LogWarning("UIElement: No UIDocument found. Please check if you have really added this as a CHILD of a UIDocument!", this.gameObject);
                }
#endif

                return _document;
            }
        }


        [System.NonSerialized]
        public List<VisualElement> Elements = new List<VisualElement>();

        /// <summary>
        /// If set then the elements found by BindingClass and/or BindingName are filtered by this predicate.
        /// </summary>
        public System.Predicate<VisualElement> BindingPredicate;

        public virtual void OnEnable()
        {
            RefreshElements();
            RegisterEvents();
        }

        public void RefreshElements()
        {
            if (Document == null || Document.rootVisualElement == null)
            {
                return;
            }

            Elements.Clear();

            if (MultipleResults)
            {
                Document.QueryTypes(
                    Type,
                    name: string.IsNullOrEmpty(BindingName) ? null : BindingName,
                    className: string.IsNullOrEmpty(BindingClass) ? null : BindingClass,
                    list: Elements);
            }
            else
            {
                var ele = Document.QueryType(
                    Type,
                    name: string.IsNullOrEmpty(BindingName) ? null : BindingName,
                    className: string.IsNullOrEmpty(BindingClass) ? null : BindingClass);
                if (ele != null)
                {
                    Elements.Add(ele);
                }
            }
        }

        public virtual void OnDisable()
        {
            UnregisterEvents();
        }

        public virtual void OnDestroy()
        {
            UnregisterEvents();
        }

        public virtual void RegisterEvents()
        {
            if (Elements.Count == 0)
                return;

            foreach (var ele in Elements)
            {
                if (OnClick != null) ele.RegisterCallback<ClickEvent>(onClick);
            }
        }

        public virtual void UnregisterEvents()
        {
            if (Elements.Count == 0)
                return;

            foreach (var ele in Elements)
            {
                if (OnClick != null) ele.UnregisterCallback<ClickEvent>(onClick);
            }
        }

        protected virtual void onClick(ClickEvent evt)
        {
            OnClick?.Invoke(evt);
        }
#endif
    }
}
