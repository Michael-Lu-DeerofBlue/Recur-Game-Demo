using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.LocalizationForSettings
{
    [System.Serializable]
    public class Translation : ITranslation
    {
        [SerializeField, HideInInspector]
        protected string _term;

        [SerializeField, HideInInspector]
        protected List<string> _texts = new List<string>();

        public Translation(string term, int languageCount)
        {
            _term = term;

            _texts = new List<string>();
            for (int i = 0; i < languageCount; i++)
            {
                _texts.Add("");
            }
        }

        public Translation(string term, List<string> texts)
        {
            _term = term;
            _texts = texts;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public string GetTerm()
        {
            return _term;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public bool HasText(int languageIndex)
        {
            return !(_texts == null || _texts.Count == 0 || languageIndex < 0 || languageIndex > _texts.Count - 1);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="languageIndex"></param>
        /// <returns></returns>
        public string GetText(int languageIndex)
        {
            if (!HasText(languageIndex))
                return _term;

            return _texts[languageIndex];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="languageIndex"></param>
        /// <param name="text"></param>
        public void SetText(int languageIndex, string text)
        {
            if (_texts == null || text == null)
                return;

            if (languageIndex < 0 )
            {
                _texts.Add(text);
            }
            else
            {
                // Fill with null values until the languageIndex exists
                if (languageIndex > _texts.Count - 1)
                {
                    while (_texts.Count - 1 < languageIndex)
                        _texts.Add(null);
                }

                _texts[languageIndex] = text;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void ClearTexts()
        {
            _texts.Clear();
        }
    }
}
