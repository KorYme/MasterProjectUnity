using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AdvancedDebugTool
{
    public interface IMethodContextSetter
    {
        /// <summary>
        /// Change the method context
        /// </summary>
        /// <param name="instanceId">Index of the methodInstance</param>
        /// <param name="methodId">Index of the method called of this methodInstance</param>
        /// <returns>The current methodId, to store in case you'll come back to same instance</returns>
        uint SetCurrentMethodContext(uint instanceId, uint methodId = 0);
    }
    
    public class DebugContext : IMethodContextSetter
    {
        private DebugToolStyles m_Styles;
        private readonly Dictionary<Type, object[]> m_EnumValues = new Dictionary<Type, object[]>();

        private MethodContext m_CurrentDropdownMethod;
            
        private MethodContext m_CurrentMethodContext;
        
        uint IMethodContextSetter.SetCurrentMethodContext(uint instanceId, uint methodId)
        {
            uint currentMethodId = m_CurrentMethodContext.MethodId;
            m_CurrentMethodContext.InstanceId = instanceId;
            m_CurrentMethodContext.MethodId = methodId;
            return currentMethodId;
        }
        
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
            IncrementMethodIndex();
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
            IncrementMethodIndex();
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
            IncrementMethodIndex();
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
            IncrementMethodIndex();
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
            IncrementMethodIndex();
            GUILayout.BeginHorizontal();
            GUILayout.Label(title, m_Styles.StyleLabelText, GUILayout.Width(110));
            GUILayout.Label(body, m_Styles.StyleLabelText);
            GUILayout.EndHorizontal();
        }
        
        // Simple text field
        public bool DrawTextField(string label, ref string value)
        {
            IncrementMethodIndex();
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
            IncrementMethodIndex();
            selected = Mathf.Clamp(selected, 0, options.Length - 1);
            bool changed = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            if (GUILayout.Button("▾ " + options[selected], m_Styles.StyleTextField))
            {
                m_CurrentDropdownMethod = m_CurrentDropdownMethod != m_CurrentMethodContext 
                    ? m_CurrentMethodContext : default;
            }
            GUILayout.EndHorizontal();
            
            // Dropdown menu part
            if (m_CurrentDropdownMethod == m_CurrentMethodContext)
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
                        m_CurrentDropdownMethod = default;
                    }
                }
                GUILayout.EndVertical();
            }
            return changed;
        }

        public bool DrawEnumDropdown<TEnum>(string label, ref TEnum value) where TEnum : Enum, IConvertible
        {
            IncrementMethodIndex();
            if (!m_EnumValues.TryGetValue(typeof(TEnum), out object[] objectValues))
            {
                m_EnumValues[typeof(TEnum)] = objectValues = 
                    Enum.GetValues(typeof(TEnum))
                    .Cast<object>()
                    .ToArray();
            }
            
            bool changed = false;
            TEnum tmpValue = value;

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            GUILayout.BeginVertical("box");
            if (GUILayout.Button("▾ " + value.ToString(), m_Styles.StyleTextField))
            {
                m_CurrentDropdownMethod = m_CurrentDropdownMethod != m_CurrentMethodContext 
                    ? m_CurrentMethodContext : default;
            }
            
            // Dropdown menu part
            if (m_CurrentDropdownMethod == m_CurrentMethodContext)
            {
                GUILayout.BeginVertical(m_Styles.StyleDropdownMenu);

                foreach (TEnum enumValue in objectValues)
                {
                    bool isSelected = Equals(enumValue, value);
                    GUIStyle style = isSelected
                        ? m_Styles.StyleDropdownItemSelected
                        : m_Styles.StyleDropdownItem;

                    if (GUILayout.Button(enumValue.ToString(), style))
                    {
                        tmpValue = enumValue;
                        changed |= !isSelected;
                        m_CurrentDropdownMethod = default;
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            value = tmpValue;
            return changed;
        }

        public bool DrawButton(string label)
        {
            IncrementMethodIndex();
            GUILayout.BeginHorizontal("button");
            bool hasBeenClicked = GUILayout.Button(label, m_Styles.StyleButton);
            GUILayout.EndHorizontal();
            return hasBeenClicked;
        }
        
        // Toggle
        public bool DrawToggle(string label, ref bool value)
        {
            IncrementMethodIndex();
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

        /// <summary>
        /// Should be called each time a dropdown is required
        /// For the moment, only on each draw method called
        /// </summary>
        private void IncrementMethodIndex()
        {
            m_CurrentMethodContext.MethodId++;
        }
    }
}