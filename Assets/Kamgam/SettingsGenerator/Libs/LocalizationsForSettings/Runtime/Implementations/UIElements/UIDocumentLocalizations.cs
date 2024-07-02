// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.LocalizationForSettings.UIElements
{
    public class UIDocumentLocalizations : MonoBehaviour
    {
        public static int ParentLevelsToSearch = 3;

        public LocalizationProvider LocalizationProvider;

        protected UIDocument _document;
        public UIDocument Document
        {
            get
            {
                if (_document == null)
                {
                    _document = GetComponentInParent<UIDocument>();
                }

                return _document;
            }
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/Settings Generator/Create Localizations", priority = 1001)]
        public static void CreateOrUpdateLocalizationsForSelected()
        {
            var go = Selection.activeGameObject;
            var documentResolver = GetOrCreateLocalizationsRoot(go);
            if (documentResolver != null)
            {
                documentResolver.CreateOrUpdateLocalizers();
            }
        }

        [MenuItem("GameObject/Settings Generator/Create Localizations", priority = 1001, validate = true)]
        public static bool CreateOrUpdateLocalizationsForSelectedValidate()
        {
            if (Selection.activeGameObject == null)
                return false;

            return Selection.activeGameObject.GetComponent<UIDocument>() != null;
        }
#endif

        public void CreateOrUpdateLocalizers()
        {
            Debug.Log("Creating localizations on UIDocument.");

            if (Document == null)
            {
                Debug.LogError("No UIDocument found: There is no UIDocument Component on the selected object -> aborting.");
                return;
            }

#if UNITY_EDITOR
            _EditorFindAndAssignProviders();
#endif

            // Destroy all
            var localizers = Document.transform.GetComponentsInChildren<LocalizeBase>();
            for (int i = localizers.Length - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    DestroyImmediate(localizers[i].gameObject);
                }
                else
                {
                    Destroy(localizers[i].gameObject);
                }
#else
                Destroy(localizers[i].gameObject);
#endif
            }

            // Create Resolvers for types
            int createdLocalizersCount = 0;
            var uniqueClassNames = new List<string>();
            createdLocalizersCount += createOrUpdateLocalizer<Label, LocalizeLabel>(uniqueClassNames);

            // Log results
            Debug.Log("Created " + createdLocalizersCount + " localizer on UIDocument.");
            if (createdLocalizersCount == 0)
            {
                Debug.LogWarning("Please add a class name starting with '" + LocalizeVisualElement.LocalizationClassNamePrefix + "' to each element (or parent) that you wish to mark as localizable.\nDon't forget to assign a TERM to the localizer afterwards.");
            }
        }

        public static UIDocumentLocalizations GetOrCreateLocalizationsRoot(GameObject gameObjectWithUIDocument)
        {
            var document = gameObjectWithUIDocument.GetComponent<UIDocument>();
            if (document == null)
                return null;

            string name = "Localizations";
            var root = gameObjectWithUIDocument.transform.Find(name);
            if(root == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(document.gameObject.transform);
                go.transform.rotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;

                root = go.transform;
            }

            var localizer = root.GetComponentInChildren<UIDocumentLocalizations>();
            if (localizer == null)
            {
                localizer = root.gameObject.AddComponent<UIDocumentLocalizations>();
            }

            return localizer;
        }

        int createOrUpdateLocalizer<TVisualElement, TLocalizer>(List<string> uniqueClassNames)
            where TVisualElement : VisualElement
            where TLocalizer : LocalizeVisualElement
        {
            var elements = Document.rootVisualElement.Query<VisualElement>().Build();

            // recreate
            int createdResolverCount = 0;
            foreach (var element in elements)
            {
                // Skip elements which are not marked for localization.
                if (!LocalizeVisualElement.HasLocalizationClass(element))
                    continue;

                // Skip duplicate class names
                var className = LocalizeVisualElement.GetLocalizationClassName(element);
                if (uniqueClassNames.Contains(className))
                {
                    Debug.LogError("The class name '" + className + "' on '" + element.name + "' has already been used. Skipping '" + element.name + "'.");
                    continue;
                }
                uniqueClassNames.Add(className);

                // Create
                CreateGameObjectWithLocalizer<TVisualElement, TLocalizer>(element);
                createdResolverCount++;
            }

            return createdResolverCount;
        }

        /// <summary>
        /// Adds a gameobject with a localizer as a child to the UIDocument component.
        /// </summary>
        /// <typeparam name="TVisualElement"></typeparam>
        /// <typeparam name="TLocalizer"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public TLocalizer CreateGameObjectWithLocalizer<TVisualElement, TLocalizer>(VisualElement element)
            where TVisualElement : VisualElement
            where TLocalizer : LocalizeVisualElement
        {
            string name = typeof(TLocalizer).Name + " (" + LocalizeVisualElement.GetLocalizationClassName(element) + ")";
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            go.transform.rotation = Quaternion.identity;
            go.transform.localPosition = Vector3.zero;

            var localizer = go.AddComponent<TLocalizer>();
            localizer.BindTo(element);

            if (LocalizationProvider != null)
                localizer.LocalizationProvider = LocalizationProvider;

            return localizer;
        }

#if UNITY_EDITOR
        public void _EditorFindAndAssignProviders()
        {
            // Auto select if provider is null
            if (LocalizationProvider == null)
            {
                string[] localizationsGUIDs = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(LocalizationProvider).Name);
                if (localizationsGUIDs.Length > 0)
                {
                    LocalizationProvider = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizationProvider>(UnityEditor.AssetDatabase.GUIDToAssetPath(localizationsGUIDs[0]));
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}

#endif