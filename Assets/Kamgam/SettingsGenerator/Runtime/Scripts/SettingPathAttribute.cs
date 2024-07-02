using System;

namespace Kamgam.SettingsGenerator
{
    public class SettingsPathAttribute : Attribute
    {
        public string Path;
        public string[] Tags;

        public SettingsPathAttribute(string path, params string[] tags)
        {
            Path = path;
            Tags = tags;
        }
    }
}
