using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamgam.UGUIComponentsForSettings
{
    public class ColorPickerUGUI : MonoBehaviour
    {
        public GameObject Active;
        public Image ColorImage;

        public delegate void OnColorChangedDelegate(Color color);
        public delegate void OnSelectionChangedDelegate(int selectedIndex);

        public UnityEvent<Color> OnColorChangedEvent;
        public OnColorChangedDelegate OnColorChanged;

        public UnityEvent<int> OnSelectionChangedEvent;
        public OnSelectionChangedDelegate OnSelectionChanged;

        protected ColorPickerButtonUGUI[] _colorButtons;
        public ColorPickerButtonUGUI[] ColorButtons
        {
            get
            {
                if (_colorButtons == null)
                {
                    _colorButtons = GetComponentsInChildren<ColorPickerButtonUGUI>(includeInactive: true);
                    foreach (var colorBtn in _colorButtons)
                    {
                        colorBtn.GetComponent<Button>().onClick.AddListener(() => onColorButtonClick(colorBtn));
                    }
                }
                return _colorButtons;
            }
        }

        public bool IsActive
        {
            get => Active.gameObject.activeSelf;
        }

        protected int _selectedIndex = -1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex)
                    return;

                _selectedIndex = value;

                if (ColorButtons != null && ColorButtons.Length > _selectedIndex)
                {
                    var color = ColorButtons[_selectedIndex].Color;
                    updateColorImage(color);

                    OnColorChangedEvent?.Invoke(color);
                    OnColorChanged?.Invoke(color);
                }

                OnSelectionChangedEvent?.Invoke(_selectedIndex);
                OnSelectionChanged?.Invoke(_selectedIndex);
            }
        }

        public void Update()
        {
            if (InputUtils.CancelUp())
            {
                if (IsActive)
                    SetActive(false);
            }
        }

        public void Toggle()
        {
            Active.gameObject.SetActive(!IsActive);
        }

        public void SetActive(bool active)
        {
            // On Deactivate
            if(active != IsActive && !active)
            {
                SelectionUtils.SetSelected(GetComponent<Selectable>().gameObject);
            }

            Active.gameObject.SetActive(active);
        }

        protected void updateColorImage(Color color)
        {
            ColorImage.color = color;
        }

        private void onColorButtonClick(ColorPickerButtonUGUI button)
        {
            if (ColorButtons == null || ColorButtons.Length == 0)
                return;

            for (int i = 0; i < ColorButtons.Length; i++)
            {
                if (ColorButtons[i] == button)
                {
                    SelectedIndex = i;
                    break;
                }
            }
            
            SetActive(false);
        }

        public void SetColorOptions(IList<Color> colorOptions)
        {
            int max = Mathf.Max(colorOptions.Count, ColorButtons.Length);
            for (int i = 0; i < max; i++)
            { 
                if (i < _colorButtons.Length && i < colorOptions.Count)
                {
                    _colorButtons[i].Color = colorOptions[i];
                    _colorButtons[i].gameObject.SetActive(true);
                }
                else if (i >= ColorButtons.Length)
                {
                    // More colors than buttons
                    Debug.LogWarning(
                        "ColorPickerUGUI: There are more color options (" + colorOptions.Count + ") in the " +
                        "than there are ColorPickerButtonUGUI buttons (" + ColorButtons.Length + "). " +
                        "Please add more buttons to the UI.");
                }
                else
                {
                    // More color buttons than color options. Hide unneeded buttons.
                    ColorButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public List<Color> GetColorOptions()
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < ColorButtons.Length; i++)
            {
                if (!ColorButtons[i].gameObject.activeSelf)
                    continue;

                colors.Add(ColorButtons[i].Color);
            }

            return colors;
        }
    }
}
