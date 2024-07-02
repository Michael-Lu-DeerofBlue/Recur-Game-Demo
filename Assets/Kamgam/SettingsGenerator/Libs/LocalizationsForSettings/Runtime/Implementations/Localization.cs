using System.Collections.Generic;
using UnityEngine;
using static Kamgam.LocalizationForSettings.ILocalization;

namespace Kamgam.LocalizationForSettings
{
    [System.Serializable]
    public class Localization : ILocalization
    {
        [Tooltip("The default language used if language detection fails to find a language (or if auto detect is disabled.")]
        public string DefaultLanguage = "English";

        [Tooltip("Should the language be detected at start? If disabled then the default language is used.")]
        public bool AutoDetectLanguage = true;

        [SerializeField]
        protected List<string> _languages = new List<string>();

        [SerializeField, HideInInspector]
        protected List<Translation> _translations = new List<Translation>();

        /// <summary>
        /// Called if the language is changed.
        /// </summary>
        public OnLanguageChangedDelegate OnLanguageChanged;

        /// <summary>
        /// Called if the language is changed (called after the delegate).
        /// </summary>
        public UnityEngine.Events.UnityEvent<string> OnLanguageChangedEvent;

        [System.NonSerialized]
        protected TranslateTermCallback _translateTermCallback;

        [System.NonSerialized]
        protected LocalizationSourceBehaviour _sourceBehaviour = LocalizationSourceBehaviour.PreferDynamic;

        [System.NonSerialized]
        protected int _currentLanguageIndex = -1;

        public void SetDynamicLocalizationCallback(TranslateTermCallback translateTermCallback)
        {
            _translateTermCallback = translateTermCallback;
        }

        /// <summary>
        /// Defines whether to prioritize the static or the dynamic translations.<br />
        /// Dynamic translations are translations provided by the TranslateTermCallback set
        /// via SetDynamicLocalizationCallback(translateTermCallback).
        /// </summary>
        /// <param name="behaviour"></param>
        [System.Obsolete("Please use Get(string term, bool ignoreDynamic) instead.")]
        public void SetLocalizationSourceBehaviour(LocalizationSourceBehaviour behaviour)
        {
            _sourceBehaviour = behaviour;
        }

        void autoDetectLanguage()
        {
            // Detect language if none is set.
            if (_currentLanguageIndex < 0)
            {
                if (AutoDetectLanguage)
                {
                    DetectLanguage(setAsCurrent: true);
                }
                else
                {
                    SetLanguage(DefaultLanguage);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int DetectLanguage(bool setAsCurrent = true)
        {
            // Detect
            string language = Application.systemLanguage.ToString();
            if (language == null)
            {
                language = DefaultLanguage;
            }

            // Fetch or create if new
            int index = GetLanguageIndex(language);
            if (index < 0)
                index = AddLanguage(language);

            // set as current
            if (setAsCurrent)
            {
                SetLanguage(index);
            }

            return index;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public int AddLanguage(string newLanguage)
        {
            int index = _languages.IndexOf(newLanguage);
            if (index < 0)
            {
                _languages.Add(newLanguage);
            }

            return index;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public string GetLanguage()
        {
            if (_currentLanguageIndex < 0 || _languages == null || _languages.Count == 0 || _currentLanguageIndex > _languages.Count - 1)
                return null;

            return _languages[_currentLanguageIndex];
        }

        public int GetLanguageIndex()
        {
            return _currentLanguageIndex;
        }

        public string GetLanguageAt(int languageIndex)
        {
            if (languageIndex < 0 || _languages == null || _languages.Count == 0 || languageIndex > _languages.Count - 1)
                return null;

            return _languages[languageIndex];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public int GetLanguageIndex(string language)
        {
            for (int i = 0; i < _languages.Count; i++)
            {
                if (_languages[i] == language)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="languageIndex"></param>
        public void SetLanguageIndex(int languageIndex)
        {
            SetLanguage(languageIndex);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="language"></param>
        public void SetLanguage(string language)
        {
            int index = _languages.IndexOf(language);
            if (index >= 0)
                SetLanguage(index);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="languageIndex"></param>
        public void SetLanguage(int languageIndex)
        {
            if (languageIndex < 0 || languageIndex >= _languages.Count)
                return;

            if (_currentLanguageIndex != languageIndex)
            {
                _currentLanguageIndex = languageIndex;

                string language = GetLanguageAt(languageIndex);
                OnLanguageChanged?.Invoke(language);
                OnLanguageChangedEvent?.Invoke(language);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<string> GetLanguages()
        {
            return new List<string>(_languages);
        }

        public int GetLanguageCount()
        {
            return _languages.Count;
        }

        public int GetTranslationCount()
        {
            return _translations.Count;
        }

        public Translation GetTranslationAt(int index)
        {
            if (index < 0 || index >= _translations.Count)
                return null;

            return _translations[index];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="term"></param>
        /// <param name="language"></param>
        /// <param name="text"></param>
        public int CreateOrUpdateTranslation(string term, string language, string text)
        {
            if (term == null)
                return -1;

            int languageIndex = GetLanguageIndex(language);

            for (int i = 0; i < _translations.Count; i++)
            {
                if (_translations[i].GetTerm() == term)
                {
                    _translations[i].SetText(languageIndex, text);
                    return i;
                }
            }

            var newTranslation = new Translation(term, GetLanguageCount());
            _translations.Add(newTranslation);
            return _translations.Count - 1;
        }

        public void DeleteTranslation(string term)
        {
            for (int i = 0; i < _translations.Count; i++)
            {
                if (_translations[i].GetTerm() == term)
                {
                    _translations.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public bool HasTerm(string term)
        {
            for (int i = 0; i < _translations.Count; i++)
            {
                if (_translations[i].GetTerm() == term)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public string Get(string term)
        {
            return Get(term, ignoreDynamic: false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="term"></param>
        /// <param name="ignoreDynamic">If TRUE then the dynamic translation callback is ignored.</param>
        /// <returns></returns>
        public string Get(string term, bool ignoreDynamic)
        {
            // Detect language if none is set.
            autoDetectLanguage();

            // Prefer dynamic translation?
            if (!ignoreDynamic && _sourceBehaviour == LocalizationSourceBehaviour.PreferDynamic && _translateTermCallback != null)
            {
                return _translateTermCallback(term, GetLanguage());
            }

            for (int i = 0; i < _translations.Count; i++)
            {
                if (_translations[i].GetTerm() == term)
                {
                    return _translations[i].GetText(_currentLanguageIndex);
                }
            }

            return term;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="terms"></param>
        public T LocalizeListAsCopy<T>(T terms) where T : IList<string>, new()
        {
            var localizedCopy = new T();
            for (int i = 0; i < terms.Count; i++)
            {
                localizedCopy.Add(Get(terms[i]));
            }
            return localizedCopy;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="terms"></param>
        /// <param name="target"></param>
        public void LocalizeList<T>(T terms, T target) where T : IList<string>, new()
        {
            target.Clear();

            for (int i = 0; i < terms.Count; i++)
            {
                target.Add(Get(terms[i]));
            }
        }

        /// <summary>
        /// Called if the language is changed.
        /// </summary>
        public void AddOnLanguageChangedListener(OnLanguageChangedDelegate listener)
        {
            OnLanguageChanged -= listener;
            OnLanguageChanged += listener;
        }

        public void RemoveOnLanguageChangedListener(OnLanguageChangedDelegate listener)
        {
            OnLanguageChanged -= listener;
        }

        public void Sort()
        {
            _translations.Sort(sortByTerm);
        }

        protected int sortByTerm(Translation a, Translation b)
        {
            if (a == null || b == null || a.GetTerm() == null || b.GetTerm() == null)
                return 0;

            return string.Compare(a.GetTerm(), b.GetTerm());
        }

        public void TriggerLanguageChangeEvent()
        {
            OnLanguageChanged?.Invoke(GetLanguage());
            OnLanguageChangedEvent?.Invoke(GetLanguage());
        }
    }
}