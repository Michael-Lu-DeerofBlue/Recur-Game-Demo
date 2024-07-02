// Recompile at 1/21/2023 3:18:05 PM
// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers
{

    /// <summary>
    /// Sets a Text or TextMeshProUGUI component's font according to the current
    /// language and the settings in a LocalizedFonts asset.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class SetLocalizedFont : MonoBehaviour
    {

        [SerializeField] protected bool m_setOnEnable = true;

        [Tooltip("Overrides UILocalizationManager's Localized Fonts if set.")]
        [SerializeField] protected LocalizedFonts m_localizedFonts = null;

        protected bool m_started = false;
        protected float m_initialFontSize = -1;
        protected UnityEngine.UI.Text text;
#if TMP_PRESENT
        protected TMPro.TextMeshProUGUI textMeshPro;
#endif

        protected virtual void Awake()
        {
            text = GetComponent<UnityEngine.UI.Text>();
#if TMP_PRESENT
            textMeshPro = GetComponent<TMPro.TextMeshProUGUI>();
#endif
        }

        protected virtual void Start()
        {
            m_started = true;
            if (m_setOnEnable) SetCurrentLocalizedFont();
            UILocalizationManager.languageChanged += OnLanguageChanged;
        }

        protected virtual void OnDestroy()
        {
            UILocalizationManager.languageChanged -= OnLanguageChanged; 
        }

        protected virtual void OnEnable()
        {
            if (m_started) SetCurrentLocalizedFont();
        }

        protected virtual void OnLanguageChanged(string language)
        {
            SetCurrentLocalizedFont();
        }

        public virtual void SetCurrentLocalizedFont()
        {
            // Record initial font size if necessary:
            if (m_initialFontSize == -1)
            {
                if (text != null) m_initialFontSize = text.fontSize;
#if TMP_PRESENT
            if (textMeshPro != null) m_initialFontSize = textMeshPro.fontSize;
#endif
            }

            // Get LocalizedFonts asset:
            var localizedFonts = (m_localizedFonts != null) ? m_localizedFonts : UILocalizationManager.instance.localizedFonts;
            if (localizedFonts == null) return;

            if (text != null)
            {
                var localizedFont = localizedFonts.GetFont(UILocalizationManager.instance.currentLanguage);
                if (localizedFont != null)
                {
                    text.font = localizedFont;
                    text.fontSize = Mathf.RoundToInt(localizedFonts.GetFontScale(UILocalizationManager.instance.currentLanguage) * m_initialFontSize);
                }
            }

#if TMP_PRESENT
            if (textMeshPro != null)
            {
                var localizedTextMeshProFont = localizedFonts.GetTextMeshProFont(UILocalizationManager.instance.currentLanguage);
                if (localizedTextMeshProFont != null) 
                {
                    textMeshPro.font = localizedTextMeshProFont;
                    textMeshPro.fontSize = localizedFonts.GetFontScale(UILocalizationManager.instance.currentLanguage) * m_initialFontSize;
                }
            }
#endif
        }
    }
}
