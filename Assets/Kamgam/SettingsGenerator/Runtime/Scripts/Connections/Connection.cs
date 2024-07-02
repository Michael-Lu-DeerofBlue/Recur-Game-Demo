using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public static class ConnectionDefaults
    {
        public const int Order = 0;
    }

    /// <summary>
    /// A Connection can GET and SET dynamic values.<br />
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class Connection<TValue> : IConnection<TValue>
    {
        public event System.Action<int> QualityChanged;

        protected List<System.Action<TValue>> _onChangedListeners;
        protected TValue lastKnownValue;

        /// <summary>
        /// Use this to indicate that multiple connection ned to be set in order.<br />
        /// The lower the earlier the connection Set() should be called.<br />
        /// This is useful if there are multiple connections which set the
        /// same value.
        /// </summary>
        public int Order = ConnectionDefaults.Order;

        public abstract TValue Get();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual TValue GetDefault()
        {
            return Get();
        }

        public virtual int GetOrder()
        {
            return Order;
        }

        public virtual void SetOrder(int order)
        {
            Order = order;
        }

        public abstract void Set(TValue value);

        public virtual void NotifyListenersIfChanged(TValue value)
        {
            if (!value.Equals(lastKnownValue))
            {
                lastKnownValue = value;

                if (_onChangedListeners != null)
                {
                    foreach (var listener in _onChangedListeners)
                    {
                        listener?.Invoke(value);
                    }
                }
            }
        }

        public void AddChangeListener(System.Action<TValue> listener)
        {
            if (_onChangedListeners == null)
            {
                _onChangedListeners = new List<System.Action<TValue>>();
            }

            if (!_onChangedListeners.Contains(listener))
                _onChangedListeners.Add(listener);
        }

        public void RemoveChangeListener(System.Action<TValue> listener)
        {
            if (_onChangedListeners == null)
                return;

            _onChangedListeners.Remove(listener);
        }

        public virtual void OnQualityChanged(int qualityLevel)
        {
            QualityChanged?.Invoke(qualityLevel);
        }

        public virtual void Destroy() { }
    }
}
