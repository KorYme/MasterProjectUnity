using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const string FLAGS_NONE = "None";
        private const string FLAGS_ALL = "All";
        
        private struct EnumFlagValues
        {
            public object[] Values;
            public string NoneName;
            public string AllName;
            public int ValueAll;
            public int RemovedValues;
        }
        
        private DebugToolStyles m_Styles;
        private readonly Dictionary<Type, object[]> m_EnumValues = new Dictionary<Type, object[]>();
        private readonly Dictionary<Type, EnumFlagValues> m_EnumFlagsValues = new Dictionary<Type, EnumFlagValues>();

        private MethodContext m_CurrentMethodContext;
        
        private MethodContext m_CurrentDropdownMethod;
        private Vector2 m_CurrentDropdownScroll;

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
        
        // ================================
        // Dropdowns 
        // ================================

        public bool DrawDropdown(string label, ref int selected, string[] options)
        {
            IncrementMethodIndex();
            selected = Mathf.Clamp(selected, 0, options.Length - 1);
            bool changed = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            if (GUILayout.Button("▾ " + options[selected], m_Styles.StyleTextField))
            {
                SetAsCurrentDropdown();
            }
            GUILayout.EndHorizontal();
            
            // Dropdown menu part
            if (m_CurrentDropdownMethod == m_CurrentMethodContext)
            {
                GUILayout.BeginVertical(m_Styles.StyleDropdownMenu, GUILayout.Height(GetDropdownHeight(options.Length)));
                m_CurrentDropdownScroll = GUILayout.BeginScrollView(m_CurrentDropdownScroll);
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
                GUILayout.EndScrollView();
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
                SetAsCurrentDropdown();
            }
            
            // Dropdown menu part
            if (m_CurrentDropdownMethod == m_CurrentMethodContext)
            {
                GUILayout.BeginVertical(m_Styles.StyleDropdownMenu, GUILayout.Height(GetDropdownHeight(objectValues.Length)));
                m_CurrentDropdownScroll = GUILayout.BeginScrollView(m_CurrentDropdownScroll);
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
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            value = tmpValue;
            return changed;
        }
        
        public bool DrawEnumFlagsDropdown<TEnum>(string label, ref TEnum value) where TEnum : Enum, IConvertible
        {
            IncrementMethodIndex();
            if (!m_EnumFlagsValues.TryGetValue(typeof(TEnum), out EnumFlagValues enumValues))
            {
                enumValues.Values = Enum.GetValues(typeof(TEnum))
                    .Cast<object>()
                    .ToArray();
                enumValues.ValueAll = 0;
                foreach (TEnum enumValue in enumValues.Values)
                {
                    enumValues.ValueAll |= enumValue.ToInt32(null);
                }

                enumValues.NoneName = Enum.IsDefined(typeof(TEnum), 0) ? Enum.ToObject(typeof(TEnum), 0).ToString() : FLAGS_NONE;
                enumValues.AllName = Enum.IsDefined(typeof(TEnum), enumValues.ValueAll) ? Enum.ToObject(typeof(TEnum), enumValues.ValueAll).ToString() : FLAGS_ALL;
                enumValues.RemovedValues = (Enum.IsDefined(typeof(TEnum), enumValues.ValueAll) ? 1 : 0) + (Enum.IsDefined(typeof(TEnum), 0) ? 1 : 0) - (enumValues.ValueAll == 0 ? 1 : 0);
                m_EnumFlagsValues[typeof(TEnum)] = enumValues;
            }
        
            bool changed  = false;
            int intValue = value.ToInt32(null);
        
            string summary = BuildFlagsSummary(enumValues, intValue);
        
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            GUILayout.BeginVertical("box");

            if (GUILayout.Button("▾ " + summary, m_Styles.StyleTextField))
            {
                SetAsCurrentDropdown();
            }

            GUIStyle style;
            if (m_CurrentDropdownMethod == m_CurrentMethodContext)
            {
                GUILayout.BeginVertical(m_Styles.StyleDropdownMenu);
                m_CurrentDropdownScroll = GUILayout.BeginScrollView(m_CurrentDropdownScroll, 
                    GUILayout.Height(GetDropdownHeight(enumValues.Values.Length - enumValues.RemovedValues)));
        
                foreach (TEnum enumValue in enumValues.Values)
                {
                    int flagBit = enumValue.ToInt32(null);
                    if (flagBit == 0 || flagBit == enumValues.ValueAll)
                    {
                        continue;
                    }
                    bool isSelected = (intValue & flagBit) == flagBit;
        
                    style = isSelected
                        ? m_Styles.StyleDropdownItemSelected
                        : m_Styles.StyleDropdownItem;
        
                    if (GUILayout.Button(enumValue.ToString(), style))
                    {
                        intValue = isSelected
                            ? intValue & ~flagBit
                            : intValue | flagBit;
        
                        value = (TEnum)Enum.ToObject(typeof(TEnum), intValue);
                        changed = true;
                    }
                }
        
                GUILayout.EndScrollView();
        
                GUILayout.BeginHorizontal();
                
                style = intValue == 0
                    ? m_Styles.StyleDropdownItemSelected
                    : m_Styles.StyleDropdownItem;
                if (GUILayout.Button(enumValues.NoneName, style))
                {
                    value = (TEnum)Enum.ToObject(typeof(TEnum), 0);
                    changed = true;
                }
                style = intValue == enumValues.ValueAll
                    ? m_Styles.StyleDropdownItemSelected
                    : m_Styles.StyleDropdownItem;
                if (GUILayout.Button(enumValues.AllName, style))
                {
                    value = (TEnum)Enum.ToObject(typeof(TEnum), enumValues.ValueAll);
                    changed = true;
                }
        
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        
            return changed;
        }
        
        private string BuildFlagsSummary(EnumFlagValues enumValue, int intValue)
        {
            if (intValue == 0) return enumValue.NoneName;
            if (intValue == enumValue.ValueAll) return enumValue.AllName;
            
            List<string> active = enumValue.Values
                .Cast<IConvertible>()
                .Where(v => {
                    int bit = v.ToInt32(null);
                    return (intValue & bit) == bit && bit != 0;
                })
                .Select(v => v.ToString(CultureInfo.InvariantCulture))
                .ToList();
        
            return active.Count switch
            {
                0 => enumValue.NoneName,
                1 => active[0],
                _ => $"Mixed ({active.Count})"
            };
        }
        
        private float GetDropdownHeight(int itemCount)
        {
            float dropdownMaxHeight = m_Styles.StyleDropdownItem.CalcHeight(GUIContent.none, 110f) * itemCount
                                      + m_Styles.StyleDropdownMenu.padding.top + m_Styles.StyleDropdownMenu.padding.top;
            return Mathf.Clamp(dropdownMaxHeight, DebugToolStyles.MIN_DROPDOWN_HEIGHT, DebugToolStyles.MAX_DROPDOWN_HEIGHT);
        }
        
        private void SetAsCurrentDropdown()
        {
            if (m_CurrentDropdownMethod != m_CurrentMethodContext)
            {
                m_CurrentDropdownMethod = m_CurrentMethodContext;
                m_CurrentDropdownScroll = Vector2.zero;
            }
            else
            {
                m_CurrentDropdownMethod = default;
            }
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