using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Kamgam.SettingsGenerator;
using System;
using TMPro;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

using SettingsProvider = Kamgam.SettingsGenerator.SettingsProvider;

namespace Kamgam.SettingsGenerator
{
    public partial class CreateSettingUGUIWindow : EditorWindow
    {

        [System.Serializable]
        public class PrefabEntry
        {
            public string PrefabAssetPath;
            public string Name;
            public SettingData.DataType[] Types;

            [System.NonSerialized]
            protected GameObject _prefab;

            public PrefabEntry(string prefabAssetPath, string name)
            {
                PrefabAssetPath = prefabAssetPath;
                Name = name;
                Types = new SettingData.DataType[] { SettingData.DataType.Unknown };
            }

            public String GetName()
            {
                if (!string.IsNullOrEmpty(Name))
                    return Name;

                if (!string.IsNullOrEmpty(PrefabAssetPath))
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(PrefabAssetPath);
                    fileName = fileName.Replace("(SettingPrefabUGUI)", "");
                    fileName = InsertSpaceBeforeUpperCase(fileName);
                    return fileName;
                }

                return "Generic: " + Types[0];
            }

            public GameObject GetPrefab()
            {
                if (_prefab == null)
                    _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);

                return _prefab;
            }

            public SettingData.DataType[] GetSupportedDataTypes()
            {
                if ((Types.Length == 0 || Types[0] == SettingData.DataType.Unknown) && !string.IsNullOrEmpty(PrefabAssetPath))
                {
                    var prefab = GetPrefab();
                    if (prefab != null)
                    {
                        var resolver = prefab.GetComponentInChildren<SettingResolver>();
                        if (resolver != null)
                        {
                            var typesOfResolver = resolver.GetSupportedDataTypes();
                            Types = new SettingData.DataType[typesOfResolver.Length];
                            Array.Copy(typesOfResolver, Types, typesOfResolver.Length);
                        }
                    }
                }

                return Types;
            }

            public static List<PrefabEntry> FindAllSettingPrefabs()
            {
                var entries = new List<PrefabEntry>();

                var guids = AssetDatabase.FindAssets("t:Prefab (SettingPrefabUGUI)");
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    string name = System.IO.Path.GetFileNameWithoutExtension(path);
                    name = name.Replace(" (SettingPrefabUGUI)", "");
                    var entry = new PrefabEntry(path, name);
                    entries.Add(entry);
                }

                return entries;
            }

        }

        [System.Serializable]
        public class SettingTypeEntry
        {
            public string ConnectionAssetPath;
            public string Name;
            public SettingData.DataType Type;

            [System.NonSerialized]
            protected ConnectionSO _connectionSO;

            public bool IsGeneric => string.IsNullOrEmpty(ConnectionAssetPath);

            public SettingTypeEntry(string connectionAssetPath, string name, SettingData.DataType type)
            {
                ConnectionAssetPath = connectionAssetPath;
                Name = name;
                Type = type;
            }

            public String GetName()
            {
                if (!string.IsNullOrEmpty(Name))
                    return Name;

                if (!string.IsNullOrEmpty(ConnectionAssetPath))
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(ConnectionAssetPath);
                    fileName = fileName.Replace("Connection", "");
                    fileName = InsertSpaceBeforeUpperCase(fileName);
                    return fileName;
                }

                return "Generic: " + Type;
            }

            public ConnectionSO GetConnectionSO()
            {
                if (_connectionSO == null)
                    _connectionSO = AssetDatabase.LoadAssetAtPath<ConnectionSO>(ConnectionAssetPath);

                return _connectionSO;
            }

            public SettingData.DataType GetSupportedDataType()
            {
                if (Type == SettingData.DataType.Unknown && !string.IsNullOrEmpty(ConnectionAssetPath))
                {
                    var connectionSO = GetConnectionSO();
                    if (connectionSO != null)
                    {
                        if (connectionSO is FloatConnectionSO) Type = SettingData.DataType.Float;
                        else if (connectionSO is IntConnectionSO) Type = SettingData.DataType.Int;
                        else if (connectionSO is BoolConnectionSO) Type = SettingData.DataType.Bool;
                        else if (connectionSO is StringConnectionSO) Type = SettingData.DataType.String;
                        else if (connectionSO is OptionConnectionSO) Type = SettingData.DataType.Option;
                        else if (connectionSO is ColorConnectionSO) Type = SettingData.DataType.Color;
                        else if (connectionSO is ColorOptionConnectionSO) Type = SettingData.DataType.ColorOption;
                        else if (connectionSO is KeyCombinationConnectionSO) Type = SettingData.DataType.KeyCombination;
                    }
                }

                return Type;
            }

            public bool SupportsAny(SettingData.DataType[] dataTypes)
            {
                var dataType = GetSupportedDataType();
                for (int i = 0; i < dataTypes.Length; i++)
                {
                    if (dataTypes[i] == dataType)
                        return true;
                }

                return false;
            }
        }




        [System.Serializable]
        public class SettingVisualEntry
        {
            public string Name;
            public string PrefabAssetPath;
            public SettingData.DataType[] SupportedTypes;

            protected GameObject _prefab;

            public SettingVisualEntry(string prefabAssetPath, string name, params SettingData.DataType[] supportedTypes)
            {
                PrefabAssetPath = prefabAssetPath;

                name = name.Replace("UGUI (Setting)", "");
                name = InsertSpaceBeforeUpperCase(name);
                Name = name;

                SupportedTypes = supportedTypes;
            }

            public String GetName()
            {
                if (!string.IsNullOrEmpty(Name))
                    return Name;

                return "VisualsEntry";
            }

            public GameObject GetPrefab()
            {
                if (_prefab == null)
                    _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabAssetPath);

                return _prefab;
            }
        }

        public static string InsertSpaceBeforeUpperCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // Regular expression to match an uppercase letter that is not
            // preceded by a number or another uppercase letter
            string pattern = @"(?<![\dA-Z])([A-Z])";

            return System.Text.RegularExpressions.Regex.Replace(input, pattern, " $1");
        }
    }
}