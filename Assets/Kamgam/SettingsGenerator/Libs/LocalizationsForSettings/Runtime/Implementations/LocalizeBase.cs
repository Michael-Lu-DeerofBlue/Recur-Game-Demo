using System;
using UnityEngine;

namespace Kamgam.LocalizationForSettings
{
    public abstract class LocalizeBase : MonoBehaviour
    {
        public LocalizationProvider LocalizationProvider;

        public string Term;

        /// <summary>
        /// Translates the given term and sets the text of the TMPro Textfield.<br /><br />
        /// A string.Format(format) string can be specified. The translated text will
        /// always be appended as an additional LAST parameter to the parameters list.<br /><br />
        /// Example Format: "{0} %"
        /// </summary>
        [Tooltip("Translates the given term and sets the text of the TMPro Textfield.\n\n" +
            "A string.Format(format) string can be specified. The translated text will " +
            "always be appended as an additional LAST parameter to the parameters list.\n\n" +
            "Example Format: {0} %")]
        public string Format;

        /// <summary>
        /// EDITOR ONLY Setting: Update the term with the text from the textfield if the text
        /// in the textfield is a valid localization term.
        /// </summary>
        [Tooltip("EDITOR ONLY: If enabled then the inspector will try to find the term based on the content of the textfield.\n" +
            "This is done ONLY at edit-time NOT at runtime. It's a convenience feature only.\n" +
            "If you want to dynamically update the localization then please set the 'Term' property and then call 'Localize()'.")]
        public bool UpdateTermFromText = true;

        // Remember format string and parameters to be able to re-localized on language change.
        protected object[] _lastUsedParameters;
        protected string _lastUsedFormat;

        public virtual void Awake()
        {
            if (LocalizationProvider != null && LocalizationProvider.HasLocalization())
            {
                var loc = LocalizationProvider.GetLocalization();
                if (loc != null)
                    loc.AddOnLanguageChangedListener(onLanguageChanged);
            }
        }

        protected virtual void onLanguageChanged(string language)
        {
            Localize();
        }

        public virtual void OnEnable()
        {
            if (LocalizationProvider == null || !LocalizationProvider.HasLocalization())
                return;

            Clear();
            if (Term == null)
            {
                Term = GetText();
            }
            Localize(Term);
        }

        public virtual void OnDisable() { }

        public abstract string GetText();

        public abstract void SetText(string text);

        /// <summary>
        /// Clears the last known format and parameters.</param>
        /// </summary>
        public virtual void Clear()
        {
            _lastUsedFormat = null;
            _lastUsedParameters = null;
        }

        /// <summary>
        /// Updates the textfield with the current translation.
        /// </summary>
        public virtual void Localize()
        {
            this.Localize(Term, _lastUsedFormat, _lastUsedParameters);
        }

        /// <summary>
        /// Translates the given term and sets the text of the TMPro Textfield.<br />
        /// A string.Format(format) string can be specified. The translated text will
        /// always be appended as an additional LAST parameter to the parameters list.
        /// <example>Example (where the term "cost" is localized as "Price"):
        /// <code>
        /// Localize("cost", "{0}: {1:C2}", 15.99f);
        ///            \_______^    ^_______/
        /// // Result: "Price: $15.99"
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="term">The term which to translate. If null then the current 'Term' of this object is used.</param>
        /// <param name="format">Custom format with the translation as the first parameter.</param>
        /// <param name="parameters">The parameters for the formatted string. Keep in mind that the translation is prepended as the FIRST parameter in the list (index 0).</param>
        public virtual void Localize(string term, string format = null, params object[] parameters)
        {
            // Fall back on the 'Term' field if no term is specified. 
            if (string.IsNullOrEmpty(term))
                term = Term;

            if (string.IsNullOrEmpty(term))
                return;

            var loc = LocalizationProvider.GetLocalization();
            if (loc != null)
            {
                string translation = loc.Get(term);

                if (format == null)
                    format = Format;
                if (!string.IsNullOrEmpty(format))
                {
                    if (parameters == null || parameters.Length == 0)
                    {
#if UNITY_EDITOR
                        if(UnityEditor.EditorApplication.isPlaying)
                        {
#endif
                            translation = string.Format(format, translation);
#if UNITY_EDITOR
                        }
#endif
                    }
                    else
                    {
                        object[] finalParameters = new object[parameters.Length + 1];
                        finalParameters[0] = translation;
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            finalParameters[i + 1] = parameters[i];
                        }

#if UNITY_EDITOR
                        if (UnityEditor.EditorApplication.isPlaying)
                        {
#endif
                            translation = string.Format(format, finalParameters);
#if UNITY_EDITOR
                        }
#endif
                    }
                }

                SetText(translation);
            }
        }

#if UNITY_EDITOR

        public virtual void OnValidate()
        {
            if (!string.IsNullOrEmpty(Term))
                Term = Term.Trim();

            if (!string.IsNullOrEmpty(Format))
                Format = Format.Trim();
        }

        public virtual void Reset()
        {
            Clear();
            
            Term = GetText();

            autoAssignLocalizationProvider();
            markAsChangedIfInEditMode();
        }

        protected virtual void autoAssignLocalizationProvider()
        {
            // Auto select if localization provider is null
            if (LocalizationProvider == null)
            {
                var providerGUIDs = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(LocalizationProvider).Name);
                if (providerGUIDs.Length > 0)
                {
                    LocalizationProvider = UnityEditor.AssetDatabase.LoadAssetAtPath<LocalizationProvider>(UnityEditor.AssetDatabase.GUIDToAssetPath(providerGUIDs[0]));
                }
            }
        }

        protected virtual void markAsChangedIfInEditMode()
        {
            if (UnityEditor.EditorApplication.isPlaying)
                return;

            // Schedule an update to the scene view will rerender (otherwise
            // the change would not be visible unless clicked into the scene view).
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();

            // Make sure the scene can be saved
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(this.gameObject);
        }

#endif
    }
}
