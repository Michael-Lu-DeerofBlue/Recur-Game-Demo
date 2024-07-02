// With 2021.2 UIToolkit was integrated with Unity instead of being a package.
#if KAMGAM_UI_ELEMENTS || UNITY_2021_2_OR_NEWER
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kamgam.LocalizationForSettings.UIElements
{
    public class LocalizeLabel : LocalizeVisualElement
    {
        protected Label _label;
        public Label Label
        {
            get
            {
                if ((_label == null && VisualElement != null) || _label != VisualElement)
                {
                    _label = VisualElement as Label;
                }
                return _label;
            }
        }

        public override Type GetElementType()
        {
            return typeof(Label);
        }

        public override void Awake()
        {
            base.Awake();
        }

        public override void OnDisable()
        {
            // Reset to trigger fetching the label again OnEnable.
            _label = null;
            base.OnDisable();
        }

        public override string GetText()
        {
            if (Label != null)
                return Label.text;

            return null;
        }

        public override void SetText(string text)
        {
            if (Label != null)
            {
                Label.text = text;
            }
        }

#if UNITY_EDITOR
        public override void Reset()
        {
            _label = null;
            base.Reset();
        }
#endif
    }
}
#endif
