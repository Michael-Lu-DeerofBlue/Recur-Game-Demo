#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.SettingsGenerator
{
    public static class UIToolkitExtensions
    {
        // API fix for Unitys breaking change 2020 > 2021. Thanks Unity.
#if !UNITY_2021_2_OR_NEWER
        public static void Rebuild(this ListView listView)
        {
            listView.Refresh();
        }
#endif

        public static void AddMultipleToClassList(this VisualElement ele, params string[] classNames)
        {
            foreach (var c in classNames)
            {
                ele.AddToClassList(c);
            }
        }

        public static SerializedProperty GetItemSourcePropertyAt(this ListView listView, int index)
        {
            var serializedObjectList = listView.itemsSource;

            // ignore first element if showBoundCollectionSize is on.
            if (listView.showBoundCollectionSize && index >= serializedObjectList.Count - 1)
                return null;

            // "itemsSource" is an object of internal Type SerializedObjectList
            // whose first entry is some info on the collectin size BUT only if
            // showBoundCollectionSize is set to true.
            int indexForData = listView.showBoundCollectionSize ? index + 1 : index;
            var prop = serializedObjectList[indexForData] as SerializedProperty;
            return prop;
        }

        public static T GetItemSourceAt<T>(this ListView listView, int index) where T : class
        {
            var prop = listView.GetItemSourcePropertyAt(index);

            if (prop == null || prop.objectReferenceValue == null)
                return null;

            return prop.objectReferenceValue as T;
        }

        public static T GetFirstSelectedValue<T>(this ListView listView)
        {
            if (listView.selectedIndex < 0)
                return default(T);

            return (T) listView.itemsSource[listView.selectedIndex];
        }

        /// <summary>
        /// Scrolls the list view to the selected element. Needed because it looses the position if hidden.
        /// <br />
        /// It seems this was fixed in Unity 2023.1.x, see:
        /// https://forum.unity.com/threads/scrollview-loses-scroll-position-after-hide-and-show-display-none-display-flex.1084706/
        /// </summary>
        /// <param name="listView"></param>
        public static void ScrollToSelectedItem(this ListView listView)
        {
            if (listView.selectedIndex == -1)
                return;

            // Schedule to avoid resolvedStyle.height being 0.
            listView.schedule.Execute(() =>
            {
                ScrollView scrollView = listView.Q<ScrollView>();

                if (scrollView == null)
                    return;

                // Calculate the position of the selected item
#if UNITY_2021_2_OR_NEWER
                float itemHeight = listView.fixedItemHeight;
#else
                float itemHeight = listView.resolvedItemHeight;
#endif
                float scrollOffset = listView.selectedIndex * itemHeight;

                // Adjust scroll offset to center the item
                float halfVisibleHeight = scrollView.contentViewport.resolvedStyle.height / 2;
                scrollOffset -= halfVisibleHeight - itemHeight / 2;

                // Ensure the scroll offset is within bounds
                scrollOffset = Mathf.Clamp(scrollOffset, 0, scrollView.contentContainer.layout.height - scrollView.contentViewport.resolvedStyle.height);

                // Apply the scroll offset
                scrollView.scrollOffset = new Vector2(0, scrollOffset);
            });
        }

        public static PropertyField CreatePropertyField(SerializedProperty serializedProperty, string label = null)
        {
            var prop = new PropertyField(serializedProperty, label);
            prop.BindProperty(serializedProperty);
            return prop;
        }
        
        public static PropertyField CreatePropertyObjectField<T>(SerializedProperty serializedProperty, string label = null, bool allowSceneObjects = true, bool allowAssets = true) where T : UnityEngine.Object
        {
            var prop = new PropertyField(serializedProperty, label);
            prop.BindProperty(serializedProperty);

            // Add a validation callback to restrict the type and scene objects
            prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>(evt =>
            {
                if (evt.newValue != null &&
                     (
                        !(evt.newValue is T) ||
                        (!allowSceneObjects && !EditorUtility.IsPersistent(evt.newValue)) ||
                        (!allowAssets && EditorUtility.IsPersistent(evt.newValue))
                     )
                   )
                {
                    // Revert the change if conditions failed.
#if !UNITY_2021_2_OR_NEWER
                    evt.PreventDefault();
#endif
                    evt.StopPropagation();
                    serializedProperty.serializedObject.Update();
                }
            });

            return prop;
        }

        public static PropertyField AddPropertyField(this VisualElement container, SerializedProperty serializedProperty, string label = null, EventCallback<ChangeEvent<UnityEngine.Object>> onChange = null, params string[] classes)
        {
            var prop = CreatePropertyField(serializedProperty, label);
            container.Add(prop);
            if (onChange != null)
            {
                container.schedule.Execute(() => prop.RegisterCallback<ChangeEvent<UnityEngine.Object>>(onChange));
            }
            foreach (var c in classes)
            {
                prop.AddToClassList(c);
            }
            return prop;
        }

        public static PropertyField AddPropertyObjectField<T>(
            this VisualElement container, SerializedProperty serializedProperty,
            string label = null, EventCallback<SerializedPropertyChangeEvent> onChange = null,
            bool allowSceneObjects = true, 
            bool allowAssets = true, 
            params string[] classes
            )
            where T : UnityEngine.Object
        {
            var prop = CreatePropertyObjectField<T>(serializedProperty, label, allowSceneObjects, allowAssets);
            container.Add(prop);
            if (onChange != null)
            {
                container.schedule.Execute(() => prop.RegisterValueChangeCallback(onChange));
            }
            foreach (var c in classes)
            {
                prop.AddToClassList(c);
            }
            return prop;
        }

        public static Button AddGenericMenuButton(
            this VisualElement container,
            string label, List<string> options, System.Action<string, int> onSelectionChanged,
            params string[] classes
            )
        {
            // Create a Dropdown button
            Button button = new Button()
            {
                text = label
            };

            // Unity 2020 UI Toolkit does not yet support DropDown, WTH?!?
            // See: https://forum.unity.com/threads/how-to-show-a-dropdownmenu.1119736/
            button.clicked += () =>
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < options.Count; i++)
                {
                    int index = i;
                    menu.AddItem(new GUIContent(options[index]), false, () => {
                        onSelectionChanged(options[index], index);
                        }); 
                }

                menu.ShowAsContext();
            };

            container.Add(button);

            return button;
        }
        
        public static ObjectField AddObjectField<T>(this VisualElement container, string label, T value, EventCallback<ChangeEvent<T>> onChange, bool allowSceneObjects = false, params string[] classes) where T : UnityEngine.Object
        {
            ObjectField objectField = new ObjectField(label)
            {
                objectType = typeof(T),
                allowSceneObjects = allowSceneObjects,
                value = value
            };

            objectField.Q<Label>().style.minWidth = 50;
            objectField.Q<Label>().style.marginRight = 5;

            foreach (var c in classes)
            {
                objectField.AddToClassList(c);
            }

            if (onChange != null)
                objectField.RegisterCallback<ChangeEvent<T>>(onChange);

            container.Add(objectField);

            return objectField;
        }

        public static VisualElement AddContainer(this VisualElement container, string name = "Container", params string[] classes)
        {
            var ctn = new VisualElement();
            ctn.name = name;
            foreach (var c in classes)
            {
                ctn.AddToClassList(c);
            }
            container.Add(ctn);
            return ctn;
        }

        public static ScrollView AddScrollView(this VisualElement container, string name = "ScrollView", params string[] classes)
        {
            var scrollView = new ScrollView();
            scrollView.name = name;
            foreach (var c in classes)
            {
                scrollView.AddToClassList(c);
            }
            container.Add(scrollView);
            return scrollView;
        }

        public static TextElement AddLabel(this VisualElement container, string text, params string[] classes)
        {
            var label = new Label(text);
            foreach (var c in classes)
            {
                label.AddToClassList(c);
            }
            container.Add(label);
            return label;
        }

        public static Toggle AddToggle(this VisualElement container, string text, EventCallback<ChangeEvent<bool>> onValueChanged, params string[] classes)
        {
            var toggle = new Toggle(text);
            if(onValueChanged != null)
                toggle.RegisterValueChangedCallback(onValueChanged);
            foreach (var c in classes)
            {
                toggle.AddToClassList(c);
            }
            container.Add(toggle);
            return toggle;
        }

        public static Toggle AddToggleLeft(this VisualElement container, string text, bool value, EventCallback<ChangeEvent<bool>> onValueChanged, params string[] classes)
        {
            var toggle = AddToggle(container, text, onValueChanged, classes);
            toggle.value = value;
            var label = toggle.Q<Label>();
            label.style.flexGrow = 1;
            toggle.style.flexDirection = FlexDirection.RowReverse;
            return toggle;
        }

        public static TextField AddTextField(this VisualElement container, EventCallback<ChangeEvent<string>> onValueChanged, params string[] classes)
        {
            var input = new TextField();
            if (onValueChanged != null)
                input.RegisterValueChangedCallback(onValueChanged);
            
            foreach (var c in classes)
            {
                input.AddToClassList(c);
            }

            container.Add(input);
            return input;
        }

        public static Button AddButton(this VisualElement container, string text, EventCallback<ClickEvent> onClick, params string[] classes)
        {
            var button = new Button();
            button.text = text;
            if(onClick != null)
                button.RegisterCallback<ClickEvent>(onClick);
            foreach (var c in classes)
            {
                button.AddToClassList(c);
            }
            container.Add(button);
            return button;
        }

        public static TextElement AddHeader(this VisualElement container, string propertyPath, string text, float marginTop = 10, bool bold = true)
        {
            var label = new Label(text);
            if (bold)
                label = label.Bold();
            var propField = FindPropertyField(container, propertyPath);
            InsertBefore(label, propField);

            label.style.marginTop = marginTop;
            label.style.marginBottom = Mathf.RoundToInt(marginTop * 0.3f);
            return label;
        }

        public static void HideProperty(this VisualElement container, string propertyPath)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.display = DisplayStyle.None;
        }

        public static void ShowProperty(this VisualElement container, string propertyPath)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.display = DisplayStyle.Flex;
        }

        public static void SetPropertyDisplay(this VisualElement container, string propertyPath, bool display)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public static void SetPropertyEnabled(this VisualElement container, string propertyPath, bool enabled)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.SetEnabled(enabled);
        }

        public static void SetPropertyMarginTop(this VisualElement container, string propertyPath, float marginTop)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.style.marginTop = marginTop;
        }

        public static void SetPropertyTooltip(this VisualElement container, string propertyPath, string tooltip)
        {
            var propField = FindPropertyField(container, propertyPath);
            propField.tooltip = tooltip;
        }

        public static T Bold<T>(this T te) where T : TextElement
        {
            te.style.unityFontStyleAndWeight = FontStyle.Bold;
            return te;
        }

        public static T Color<T>(this T te, Color color) where T : TextElement
        {
            te.style.color = color;
            return te;
        }

        public static T Red<T>(this T te) where T : TextElement
        {
            te.style.color = UnityEngine.Color.red;
            return te;
        }

        public static T Green<T>(this T te) where T : TextElement
        {
            te.style.color = UnityEngine.Color.green;
            return te;
        }

        public static T Blue<T>(this T te) where T : TextElement
        {
            te.style.color = UnityEngine.Color.blue;
            return te;
        }

        public static T Wrap<T>(this T te) where T : TextElement
        {
            te.style.whiteSpace = WhiteSpace.Normal;
            return te;
        }

        public static T Padding<T>(this T te, float padding) where T : TextElement
        {
            te.style.paddingTop = padding;
            te.style.paddingRight = padding;
            te.style.paddingBottom = padding;
            te.style.paddingLeft = padding;
            return te;
        }

        public static T Margin<T>(this T te, float margin) where T : TextElement
        {
            te.style.marginTop = margin;
            te.style.marginRight = margin;
            te.style.marginBottom = margin;
            te.style.marginLeft = margin;
            return te;
        }

        public static T Margin<T>(this T te, float marginsVertical, float marginsHorizontal) where T : TextElement
        {
            te.style.marginTop = marginsVertical;
            te.style.marginRight = marginsHorizontal;
            te.style.marginBottom = marginsVertical;
            te.style.marginLeft = marginsHorizontal;
            return te;
        }

        public static T Background<T>(this T te, Color color, float roundCornerWidth = 0f) where T : TextElement
        {
            te.style.backgroundColor = color;
            if (roundCornerWidth != 0f)
            {
                te.style.borderTopRightRadius = roundCornerWidth;
                te.style.borderTopLeftRadius = roundCornerWidth;
                te.style.borderBottomLeftRadius = roundCornerWidth;
                te.style.borderBottomRightRadius = roundCornerWidth;
            }
            return te;
        }

        public static VisualElement FindPropertyField(this VisualElement ve, string propertyPath)
        {
            return ve.Query<PropertyField>().Where(v => v.bindingPath == propertyPath).First();
        }

        public static void RemovePropertyField(this VisualElement ve, string propertyPath)
        {
            var field = ve.Query<PropertyField>().Where(v => v.bindingPath == propertyPath).First();
            if (field != null)
                ve.Remove(field);
        }

        public static void InsertBefore(VisualElement newElement, VisualElement anchorElement)
        {
            InsertBefore(anchorElement.parent, newElement, anchorElement);
        }

        public static void InsertBefore(this VisualElement container, VisualElement newElement, VisualElement anchorElement)
        {
            container.Insert(container.IndexOf(anchorElement), newElement);
        }

        public static void InsertAfter(VisualElement newElement, VisualElement anchorElement)
        {
            InsertAfter(anchorElement.parent, newElement, anchorElement);
        }

        public static void InsertAfter(this VisualElement container, VisualElement newElement, VisualElement anchorElement)
        {
            container.Insert(container.IndexOf(anchorElement) + 1, newElement);
        }

        public static VisualElement InsertBeforeProperty(this VisualElement container, VisualElement newElement, string anchorPropertyPath)
        {
            return InsertRelativeToProperty(container, newElement, anchorPropertyPath, 0);
        }

        public static VisualElement InsertAfterProperty(this VisualElement container, VisualElement newElement, string AnchorPropertyPath)
        {
            return InsertRelativeToProperty(container, newElement, AnchorPropertyPath, 1);
        }

        public static VisualElement InsertRelativeToProperty(this VisualElement container, VisualElement newElement, string anchorPropertyPath, int indexDelta)
        {
            var sibling = container.FindPropertyField(anchorPropertyPath);
            if (sibling == null)
                container.Add(newElement);
            else
                container.Insert(container.IndexOf(sibling) + indexDelta, newElement);

            return newElement;
        }

        public static VisualElement PlaceBeforeProperty(this VisualElement container, string propertyPath, VisualElement element)
        {
            return PlaceRelativeToProperty(container, propertyPath, element, -1);
        }

        public static VisualElement PlaceAfterProperty(this VisualElement container, string propertyPath, VisualElement element)
        {
            return PlaceRelativeToProperty(container, propertyPath, element, 0);
        }

        public static VisualElement PlaceRelativeToProperty(this VisualElement container, string propertyPath, VisualElement element, int indexDelta)
        {
            var sibling = container.FindPropertyField(propertyPath);
            if (sibling == null)
                container.Add(element);
            else
                container.Insert(container.IndexOf(sibling) + indexDelta, element);

            return element;
        }

        public static VisualElement AddTabbar(this VisualElement root, List<string> tabs, List<VisualElement> contents, int activeTab = 0, System.Action<Button, int, List<Button>> onTabButtonPressed = null, List<Button> tabButtons = null)
        {
            var tabBar = new VisualElement();
            tabBar.AddToClassList("tab-bar");
            tabBar.style.flexDirection = FlexDirection.Row;

            if (tabButtons == null)
                tabButtons = new List<Button>();

            for (int i = 0; i < tabs.Count; i++)
            {
                string tabName = tabs[i];

                var button = new Button() { text = tabName };
                button.AddToClassList("tab-button");
                button.style.flexGrow = 1;

                if(i == activeTab)
                {
                    button.AddToClassList("active");
                }

                tabButtons.Add(button);

                // Add button and content to their respective containers
                tabBar.Add(button);
            }

            for (int i = 0; i < tabs.Count; i++)
            {
                int index = i;
                var button = tabButtons[i];
                button.clicked += () =>
                {
                    // Hide all contents
                    foreach (var content in contents)
                    {
                        content.style.display = DisplayStyle.None;
                    }

                    // Show the clicked tab content
                    contents[index].style.display = DisplayStyle.Flex;

                    foreach (var btn in tabButtons)
                    {
                        btn.RemoveFromClassList("active");
                    }
                    button.AddToClassList("active");

                    // Call button pressed
                    onTabButtonPressed?.Invoke(button, index, tabButtons);
                };
            }

            root.Add(tabBar);

            return tabBar;

        }
    }
}
#endif
