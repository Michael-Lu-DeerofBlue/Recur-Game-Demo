using System;

namespace Kamgam.SettingsGenerator
{
    public class GetSetConnection<T> : Connection<T>
    {
        public event Func<T> Getter;
        public event Action<T> Setter;

        protected T _value;
        
        public GetSetConnection(Func<T> getter, Action<T> setter)
        {
            Getter += getter;
            Setter += setter;
        }

        public override T Get()
        {
            if(Getter != null)
                _value = Getter.Invoke();

            return _value;
        }

        public override void Set(T value)
        {
            _value = value;

            if(Setter != null)
                Setter.Invoke(value);
        }

        public T GetLastKnownValue()
        {
            return _value;
        }

        public void SetLastKnownValue(T value)
        {
            _value = value;
        }
    }
}
