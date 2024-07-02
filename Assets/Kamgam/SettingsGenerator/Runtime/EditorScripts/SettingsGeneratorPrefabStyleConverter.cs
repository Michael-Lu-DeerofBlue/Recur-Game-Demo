#if UNITY_EDITOR
using Kamgam.LocalizationForSettings;
using Kamgam.UGUIComponentsForSettings;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.SettingsGenerator
{
    public static class SettingsGeneratorPrefabStyleConverter
    {
        static Selectable[] _selectables = new Selectable[10];

        public static string OptionsButtonPrefabName = "OptionsButtonUGUI (Setting)";
        public static string DropDownPrefabName = "DropDownUGUI (Setting)";
        public static string DropDownWithLabelPrefabName = "DropdownUGUIWithLabel (Setting)";
        public static string OptionsButtonConsolePrefabName = "OptionsButtonConsoleUGUI (Setting)";

        public static string SliderPrefabName = "SliderUGUI (Setting)";
        public static string SliderConsolePrefabName = "SliderConsoleUGUI (Setting)";
        
        public static string TogglePrefabName = "ToggleUGUI (Setting)";
        public static string ToggleConsolePrefabName = "ToggleConsoleUGUI (Setting)";
        
        public static string InputKeyPrefabName = "InputKeyUGUI (Setting)";
        public static string InputKeyConsolePrefabName = "InputKeyConsoleUGUI (Setting)";

        public static string StepperPrefabName = "StepperUGUI (Setting)";
        public static string StepperConsolePrefabName = "StepperUGUI (Setting)";
        public static string StepperTwoButtonsConsolePrefabName = "StepperTwoButtonsConsoleUGUI (Setting)";
        public static string StepperOneButtonConsolePrefabName = "StepperOneButtonConsoleUGUI (Setting)";
        public static string StepperOneButtonStepsConsolePrefabName = "StepperOneButtonStepsConsoleUGUI (Setting)";

        public static string TextfieldPrefabName = "TextfieldUGUI (Setting)";
        public static string TextfieldConsoleUGUIPrefabName = "TextfieldConsoleUGUI (Setting)";

        public static string ColorPickerPrefabName = "ColorPickerUGUI (Setting)";
        public static string ColorPickerConsolePrefabName = "ColorPickerConsoleUGUI (Setting)";

        static void getBasicInfo(GameObject go, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab)
        {
            resolver = go.GetComponentInChildren<SettingResolver>();
            // Search in parent too
            if (resolver == null)
            {
                resolver = go.GetComponentInParent<SettingResolver>();
                if (resolver != null)
                    go = resolver.gameObject;
            }
            isPrefab = PrefabUtility.GetPrefabInstanceStatus(go) != PrefabInstanceStatus.NotAPrefab;
            // We assume the resolver or the prefab is the root of the UI element.
            root = isPrefab ? PrefabUtility.GetNearestPrefabInstanceRoot(go) : resolver.gameObject;
            id = resolver.GetID();
            settingsProvider = resolver.SettingsProvider;
        }

        static GameObject createUI<T>(string id, SettingsProvider settingsProvider, GameObject oldUI, string prefabName, bool undo = true)
            where T : SettingResolver
        {
            // Instantiate new ui
            var prefab = loadPrefab(prefabName);
            var newUI = PrefabUtility.InstantiatePrefab(prefab, oldUI.transform.parent) as GameObject;
            var oldRect = (oldUI.transform as RectTransform);
            var newRect = (newUI.transform as RectTransform);
            newRect.pivot = oldRect.pivot;
            newRect.anchorMin = oldRect.anchorMin;
            newRect.anchorMax = oldRect.anchorMax;
            newRect.anchoredPosition = oldRect.anchoredPosition;
            newRect.sizeDelta = oldRect.sizeDelta;

            // Replace "(Setting)" with any existing text inside brackets like "(Quality)".
            if (newUI.name.Contains("(Setting)"))
            {
                var matchesInOldName = Regex.Matches(oldUI.name, @"\([^()]*\)");
                var matchesInNewName = Regex.Matches(newUI.name, @"\([^()]*\)");
                if (matchesInOldName.Count > 0)
                {
                    Match lastOldMatch = matchesInOldName[matchesInNewName.Count - 1];
                    if (matchesInNewName.Count > 0)
                    {
                        Match lastNewMatch = matchesInNewName[matchesInNewName.Count - 1];
                        newUI.name = newUI.name.Substring(0, lastNewMatch.Index) + lastOldMatch.Value + newUI.name.Substring(lastNewMatch.Index + lastNewMatch.Length);
                    }
                    else
                    {
                        newUI.name = newUI.name + " " + lastOldMatch.Value;
                    }
                }
            }

            if (undo)
            {
                Undo.RegisterCreatedObjectUndo(newUI, "SettingsGeneratorPrefabStyleConverter.Create");
                Undo.RegisterFullObjectHierarchyUndo(newUI, "SettingsGeneratorPrefabStyleConverter.Modify");
            }

            // Assign resolver
            var newResolver = newUI.GetComponent<T>();
            newResolver.SettingsProvider = settingsProvider;
            newResolver.ID = id;

            // Copy label
            var oldLabel = findLabel(oldUI);
            var newLabel = findLabel(newUI);
            if (oldLabel != null && newLabel != null)
            {
                newLabel.text = oldLabel.text;
            }

            // Copy Navigation Overrides
            var oldNavigation = oldUI.GetComponentInChildren<AutoNavigationOverrides>();
            var newNavigation = newUI.GetComponentInChildren<AutoNavigationOverrides>();
            if (oldNavigation != null && newNavigation != null)
            {
                newNavigation.BlockUp = oldNavigation.BlockUp;
                newNavigation.BlockDown = oldNavigation.BlockDown;
                newNavigation.BlockLeft = oldNavigation.BlockLeft;
                newNavigation.BlockRight = oldNavigation.BlockRight;

                if(oldNavigation.SelectOnUpOverride != null && !oldNavigation.SelectOnUpOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnUpOverride = oldNavigation.SelectOnUpOverride;

                if (oldNavigation.SelectOnDownOverride != null && !oldNavigation.SelectOnDownOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnDownOverride = oldNavigation.SelectOnDownOverride;

                if (oldNavigation.SelectOnLeftOverride != null && !oldNavigation.SelectOnLeftOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnLeftOverride = oldNavigation.SelectOnLeftOverride;

                if (oldNavigation.SelectOnRightOverride != null && !oldNavigation.SelectOnRightOverride.transform.IsChildOf(oldNavigation.transform))
                    newNavigation.SelectOnRightOverride = oldNavigation.SelectOnRightOverride;

                newNavigation.DisableOnAwakeIfNotNeeded = oldNavigation.DisableOnAwakeIfNotNeeded;
            }

            // Patch other navigations and overrides that point to this selectable
            if (Selectable.allSelectableCount > _selectables.Length)
            {
                _selectables = new Selectable[Selectable.allSelectableCount];
            }
            for (int i = 0; i < _selectables.Length; i++)
            {
                _selectables[i] = null;
            }
            Selectable.AllSelectablesNoAlloc(_selectables);
            for (int i = 0; i < Selectable.allSelectableCount; i++)
            {
                var nav = _selectables[i].gameObject.GetComponent<AutoNavigationOverrides>();
                if (nav == null)
                    continue;

                if (nav.SelectOnUpOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnUpOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }

                if (nav.SelectOnDownOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnDownOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }

                if (nav.SelectOnLeftOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnLeftOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }

                if (nav.SelectOnRightOverride == oldUI.GetComponentInChildren<Selectable>())
                {
                    Undo.RegisterCompleteObjectUndo(nav, "Assigned new selectables");
                    nav.SelectOnRightOverride = newUI.GetComponentInChildren<Selectable>();
                    EditorUtility.SetDirty(nav);
                }
            }

            // Move to position of old ui
            newUI.transform.SetSiblingIndex(oldUI.transform.GetSiblingIndex());

            Selection.objects = new GameObject[] { newUI };

            return newUI;
        }

        static GameObject loadPrefab(string prefabName)
        {
            var guids = AssetDatabase.FindAssets("t:Prefab " + prefabName);
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            return null;
        }

        static TextMeshProUGUI findLabel(GameObject go)
        {
            var loc = go.GetComponentInChildren<LocalizeTMPro>();
            if (loc != null)
                return loc.Textfield;
            return null;
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/OptionsButton", priority = 2001)]
        public static void ReplaceWithOptionsButton()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithOptionsButton(go);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/OptionsButton Console", priority = 2001)]
        public static void ReplaceWithConsoleOptionsButtonConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithOptionsButtonConsole(go);
            }
        }

        private static void replaceWithOptionsButtonConsole(GameObject target)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var optionsButtonUGUIResolver = resolver as OptionsButtonUGUIResolver;
            var dropDownUGUIResolver = resolver as DropDownUGUIResolver;

            if (optionsButtonUGUIResolver == null && dropDownUGUIResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (dropDownUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonConsolePrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var dropDown = dropDownUGUIResolver.GetComponent<TMP_Dropdown>();
                var options = dropDown.options;
                newGUI.SetOptions(options.Select(o => o.text).ToList());
                Undo.DestroyObjectImmediate(root);
            }
            else if (optionsButtonUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonConsolePrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var oldGUI = optionsButtonUGUIResolver.gameObject.GetComponent<OptionsButtonUGUI>();
                newGUI.SetOptions(oldGUI.GetOptions());
                Undo.DestroyObjectImmediate(root);
            }
        }

        private static void replaceWithOptionsButton(GameObject target)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var optionsButtonUGUIResolver = resolver as OptionsButtonUGUIResolver;
            var dropDownUGUIResolver = resolver as DropDownUGUIResolver;

            if (optionsButtonUGUIResolver == null && dropDownUGUIResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (dropDownUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonPrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var dropDown = dropDownUGUIResolver.GetComponent<TMP_Dropdown>();
                var options = dropDown.options;
                var oldGUI = optionsButtonUGUIResolver.gameObject.GetComponent<OptionsButtonUGUI>();
                newGUI.SetOptions(options.Select(o => o.text).ToList());
                newGUI.Loop = oldGUI.Loop;
                newGUI.EnableButtonControls = oldGUI.EnableButtonControls;
                Undo.DestroyObjectImmediate(root);
            }
            else if (optionsButtonUGUIResolver != null)
            {
                GameObject newGo = createUI<OptionsButtonUGUIResolver>(id, settingsProvider, root, OptionsButtonPrefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<OptionsButtonUGUI>();
                var oldGUI = optionsButtonUGUIResolver.gameObject.GetComponent<OptionsButtonUGUI>();
                newGUI.SetOptions(oldGUI.GetOptions());
                newGUI.Loop = oldGUI.Loop;
                newGUI.EnableButtonControls = oldGUI.EnableButtonControls;
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/DropDown", priority = 2001)]
        public static void ReplaceWithDropDown()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithDropDown(go, DropDownPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/DropDownWithLabel", priority = 2001)]
        public static void ReplaceWithDropDownWithLabel()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithDropDown(go, DropDownWithLabelPrefabName);
            }
        }

        private static void replaceWithDropDown(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var optionsButtonUGUIResolver = resolver as OptionsButtonUGUIResolver;

            if (optionsButtonUGUIResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (optionsButtonUGUIResolver != null)
            {
                GameObject newGo = createUI<DropDownUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<DropDownUGUI>();
                var oldGUI = optionsButtonUGUIResolver.gameObject.GetComponent<OptionsButtonUGUI>();
                newGUI.SetOptions(oldGUI.GetOptions());
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Slider", priority = 2001)]
        public static void ReplaceWithSlider()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithSlider(go, SliderPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Slider Console", priority = 2001)]
        public static void ReplaceWithSliderConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithSlider(go, SliderConsolePrefabName);
            }
        }

        private static void replaceWithSlider(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var sliderResolver = resolver as SliderUGUIResolver;

            if (sliderResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (sliderResolver != null)
            {
                GameObject newGo = createUI<SliderUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<SliderUGUI>();
                var oldGUI = sliderResolver.gameObject.GetComponent<SliderUGUI>();
                newGUI.WholeNumbers = oldGUI.WholeNumbers;
                newGUI.MinValue = oldGUI.MinValue;
                newGUI.MaxValue = oldGUI.MaxValue;
                newGUI.Value = oldGUI.Value;
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Toggle Console", priority = 2001)]
        public static void ReplaceWithToggleConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithToggle(go, ToggleConsolePrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Toggle", priority = 2001)]
        public static void ReplaceWithToggle()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithToggle(go, TogglePrefabName);
            }
        }

        private static void replaceWithToggle(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var toggleResolver = resolver as ToggleUGUIResolver;

            if (toggleResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (toggleResolver != null)
            {
                GameObject newGo = createUI<ToggleUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<ToggleUGUI>();
                var oldGUI = toggleResolver.gameObject.GetComponent<ToggleUGUI>();
                newGUI.Value = oldGUI.Value;
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/InputKey Console", priority = 2001)]
        public static void ReplaceWithInputBindingConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithInputKey(go, InputKeyConsolePrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/InputKey", priority = 2001)]
        public static void ReplaceWithInputKey()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithInputKey(go, InputKeyPrefabName);
            }
        }

        private static void replaceWithInputKey(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var toggleResolver = resolver as InputKeyUGUIResolver;

            if (toggleResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (toggleResolver != null)
            {
                GameObject newGo = createUI<InputKeyUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<InputKeyUGUI> ();
                var oldGUI = toggleResolver.gameObject.GetComponent<InputKeyUGUI>();
                newGUI.Key = oldGUI.Key;
                newGUI.ModifierKey = oldGUI.ModifierKey;
                Undo.DestroyObjectImmediate(root);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Stepper", priority = 2001)]
        public static void ReplaceWithStepper()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithStepper(go, StepperPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/StepperTwoButtons Console", priority = 2001)]
        public static void ReplaceWithStepperTwoButtonsConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithStepper(go, StepperTwoButtonsConsolePrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/StepperOneButton Console", priority = 2001)]
        public static void ReplaceWithStepperOneButtonConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithStepper(go, StepperOneButtonConsolePrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/StepperOneButtonSteps Console", priority = 2001)]
        public static void StepperOneButtonStepsConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithStepper(go, StepperOneButtonStepsConsolePrefabName);
            }
        }

        private static void replaceWithStepper(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var stepperResolver = resolver as StepperUGUIResolver;

            if (stepperResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (stepperResolver != null)
            {
                GameObject newGo = createUI<StepperUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<StepperUGUI>();
                var oldGUI = stepperResolver.gameObject.GetComponent<StepperUGUI>();
                newGUI.DisableButtons = oldGUI.DisableButtons;
                newGUI.EnableButtonControls = oldGUI.EnableButtonControls;
                newGUI.ValueFormat = oldGUI.ValueFormat;
                newGUI.WholeNumbers = oldGUI.WholeNumbers;
                newGUI.StepSize = oldGUI.StepSize;
                newGUI.MinValue = oldGUI.MinValue;
                newGUI.MaxValue = oldGUI.MaxValue;
                newGUI.Value = oldGUI.Value;
                newGUI.Refresh();
                Undo.DestroyObjectImmediate(root);
            }
        }


        [MenuItem("GameObject/UI/Settings Generator/Convert To/Texfield", priority = 2001)]
        public static void ReplaceWithTextfield()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithTextfield(go, TextfieldPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/Textfield Console", priority = 2001)]
        public static void ReplaceWithTextfieldConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithTextfield(go, TextfieldConsoleUGUIPrefabName);
            }
        }

        private static void replaceWithTextfield(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var textfieldResolver = resolver as TextfieldUGUIResolver;

            if (textfieldResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (textfieldResolver != null)
            {
                GameObject newGo = createUI<TextfieldUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<TextfieldUGUI>();
                var oldGUI = textfieldResolver.gameObject.GetComponent<TextfieldUGUI>();
                newGUI.Text = oldGUI.Text;
                Undo.DestroyObjectImmediate(root);
            }
        }


        [MenuItem("GameObject/UI/Settings Generator/Convert To/ColorPicker", priority = 2001)]
        public static void ReplaceWithColorPicker()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithColorPicker(go, ColorPickerPrefabName);
            }
        }

        [MenuItem("GameObject/UI/Settings Generator/Convert To/ColorPicker Console", priority = 2001)]
        public static void ReplaceWithColorPickerConsole()
        {
            if (Selection.gameObjects.Length == 0)
                return;

            var objects = Selection.gameObjects;
            foreach (var go in objects)
            {
                replaceWithColorPicker(go, ColorPickerConsolePrefabName);
            }
        }

        private static void replaceWithColorPicker(GameObject target, string prefabName)
        {
            getBasicInfo(target, out string id, out SettingsProvider settingsProvider, out SettingResolver resolver, out GameObject root, out bool isPrefab);

            // Can we convert?
            var textfieldResolver = resolver as ColorPickerUGUIResolver;

            if (textfieldResolver == null)
            {
                Logger.LogWarning("Conversion not possible.");
            }

            if (textfieldResolver != null)
            {
                GameObject newGo = createUI<ColorPickerUGUIResolver>(id, settingsProvider, root, prefabName, undo: true);

                // Copy data
                var newGUI = newGo.GetComponent<ColorPickerUGUI>();
                var oldGUI = textfieldResolver.gameObject.GetComponent<ColorPickerUGUI>();
                newGUI.SelectedIndex = oldGUI.SelectedIndex;
                var oldColorButtons = oldGUI.GetComponentsInChildren<ColorPickerButtonUGUI>(includeInactive: true);
                var newColorButtons = newGUI.GetComponentsInChildren<ColorPickerButtonUGUI>(includeInactive: true);
                int range = Mathf.Max(oldColorButtons.Length, newColorButtons.Length);
                var template = newColorButtons[0];
                var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(template);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                for (int i = 0; i < range; i++)
                {
                    if (i < oldColorButtons.Length && i < newColorButtons.Length)
                    {
                        // Copy color
                        Undo.RegisterCompleteObjectUndo(newColorButtons[i], "SettingsGeneratorPrefabStyleConverter.Create.CopyColor");
                        newColorButtons[i].Color = oldColorButtons[i].Color;
                    }
                    else if(i < oldColorButtons.Length && i >= newColorButtons.Length)
                    {
                        // Add new instance
                        var newColorObject = PrefabUtility.InstantiatePrefab(prefab, template.transform.parent) as GameObject;
                        PrefabUtility.SetPropertyModifications(newColorObject, PrefabUtility.GetPropertyModifications(template));
                        var newColorButton = newColorObject.GetComponent<ColorPickerButtonUGUI>();
                        newColorButton.Color = oldColorButtons[i].Color;

                        Undo.RegisterCreatedObjectUndo(newColorObject, "SettingsGeneratorPrefabStyleConverter.Create.ColorButton");
                        Undo.RegisterFullObjectHierarchyUndo(newColorObject, "SettingsGeneratorPrefabStyleConverter.Modify.ColorButton");
                    }
                }

                Undo.DestroyObjectImmediate(root);
            }
        }

    }
}
#endif