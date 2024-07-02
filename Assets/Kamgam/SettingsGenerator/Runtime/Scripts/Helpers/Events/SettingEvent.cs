using System;
using UnityEngine;
using UnityEngine.Events;

namespace Kamgam.SettingsGenerator
{
    public abstract class SettingEvent : SettingResolver
    {
        public bool TriggerOnStart = true;
        public bool TriggerOnEnable = false;

        [Tooltip("If set to false then this event will not trigger if the gameobject or component is disabled.\n\n" +
            "NOTICE: The event registers itself in OnEnable() so if the object starts disabled then it will NEVER be " +
            "triggered not matter what this was set to.")]
        public bool TriggerIfDisabled = false;

        [System.NonSerialized]
        protected SettingData.DataType[] _supportedDataTypes;

        public abstract override SettingData.DataType[] GetSupportedDataTypes();

        public ISetting GetSetting()
        {
            if (HasActiveSettingForID(ID))
                return SettingsProvider.Settings.GetSetting(ID);
            
            return null;
        }

        public override void Start()
        {
            base.Start();

            if (TriggerOnStart)
                TriggerEvent();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            Register();

            if (TriggerOnEnable)
                TriggerEvent();
        }

        public override void OnDisable()
        {
            UnRegister();
        }

        public void Register()
        {
            if (HasActiveSettingForID(ID))
                GetSetting().OnSettingChanged += onChanged;
        }

        public void UnRegister()
        {
            if (HasActiveSettingForID(ID))
                GetSetting().OnSettingChanged -= onChanged;
        }

        protected virtual void onChanged(ISetting setting)
        {
            if(shoudTrigger())
                TriggerEvent();
        }

        /// <summary>
        /// Updates the target with the value from the setting (a manual way to trigger the event)<br />
        /// </summary>
        public override void Refresh()
        {
            TriggerEvent();
        }

        public virtual bool shoudTrigger()
        {
            if (TriggerIfDisabled)
            {
                return true;
            }
            else
            {
                return gameObject != null && gameObject.activeInHierarchy && this.isActiveAndEnabled;
            }
        }

        public abstract void TriggerEvent();
    }

    public abstract class SettingEvent<T> : SettingEvent
    {
        [Space(10)]
        public UnityEvent<T> OnValueChanged;

        public void Log(T value)
        {
            Debug.Log(value);
        }
    }
}
