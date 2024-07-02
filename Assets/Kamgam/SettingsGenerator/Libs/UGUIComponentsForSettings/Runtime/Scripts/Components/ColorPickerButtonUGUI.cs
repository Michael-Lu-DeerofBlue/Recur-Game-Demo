using UnityEngine;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class ColorPickerButtonUGUI : MonoBehaviour
    {
        public Image ColorImage;

        [SerializeField]
        protected Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                if (_color == value)
                    return;

                _color = value;
                updateImageColor();
            }
        }

        public void Start()
        {
            updateImageColor();
        }

        protected void updateImageColor()
        {
            ColorImage.color = Color;
        }
    }
}
