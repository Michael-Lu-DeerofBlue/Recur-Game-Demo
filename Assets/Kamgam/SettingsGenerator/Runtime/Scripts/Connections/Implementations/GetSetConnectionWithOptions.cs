using System;
using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    public class GetSetConnectionWithOptions<T> : ConnectionWithOptions<T>
    {
        public event Func<int> Getter;
        public event Action<int> Setter;

        public event Func<List<T>> OptionLabelGetter;
        public event Action<List<T>> OptionLabelSetter;

        protected int _selectedIndex;
        protected List<T> _optionLabels;

        public GetSetConnectionWithOptions(Func<int> getter, Action<int> setter, Func<List<T>> optionLabelGetter = null, Action<List<T>> optionLabelSetter = null)
        {
            Getter += getter;
            Setter += setter;
            
            if (optionLabelGetter != null)
                OptionLabelGetter += optionLabelGetter;

            if (optionLabelSetter != null)
                OptionLabelSetter += optionLabelSetter;
        }

        public override int Get()
        {
            _selectedIndex = Getter.Invoke();
            return _selectedIndex;
        }

        public override void Set(int selectedIndex)
        {
            _selectedIndex = selectedIndex;
            Setter.Invoke(selectedIndex);
        }

        public int GetLastKnownValue()
        {
            return _selectedIndex;
        }

        public override List<T> GetOptionLabels()
        {
            if (OptionLabelGetter != null)
            {
                _optionLabels = OptionLabelGetter.Invoke();
            }

            return _optionLabels;
        }

        public override void SetOptionLabels(List<T> optionLabels)
        {
            _optionLabels = optionLabels;

            if (OptionLabelSetter != null)
            {
                OptionLabelSetter.Invoke(optionLabels);
            }
        }

        public override void RefreshOptionLabels()
        {
            _optionLabels = null;
            GetOptionLabels();
        }

        public int GetLastSelectedIndex()
        {
            return _selectedIndex;
        }

        public void SetLastSelectedIndex(int index)
        {
            _selectedIndex = index;
        }
    }
}
