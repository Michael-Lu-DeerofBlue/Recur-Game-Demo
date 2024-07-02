// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Kamgam.LocalizationForSettings;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kamgam.SettingsGenerator
{
    public class UIDocumentSettingsResolver : MonoBehaviour
    {
        public SettingsProvider SettingsProvider;
        public LocalizationProvider LocalizationProvider;

        /// <summary>
        /// Signature of custom resolver creation method.<br />
        /// Hint: Use documentResolver.CreateGameObjectWithResolver() to create your object.
        /// </summary>
        /// <param name="documentResolver">The UIDocumentResolver component.</param>
        /// <param name="element">Each element in the visual tree. It's your responsibility to filter out what you don't need.</param>
        /// <param name="uniqueClassNames">List of already used unique settings USS class names. You can add to it or use it as a predicate.</param>
        /// <returns>Null if no resolver has been created.</returns>
        public delegate SettingResolverForVisualElement CreateResolverDelegate(UIDocumentSettingsResolver documentResolver, VisualElement element, List<string> uniqueClassNames);

        /// <summary>
        /// If set then this is called once on every element in the visual tree within the CreateOrUpdateResolvers() method.<br />
        /// Use it to add your own custom resolvers.
        /// </summary>
        [System.NonSerialized]
        public CreateResolverDelegate CustomCreateResolverMethod;

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
        [MenuItem("GameObject/Settings Generator/Create Resolvers", priority = 1001)]
        public static void CreateOrUpdateResolversForSelected()
        {
            var go = Selection.activeGameObject;
            var documentResolver = GetOrCreateResolversRoot(go);
            if (documentResolver != null)
            {
                documentResolver.CreateOrUpdateResolvers();
            }
        }

        [MenuItem("GameObject/Settings Generator/Create Resolvers", priority = 1001, validate = true)]
        public static bool CreateOrUpdateResolversForSelectedValidate()
        {
            if (Selection.activeGameObject == null)
                return false;

            return Selection.activeGameObject.GetComponent<UIDocument>() != null;
        }
#endif

        public void CreateOrUpdateResolvers()
        {
            Logger.Log("Creating resolver on UIDocument.");

            if (Document == null)
            {
                Logger.LogError("No UIDocument found: There is no UIDocument Component on the selected object -> aborting.");
                return;
            }

#if UNITY_EDITOR
            _EditorFindAndAssignProviders();
#endif

            // Destroy all
            var resolvers = Document.transform.GetComponentsInChildren<SettingResolverForVisualElement>();
            for (int i = resolvers.Length - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    DestroyImmediate(resolvers[i].gameObject);
                }
                else
                {
                    Destroy(resolvers[i].gameObject);
                }
#else
                Destroy(resolvers[i].gameObject);
#endif
            }

            // Create Resolvers for types
            int createdResolverCount = 0;
            var uniqueClassNames = new List<string>();
            createdResolverCount += createOrUpdateResolvers<Toggle, ToggleUIElementResolver>(uniqueClassNames);
            createdResolverCount += createOrUpdateResolvers<DropdownField, DropdownFieldUIElementResolver>(uniqueClassNames);
            createdResolverCount += createOrUpdateResolvers<Slider, SliderUIElementResolver>(uniqueClassNames);
            createdResolverCount += createOrUpdateResolvers<TextField, TextFieldUIElementResolver>(uniqueClassNames);

            // Custom resolvers
            if (CustomCreateResolverMethod != null)
            {
                createdResolverCount += createOrUpdateCustomResolvers(uniqueClassNames);
            }

            // Log results
            Logger.LogMessage("Created " + createdResolverCount + " resolvers on UIDocument.");
            if (createdResolverCount == 0)
            {
                Logger.LogWarning("Please add a class name starting with '" + SettingResolverForVisualElement.SettingsClassNamePrefix + "' to each element that you wish to mark as a setting.\nDon't forget to assign Settings IDs to the resolvers afterwards.");
            }
        }

        public static UIDocumentSettingsResolver GetOrCreateResolversRoot(GameObject gameObjectWithUIDocument)
        {
            var document = gameObjectWithUIDocument.GetComponent<UIDocument>();
            if (document == null)
                return null;

            string name = "SettingResolvers";
            var root = gameObjectWithUIDocument.transform.Find(name);
            if(root == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(document.gameObject.transform);
                go.transform.rotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;

                root = go.transform;
            }

            var resolver = root.GetComponentInChildren<UIDocumentSettingsResolver>();
            if (resolver == null)
            {
                resolver = root.gameObject.AddComponent<UIDocumentSettingsResolver>();
            }

            return resolver;
        }

        int createOrUpdateResolvers<TVisualElement, TResolver>(List<string> uniqueClassNames)
            where TVisualElement : VisualElement
            where TResolver : SettingResolverForVisualElement
        {
            if (Document == null || Document.rootVisualElement == null)
            {
                Logger.LogWarning("No root for document found. Maybe it's disabled or you are in PrefabMode?");
                return 0;
            }

            var elements = Document.rootVisualElement.Query<TVisualElement>().Build();

            // recreate
            int createdResolverCount = 0;
            foreach (var element in elements)
            {
                // Skip elements which are not settings.
                if (!SettingResolverForVisualElement.HasSettingClass(element))
                    continue;

                // Skip duplicate class names
                var className = SettingResolverForVisualElement.GetSettingClassName(element);
                if (uniqueClassNames.Contains(className))
                {
                    Logger.LogError("The class name '" + className + "' on '" + element.name + "' has already been used. Skipping '" + element.name + "'.");
                    continue;
                }
                uniqueClassNames.Add(className);

                // Create
                CreateGameObjectWithResolver<TVisualElement, TResolver>(element);
                createdResolverCount++;
            }

            return createdResolverCount;
        }

        int createOrUpdateCustomResolvers(List<string> uniqueClassNames)
        {
            if (CustomCreateResolverMethod == null)
                return 0;

            var elements = Document.rootVisualElement.Query<VisualElement>().Build();

            // recreate
            int createdResolverCount = 0;
            foreach (var element in elements)
            {
                // Create
                var resolver = CustomCreateResolverMethod.Invoke(this, element, uniqueClassNames);
                if (resolver != null)
                    createdResolverCount++;
            }

            return createdResolverCount;
        }

        /// <summary>
        /// Adds a gameobject with a resolver as a chld to the UIDocument component.<br />
        /// </summary>
        /// <typeparam name="TUIElement"></typeparam>
        /// <typeparam name="TResolver"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public TResolver CreateGameObjectWithResolver<TVisualElement, TResolver>(TVisualElement element)
            where TVisualElement : VisualElement
            where TResolver : SettingResolverForVisualElement
        {
            string name = typeof(TResolver).Name + " (" + SettingResolverForVisualElement.GetSettingClassName(element) + ")";
            var resolverGO = new GameObject(name);
            resolverGO.transform.SetParent(transform);
            resolverGO.transform.rotation = Quaternion.identity;
            resolverGO.transform.localPosition = Vector3.zero;

            var resolver = resolverGO.AddComponent<TResolver>();
            resolver.BindTo(element);

            if (SettingsProvider != null)
                resolver.SettingsProvider = SettingsProvider;

            if (LocalizationProvider != null)
                resolver.LocalizationProvider = LocalizationProvider;

            return resolver;
        }

#if UNITY_EDITOR
        public void _EditorFindAndAssignProviders()
        {
            // Auto select if provider is null
            if (SettingsProvider == null)
            {
                string[] providerGUIDs = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(SettingsProvider).Name);
                if (providerGUIDs.Length > 0)
                {
                    SettingsProvider = UnityEditor.AssetDatabase.LoadAssetAtPath<SettingsProvider>(UnityEditor.AssetDatabase.GUIDToAssetPath(providerGUIDs[0]));
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }

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