using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using I2.Loc;

public class LoclizationFontSwitcher : MonoBehaviour
{
    public TMP_FontAsset englishFont;
    public TMP_FontAsset chineseFont;

    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        // Subscribe to the OnLocalizeEvent
        LocalizationManager.OnLocalizeEvent += OnLanguageChanged;

        // Initialize with the correct font
        OnLanguageChanged();
    }

    void OnDestroy()
    {
        // Unsubscribe from the OnLocalizeEvent
        LocalizationManager.OnLocalizeEvent -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case "English":
                textMeshPro.font = englishFont;
                break;
            case "Chinese (Simplified)":
                textMeshPro.font = chineseFont;
                break;
            // Add more cases for other languages if needed
            default:
                textMeshPro.font = englishFont;
                break;
        }

        // Force the text to update to apply the new font
        textMeshPro.SetAllDirty();
    }
}
