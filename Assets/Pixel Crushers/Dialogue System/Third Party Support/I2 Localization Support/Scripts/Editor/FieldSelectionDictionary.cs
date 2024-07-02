// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.I2Support
{

    [Serializable]
    public class FieldSelectionDictionary
    {
        public List<string> titles = new List<string>();

        public List<bool> includes = new List<bool>();

        public List<FieldType> fieldTypes = new List<FieldType>();

        public void Clear()
        {
            titles.Clear();
            includes.Clear();
            fieldTypes.Clear();
        }

        public void PopulateDictionary<T>(List<T> assets) where T : Asset
        {
            var dict = new Dictionary<string, bool>();
            var dict2 = new Dictionary<string, FieldType>();
            var inList = new HashSet<string>();
            if (assets != null)
            {
                for (int i = 0; i < assets.Count; i++)
                {
                    if (assets[i] == null) continue;
                    AddFieldsToDictionary(assets[i].fields, dict, dict2, inList);
                }
            }
            SetDictionary(dict, dict2);
        }

        public void PopulateDictionaryWithDialogueEntries(List<Conversation> conversations)
        {
            var dict = new Dictionary<string, bool>();
            var dict2 = new Dictionary<string, FieldType>();
            var inList = new HashSet<string>();
            for (int i = 0; i < conversations.Count; i++)
            {
                for (int j = 0; j < conversations[i].dialogueEntries.Count; j++)
                {
                    AddFieldsToDictionary(conversations[i].dialogueEntries[j].fields, dict, dict2, inList);
                }
            }
            SetDictionary(dict, dict2);
        }

        private void AddFieldsToDictionary(List<Field> fields, Dictionary<string, bool> dict, Dictionary<string, FieldType> dict2, HashSet<string> inList)
        {
            if (fields != null)
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    var title = fields[i].title;
                    if (!inList.Contains(title))
                    {
                        inList.Add(title);
                        var index = titles.IndexOf(title);
                        var include = (0 <= index && index < includes.Count) ? includes[index] : false;
                        dict.Add(title, include);
                        dict2.Add(title, fields[i].type);
                    }
                }
            }
        }

        public void PopulateDictionaryWithTextTable(TextTable textTable, bool append = false)
        {
            var dict = new Dictionary<string, bool>();
            var dict2 = new Dictionary<string, FieldType>();
            var inList = new HashSet<string>();
            foreach (var kvp in textTable.fields)
            {
                var field = kvp.Value;
                var title = field.fieldName;
                if (!inList.Contains(title))
                {
                    inList.Add(title);
                    var index = titles.IndexOf(title);
                    var include = (index >= 0) ? includes[index] : false;
                    dict.Add(title, include);
                    dict2.Add(title, FieldType.Text);
                }
            }            
            SetDictionary(dict, dict2, append);
        }

        public void PopulateDictionaryWithLocalizedTextTable(LocalizedTextTable localizedTextTable)
        {
            var dict = new Dictionary<string, bool>();
            var dict2 = new Dictionary<string, FieldType>();
            var inList = new HashSet<string>();
            for (int i = 0; i < localizedTextTable.fields.Count; i++)
            {
                var field = localizedTextTable.fields[i];
                var title = field.name;
                if (!inList.Contains(title))
                {
                    inList.Add(title);
                    var index = titles.IndexOf(title);
                    var include = (index > 0) ? includes[index] : false;
                    dict.Add(title, include);
                    dict2.Add(title, FieldType.Text);
                }
            }
            SetDictionary(dict, dict2);
        }

        private void SetDictionary(Dictionary<string, bool> dict, Dictionary<string, FieldType> dict2, bool append = false)
        {
            if (!append)
            {
                titles.Clear();
                includes.Clear();
                fieldTypes.Clear();
            }
            var keyList = new List<string>(dict.Keys);
            keyList.Sort();
            for (int i = 0; i < keyList.Count; i++)
            {
                var key = keyList[i];
                if (!(append && titles.Contains(key)))
                {
                    titles.Add(key);
                    includes.Add(dict[key]);
                    fieldTypes.Add(dict2[key]);
                }
            }
        }

        public void SetAllSelections(bool value)
        {
            for (int i = 0; i < includes.Count; i++)
            {
                includes[i] = value;
            }
        }

        public bool ShouldInclude(string title)
        {
            if (!titles.Contains(title)) return false;
            var index = titles.IndexOf(title);
            return (index >= 0) && includes[index];
        }

    }
}
