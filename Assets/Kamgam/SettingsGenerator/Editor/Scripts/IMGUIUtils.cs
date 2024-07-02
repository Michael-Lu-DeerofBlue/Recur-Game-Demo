using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace Kamgam.SettingsGenerator
{
    public static class IMGUIUtils
    {
        public static class GUIColorFactory
        {
            public static Dictionary<Color, Texture2D> textureCache = new Dictionary<Color, Texture2D>();

            public static Texture2D GetTexture(Color color)
            {
                if (textureCache.ContainsKey(color))
                    return textureCache[color];

                Color[] pixels = new Color[2 * 2];
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = color;
                }

                Texture2D texture = new Texture2D(2, 2);
                texture.SetPixels(pixels);
                texture.Apply();

                textureCache.Add(color, texture);

                return texture;
            }

            public static Dictionary<Color, GUIStyle> backgroundStyleCache = new Dictionary<Color, GUIStyle>();

            public static GUIStyle GetBackgroundStyle(Color color)
            {
                if (backgroundStyleCache.ContainsKey(color))
                    return backgroundStyleCache[color];

                var style = new GUIStyle();
                style.normal.background = GetTexture(color);

                backgroundStyleCache.Add(color, style);

                return style;
            }

            public static void Clear()
            {
                backgroundStyleCache.Clear();
                textureCache.Clear();
            }
        }

        public static void DrawOverrideField(ref string field, ref bool overrideFlag, string defaultValue, string fieldLabel, string fieldTooltip = "", string overrideLabel = "override", string overrideTooltip = "")
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = overrideFlag;
            field = EditorGUILayout.TextField(new GUIContent(fieldLabel, fieldTooltip), field);
            GUI.enabled = true;
            bool oldFlag = overrideFlag;
            overrideFlag = EditorGUILayout.ToggleLeft(new GUIContent(overrideLabel, overrideTooltip), overrideFlag, GUILayout.MaxWidth(80));
            if (overrideFlag != oldFlag && !overrideFlag)
            {
                field = defaultValue;
            }
            GUILayout.EndHorizontal();
        }

        public static string PathDropAreaGUI(string path, string labelText, string labelTooltip, string dropFieldText)
        {
            if (!string.IsNullOrEmpty(labelText))
            {
                if (string.IsNullOrEmpty(labelTooltip))
                    EditorGUILayout.LabelField(new GUIContent(labelText));
                else
                    EditorGUILayout.LabelField(new GUIContent(labelText, labelTooltip));
            }

            path = EditorGUILayout.TextField(path);

            Rect dropArea = GUILayoutUtility.GetRect(0.0f, EditorGUIUtility.singleLineHeight * 1.2f, GUILayout.ExpandWidth(true));
            dropArea.x += 10;
            dropArea.y -= 2;
            dropArea.width -= 20;
            var style = new GUIStyle(GUI.skin.box);
            var color = GUI.skin.box.normal.textColor;
            color.a = 0.6f;
            style.normal.textColor = color;
            GUI.Box(dropArea, dropFieldText, style);

            // modify dropArea to also include the textfield above
            dropArea.y -= EditorGUIUtility.singleLineHeight * 1.2f;
            dropArea.height += EditorGUIUtility.singleLineHeight * 1.2f;

            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                    if (!dropArea.Contains(evt.mousePosition))
                        return path;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Event.current.Use();
                    break;

                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return path;

                    DragAndDrop.AcceptDrag();

                    // Object outside project. Probably from Finder.
                    if (DragAndDrop.paths.Length > 0 && DragAndDrop.objectReferences.Length == 0)
                    {
                        return DragAndDrop.paths[0];
                    }
                    break;
            }

            return path;
        }

        public static void DrawLabel(string text, string tooltip = null, Color? color = null, bool bold = false, bool wordwrap = true, bool richText = true, Texture icon = null, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (!color.HasValue)
                color = GUI.skin.label.normal.textColor;

            if (style == null)
                style = new GUIStyle(GUI.skin.label);
            if (bold)
                style.fontStyle = FontStyle.Bold;
            else
                style.fontStyle = FontStyle.Normal;

            style.normal.textColor = color.Value;
            style.hover.textColor = color.Value;
            style.wordWrap = wordwrap;
            style.richText = richText;
            style.imagePosition = ImagePosition.ImageLeft;

            var content = new GUIContent(text);
            if (tooltip != null)
                content.tooltip = tooltip;
            if (icon != null)
            {
                GUILayout.Space(16);
                var position = GUILayoutUtility.GetRect(content, style, options);
                GUI.DrawTexture(new Rect(position.x - 16, position.y, 16, 16), icon);
                GUI.Label(position, content, style);
            }
            else
            {
                GUILayout.Label(content, style, options);
            }
        }

        public static void DrawSelectableLabel(string text, Color? color = null, bool bold = false, bool wordwrap = true, bool richText = true, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (!color.HasValue)
                color = GUI.skin.label.normal.textColor;

            if (style == null)
                style = new GUIStyle(GUI.skin.label);
            if (bold)
                style.fontStyle = FontStyle.Bold;
            else
                style.fontStyle = FontStyle.Normal;
            style.normal.textColor = color.Value;
            style.active.textColor = color.Value;
            style.wordWrap = wordwrap;
            style.richText = richText;

            var content = new GUIContent(text);
            var position = GUILayoutUtility.GetRect(content, style, options);
            EditorGUI.SelectableLabel(position, text, style);
        }

        public static bool DrawButton(string text, string tooltip = null, string icon = null, Color? backgroundColor = null, params GUILayoutOption[] options)
        {
            GUIContent content;

            // icon
            if (!string.IsNullOrEmpty(icon))
                content = EditorGUIUtility.IconContent(icon);
            else
                content = new GUIContent();

            // text
            content.text = text;

            // tooltip
            if (!string.IsNullOrEmpty(tooltip))
                content.tooltip = tooltip;

            var bgColor = GUI.backgroundColor;
            if (backgroundColor.HasValue)
            {
                GUI.backgroundColor = backgroundColor.Value;
            }
            var result = GUILayout.Button(content, options);
            GUI.backgroundColor = bgColor;
            return result;
        }

        public static void BeginHorizontalIndent(int indentAmount = 10, bool beginVerticalInside = true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(indentAmount);
            if (beginVerticalInside)
                GUILayout.BeginVertical();
        }

        public static void EndHorizontalIndent(float indentAmount = 10, bool begunVerticalInside = true, bool bothSides = false)
        {
            if (begunVerticalInside)
                GUILayout.EndVertical();
            if (bothSides)
                GUILayout.Space(indentAmount);
            GUILayout.EndHorizontal();
        }

        public static string WrapInRichTextColor(string text, Color color)
        {
            var hexColor = ColorUtility.ToHtmlStringRGB(color);
            return "<color=#" + hexColor + ">" + text + "</color>";
        }
    }
}
