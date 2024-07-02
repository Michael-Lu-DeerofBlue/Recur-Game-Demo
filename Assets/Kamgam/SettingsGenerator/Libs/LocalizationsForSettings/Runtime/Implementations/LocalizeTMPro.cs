using System;
using TMPro;
using UnityEngine;

namespace Kamgam.LocalizationForSettings
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizeTMPro : LocalizeBase
    {
        public TextMeshProUGUI Textfield;

        public override void Awake()
        {
            Textfield = this.GetComponent<TextMeshProUGUI>();

            base.Awake();
        }

#if UNITY_EDITOR

        public override void Reset()
        {
            Textfield = this.GetComponent<TextMeshProUGUI>();

            base.Reset();
        }
#endif

        public override string GetText()
        {
            if (Textfield != null)
                return Textfield.text;

            return null;
        }

        public override void SetText(string text)
        {
            if (Textfield != null)
                Textfield.text = text;
        }
    }
}
