using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugContext
    {
        private DebugToolStyles m_Styles;
        
        public DebugContext(DebugToolStyles styles)
        {
            m_Styles = styles;
        }

        // ================================
        // Numeric fields 
        // ================================
        
        // Int field with label
        public int DrawIntField(string label, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            string raw = GUILayout.TextField(value.ToString(), m_Styles.StyleTextField, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            return int.TryParse(raw, out int result) ? result : value;
        }
        
        // Float field with label
        public float DrawFloatField(string label, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            string raw = GUILayout.TextField(value.ToString("F2"), m_Styles.StyleTextField, GUILayout.Width(80));
            GUILayout.EndHorizontal();
            return float.TryParse(raw, out float result) ? result : value;
        }

        // Vector2 field
        public Vector2 DrawVector2Field(string label, Vector2 v)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            v.x = DrawAxisField("X", v.x, Color.red);
            v.y = DrawAxisField("Y", v.y, new Color(.2f, .8f, .2f));
            GUILayout.EndHorizontal();
            return v;
        }

        // Vector3 field
        public Vector3 DrawVector3Field(string label, Vector3 v)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            v.x = DrawAxisField("X", v.x, Color.red);
            v.y = DrawAxisField("Y", v.y, new Color(.2f, .8f, .2f));
            v.z = DrawAxisField("Z", v.z, new Color(.3f, .55f, 1f));
            GUILayout.EndHorizontal();
            return v;
        }

        private float DrawAxisField(string axis, float value, Color color)
        {
            Color old = GUI.color;
            GUI.color = color;
            GUILayout.Label(axis, m_Styles.StyleLabelText, GUILayout.Width(14));
            GUI.color = old;
            string raw = GUILayout.TextField(value.ToString("F2"), m_Styles.StyleTextField, GUILayout.Width(56));
            return float.TryParse(raw, out float r) ? r : value;
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
        public string DrawTextField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            value = GUILayout.TextField(value, m_Styles.StyleTextField);
            GUILayout.EndHorizontal();
            return value;
        }

        // Dropdown (popup)
        public int DrawDropdown(string label, int selected, string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            // GUISkin propre, donc le SelectionGrid 1-colonne fait un dropdown vertical lisible
            // Pour un vrai popup tu peux utiliser EditorGUILayout en Editor, ou ce pattern en Runtime :
            selected = Mathf.Clamp(selected, 0, options.Length - 1);
            if (GUILayout.Button(options[selected] + "  ▾", m_Styles.StyleTextField))
            {
                // toggle un mini-menu manuel : à implémenter avec un flag booléen si besoin
            }
            GUILayout.EndHorizontal();
            return selected;
        }

        // Toggle
        public bool DrawToggle(string label, bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, m_Styles.StyleLabelText, GUILayout.Width(110));
            value = GUILayout.Toggle(value, value ? "ON" : "OFF", m_Styles.StyleToggle);
            GUILayout.EndHorizontal();
            return value;
        }
    }
}