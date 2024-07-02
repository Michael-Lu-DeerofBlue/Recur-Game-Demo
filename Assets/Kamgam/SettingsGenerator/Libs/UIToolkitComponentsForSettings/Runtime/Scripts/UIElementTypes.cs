// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using UnityEngine.UIElements;

using System.Collections.Generic;

namespace Kamgam.UIToolkitComponentsForSettings
{
    public enum UIElementType
    {
        VisualElement,
        BindableElement,
        Button,
        Label,
        Scroller,
        TextField,
        Foldout,
        Slider,
        SliderInt,
        // ProgressBar, // Removed because it is not included in the com.unity.ui package.
        DropdownField,
        DropdownMenu
    }

    public static class UIElementTypes
    {
        public static VisualElement QueryType(this UIDocument document, UIElementType type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null)
        {
            var sysType = GetType(type);
            if(sysType != null)
            {
                return QueryType(document, sysType, name, className, predicate);
            }

            return null;
        }

        public static VisualElement QueryType(this UIDocument document, System.Type type, string name = null, string className = null, System.Predicate<VisualElement> predicate = null)
        {
            if (document == null || document.rootVisualElement == null)
            {
                return null;
            }

            var iter = document.rootVisualElement.Query<VisualElement>(name, className).Build();
            foreach (var ele in iter)
            {
                if (predicate != null && !predicate.Invoke(ele))
                    continue;

                if (ele.GetType() == type)
                {
                    return ele;
                }
            }

            return null;
        }

        public static List<VisualElement> QueryTypes(this UIDocument document, UIElementType type, string name = null, string className = null, List<VisualElement> list = null, System.Predicate<VisualElement> predicate = null)
        {
            var sysType = GetType(type);
            if (sysType != null)
            {
                return QueryTypes(document, sysType, name, className, list, predicate);
            }

            return list;
        }

        public static List<VisualElement> QueryTypes(this UIDocument document, System.Type type, string name = null, string className = null, List<VisualElement> list = null, System.Predicate<VisualElement> predicate = null)
        {
            if (list == null)
                list = new List<VisualElement>();

            list.Clear();

            if (document == null || document.rootVisualElement == null)
            {
                return list;
            }

            var iter = document.rootVisualElement.Query<VisualElement>(name, className).Build();
            foreach (var ele in iter)
            {
                if (predicate != null && !predicate.Invoke(ele))
                    continue;

                if (ele.GetType() == type)
                {
                    list.Add(ele);
                }
            }

            return list;
        }

        public static System.Type GetType(UIElementType type)
        {
            switch (type)
            {
                case UIElementType.VisualElement:
                    return typeof(VisualElement);

                case UIElementType.BindableElement:
                    return typeof(BindableElement);

                case UIElementType.Button:
                    return typeof(Button);

                case UIElementType.Label:
                    return typeof(Label);

                case UIElementType.Scroller:
                    return typeof(Scroller);

                case UIElementType.TextField:
                    return typeof(TextField);

                case UIElementType.Foldout:
                    return typeof(Foldout);

                case UIElementType.Slider:
                    return typeof(Slider);

                case UIElementType.SliderInt:
                    return typeof(SliderInt);

                //case UIElementType.ProgressBar:
                //    return typeof(ProgressBar);

                case UIElementType.DropdownField:
                    return typeof(DropdownField);

                case UIElementType.DropdownMenu:
                    return typeof(DropdownMenu);

                default:
                    return null;
            }
        }
    }
}
#endif