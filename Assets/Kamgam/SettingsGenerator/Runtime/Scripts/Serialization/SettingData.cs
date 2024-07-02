using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    [System.Serializable]
    public class SettingData
    {
        /// <summary>
        /// Thou SHALL NOT CHANGE these values.
        /// The int values of this enum are used for serialization and therefore
        /// you should not change them unless you have some very good reason.
        /// </summary>
        public enum DataType {
            Unknown = 0,
            Int = 1,
            Float = 2,
            Bool = 3,
            String = 4,
            Color = 5,
            KeyCombination = 6,
            Option = 7,
            ColorOption = 8
        }

        public string ID;
        public DataType Type;

        [SerializeField]
        public int[] IntValues;

        [SerializeField]
        public float[] FloatValues;

        [SerializeField]
        public string[] StringValues;

        public SettingData(string path, DataType type, int[] intValues, float[] floatValues, string[] stringValues) : this(path, type)
        {
            IntValues = intValues;
            FloatValues = floatValues;
            StringValues = stringValues;
        }

        public SettingData(string path, DataType type)
        {
            ID = path;
            Type = type;
        }
    }
}
