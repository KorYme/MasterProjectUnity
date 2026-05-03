using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugToolView<TEnum> where TEnum : Enum, IConvertible
    {
        public const int WINDOW_ID = 1000;
        public const float WINDOW_WIDTH = 420f;
        public const float WINDOW_HEIGHT = 600f;
        public static readonly Vector2 WINDOW_POS = new Vector2(2, 2);
        
        private DebugContext m_DebugContext;
        private IMethodContextSetter m_MethodContextSetter;
        private object[] m_Arguments;
        private IDebugInfoProvider m_DebugInfoProvider;
        private DebugToolStyles m_Styles;

        private Dictionary<string, bool> m_CategoryFoldouts = new Dictionary<string, bool>();
        private Rect m_WindowRect;
        private Vector2 m_Scroll;
        private GUISkin m_PreviousSkin;
        private TEnum m_DebugCategory;

        public event Action OnCloseRequest; 

        public DebugToolView(IDebugInfoProvider debugInfoProvider)
        {
            m_DebugInfoProvider = debugInfoProvider;
            
            m_WindowRect = new Rect(WINDOW_POS.x, WINDOW_POS.y, WINDOW_WIDTH, WINDOW_HEIGHT);
        }

        ~DebugToolView()
        {
            OnCloseRequest = null;
        }
        
        public void OnGUI()
        {
            if (m_Styles == null) 
            {
                m_Styles = new DebugToolStyles();
                m_DebugContext = new DebugContext(m_Styles);
                m_MethodContextSetter = m_DebugContext;
                m_Arguments = new object[] { m_DebugContext };
            }
            
            m_PreviousSkin = GUI.skin;
            GUI.skin = m_Styles.Skin;
            
            m_WindowRect = GUILayout.Window(
                id: WINDOW_ID,
                screenRect: m_WindowRect,
                func: DrawWindow,
                text: "Debug Tools",
                style: m_Styles.StyleWindow,
                options: GUILayout.Width(WINDOW_WIDTH)
            );
            
            GUI.skin = m_PreviousSkin;
            m_PreviousSkin = null;
        }

        private void DrawWindow(int id)
        {
            uint debugToolViewMethodIndex = 0;
            m_MethodContextSetter.SetCurrentMethodContext(MethodContext.DEBUG_TOOL_VIEW_ID, debugToolViewMethodIndex);
            DrawStatusBar();
            DrawSeparator();

            m_DebugContext.DrawEnumDropdown("Category", ref m_DebugCategory);
            
            // Scrollable area
            m_Scroll = GUILayout.BeginScrollView(m_Scroll);
            
            foreach (DebugMethodInfoInstance methodInstance in m_DebugInfoProvider.GetDebugInfos(m_DebugCategory.ToInt32(null)))
            {
                GUILayout.BeginHorizontal(m_Styles.StyleCategoryBar);
                if (!m_CategoryFoldouts.TryGetValue(methodInstance.Title, out bool isFoldedOut))
                {
                    m_CategoryFoldouts[methodInstance.Title] = isFoldedOut = false;
                }
                string arrow = isFoldedOut ? "▼  " : "▶  ";
                if (GUILayout.Button(arrow + methodInstance.Title, m_Styles.StyleCategoryLabel))
                {
                    m_CategoryFoldouts[methodInstance.Title] = isFoldedOut = !isFoldedOut;
                }
                GUILayout.EndHorizontal();

                if (!isFoldedOut) continue;
                
                GUILayout.BeginVertical("box");
                debugToolViewMethodIndex = m_MethodContextSetter.SetCurrentMethodContext(methodInstance.Id);
                methodInstance.Invoke(m_Arguments);
                m_MethodContextSetter.SetCurrentMethodContext(MethodContext.DEBUG_TOOL_VIEW_ID, debugToolViewMethodIndex);
                GUILayout.EndVertical();
            }
            
            GUILayout.EndScrollView();

            // Drag
            GUI.DragWindow(new Rect(0, 0, WINDOW_WIDTH, 24));
            m_MethodContextSetter.SetCurrentMethodContext(MethodContext.DEFAULT_ID);
        }
        
        private void DrawStatusBar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", m_Styles.StyleButtonDanger, GUILayout.Width(72)))
            {
                OnCloseRequest?.Invoke();
            }
            GUILayout.EndHorizontal();
        }
        
        private void DrawSeparator()
        {
            GUILayout.Box(GUIContent.none, m_Styles.StyleSeparator, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(4f);
        }
    }
}