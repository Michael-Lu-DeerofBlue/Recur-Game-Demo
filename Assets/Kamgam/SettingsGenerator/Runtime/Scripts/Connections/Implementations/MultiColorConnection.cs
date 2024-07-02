using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class MultiColorConnection : ConnectionWithOptions<Color>
    {
        List<Color> _colors;
        int _selectedIndex;

        public MultiColorConnection(int selectedIndex)
        {
            _colors = new List<Color>()
            {
                new Color(1f, 0.102f, 0f),
                new Color(1f, 0.553f, 0f),
                new Color(0.89f, 1f, 0f),
                new Color(0f, 1f, 0.016f),
                new Color(0f, 0.318f, 1f)
            };

            _selectedIndex = selectedIndex;
        }

        public override void SetOptionLabels(List<Color> colors)
        {
            if (colors.IsNullOrEmpty())
                return;

            _colors = new List<Color>(colors);

            // clamp selected index to new range
            _selectedIndex = Mathf.Min(colors.Count-1, _selectedIndex);
        }

        public override void RefreshOptionLabels()
        {
            _colors = null;
            GetOptionLabels();
        }

        public override int Get()
        {
            return _selectedIndex;
        }

        public override List<Color> GetOptionLabels()
        {
            return _colors;
        }

        public override void Set(int selectedIndex)
        {
            _selectedIndex = selectedIndex;
            NotifyListenersIfChanged(selectedIndex);
        }
    }
}
