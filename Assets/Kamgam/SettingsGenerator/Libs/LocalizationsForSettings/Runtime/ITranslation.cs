namespace Kamgam.LocalizationForSettings
{
    public interface ITranslation
    {
        /// <summary>
        /// Returns the term of the translation. That's the key which is used to find the translation.<br />
        /// If there are no texts set then the term itself will be returned.
        /// </summary>
        /// <returns></returns>
        string GetTerm();

        /// <summary>
        /// Returns whether or not a text for the given languageID is set.
        /// </summary>
        /// <param name="languageIndex"></param>
        /// <returns></returns>
        bool HasText(int languageIndex);

        /// <summary>
        /// Returns the text for the given landuageIndex (or null if not found).
        /// </summary>
        /// <param name="languageIndex"></param>
        /// <returns></returns>
        string GetText(int languageIndex);

        /// <summary>
        /// Sets the text for the given languageIndex.<br />
        /// If the languageIndex is < 0 then the text will added as a new index at the end.<br />
        /// If the languageIndex is > the current text list length then the missing indices are filled with NULL.
        /// </summary>
        /// <param name="languageIndex"></param>
        /// <param name="text"></param>
        void SetText(int languageIndex, string text);

        /// <summary>
        /// Clears the text list.
        /// </summary>
        void ClearTexts();
    }
}
