using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class RandomColorUGUI : MonoBehaviour
    {
        public Image ColorImage;

        public delegate void OnColorChangedDelegate(Color color);

        /// <summary>
        /// The index of the selected option.
        /// </summary>
        public UnityEvent<Color> OnColorChangedEvent;
        public OnColorChangedDelegate OnColorChanged;

        protected Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (value == _color)
                    return;

                _color = value;

                updateColorImage(Color);

                OnColorChangedEvent?.Invoke(Color);
                OnColorChanged?.Invoke(Color);
            }
        }

        public void Randomize()
        {
            Color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, 1f);
        }

        protected void updateColorImage(Color color)
        {
            ColorImage.color = color;
        }
    }
}
