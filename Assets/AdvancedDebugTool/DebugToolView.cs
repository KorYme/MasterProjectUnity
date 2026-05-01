using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdvancedDebugTool
{
    public class DebugToolView
    {
        public const int WINDOW_ID = 1000;
        public const float WINDOW_WIDTH = 420f;
        public const float WINDOW_HEIGHT = 600f;
        public static readonly Vector2 WINDOW_POS = new Vector2(2, 2);
        
        private DebugContext m_DebugContext;
        private object[] m_Arguments;
        private IDebugInfoProvider m_DebugInfoProvider;
        private DebugToolStyles m_Styles;

        private Dictionary<string, bool> m_CategoryFoldouts = new Dictionary<string, bool>();
        private Rect m_WindowRect;
        private Vector2 m_Scroll;
        private GUISkin m_PreviousSkin;

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
            // Barre de statut
            DrawStatusBar();

            // Zone scrollable
            m_Scroll = GUILayout.BeginScrollView(m_Scroll);
            
            // for (int i = 0; i < _categories.Count; i++)
            // {
            //     DrawCategory(i, group[0].Category, group);
            //     GUILayout.Space(6f);
            // }
            
            foreach (DebugTypeDefinition definition in m_DebugInfoProvider.GetDebugInfos())
            {
                definition.Instances.RemoveWhere(obj => obj == null);
                foreach (object obj in definition.Instances)
                {
                    foreach (DebugMethod method in definition.Methods)
                    {
                        GUILayout.BeginHorizontal(m_Styles.StyleCategoryBar);
                        if (!m_CategoryFoldouts.TryGetValue(method.Title, out bool isFoldedOut))
                        {
                            m_CategoryFoldouts[method.Title] = isFoldedOut = false;
                        }
                        string arrow = isFoldedOut ? "▼  " : "▶  ";
                        if (GUILayout.Button(arrow + method.Title, m_Styles.StyleCategoryLabel))
                        {
                            m_CategoryFoldouts[method.Title] = isFoldedOut = !isFoldedOut;
                        }
                        GUILayout.EndHorizontal();

                        if (!isFoldedOut) continue;
                        
                        GUILayout.BeginVertical("box");
                        method.Invoke(obj, m_Arguments);
                        GUILayout.EndVertical();
                    }
                }
            }
            
            GUILayout.EndScrollView();

            // Drag
            GUI.DragWindow(new Rect(0, 0, WINDOW_WIDTH, 24));
        }
        
        // private void DrawCategory(int index, string title)
        // {
        //     // Header de catégorie (cliquable pour fold/unfold)
        //     GUILayout.BeginHorizontal(m_Styles.StyleCategoryBar);
        //     string arrow = m_CategoryFoldouts[index] ? "▼  " : "▶  ";
        //     if (GUILayout.Button(arrow + title, m_Styles.StyleCategoryLabel))
        //         m_CategoryFoldouts[index] = !m_CategoryFoldouts[index];
        //     GUILayout.EndHorizontal();
        //
        //     if (!m_CategoryFoldouts[index]) return;
        //
        //     GUILayout.BeginVertical("box");    // utilise le style "box" du GUISkin
        //
        //     // TODO : DRAW HERE
        //     
        //     GUILayout.EndVertical();
        // }
        
        private void DrawStatusBar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{Time.frameCount}", m_Styles.StyleLabelText);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", m_Styles.StyleButtonDanger, GUILayout.Width(72)))
            {
                OnCloseRequest?.Invoke();
            }
            GUILayout.EndHorizontal();

            DrawHR();
        }
        
        private void DrawHR()
        {
            GUILayout.Box(GUIContent.none, m_Styles.StyleHR, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(4f);
        }
    }
}