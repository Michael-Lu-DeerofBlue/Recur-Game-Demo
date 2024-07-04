using UnityEngine;

namespace I2.Loc
{
    public class SetStartLanguage : MonoBehaviour
    {
        public enum Language
        {
            English,
            Chinese
        }

        public Language languageToSet;

        private void Start()
        {
            switch (languageToSet)
            {
                case Language.English:
                    SetLanguage("en");
                    break;
                case Language.Chinese:
                    SetLanguage("zh-CN");
                    break;
                default:
                    Debug.Log("Language not supported");
                    break;
            }
        }
        public void SetLanguage(string LangName)
        {
            if (LocalizationManager.HasLanguage(LangName))
            {
                LocalizationManager.CurrentLanguage = LangName;
            }
            else
            {
                Debug.Log("Could not set the language. Check to make sure it is in the localization manager.");
            }
        }
    }

}
