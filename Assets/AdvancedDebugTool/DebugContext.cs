using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugContext
    {
        private DebugToolStyles m_Styles;        
        private readonly Dictionary<string, bool> m_DropdownOpen = new Dictionary<string, bool>();
        private readonly Dictionary<Type, string[]> m_EnumValues = new Dictionary<Type, string[]>();
        
        public DebugContext(DebugToolStyles styles)
        {
            m_Styles = styles;
        }

        // ================================
        // Numeric fields 
        // ================================
        
        // Int field with label
        public bool DrawIntField(string label, ref int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            string raw = GUILayout.TextField(value.ToString(), m_Styles.StyleTextField, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            if (int.TryParse(raw, out int result) && result != value)
            {
                 value = result;
                 return true;
            }
            return false;
        }
        
        // Float field with label
        public bool DrawFloatField(string label, ref float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            string raw = GUILayout.TextField(value.ToString("F2"), m_Styles.StyleTextField, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            if (float.TryParse(raw, out float result) && !Mathf.Approximately(result, value))
            {
                value = result;
                return true;
            }
            return false;
        }

        // Vector2 field
        public bool DrawVector2Field(string label, ref Vector2 v)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            bool hasValueChanged = DrawAxisField("X", ref v.x, Color.red) |
                  DrawAxisField("Y", ref v.y, new Color(.2f, .8f, .2f));
            GUILayout.EndHorizontal();
            return hasValueChanged;
        }

        // Vector3 field
        public bool DrawVector3Field(string label, ref Vector3 v)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            bool valueChanged = DrawAxisField("X", ref v.x, Color.red) |
                DrawAxisField("Y", ref v.y, new Color(.2f, .8f, .2f)) |
                DrawAxisField("Z", ref v.z, new Color(.3f, .55f, 1f));
            GUILayout.EndHorizontal();
            return valueChanged;
        }

        private bool DrawAxisField(string axis, ref float value, Color color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUILayout.Label(axis, m_Styles.StyleLabelText, GUILayout.Width(14));
            GUI.color = old;
            string raw = GUILayout.TextField(value.ToString("F2"), m_Styles.StyleTextField, GUILayout.Width(56));
            if (float.TryParse(raw, out float result) && !Mathf.Approximately(result, value))
            {
                value = result;
                return true;
            }
            return false;
        }
        
        // ================================
        // Text fields 
        // ================================
        
        // Label title + text
        public void DrawLabelBlock(string title, string body)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, m_Styles.StyleLabelText, GUILayout.Width(110));
            GUILayout.Label(body, m_Styles.StyleLabelText);
            GUILayout.EndHorizontal();
        }
        
        // Simple text field
        public bool DrawTextField(string label, ref string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            string tmpValue = GUILayout.TextField(value, m_Styles.StyleTextField);
            GUILayout.EndHorizontal();
            if (tmpValue != value)
            {
                value = tmpValue;
                return true;
            }
            return false;
        }

        // Dropdown (popup)
        public bool DrawDropdown(string label, ref int selected, string[] options)
        {
            selected = Mathf.Clamp(selected, 0, options.Length - 1);
            bool changed = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            if (GUILayout.Button("▾ " + options[selected], m_Styles.StyleTextField))
            {
                m_DropdownOpen[label] = !m_DropdownOpen.GetValueOrDefault(label);
            }
            GUILayout.EndHorizontal();
            
            // Dropdown menu part
            if (m_DropdownOpen.GetValueOrDefault(label))
            {
                GUILayout.BeginVertical(m_Styles.StyleDropdownMenu);
                for (int i = 0; i < options.Length; i++)
                {
                    GUIStyle style = (i == selected)
                        ? m_Styles.StyleDropdownItemSelected
                        : m_Styles.StyleDropdownItem;

                    if (GUILayout.Button(options[i], style))
                    {
                        if (selected != i)
                        {
                            selected = i;
                            changed  = true;
                        }
                        m_DropdownOpen[label] = false;
                    }
                }
                GUILayout.EndVertical();
            }
            return changed;
        }

        public bool DrawEnumDropdown<TEnum>(string label, ref TEnum value) where TEnum : struct, Enum
        {
            if (!m_EnumValues.TryGetValue(typeof(Enum), out string[] stringValues))
            {
                m_EnumValues[typeof(Enum)] = stringValues = Enum.GetNames(typeof(TEnum));
            }

            string tmpValue = value.ToString();
            int selectedIndex = Array.FindIndex(stringValues, x => x == tmpValue);
            bool changed = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            if (GUILayout.Button("▾ " + stringValues[selectedIndex], m_Styles.StyleTextField))
            {
                m_DropdownOpen[label] = !m_DropdownOpen.GetValueOrDefault(label);
            }
            GUILayout.EndHorizontal();
            
            // Dropdown menu part
            if (m_DropdownOpen.GetValueOrDefault(label))
            {
                GUILayout.BeginVertical(m_Styles.StyleDropdownMenu);
                for (int i = 0; i < stringValues.Length; i++)
                {
                    GUIStyle style = (i == selectedIndex)
                        ? m_Styles.StyleDropdownItemSelected
                        : m_Styles.StyleDropdownItem;

                    if (GUILayout.Button(stringValues[i], style))
                    {
                        if (selectedIndex != i)
                        {
                            selectedIndex = i;
                            changed  = true;
                        }
                        m_DropdownOpen[label] = false;
                    }
                }
                GUILayout.EndVertical();
            }

            if (changed)
            {
                value = Enum.Parse<TEnum>(stringValues[selectedIndex]);
            }
            return changed;
        }
        
        // Toggle
        public bool DrawToggle(string label, ref bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            bool tmpValue = GUILayout.Toggle(value, value ? "ON" : "OFF", m_Styles.StyleToggle);
            GUILayout.EndHorizontal();
            if (tmpValue != value)
            {
                value = tmpValue;
                return true;
            }
            return false;
        }
    }
}