using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AdvancedDebugTool
{
    public class DebugToolStyles
    {
        private const string FONT_RESOURCE_PATH = "DebugTool/Fonts/LiberationSans";
        private const string GUI_SKIN_PATH = "DebugTool/DebugToolStyles";
        private Dictionary<int, int> m_CachedTextureIndexes = new Dictionary<int, int>();
        private List<Texture2D> m_Textures = new List<Texture2D>();
        
        public GUISkin Skin { get; private set; }
        public Font Font { get; private set; }
        
        public GUIStyle StyleWindow { get; private set; }
        public GUIStyle StyleCategoryBar { get; private set; }
        public GUIStyle StyleCategoryLabel { get; private set; }
        public GUIStyle StyleButton { get; private set; }
        public GUIStyle StyleButtonDanger { get; private set; }
        public GUIStyle StyleLabelTitle { get; private set; }
        public GUIStyle StyleLabelText { get; private set; }
        public GUIStyle StyleTextField { get; private set; }
        public GUIStyle StyleToggle { get; private set; }
        public GUIStyle StyleSeparator { get; private set; }
        
        public DebugToolStyles()
        {
            Skin = Resources.Load<GUISkin>(GUI_SKIN_PATH);
            if (!Skin)
            {
                throw new NullReferenceException($"No GUISkin found in Resource folder at path {GUI_SKIN_PATH}");
            }
            Font = Resources.Load<Font>(FONT_RESOURCE_PATH) ?? Skin.font;
            
            Texture2D bgDark = GetTexture(new Color(.12f, .12f, .14f, .97f));
            Texture2D bgMid = GetTexture(new Color(.17f, .17f, .20f, 1f));
            Texture2D bgLight = GetTexture(new Color(.22f, .22f, .26f, 1f));
            Texture2D bgInput = GetTexture(new Color(.10f, .10f, .13f, 1f));
            Texture2D bgAccent = GetTexture(new Color(.15f, .40f, .80f, .85f));
            Texture2D bgDanger = GetTexture(new Color(.70f, .18f, .18f, .85f));
            Color colorText = Color.white;
            Color colorMuted = new Color(.65f, .65f, .70f, 1f);
            int fontSize = 16;
    
            // Window
            StyleWindow = new GUIStyle(GUI.skin.window)
            {
                padding = new RectOffset(8, 8, 24, 8),
                fontSize = fontSize + 4,
                font = Font,
            };
            StyleWindow.normal.background = bgDark;
            StyleWindow.normal.textColor = colorText;
            StyleWindow.onNormal.background = bgDark;
            StyleWindow.onNormal.textColor = colorText;
    
            // Category bar
            StyleCategoryBar = new GUIStyle
            {
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
            };
    
            StyleCategoryLabel = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Bold,
                fontSize = fontSize,
                font = Font,
                padding = new RectOffset(10, 6, 4, 4),
                margin = new RectOffset(0, 0, 0, 0),
                border = new RectOffset(4, 4, 4, 4),
            };
            StyleCategoryLabel.normal.background = bgMid;
            StyleCategoryLabel.normal.textColor = colorText;
            StyleCategoryLabel.hover.background = bgLight;
            StyleCategoryLabel.hover.textColor = colorText;
            StyleCategoryLabel.active.background = bgAccent;
            StyleCategoryLabel.active.textColor = colorText;
    
            // Standard button 
            StyleButton = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                font = Font,
                padding = new RectOffset(10, 10, 5, 5),
                margin = new RectOffset(2, 2, 2, 2),
            };
            StyleButton.normal.background = bgLight;
            StyleButton.normal.textColor = colorText;
            StyleButton.hover.background = bgAccent;
            StyleButton.hover.textColor = colorText;
            StyleButton.active.background = bgAccent;
            StyleButton.active.textColor = colorText;
    
            // Button danger
            StyleButtonDanger = new GUIStyle(StyleButton);
            StyleButtonDanger.normal.background = bgDanger;
            StyleButtonDanger.hover.background = GetTexture(new Color(.85f, .25f, .25f, 1f));
            StyleButtonDanger.active.background = bgDanger;
    
            // Label titre
            StyleLabelTitle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = fontSize + 1,
                font = Font,
                padding = new RectOffset(4, 0, 2, 0),
            };
            StyleLabelTitle.normal.textColor = colorText;
    
            // Label text
            StyleLabelText = new GUIStyle(GUI.skin.label)
            {
                fontSize = fontSize,
                font = Font,
                padding = new RectOffset(4, 0, 2, 2),
            };
            StyleLabelText.normal.textColor = colorMuted;
    
            // Text field
            StyleTextField = new GUIStyle(GUI.skin.textField)
            {
                fontSize = fontSize,
                font = Font,
                padding = new RectOffset(6, 6, 4, 4),
                alignment = TextAnchor.MiddleLeft,
            };
            StyleTextField.normal.background = bgInput;
            StyleTextField.normal.textColor = colorText;
            StyleTextField.focused.background = bgInput;
            StyleTextField.focused.textColor = colorText;
    
            // Toggle
            StyleToggle = new GUIStyle(GUI.skin.toggle)
            {
                fontSize = fontSize,
                font = Font,
                padding = new RectOffset(22, 0, 3, 3),
            };
            StyleToggle.normal.textColor  = colorMuted;
            StyleToggle.onNormal.textColor = new Color(.3f, .85f, .45f);
    
            // Span
            StyleSeparator = new GUIStyle
            {
                margin = new RectOffset(0, 0, 4, 4),
                padding = new RectOffset(0, 0, 0, 0),
                fixedHeight = 1f,
                normal = { background = GetTexture(new Color(.35f, .35f, .40f, 1f)) }
            };
        }

        private Texture2D GetTexture(in Color color)
        {
            int hash = color.GetHashCode();
            if (m_CachedTextureIndexes.TryGetValue(hash, out int texIndex))
            {
                return m_Textures[texIndex];
            }
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            m_CachedTextureIndexes[hash] = m_Textures.Count;
            m_Textures.Add(texture);
            return texture;
        }
        
        ~DebugToolStyles()
        {
            m_CachedTextureIndexes.Clear();
            for (int i = m_Textures.Count - 1; i >= 0; i--)
            {
                Object.Destroy(m_Textures[i]);
            }
            m_Textures.Clear();
        }
    }
}