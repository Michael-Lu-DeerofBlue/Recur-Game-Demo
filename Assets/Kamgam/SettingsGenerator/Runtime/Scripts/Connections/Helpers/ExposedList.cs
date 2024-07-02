using System.Collections.Generic;

namespace Kamgam.SettingsGenerator
{
    /// <summary>
    /// A list that uses an array as backend and uses NULL values as placeholders
    /// for unset values. The difference to a list is that the underlying array
    /// is exposed to allow NonAlloc Method to fill it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExposedList<T> where T : class
    {
        public const int k_DefaultCapacity = 10;
        private const int k_MaxAutoIncrease = 1000;

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set => resizeTo(value);
        }

        public T[] Values;

        public ExposedList()
        {
            Values = new T[k_DefaultCapacity];
            Clear();
        }

        public ExposedList(int capacity)
        {
            Values = new T[capacity];
            Clear();
        }

        public ExposedList(IList<T> list)
        {
            if (list == null)
            {
                Values = new T[k_DefaultCapacity];
                Clear();
            }
            else
            {
                Values = new T[list.Count];
                for (int i = 0; i < Values.Length; i++)
                {
                    Values[i] = list[i];
                }
            }
        }

        protected void resizeTo(int newCapacity)
        {
            _capacity = newCapacity;

            // SKip if no change
            if (Values.Length == _capacity)
                return;

            // Create new and clear
            var tmp = Values;
            Values = new T[newCapacity];
            Clear();

            // Copy values
            int limit = System.Math.Min(newCapacity, tmp.Length);
            for (int i = 0; i < limit; i++)
            {
                Values[i] = tmp[i];
            }
        }

        protected void autoIncreaseCapacity()
        {
            int newCapacity = Values.Length + System.Math.Min(k_MaxAutoIncrease, Values.Length / 2);
            resizeTo(newCapacity);
        }

        public void Clear()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = null;
            }
        }

        public void Add(T value)
        {
            if (value == null)
                return;

            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] == null)
                {
                    Values[i] = value;
                    return;
                }
            }

            // Not yet inserted?
            // Then the list is full and we need to append it.
            int nextIndex = Values.Length;
            autoIncreaseCapacity();
            Values[nextIndex] = value;
        }

        public void Add(IList<T> values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                Add(value);
            }
        }

        public void Remove(T value)
        {
            if (value == null)
                return;

            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] == value)
                {
                    Values[i] = null;
                }
            }
        }

        protected bool Contains(T value)
        {
            if (value == null)
                return false;

            for (int i = 0; i < Values.Length; i++)
            {
                if (Values[i] == value)
                    return true;
            }

            return false;
        }
    }
}
